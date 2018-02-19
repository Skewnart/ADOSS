using Server.System.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server.System
{
    public static class Network
    {
        /// <summary>
        /// Listening method.
        /// </summary>
        /// <param name="listener"></param>
        public static void Listen(Socket listener)
        {
            bool keepgoing = true;
            while (keepgoing)
            {
                try
                {
                    //Each client who initiate a connection create a new thread to proceed the request.
                    Socket client = listener.Accept();
                    new Thread(() => HandleClient(client)).Start();
                }
                catch (Exception)
                { keepgoing = false; }
            }
        }

        //Method thread to process the client
        public static void HandleClient(Socket client)
        {
            try
            {
                //First, the server send its public key. The client will use it to converse.
                client.Send(Encoding.UTF8.GetBytes(RSA.publickey));
                
                byte[] bytes = null;
                List<byte> result = new List<byte>();
                int len = 0;
                // The server read the response 2048 bytes by 2048 bits.
                do
                {
                    bytes = new byte[2048];
                    len = client.Receive(bytes);
                    result = result.Concat(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(bytes, 0, len))).ToList();
                } while (!Encoding.UTF8.GetString(bytes, 0, len).Substring(len-2).Equals(".."));
                
                //Decrypt
                RSAResponse response = RSA.Decrypt(result.ToArray(), result.Count);

                if (String.IsNullOrEmpty(response.Message)) throw new Exception();
                //Process the command and send the answer.
                client.Send(RSA.Encrypt(Command.ProcessCommand(client, response.Message, CommandSource.Socket), response.PublicKey, false));
            }
            catch (Exception ex)
            {
            }
            finally
            {
                try
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
