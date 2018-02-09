using Server.System.Cryptography;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server.System
{
    public static class Network
    {
        public static void Listen(Socket listener)
        {
            bool keepgoing = true;
            while (keepgoing)
            {
                try
                {
                    Socket client = listener.Accept();
                    new Thread(() => HandleClient(client)).Start();
                }
                catch (Exception)
                { keepgoing = false; }
            }
        }

        public static void HandleClient(Socket client)
        {
            string data = null;

            try
            {
                byte[] bytes = new byte[1024];
                int bytesRec = client.Receive(bytes);
                data = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                if (String.IsNullOrEmpty(data)) throw new Exception();
                data = Tornado.Decrypt(data);

                //Console.WriteLine($"{((IPEndPoint)client.RemoteEndPoint).Address.ToString()} : {data.LineFormat()}");
                client.Send(Encoding.UTF8.GetBytes(Tornado.Encrypt(Command.ProcessCommand(client, data, CommandSource.Socket))));
            }
            catch (Exception)
            {
            }
            finally
            {
                try
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
