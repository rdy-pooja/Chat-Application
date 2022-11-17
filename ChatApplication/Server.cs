using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class Server
    {
        TcpClient tcpClient;
        public PacketReader packetReader;

        public event Action connectEvent;
        public event Action msgReceivedEvent;
        public event Action userDisconnected;

        public Server()
        {
            tcpClient = new TcpClient();
        }

        public void ConnectToServer(string Username)
        {
            if(!tcpClient.Connected)
            {
                tcpClient.Connect("127.0.0.1",7891);
                packetReader = new PacketReader(tcpClient.GetStream());
                
                if(!string.IsNullOrEmpty(Username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(Username);
                    tcpClient.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while(true)
                {
                    var opCode = packetReader.ReadByte();
                    switch(opCode)
                    {
                        case 1:
                            connectEvent?.Invoke();
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnected?.Invoke();
                            break;
                        default:
                            Console.WriteLine();
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            tcpClient.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
