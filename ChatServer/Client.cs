using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        PacketReader packetReader;
        public Client(TcpClient tcpClient)
        {
            ClientSocket = tcpClient;
            UID = Guid.NewGuid();
            packetReader = new PacketReader(ClientSocket.GetStream());

            var opCode = packetReader.ReadByte();
            Username = packetReader.ReadMessage();

            Console.WriteLine($"[{ DateTime.Now}]: Client connected: Username is {Username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            while(true)
            {
                try
                {
                    var opCode = packetReader.ReadByte();
                    switch(opCode)
                    {
                        case 5:
                            var msg = packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message Received! {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                            break;
                        default:
                            break;
                    }
                }
                catch(Exception)
                {
                    Console.WriteLine($"[{UID.ToString()}]:[{Username}]: Disconnected!");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;

                }
            }
        }
    }
}
