using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public static class SocketHelper
    {
        public static void Listen(Socket listener)
        {
            bool keepgoing = true;
            while (keepgoing)
            {
                try
                {
                    Socket handler = listener.Accept();
                    Console.WriteLine($"{((IPEndPoint)handler.RemoteEndPoint).Address.ToString()} connecté.");
                    Global.Clients.Add(handler);

                    Thread myNewThread = new Thread(() => SocketHelper.HandleClient(handler));
                    myNewThread.Start();
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
                while (true)
                {
                    byte[] bytes = new byte[1024];
                    int bytesRec = client.Receive(bytes);
                    data = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    if (String.IsNullOrEmpty(data)) throw new Exception();

                    Console.WriteLine($"{((IPEndPoint)client.RemoteEndPoint).Address.ToString()} : {data.LineFormat()}");
                    byte[] msg = Encoding.UTF8.GetBytes($"echo {data}");

                    client.Send(msg);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                try
                {
                    string ip = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    Global.Clients.Remove(client);

                    Console.WriteLine($"{ip} déconnecté.");
                }
                catch(Exception)
                {

                }
            }
        }
    }
}
