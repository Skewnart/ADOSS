namespace Client
{
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