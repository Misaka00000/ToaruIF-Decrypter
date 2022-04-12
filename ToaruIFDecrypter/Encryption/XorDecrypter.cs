using System.IO;

namespace ToaruIFDecrypter.Encryption
{
    class XorDecrypter : IDecrypter
    {
        public byte[] Key { get; private set; }

        public XorDecrypter(byte[] key)
        {
            Key = key;
        }

        public Stream Decrypt(Stream input)
        {
            MemoryStream ms = new MemoryStream((int)input.Length);
            Decrypt(input, ms);
            return ms;
        }

        public void Decrypt(Stream input, Stream output)
        {
            input.Seek(0, SeekOrigin.Begin);

            int len = Key.Length;
            int ptr = len - 7;

            byte[] buffer = new byte[input.Length];
            input.Read(buffer, 0, buffer.Length);

            if (ptr < 0)
            {
                ptr = 0;
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ Key[ptr]);
                ptr++;
                if (ptr == len)
                    ptr = 0;
            }

            output.Write(buffer, 0, buffer.Length);
        }
    }
}
