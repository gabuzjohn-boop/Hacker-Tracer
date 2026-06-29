using System;
using System.Collections.Generic;
using System.Text;

namespace Hacker_Tracer
{
    internal class Terminal
    {

        public static void SetGreen() => Console.ForegroundColor = ConsoleColor.Green;
        public static void SetRed() => Console.ForegroundColor = ConsoleColor.Red;
        public static void SetYellow() => Console.ForegroundColor = ConsoleColor.Yellow;
        public static void SetCyan() => Console.ForegroundColor = ConsoleColor.Cyan;
        public static void SetWhite() => Console.ForegroundColor = ConsoleColor.White;
        public static void SetGray() => Console.ForegroundColor = ConsoleColor.DarkGray;
        public static void Reset() => Console.ForegroundColor = ConsoleColor.Green;


        public static void Boot()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            SetGreen();

            var banner = @"
  ██████╗ ██╗  ██╗ ██████╗ ███████╗████████╗
 ██╔════╝ ██║  ██║██╔═══██╗██╔════╝╚══██╔══╝
 ██║  ███╗███████║██║   ██║███████╗   ██║   
 ██║   ██║██╔══██║██║   ██║╚════██║   ██║   
 ╚██████╔╝██║  ██║╚██████╔╝███████║   ██║   
  ╚═════╝ ╚═╝  ╚═╝ ╚═════╝ ╚══════╝   ╚═╝  
            С Е Т Е В О Й   Т Е Р М И Н А Л  v2.7";
            SetGreen();
            Console.WriteLine(banner);
            Thread.Sleep(300);

            var bootSteps = new[]
            {
            ("Инициализация модулей ядра",        true),
            ("Загрузка сетевых интерфейсов",       true),
            ("Монтирование зашифрованных томов",   true),
            ("Обход правил файрвола",              true),
            ("Установка анонимного туннеля",       true),
            ("Подмена MAC-адреса",                 true),
            ("Подключение к ретранслятору GhostNet", true),
            ("Отключение системы обнаружения вторжений", true),
            ("Загрузка параметров операции",       true),
        };
    }

        public static void Print(string text, ConsoleColor? color = null)
        {
            var prev = Console.ForegroundColor;
            if (color.HasValue) Console.ForegroundColor = color.Value;
            Console.WriteLine(text);
            Console.ForegroundColor = prev;
        }

        public static void Error(string msg) => Print($"  [!] {msg}", ConsoleColor.Red);
    }
}
