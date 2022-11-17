using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class Program
    {
        static List<Client> clients;
        static TcpListener tcpListener;
        static void Main(string[] args)
        {
            clients = new List<Client>();
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            tcpListener.Start();

            while (true)
            {
                var client = new Client(tcpListener.AcceptTcpClient());
                clients.Add(client);

                BroadcastConnecttion();
            }
        }

        static void BroadcastConnecttion()
        {
            foreach(var user in clients)
            {
                foreach(var usr in clients)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }
        public static void BroadcastMessage(string message)
        {
            foreach (var user in clients)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string UID)
        {
            var disconnectedUser = clients.Where(x => x.UID.ToString() == UID).FirstOrDefault();
            clients.Remove(disconnectedUser);
            foreach (var user in clients)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(UID);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            BroadcastMessage($"[{disconnectedUser.Username}]: Disconnected!");
        }
    }
}
