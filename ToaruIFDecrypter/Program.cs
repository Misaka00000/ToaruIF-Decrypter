using System.CommandLine;

namespace ToaruIFDecrypter
{
    class Program
    {
        private static void Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                Commands.DecryptCommand.Command
            };
            rootCommand.Invoke(args);
        }
    }
}
