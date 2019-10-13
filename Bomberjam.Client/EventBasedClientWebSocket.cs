using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Bomberjam.Client.Colyseus;

namespace Bomberjam.Client
{
    internal class EventBasedClientWebSocket : IDisposable
    {
        private static readonly ISet<string> SuppportedSchemes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ws", "wss"
        };
        
        private const int SendChunkSize = 1024;
        private const int RecvChunkSize = 1024;
        
        private readonly Uri _uri;
        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _token;
        private readonly ClientWebSocket _socket;

        public EventBasedClientWebSocket(Uri uri)
        {
            if (uri == null || !uri.IsAbsoluteUri)
                throw new ArgumentException($"Unsupported url: {uri}");

            if (!SuppportedSchemes.Contains(uri.Scheme))
                throw new ArgumentException($"Unsupported protocol: {uri.Scheme}");
            
            this._uri = uri;
            this._cts = new CancellationTokenSource();
            this._token = this._cts.Token;
            this._socket = new ClientWebSocket();
        }
        
        public event EventHandler OnOpen;
        public event EventHandler OnClose;
        public event EventHandler<ExceptionEventArgs> OnError;
        public event EventHandler<MessageEventArgs<byte[]>> OnMessage;
        
        public bool IsOpened
        {
            get => this._socket.State == WebSocketState.Open;
        }

        public async Task OpenAsync()
        {
            if (!this.IsOpened)
            {
                await this._socket.ConnectAsync(this._uri, this._token);
                
                this.OnOpen?.Invoke(this, EventArgs.Empty);
                this.StartListening();
            }
        }
        
        private async void StartListening()
        {
            var buffer = new byte[RecvChunkSize];
            var segment = new ArraySegment<byte>(buffer);
            
            while (this.IsOpened && !this._token.IsCancellationRequested)
            {
                try
                {
                    using (var ms = new MemoryStream(buffer.Length))
                    {
                        WebSocketReceiveResult result;
                    
                        do
                        {
                            result = await this._socket.ReceiveAsync(segment, this._token);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await this._socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                                this.OnClose?.Invoke(this, EventArgs.Empty);
                                return;
                            }

                            if (segment.Array != null)
                            {
                                ms.Write(segment.Array, 0, result.Count);
                            }

                        } while (!result.EndOfMessage && this.IsOpened && !this._token.IsCancellationRequested);

                        ms.Seek(0, SeekOrigin.Begin);

                        var rawMessage = ms.ToArray();
                        this.OnMessage?.Invoke(this, new MessageEventArgs<byte[]>(rawMessage));
                    }
                }
                catch (Exception ex)
                {
                    if (ex is WebSocketException wsex)
                    {
                        // Console.WriteLine(wsex.WebSocketErrorCode);
                        // Console.WriteLine(wsex.ErrorCode);
                    }
                    
                    this.OnError?.Invoke(this, new ExceptionEventArgs(ex));
                }
            }
        }

        public Task CloseAsync()
        {
            return this.IsOpened
                ? this._socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, this._token)
                : Task.CompletedTask;
        }
        
        public async Task SendAsync(byte[] bytes)
        {
            if (!this.IsOpened)
            {
                throw new Exception("Connection is not open.");
            }

            var messagesCount = (int)Math.Ceiling((double)bytes.Length / SendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = SendChunkSize * i;
                var count = SendChunkSize;
                var isLastChunk = i + 1 == messagesCount;

                if (count * (i + 1) > bytes.Length)
                {
                    count = bytes.Length - offset;
                }

                await this._socket.SendAsync(new ArraySegment<byte>(bytes, offset, count), WebSocketMessageType.Binary, isLastChunk, this._token);
            }
        }

        public void Dispose()
        {
            this._cts.Dispose();
            this._socket.Dispose();
        }
    }
}