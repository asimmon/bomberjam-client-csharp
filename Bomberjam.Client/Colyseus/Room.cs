using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Bomberjam.Client.Colyseus.Serializer;
using Bomberjam.Client.Colyseus.StateListener;
using Hackathon.Framework.GameDevWare.Serialization;

namespace Bomberjam.Client.Colyseus
{
    public class RoomAvailable
    {
        public string roomId { get; set; }
        public uint clients { get; set; }
        public uint maxClients { get; set; }
        public object metadata { get; set; }
    }

    public interface IRoom : IDisposable
    {
        string Id { get; set; }
        Dictionary<string, object> Options { get; }
        event EventHandler OnLeave;
        void SetConnection(Connection connection);
        Task CloseAsync();
    }

    public class Room<T> : IRoom
    {
        private Connection _connection;
        private ISerializer<T> _serializer;
        private string _serializerId;
        private byte _previousCode;

        public string Id { get; set; }
        public string Name { get; set; }
        public string SessionId { get; set; }
        public Dictionary<string, object> Options { get; set; }

        public event EventHandler OnReadyToConnect;
        public event EventHandler OnJoin;
        public event EventHandler<ExceptionEventArgs> OnError;
        public event EventHandler OnLeave;
        public event EventHandler<MessageEventArgs> OnMessage;
        public event EventHandler<StateChangeEventArgs<T>> OnStateChange;

        public Room(string name, Dictionary<string, object> options = null)
        {
            this.Name = name;
            this.Options = options;
        }

        public Task ConnectAsync()
        {
            return this._connection.OpenAsync();
        }

        public void SetConnection(Connection connection)
        {
            this._connection = connection;

            this._connection.OnClose += (sender, e) => { this.OnLeave?.Invoke(this, e); };
            this._connection.OnError += (sender, e) => { this.OnError?.Invoke(this, e); };
            this._connection.OnMessage += (sender, e) => { this.ParseMessage(e.Message); };

            this.OnReadyToConnect?.Invoke(this, EventArgs.Empty);
        }

        public T State
        {
            get { return this._serializer.GetState(); }
        }

        public Task LeaveAsync(bool consented = true)
        {
            if (this.Id != null)
            {
                return consented
                    ? this._connection.SendAsync(new object[] { Protocol.LEAVE_ROOM })
                    : this._connection.CloseAsync();
            }

            this.OnLeave?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public Task SendAsync(object data)
        {
            return _connection.SendAsync(new[] { Protocol.ROOM_DATA, Id, data });
        }

        public Listener<Action<PatchObject>> Listen(Action<PatchObject> callback)
        {
            if (string.IsNullOrEmpty(_serializerId))
            {
                throw new Exception($"{nameof(this.Listen)} should be called after {nameof(this.OnJoin)}");
            }

            return ((FossilDeltaSerializer)_serializer).State.Listen(callback);
        }

        public Listener<Action<DataChange>> Listen(string segments, Action<DataChange> callback, bool immediate = false)
        {
            if (string.IsNullOrEmpty(_serializerId))
            {
                throw new Exception($"{nameof(this.Listen)} should be called after {nameof(this.OnJoin)}");
            }

            return ((FossilDeltaSerializer)_serializer).State.Listen(segments, callback, immediate);
        }

        private void ParseMessage(byte[] bytes)
        {
            if (this._previousCode == 0)
            {
                var code = bytes[0];

                if (code == Protocol.JOIN_ROOM)
                {
                    var offset = 1;

                    this.SessionId = Encoding.UTF8.GetString(bytes, offset + 1, bytes[offset]);
                    offset += this.SessionId.Length + 1;

                    this._serializerId = Encoding.UTF8.GetString(bytes, offset + 1, bytes[offset]);
                    offset += this._serializerId.Length + 1;

                    if (this._serializerId == "schema")
                    {
                        this._serializer = new SchemaSerializer<T>();
                    }
                    else if (this._serializerId == "fossil-delta")
                    {
                        this._serializer = (ISerializer<T>)new FossilDeltaSerializer();
                    }

                    if (bytes.Length > offset)
                    {
                        this._serializer.Handshake(bytes, offset);
                    }

                    this.OnJoin?.Invoke(this, EventArgs.Empty);
                }
                else if (code == Protocol.JOIN_ERROR)
                {
                    var message = Encoding.UTF8.GetString(bytes, 2, bytes[1]);
                    this.OnError?.Invoke(this, new ExceptionEventArgs(message));
                }
                else if (code == Protocol.LEAVE_ROOM)
                {
                    this.LeaveAsync();
                }
                else
                {
                    this._previousCode = code;
                }
            }
            else
            {
                if (this._previousCode == Protocol.ROOM_STATE)
                {
                    this.SetState(bytes);
                }
                else if (this._previousCode == Protocol.ROOM_STATE_PATCH)
                {
                    this.Patch(bytes);
                }
                else if (this._previousCode == Protocol.ROOM_DATA)
                {
                    var message = MsgPack.Deserialize<object>(new MemoryStream(bytes));
                    this.OnMessage?.Invoke(this, new MessageEventArgs(message));
                }

                this._previousCode = 0;
            }
        }

        private void SetState(byte[] encodedState)
        {
            this._serializer.SetState(encodedState);
            OnStateChange?.Invoke(this, new StateChangeEventArgs<T>(this._serializer.GetState(), true));
        }

        private void Patch(byte[] delta)
        {
            this._serializer.Patch(delta);
            this.OnStateChange?.Invoke(this, new StateChangeEventArgs<T>(this._serializer.GetState()));
        }

        public Task CloseAsync()
        {
            return this.LeaveAsync(false);
        }

        public void Dispose()
        {
            this._connection?.Dispose();
        }
    }
}