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

            foreach (var (msg, ok) in bootSteps)
            {
                SetGray();
                Console.Write($"  [ .... ] {msg}");
                Thread.Sleep(120 + new Random().Next(80));
                Console.Write("\r");
                if (ok) SetGreen();
                else SetRed();
                Console.WriteLine($"  [  OK  ] {msg}");
            }

            Thread.Sleep(400);
            SetYellow();
            Console.WriteLine();
            TypeLine("  ВНИМАНИЕ: Несанкционированный доступ. Вся активность отслеживается.", 10, ConsoleColor.Red);
            Thread.Sleep(200);
            TypeLine("  (Шутка. Добро пожаловать, Призрак.)", 14, ConsoleColor.DarkGray);
            Thread.Sleep(500);
            Console.WriteLine();
            SetGreen();
            TypeLine("  Введи  help  для списка доступных команд.", 14);
            Console.WriteLine();
            Reset();
        }


        public static void Type(string text, int delayMs = 18, ConsoleColor? color = null)
        {
            var prev = Console.ForegroundColor;
            if (color.HasValue) Console.ForegroundColor = color.Value;
            foreach (char c in text)
            {
                Console.Write(c);
                if (delayMs > 0 && c != ' ')
                    Thread.Sleep(delayMs);
            }
            Console.ForegroundColor = prev;
        }


        public static string? Prompt(string path)
        {
            SetGreen();
            Console.Write($"\n[ПРИЗРАК@даркнет {path}]$ ");
            SetWhite();
            var input = Console.ReadLine();
            Reset();
            return input;
        }

        public static void Print(string text, ConsoleColor? color = null)
        {
            var prev = Console.ForegroundColor;
            if (color.HasValue) Console.ForegroundColor = color.Value;
            Console.WriteLine(text);
            Console.ForegroundColor = prev;
        }

        public static void TypeLine(string text, int delayMs = 18, ConsoleColor? color = null)
        {
            Type(text, delayMs, color);
            Console.WriteLine();
        }

        public static void Box(string title, string body, ConsoleColor borderColor = ConsoleColor.DarkGreen)
        {
            int w = Math.Max(title.Length + 4, 54);
            Print("┌" + new string('─', w) + "┐", borderColor);
            Print($"│  {title.PadRight(w - 2)}│", borderColor);
            Print("├" + new string('─', w) + "┤", borderColor);
            foreach (var line in body.Split('\n'))
                Print($"│  {line.PadRight(w - 2)}│", borderColor);
            Print("└" + new string('─', w) + "┘", borderColor);
        }

        public static void ProgressBar(string label, int steps = 20, int delayMs = 60)
        {
            SetCyan();
            Console.Write($"  {label}: [");
            SetGreen();
            for (int i = 0; i < steps; i++)
            {
                Console.Write("█");
                Thread.Sleep(delayMs);
            }
            SetCyan();
            Console.WriteLine("] ГОТОВО");
            Reset();
        }

        public static void Glitch(int lines = 3)
        {
            var rnd = new Random();
            var chars = "!@#$%^&*<>?/\\|~`";
            for (int i = 0; i < lines; i++)
            {
                SetRed();
                int len = rnd.Next(30, 60);
                for (int j = 0; j < len; j++)
                    Console.Write(chars[rnd.Next(chars.Length)]);
                Console.WriteLine();
                Thread.Sleep(60);
            }
            Reset();
        }

        public static void Divider(char ch = '─', int width = 60, ConsoleColor? color = null)
        {
            Print(new string(ch, width), color ?? ConsoleColor.DarkGreen);
        }

        public static void Error(string msg) => Print($"  [!] {msg}", ConsoleColor.Red);        public static void PrintBlank() => Console.WriteLine();
        public static void Info(string msg) => Print($"  [*] {msg}", ConsoleColor.Cyan);
        public static void Warn(string msg) => Print($"  [~] {msg}", ConsoleColor.Yellow);
        public static void Success(string msg) => Print($"  [+] {msg}", ConsoleColor.Green);
    }
}
