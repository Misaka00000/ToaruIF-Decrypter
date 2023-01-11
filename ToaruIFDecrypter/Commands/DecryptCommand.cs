using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using ToaruIFDecrypter.Encryption;
using ToaruIFDecrypter.Utils;

namespace ToaruIFDecrypter.Commands
{
    internal class DecryptCommand
    {
        public static Command Command
        {
            get
            {
                var command = new Command("decrypt", "Decrypt AssetBundles")
                {
                    new Option<DirectoryInfo>(
                        new [] { "--input-dir", "-i" },
                        "Input directory") {
                        IsRequired = true
                    },
                    new Option<DirectoryInfo>(
                        new [] { "--output-dir", "-o" },
                        getDefaultValue: () => new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "out")),
                        "Output directory"),
                    new Option<FileInfo>(
                        new [] { "--keys-file", "-k" },
                        getDefaultValue: () => new FileInfo(Path.Combine(Environment.CurrentDirectory, "keys.bin")),
                        "Keys file"
                        )
                };
                command.AddAlias("d");

                command.Handler = CommandHandler.Create<DirectoryInfo, DirectoryInfo, FileInfo>(Handler);

                return command;
            }
        }

        private static void Handler(DirectoryInfo inputDir, DirectoryInfo outputDir, FileInfo keysFile)
        {
            if (!inputDir.Exists)
            {
                ConsoleEx.Error("Input directory not found");
                return;
            }
            if (!keysFile.Exists)
            {
                ConsoleEx.Error("Keys file not found");
                return;
            }
            if (!outputDir.Exists)
                outputDir.Create();

            var factory = new DecrypterFactory(keysFile);

            ProcessFiles(inputDir, outputDir, factory);
            ConsoleEx.Read();

            //var files = inputDir.GetFiles();
            //for (var i = 0; i < files.Length; i++)
            //{
            //    var file = files[i];

            //    ConsoleEx.Write($"\"{file.FullName}\"");

            //    using (var fs = file.OpenRead())
            //    {
            //        var decrypter = factory.GetDecrypter(fs);
            //        if (decrypter != null)
            //        {
            //            var fileOutputPath = Path.Combine(outputDir.FullName, file.FullName.Replace(inputDir.FullName, ""));

            //            ConsoleEx.WriteLine(
            //                ($"[{decrypter.GetType().Name}]", ConsoleColor.Green),
            //                ($" -> \"{fileOutputPath}\"", null));

            //            using (var outFile = File.OpenWrite(outputDir.FullName + fileOutputPath))
            //            {
            //                decrypter.Decrypt(fs, outFile);
            //            }
            //        }
            //        else
            //        {
            //            ConsoleEx.WriteLine(" [Skip]", ConsoleColor.Red);
            //        }
            //    }
            //}
        }
        private static void ProcessFiles(DirectoryInfo inputDir, DirectoryInfo outputDir, DecrypterFactory factory)
        {
            var files = inputDir.GetFiles();
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];

                ConsoleEx.Write($"\"{file.FullName}\"");

                using (var fs = file.OpenRead())
                {
                    var decrypter = factory.GetDecrypter(fs);
                    if (decrypter != null)
                    {
                        bool Needenc = false;//需要加密标记
                        var fileOutputPath = Path.Combine(outputDir.FullName, file.FullName.Replace(inputDir.FullName, ""));
                        string[] tempstr = fileOutputPath.Split('_');
                        if (tempstr.Length < 2)
                        {
                            //说明是解密，需要将文件名修改
                            Needenc = false;
                        }
                        else
                        {
                            Needenc = true;
                        }
                        if (Needenc is true)
                        {
                            //如果是需要加密的数据，则说明它文件名中带有加密密钥
                            fileOutputPath = tempstr[0];
                            if(tempstr[1].Length != 8)
                            {
                                //文件名有问题
                                Needenc = false;
                                fileOutputPath = tempstr[0];
                            }
                            try
                            {
                                byte[] decBytes = HexStringConverter.ToByteArray(tempstr[1]);
                                int buff_int = BitConverter.ToInt32(decBytes, 0);
                                if (DecrypterFactory.Keys.ContainsKey(buff_int))
                                {
                                    decrypter.SetKey(DecrypterFactory.Keys[buff_int]);
                                }
                                else
                                {
                                    decrypter.SetKey(decBytes);
                                }
                            }
                            catch
                            {
                                //文件名有问题
                                Needenc = false;
                                fileOutputPath = tempstr[0];
                            }
                        }
                        if((decrypter.GetType().Name == "XorDecrypter") && (Needenc is false))
                        {
                            //解密的数据，且为异或解密，需要文件名中带有密钥
                            fileOutputPath += '_' + BitConverter.ToString(decrypter.GetKey().Skip(0).Take(4).ToArray()).Replace("-", "");
                        }
                        
                        ConsoleEx.WriteLine(
                            ($"[{decrypter.GetType().Name}]", ConsoleColor.Green),
                            ($" -> \"{fileOutputPath}\"", null));

                        using (var outFile = File.OpenWrite(outputDir.FullName + fileOutputPath))
                        {
                            decrypter.Decrypt(fs, outFile);
                        }
                    }
                    else
                    {
                        ConsoleEx.WriteLine(" [Skip]", ConsoleColor.Red);
                    }
                }
            }
            var dirs = inputDir.GetDirectories();
            for (var i = 0; i < dirs.Length; i++)
            {
                var dirdeep = dirs[i];
                ProcessFiles(dirs[i], outputDir, factory);
            }
        }
    }
}
