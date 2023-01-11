using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ToaruIFDecrypter.Utils;

namespace ToaruIFDecrypter.Encryption
{
    public class DecrypterFactory
    {
        public static Dictionary<int, byte[]> Keys;

        public DecrypterFactory(FileInfo keysFile)
        {
            if (!keysFile.Exists)
            {
                ConsoleEx.Error("Keys file not found");
                throw new FileNotFoundException("Keys file not found");
            }

            LoadKeys(keysFile);
        }

        private void LoadKeys(FileInfo keysFile)
        {
            Keys = new Dictionary<int, byte[]>();

            using (var ws = keysFile.OpenRead())
            using (var br = new BinaryReader(ws))
            {
                ws.Seek(0, SeekOrigin.Begin);
                int len = br.ReadInt32();
                for (int i = 0; i < len; i++)
                {
                    int key_sig = br.ReadInt32();
                    int key_len = br.ReadInt32();
                    byte[] key = br.ReadBytes(key_len);
                    Keys.Add(key_sig, key);
                }
            }
        }

        public IDecrypter GetDecrypter(Stream stream)
        {
            byte[] buff = new byte[4];
            stream.Seek(0x07, SeekOrigin.Begin);
            stream.Read(buff, 0, buff.Length);
            if (buff[0] == buff[1] && buff[0] == buff[2] && buff[0] == buff[3])
            {
                return new XorDecrypter(buff);
            }
            if (buff[0] == 0x53 && buff[1] == 0 && buff[2] == 0 && buff[3] == 0)
            {
                buff = new byte[7];
                long pre = -1;
                while (stream.Position + 7 < stream.Length)
                {
                    stream.Read(buff, 0, buff.Length);
                    stream.Seek(1 - buff.Length, SeekOrigin.Current);
                    if (Encoding.ASCII.GetString(buff) == "UnityFS")
                        pre = stream.Position - 1;
                    if (pre > 0 && stream.Position - pre > 1024)
                        return new OffsetDecrypter(pre);
                }
                if (pre > 0)
                    return new OffsetDecrypter(pre);
                return null;
            }

            int buff_int = BitConverter.ToInt32(buff, 0);
            if (Keys.ContainsKey(buff_int))
                return new XorDecrypter(Keys[buff_int]);

            return null;
        }
    }
}
