using System.IO;

namespace ToaruIFDecrypter.Encryption
{
    public interface IDecrypter
    {
        Stream Decrypt(Stream input);

        byte[] GetKey();

        void SetKey(byte[] key);

        void Decrypt(Stream input, Stream output);
    }
}
