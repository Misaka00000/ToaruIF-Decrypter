using System.IO;

namespace ToaruIFDecrypter.Encryption
{
    class OffsetDecrypter : IDecrypter
    {
        public long Offset { get; private set; }

        public OffsetDecrypter(long offset)
        {
            Offset = offset;
        }

        public Stream Decrypt(Stream input)
        {
            MemoryStream ms = new MemoryStream((int)(input.Length - Offset));
            Decrypt(input, ms);
            return ms;
        }

        public void Decrypt(Stream input, Stream output)
        {
            input.Seek(Offset, SeekOrigin.Begin);
            input.CopyTo(output);
        }
    }
}
