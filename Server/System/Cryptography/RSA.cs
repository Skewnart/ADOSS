using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Server.System.Cryptography
{
    public static class RSA
    {
        public static string publickey;
        private static string privatekey;

        public static void LoadKeys()
        {
            if (String.IsNullOrEmpty(publickey) || String.IsNullOrEmpty(privatekey))
            {
                var csp = new RSACryptoServiceProvider(2048);

                var privKey = csp.ExportParameters(true);
                var pubKey = csp.ExportParameters(false);

                var sw = new StringWriter();
                new XmlSerializer(typeof(RSAParameters)).Serialize(sw, pubKey);
                publickey = sw.ToString();

                sw = new StringWriter();
                new XmlSerializer(typeof(RSAParameters)).Serialize(sw, privKey);
                privatekey = sw.ToString();
            }
        }

        public static byte[] Encrypt(string message, string pubKey, bool putPublicKey)
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.GenerateKey();
                myRijndael.GenerateIV();
                string encrypted = Convert.ToBase64String(EncryptStringToBytes((putPublicKey ? $"{publickey};$;{message}" : message), myRijndael.Key, myRijndael.IV));

                string cypherKey = EncryptAsymetricBytesToString(myRijndael.Key, pubKey);
                string cypherIV = EncryptAsymetricBytesToString(myRijndael.IV, pubKey);

                string tosend = $"{cypherKey};$;{cypherIV};$;{encrypted}..";
                return Encoding.UTF8.GetBytes(tosend);
            }
        }

        public static RSAResponse Decrypt(byte[] bytes, int bytesLength)
        {
            string receivedbytes = Encoding.UTF8.GetString(bytes, 0, bytesLength);
            receivedbytes = receivedbytes.Substring(0, receivedbytes.Length - 2);

            string[] received = receivedbytes.Split(new string[] { ";$;" }, StringSplitOptions.None);

            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.Key = DecryptAsymetricBytesFromString(received[0]);
                myRijndael.IV = DecryptAsymetricBytesFromString(received[1]);
                 
                string response = DecryptStringFromBytes(Convert.FromBase64String(received[2]), myRijndael.Key, myRijndael.IV);
                string[] responses = response.Split(new string[] { ";$;" }, StringSplitOptions.None);

                return new RSAResponse(responses.Length > 1 ? responses[0] : null, responses.Length > 1 ? responses[1] : responses[0]);
            }
        }

        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.Key = Key;
                myRijndael.IV = IV;
                
                ICryptoTransform encryptor = myRijndael.CreateEncryptor(myRijndael.Key, myRijndael.IV);
                
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;

        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
        
            string plaintext = null;
            
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;
                
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }

        private static string EncryptAsymetricBytesToString(byte[] bytes, string clientPublicKey)
        {
            var sr = new StringReader(clientPublicKey);
            var xs = new XmlSerializer(typeof(RSAParameters));
            var pubKey = (RSAParameters)xs.Deserialize(sr);

            var csp = new RSACryptoServiceProvider(2048);
            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(pubKey);

            var bytesCypherText = csp.Encrypt(bytes, false);

            return Convert.ToBase64String(bytesCypherText);
        }

        private static byte[] DecryptAsymetricBytesFromString(string message)
        {
            var sr = new StringReader(privatekey);
            var xs = new XmlSerializer(typeof(RSAParameters));
            var privKey = (RSAParameters)xs.Deserialize(sr);

            var bytesCypherText = Convert.FromBase64String(message);
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privKey);
            
            return csp.Decrypt(bytesCypherText, false);
        }
    }

    public class RSAResponse
    {
        public string PublicKey { get; set; }
        public string Message { get; set; }

        public RSAResponse(string publicKey, string message)
        {
            this.PublicKey = publicKey;
            this.Message = message;
        }
    }
}
