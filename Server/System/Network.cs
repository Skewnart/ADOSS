using Server.System.Cryptography;
using System;
using System.Diagnostics;
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
            try
            {
                client.Send(Encoding.UTF8.GetBytes($"{RSA.publickey}"));
                
                byte[] bytes = new byte[2048];
                int bytesRec = client.Receive(bytes);
                RSAResponse response = RSA.Decrypt(bytes, bytesRec);

                if (String.IsNullOrEmpty(response.Message)) throw new Exception();

                //data = RSA.Decrypt(datarecieved[1]);
                client.Send(RSA.Encrypt(Command.ProcessCommand(client, response.Message, CommandSource.Socket), response.PublicKey));
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
