using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
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
                        var fileOutputPath = Path.Combine(outputDir.FullName, file.FullName.Replace(inputDir.FullName, ""));

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
        }
    }
}
