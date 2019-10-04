using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hackathon.Framework.GameDevWare.Serialization;

namespace Bomberjam.Client.Colyseus
{
    public class Connection : EventBasedClientWebSocket
    {
        private readonly Queue<byte[]> _queuedUnsentMessages;

        public Connection(Uri uri) : base(uri)
        {
            this._queuedUnsentMessages = new Queue<byte[]>();
            this.OnOpen += OnOpened;
        }

        public Task SendAsync(object[] data)
        {
            byte[] packedData;
            using (var serializationOutput = new MemoryStream())
            {
                MsgPack.Serialize(data, serializationOutput);
                packedData = serializationOutput.ToArray();
            }

            if (this.IsOpened)
            {
                return base.SendAsync(packedData);
            }

            this._queuedUnsentMessages.Enqueue(packedData);
            return Task.CompletedTask;
        }

        private async void OnOpened(object sender, EventArgs e)
        {
            while (this._queuedUnsentMessages.Count > 0)
            {
                await this.SendAsync(this._queuedUnsentMessages.Dequeue());
            }
        }
    }
}