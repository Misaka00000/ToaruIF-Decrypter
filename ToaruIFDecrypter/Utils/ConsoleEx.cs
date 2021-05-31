using System;

namespace ToaruIFDecrypter.Utils
{
    internal class ConsoleEx
    {
        public static void Write(string value) => Console.Write(value);

        public static void Write(string value, ConsoleColor? foregroundColor)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor ?? oldColor;
            Write(value);
            Console.ForegroundColor = oldColor;
        }

        public static void Write(string value, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
        {
            ConsoleColor oldColor = Console.BackgroundColor;
            Console.BackgroundColor = backgroundColor ?? oldColor;
            Write(value, foregroundColor);
            Console.BackgroundColor = oldColor;
        }

        public static void Write(params (string value, ConsoleColor? foregroundColor)[] values)
        {
            foreach (var value in values)
                Write(value.value, value.foregroundColor);
        }

        public static void Write(params (string value, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)[] values)
        {
            foreach (var value in values)
                Write(value.value, value.foregroundColor, value.backgroundColor);
        }

        public static void WriteLine() => Console.WriteLine();
        public static void WriteLine(string value) => Console.WriteLine(value);

        public static void WriteLine(string value, ConsoleColor? foregroundColor)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor ?? oldColor;
            WriteLine(value);
            Console.ForegroundColor = oldColor;
        }

        public static void WriteLine(string value, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
        {
            ConsoleColor oldColor = Console.BackgroundColor;
            Console.BackgroundColor = backgroundColor ?? oldColor;
            WriteLine(value, foregroundColor);
            Console.BackgroundColor = oldColor;
        }

        public static void WriteLine(params (string value, ConsoleColor? foregroundColor)[] values)
        {
            foreach (var value in values)
                Write(value.value, value.foregroundColor);
            WriteLine();
        }

        public static void WriteLine(params (string value, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)[] values)
        {
            foreach (var value in values)
                Write(value.value, value.foregroundColor, value.backgroundColor);
            WriteLine();
        }

        public static void Error(string value) => WriteLine(
            ("[ERROR]", ConsoleColor.White, ConsoleColor.Red), 
            (" " + value, ConsoleColor.Red, null));

        public static void Warn(string value) => WriteLine(
            ("[WARN]", ConsoleColor.White, ConsoleColor.Yellow),
            (" " + value, ConsoleColor.Yellow, null));

        public static bool Ask(string value, bool def)
        {
            Write($"{value} [{(def ? "Y/n" : "y/N")}]: ");
            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Y:
                        WriteLine("y");
                        return true;
                    case ConsoleKey.N:
                        WriteLine("n");
                        return false;
                    case ConsoleKey.Enter:
                        WriteLine(def ? "y" : "n");
                        return def;
                }
            }
        }
    }
}
