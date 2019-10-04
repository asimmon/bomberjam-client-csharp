using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberjam.Client.Colyseus.Utils;
using Hackathon.Framework.GameDevWare.Serialization;

namespace Bomberjam.Client.Colyseus
{
    public class Client : IDisposable
    {
        private readonly UriBuilder _endpoint;
        private readonly Connection _connection;

        private readonly Dictionary<string, IRoom> _rooms = new Dictionary<string, IRoom>();
        private readonly Dictionary<int, IRoom> _connectingRooms = new Dictionary<int, IRoom>();
        private readonly Dictionary<int, Action<RoomAvailable[]>> _roomsAvailableRequests = new Dictionary<int, Action<RoomAvailable[]>>();

        private int _requestCounter;
        private byte _previousCode;

        public Client(Uri endpoint, string id = null)
        {
            this._endpoint = new UriBuilder(endpoint);
            this.Id = id;

            this._connection = this.CreateConnection();
            this._connection.OnClose += (sender, e) => { this.OnClose?.Invoke(sender, e); };
            this._connection.OnError += (sender, e) => { this.OnError?.Invoke(this, e); };
            this._connection.OnMessage += (sender, e) => { this.ParseMessage(e.Message); };
        }

        public string Id { get; private set; }

        public event EventHandler OnOpen;
        public event EventHandler OnClose;
        public event EventHandler<ExceptionEventArgs> OnError;

        public Task ConnectAsync()
        {
            return this._connection.OpenAsync();
        }

        public async Task<Room<T>> Join<T>(string roomName, Dictionary<string, object> options = null)
        {
            if (options == null)
            {
                options = new Dictionary<string, object>();
            }

            var requestId = ++this._requestCounter;
            options.Add("requestId", requestId);

            var room = new Room<T>(roomName, options);
            this._connectingRooms.Add(requestId, room);

            await this._connection.SendAsync(new object[] { Protocol.JOIN_REQUEST, roomName, options });

            return room;
        }

        public Task<Room<IndexedDictionary<string, object>>> Join(string roomName, Dictionary<string, object> options = null)
        {
            return this.Join<IndexedDictionary<string, object>>(roomName, options);
        }

        public Task<Room<T>> Rejoin<T>(string roomName, string sessionId)
        {
            var options = new Dictionary<string, object>
            {
                { "sessionId", sessionId }
            };

            return this.Join<T>(roomName, options);
        }

        public Task<Room<IndexedDictionary<string, object>>> Rejoin(string roomName, string sessionId)
        {
            return this.Rejoin<IndexedDictionary<string, object>>(roomName, sessionId);
        }

        public async Task GetAvailableRooms(string roomName, Action<RoomAvailable[]> callback)
        {
            var requestId = ++this._requestCounter;
            await this._connection.SendAsync(new object[] { Protocol.ROOM_LIST, requestId, roomName });
            this._roomsAvailableRequests.Add(requestId, callback);
        }

        private Connection CreateConnection(string path = "", Dictionary<string, object> options = null)
        {
            if (options == null)
            {
                options = new Dictionary<string, object>();
            }

            if (this.Id != null)
            {
                options.Add("colyseusid", this.Id);
            }

            var queryString = new List<string>(options.Count);
            foreach (var item in options)
            {
                queryString.Add(item.Key + "=" + (item.Value != null ? Convert.ToString(item.Value) : "null"));
            }

            var uriBuilder = new UriBuilder(this._endpoint.Uri)
            {
                Path = path,
                Query = string.Join("&", queryString.ToArray())
            };

            return new Connection(uriBuilder.Uri);
        }

        private void ParseMessage(byte[] bytes)
        {
            if (this._previousCode == 0)
            {
                var code = bytes[0];

                if (code == Protocol.USER_ID)
                {
                    this.Id = Encoding.UTF8.GetString(bytes, 2, bytes[1]);
                    this.OnOpen?.Invoke(this, EventArgs.Empty);
                }
                else if (code == Protocol.JOIN_REQUEST)
                {
                    var requestId = bytes[1];

                    IRoom room;
                    if (this._connectingRooms.TryGetValue(requestId, out room))
                    {
                        room.Id = Encoding.UTF8.GetString(bytes, 3, bytes[2]);

                        this._endpoint.Path = "/" + room.Id;
                        this._endpoint.Query = "colyseusid=" + this.Id;

                        var processPath = "";
                        var nextIndex = 3 + room.Id.Length;
                        if (bytes.Length > nextIndex)
                        {
                            processPath = Encoding.UTF8.GetString(bytes, nextIndex + 1, bytes[nextIndex]) + "/";
                        }

                        room.SetConnection(this.CreateConnection(processPath + room.Id, room.Options));
                        room.OnLeave += this.OnLeaveRoom;

                        if (this._rooms.ContainsKey(room.Id))
                        {
                            this._rooms.Remove(room.Id);
                        }

                        this._rooms.Add(room.Id, room);
                        this._connectingRooms.Remove(requestId);
                    }
                    else
                    {
                        throw new Exception($"Can't join room using requestId {requestId}");
                    }
                }
                else if (code == Protocol.JOIN_ERROR)
                {
                    var message = Encoding.UTF8.GetString(bytes, 2, bytes[1]);
                    OnError?.Invoke(this, new ExceptionEventArgs(message));
                }
                else if (code == Protocol.ROOM_LIST)
                {
                    this._previousCode = code;
                }
            }
            else
            {
                if (this._previousCode == Protocol.ROOM_LIST)
                {
                    var message = MsgPack.Deserialize<List<object>>(new MemoryStream(bytes));
                    var requestId = Convert.ToInt32(message[0]);
                    var rooms = (List<object>)message[1];
                    var availableRooms = new RoomAvailable[rooms.Count];

                    for (var i = 0; i < rooms.Count; i++)
                    {
                        availableRooms[i] = ObjectExtensions.ToObject<RoomAvailable>(rooms[i]);
                    }

                    this._roomsAvailableRequests[requestId].Invoke(availableRooms);
                    this._roomsAvailableRequests.Remove(requestId);
                }

                this._previousCode = 0;
            }
        }

        private void OnLeaveRoom(object sender, EventArgs args)
        {
            var room = (IRoom)sender;
            this._rooms.Remove(room.Id);
        }

        public async Task CloseAsync()
        {
            var roomIds = this._rooms.Keys.ToArray();
            foreach (var roomId in roomIds)
            {
                await this._rooms[roomId].CloseAsync();
            }

            await this._connection.CloseAsync();
        }

        public void Dispose()
        {
            var roomIds = this._rooms.Keys.ToArray();
            foreach (var roomId in roomIds)
            {
                this._rooms[roomId].Dispose();
            }

            this._connection.Dispose();
        }
    }
}