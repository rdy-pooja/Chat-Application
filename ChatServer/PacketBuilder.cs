using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatServer
{
    class PacketBuilder
    {
        MemoryStream memoryStream;
        public PacketBuilder()
        {
            memoryStream = new MemoryStream();
        }

        public void WriteOpCode(byte opCode)
        {
            memoryStream.WriteByte(opCode);
        }

        public void WriteMessage(string msg)
        {
            var msgLength = msg.Length;
            memoryStream.Write(BitConverter.GetBytes(msgLength));
            memoryStream.Write(Encoding.ASCII.GetBytes(msg));
        }

        public byte[] GetPacketBytes()
        {
            return memoryStream.ToArray();
        }
    }
}
