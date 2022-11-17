using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    class PacketReader : BinaryReader
    {
        private NetworkStream ns;
        public PacketReader(NetworkStream networkStream) : base(networkStream)
        {
            ns = networkStream;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];
            ns.Read(msgBuffer, 0, length);

            var msg = Encoding.ASCII.GetString(msgBuffer);
            return msg;
        }
    }
}
