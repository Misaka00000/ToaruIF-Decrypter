using System.IO;

namespace ToaruIFDecrypter.Encryption
{
    public interface IDecrypter
    {
        Stream Decrypt(Stream input);

        void Decrypt(Stream input, Stream output);
    }
}
