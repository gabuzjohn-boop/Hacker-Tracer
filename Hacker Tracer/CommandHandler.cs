using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace Hacker_Tracer
{
    internal class CommandHandler
    {

        private readonly GameState _state;
        private readonly VirtualDirectory _root;
        private VirtualDirectory _current;
        private readonly Stack<VirtualDirectory> _dirStack = new();

        public CommandHandler(GameState state, VirtualDirectory root)
        {
            _state = state;
            _root = root;
            _current = root;
        }

        public bool Handle(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return true;

            var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLower();
            var arg = parts.Length > 1 ? parts[1] : "";

            switch (cmd)
            {
                case "help": CmdHelp(); break;
                case "ls":
                case "dir": CmdLs(); break;
                case "cd": CmdCd(arg); break;
                case "cat":
                case "open":
                case "read": CmdCat(arg); break;
                case "decrypt": ; break;
                case "hack": ; break;
                case "scan": ; break;
                case "connect": ; break;
                case "status": ; break;
                case "clear":
                case "cls": Console.Clear(); break;
                case "exit":
                case "quit":
                    Terminal.TypeLine("\n  Закрытие соединения. Сохраняй анонимность.", 14, ConsoleColor.DarkGray);
                    Thread.Sleep(600);
                    return false;
                default:
                    Terminal.Error($"Неизвестная команда: «{cmd}». Введи  help  для списка команд.");
                    break;
            }
            return true;
        }
        private VirtualFile? FindFile(string name) =>
        _current.Files.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        private void CmdHelp()
        {
            Terminal.PrintBlank();
            Terminal.Print("  ┌─────────────────────────────────────────────────────┐", ConsoleColor.DarkGreen);
            Terminal.Print("  │             СПРАВКА ПО КОМАНДАМ GHOSTNET            │", ConsoleColor.Green);
            Terminal.Print("  ├──────────────────────┬──────────────────────────────┤", ConsoleColor.DarkGreen);
            var cmds = new[]
            {
            ("help",              "показать эту справку"),
            ("ls / dir",          "список файлов в текущей папке"),
            ("cd <папка>",        "зайти в папку  |  cd ..  выйти назад"),
            ("cat <файл>",        "прочитать содержимое файла"),
            ("decrypt <файл>",    "расшифровать зашифрованный файл"),
            ("hack <цель>",       "попытаться взломать цель"),
            ("scan",              "просканировать систему на наличие данных"),
            ("connect <адрес>",   "подключиться к удалённому узлу"),
            ("status",            "показать прогресс, очки, ключи"),
            ("clear / cls",       "очистить экран"),
            ("exit",              "завершить сессию"),
        };
            foreach (var (c, d) in cmds)
                Terminal.Print($"  │  {c,-20}│  {d,-28}│", ConsoleColor.Green);
            Terminal.Print("  └──────────────────────┴──────────────────────────────┘", ConsoleColor.DarkGreen);
            Terminal.PrintBlank();
        }

        private void CmdLs()
        {
            Terminal.PrintBlank();
            Terminal.Print($"  Директория: {_state.CurrentPath}", ConsoleColor.Cyan);
            Terminal.Divider();

            bool any = false;
            foreach (var dir in _current.SubDirs)
            {
                if (dir.IsHidden && !_state.IsDirUnlocked(dir.Name)) continue;
                var locked = dir.IsLocked && !_state.IsDirUnlocked(dir.Name);
                var icon = locked ? "🔒" : "📁";
                var suffix = locked ? " [ЗАПЕРТО]" : "/";
                Terminal.Print($"  {icon} {dir.Name}{suffix}",
                    locked ? ConsoleColor.DarkGray : ConsoleColor.Yellow);
                any = true;
            }
            foreach (var file in _current.Files)
            {
                if (file.IsHidden) continue;
                var icon = file.Type switch
                {
                    FileType.Encrypted => "🔐",
                    FileType.Executable => "⚡",
                    _ => "📄"
                };
                var suffix = file.Type == FileType.Encrypted && !_state.IsFileDecrypted(file.Name)
                    ? " [ЗАШИФРОВАНО]" : "";
                Terminal.Print($"  {icon} {file.Name}{suffix}", ConsoleColor.Green);
                any = true;
            }
            if (!any) Terminal.Info("(папка пуста)");
            Terminal.PrintBlank();
        }

        private void CmdCd(string arg)
        {
            if (string.IsNullOrEmpty(arg)) { Terminal.Error("Использование: cd <папка>"); return; }

            if (arg == "..")
            {
                if (_dirStack.Count == 0) { Terminal.Warn("Уже в корневой директории."); return; }
                _current = _dirStack.Pop();
                // пересобираем путь
                _state.CurrentPath = BuildPath();
                return;
            }

            if (arg == "~" || arg == "/")
            {
                _dirStack.Clear();
                _current = _root;
                _state.CurrentPath = "~";
                return;
            }

            var target = _current.SubDirs.FirstOrDefault(d =>
                d.Name.Equals(arg, StringComparison.OrdinalIgnoreCase));

            if (target == null) { Terminal.Error($"Нет такой папки: {arg}"); return; }

            if (target.IsHidden && !_state.IsDirUnlocked(target.Name))
            {
                Terminal.Error($"Папка «{arg}» не найдена.");
                return;
            }

            if (target.IsLocked && !_state.IsDirUnlocked(target.Name))
            {
                Terminal.Error($"Доступ запрещён: {arg} заперта.");
                Terminal.Warn($"Найди ключ доступа и используй  hack {arg}  для разблокировки.");
                return;
            }

            _dirStack.Push(_current);
            _current = target;
            _state.CurrentPath = BuildPath();
        }

        private string BuildPath()
        {
            var parts = new List<string>();
            foreach (var d in _dirStack) parts.Insert(0, d.Name);
            parts.Add(_current.Name);
            // заменяем имя корня на ~
            if (parts[0] == "~") parts[0] = "~";
            return string.Join("/", parts).Replace("~/", "~/");
        }

        private void CmdCat(string arg)
        {
            if (string.IsNullOrEmpty(arg)) { Terminal.Error("Использование: cat <имя_файла>"); return; }

            var file = FindFile(arg);
            if (file == null) { Terminal.Error($"Файл не найден: {arg}"); return; }

            Terminal.PrintBlank();
            Terminal.Divider('─', 60, ConsoleColor.DarkGreen);
            Terminal.Print($"  ФАЙЛ: {file.Name}", ConsoleColor.Cyan);
            Terminal.Divider('─', 60, ConsoleColor.DarkGreen);

            if (file.Type == FileType.Encrypted && !_state.IsFileDecrypted(file.Name))
            {
                Terminal.Warn("Этот файл зашифрован. Показан шифротекст:");
                Terminal.PrintBlank();
                Terminal.Print(file.EncryptedContent, ConsoleColor.DarkGray);
                Terminal.PrintBlank();
                Terminal.Info($"Используй  decrypt {file.Name}  чтобы расшифровать его.");
            }
            else
            {
                Terminal.PrintBlank();
                var content = _state.IsFileDecrypted(file.Name) ? file.Content : file.Content;
                Terminal.TypeLine(content, 4, ConsoleColor.Green);
            }
            Terminal.Divider('─', 60, ConsoleColor.DarkGreen);
            Terminal.PrintBlank();
        }

       

        
    }
}
