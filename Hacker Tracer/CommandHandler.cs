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
                case "decrypt": CmdDecrypt(arg); break;
                case "hack": CmdHack(arg); break;
                case "scan": CmdScan(); break;
                case "connect": CmdConnect(arg); break;
                case "status": CmdStatus(); break;
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

        private void CmdDecrypt(string arg)
        {
            if (string.IsNullOrEmpty(arg)) { Terminal.Error("Использование: decrypt <имя_файла>"); return; }

            var file = FindFile(arg);
            if (file == null) { Terminal.Error($"Файл не найден: {arg}"); return; }
            if (file.Type != FileType.Encrypted) { Terminal.Warn("Этот файл не зашифрован."); return; }
            if (_state.IsFileDecrypted(file.Name))
            {
                Terminal.Info("Уже расшифрован. Используй  cat  чтобы прочитать его.");
                return;
            }

            Terminal.PrintBlank();
            Terminal.Info($"Попытка расшифровки: {file.Name}");
            Terminal.Info("Шифр: подстановочный шифр Цезаря");
            Terminal.PrintBlank();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("  Введи значение сдвига (1-25): ");
            Console.ForegroundColor = ConsoleColor.White;
            var input = Console.ReadLine()?.Trim() ?? "";
            Console.ForegroundColor = ConsoleColor.Green;

            if (!int.TryParse(input, out int shift) || shift < 1 || shift > 25)
            {
                Terminal.Error("Неверный сдвиг. Введи число от 1 до 25.");
                return;
            }

            Terminal.ProgressBar("Расшифровка", 15, 40);

            if (shift == file.CaesarShift)
            {
                _state.MarkDecrypted(file.Name);
                Terminal.Glitch(2);
                Terminal.PrintBlank();
                Terminal.Success("Расшифровка успешна!");
                Terminal.PrintBlank();
                Terminal.TypeLine(file.Content, 6, ConsoleColor.Green);
                HarvestKeys(file.Content);
                CheckLevelUp();
            }
            else
            {
                Terminal.PrintBlank();
                Terminal.Error("Расшифровка не удалась. Неверный сдвиг.");
                var garbled = CaesarCipher.Decrypt(file.EncryptedContent, shift);
                Terminal.Print("  Искажённый результат: " + garbled[..Math.Min(60, garbled.Length)] + "...", ConsoleColor.DarkGray);
            }
            Terminal.PrintBlank();
        }

        private void CmdHack(string arg)
        {
            if (string.IsNullOrEmpty(arg)) { Terminal.Error("Использование: hack <цель>"); return; }

            // ищем подходящую запертую папку
            var dir = FindDir(arg);
            if (dir == null) { Terminal.Error($"Цель не найдена: {arg}"); return; }
            if (!dir.IsLocked || _state.IsDirUnlocked(dir.Name))
            {
                Terminal.Warn($"{arg} уже разблокирована."); return;
            }

            Terminal.PrintBlank();
            Terminal.Info($"Запуск взлома: {arg}");
            Terminal.ProgressBar("Сканирование портов", 12, 50);
            Terminal.ProgressBar("Проверка эксплойтов", 10, 55);
            Terminal.PrintBlank();
            Terminal.Print("  Для доступа нужен ключ.", ConsoleColor.Cyan);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("  Введи ключ доступа: ");
            Console.ForegroundColor = ConsoleColor.White;
            var key = Console.ReadLine()?.Trim() ?? "";
            Console.ForegroundColor = ConsoleColor.Green;
            Terminal.PrintBlank();

            if (key.Equals(dir.RequiredKey, StringComparison.OrdinalIgnoreCase))
            {
                Terminal.ProgressBar("Обход системы контроля доступа", 18, 35);
                Terminal.Glitch(1);
                Terminal.Success($"Доступ к /{arg} получен!");
                _state.UnlockDir(dir.Name);
                dir.IsHidden = false; // делаем видимой, если была скрыта
                CheckLevelUp();
            }
            else
            {
                Terminal.Error("Доступ запрещён. Неверный ключ.");
                Terminal.Warn("Продолжай исследовать — ключ скрыт где-то в файлах.");
            }
            Terminal.PrintBlank();
        }
        private void CmdScan()
        {
            Terminal.PrintBlank();
            Terminal.Info("Сканирование текущей системы...");
            Terminal.ProgressBar("Сканирование портов", 20, 30);
            Thread.Sleep(200);
            Terminal.PrintBlank();

            if (_state.Level == GameLevel.Level0_Explore)
            {
                Terminal.Print("  РЕЗУЛЬТАТЫ СКАНИРОВАНИЯ:", ConsoleColor.Cyan);
                Terminal.Print("  ─────────────────────────────────────", ConsoleColor.DarkGreen);
                Terminal.Print("  Хост:        localhost (узел GhostNet)", ConsoleColor.Green);
                Terminal.Print("  Открытые порты: 22 (SSH), 80 (HTTP), 443 (HTTPS)", ConsoleColor.Green);
                Terminal.Print("  Пользователи: admin, shadow, root", ConsoleColor.Green);
                Terminal.Print("  Аномалия:    Недавняя загрузка зашифрованного файла", ConsoleColor.Yellow);
                Terminal.Print("  Подсказка:   Проверь /logs для деталей", ConsoleColor.Yellow);
            }
            else if (_state.Level >= GameLevel.Level2_Decrypted)
            {
                Terminal.Print("  РЕЗУЛЬТАТЫ СКАНИРОВАНИЯ:", ConsoleColor.Cyan);
                Terminal.Print("  ─────────────────────────────────────", ConsoleColor.DarkGreen);
                Terminal.Print("  Хост:        10.31.0.7 (NEXUS Corp)", ConsoleColor.Green);
                Terminal.Print("  Открытые порты: 7, 22, 443, 8443", ConsoleColor.Green);
                Terminal.Print("  Сервисы:     SSH, HTTPS, внутренняя БД", ConsoleColor.Green);
                Terminal.Print("  ПРИМЕЧАНИЕ:  Порт 7 выглядит значимым...", ConsoleColor.Yellow);
                Terminal.Print("  Подсказка:   Проверь /nexus/backups для информации о ключе", ConsoleColor.Yellow);
                _state.ScannedHosts.Add("10.31.0.7");
            }
            else
            {
                Terminal.Print("  Ничего нового не обнаружено. Продолжай исследовать файловую систему.", ConsoleColor.DarkGray);
            }
            Terminal.PrintBlank();
        }

        private void CmdConnect(string arg)
        {
            if (string.IsNullOrEmpty(arg)) { Terminal.Error("Использование: connect <адрес>"); return; }

            if (arg == "10.31.0.7")
            {
                if (_state.Level < GameLevel.Level2_Decrypted)
                {
                    Terminal.Error("Соединение отклонено: неизвестный адрес.");
                    Terminal.Warn("Сначала расшифруй Протокол Исхода, чтобы получить адрес цели.");
                    return;
                }
                if (_state.ConnectedHosts.Contains(arg))
                {
                    Terminal.Info($"Уже подключён к {arg}.");
                    return;
                }

                Terminal.PrintBlank();
                Terminal.Info($"Подключение к {arg}...");
                Terminal.ProgressBar("Маршрутизация через TOR", 15, 45);
                Terminal.ProgressBar("Установка туннеля", 12, 50);
                Terminal.Glitch(2);
                Terminal.Success($"Подключено к 10.31.0.7 — NEXUS CORP");
                Terminal.Info("Разблокирована новая папка: используй  hack nexus  с ключом из exodus_protocol.enc");
                _state.ConnectedHosts.Add(arg);
                _state.Level = GameLevel.Level3_NexusOpen;
                _state.Score += 150;
                // снимаем скрытость с nexus
                var nexus = _root.SubDirs.FirstOrDefault(d => d.Name == "nexus");
                if (nexus != null) nexus.IsHidden = false;
            }
            else
            {
                Terminal.Error($"Нет маршрута к хосту: {arg}");
            }
            Terminal.PrintBlank();
        }

        private void CmdStatus()
        {
            Terminal.PrintBlank();
            Terminal.Print("  ┌─────────────────────────────────────────────┐", ConsoleColor.DarkGreen);
            Terminal.Print("  │              СТАТУС ОПЕРАТОРА                │", ConsoleColor.Cyan);
            Terminal.Print("  ├─────────────────────────────────────────────┤", ConsoleColor.DarkGreen);
            Terminal.Print($"  │  Уровень: {LevelName(_state.Level),-33}│", ConsoleColor.Green);
            Terminal.Print($"  │  Очки   : {_state.Score,-33}│", ConsoleColor.Green);
            Terminal.Print($"  │  Ключи  : {string.Join(", ", _state.UnlockedKeys).OrNone(),-33}│", ConsoleColor.Green);
            Terminal.Print($"  │  Папки  : {string.Join(", ", _state.UnlockedDirs).OrNone(),-33}│", ConsoleColor.Green);
            Terminal.Print($"  │  Файлы  : {string.Join(", ", _state.DecryptedFiles).OrNone(),-33}│", ConsoleColor.Green);
            Terminal.Print($"  │  Хосты  : {string.Join(", ", _state.ConnectedHosts).OrNone(),-33}│", ConsoleColor.Green);
            Terminal.Print("  └─────────────────────────────────────────────┘", ConsoleColor.DarkGreen);
            Terminal.PrintBlank();
        }

        private VirtualDirectory? FindDir(string name)
        {
            return FindDirRecursive(_root, name);
        }

        private VirtualDirectory? FindDirRecursive(VirtualDirectory dir, string name)
        {
            if (dir.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) return dir;
            foreach (var sub in dir.SubDirs)
            {
                var found = FindDirRecursive(sub, name);
                if (found != null) return found;
            }
            return null;
        }

        private void HarvestKeys(string content)
        {
            // автоматически находим ключи, упомянутые в содержимом файла
            var patterns = new Dictionary<string, string>
        {
            { "r00tK3y",          "r00tK3y"         },
            { "vault_unlock_key=","r00tK3y"         },
            { "Gh0stPr0t0c0l",    "Gh0stPr0t0c0l"   },
        };
            foreach (var (pattern, key) in patterns)
            {
                if (content.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    if (!_state.HasKey(key))
                    {
                        _state.AddKey(key);
                        Terminal.PrintBlank();
                        Terminal.Success($"Получен ключ: {key}");
                    }
                }
            }
        }

        private void CheckLevelUp()
        {
            if (_state.Level == GameLevel.Level0_Explore && _state.IsDirUnlocked("vault"))
                _state.Level = GameLevel.Level1_VaultOpen;

            if (_state.Level == GameLevel.Level1_VaultOpen && _state.IsFileDecrypted("exodus_protocol.enc"))
            {
                _state.Level = GameLevel.Level2_Decrypted;
                Terminal.PrintBlank();
                Terminal.Warn("ОБНОВЛЕНИЕ МИССИИ: Ты нашёл адрес цели из Протокола Исхода.");
                Terminal.Info("Используй  connect 10.31.0.7  чтобы продолжить в NEXUS Corp.");
            }

            if (_state.Level >= GameLevel.Level3_NexusOpen && _state.IsFileDecrypted("exodus_final.enc"))
            {
                _state.Level = GameLevel.Level4_Final;
                ShowEnding();
            }
        }

        private void ShowEnding()
        {
            Thread.Sleep(500);
            Console.Clear();
            Terminal.Glitch(5);
            Thread.Sleep(300);

            Terminal.TypeLine("\n\n  ПОЛУЧЕНА ПЕРЕДАЧА...", 20, ConsoleColor.Red);
            Thread.Sleep(600);
            Terminal.TypeLine("\n  Декодирование...", 20, ConsoleColor.DarkGray);
            Terminal.ProgressBar("Обработка сигнала", 25, 30);
            Thread.Sleep(400);
            Console.Clear();

            var ending = @"
  ╔═══════════════════════════════════════════════════════╗
  ║              М И С С И Я   В Ы П О Л Н Е Н А          ║
  ╚═══════════════════════════════════════════════════════╝

  Ты прошёл этот путь.
  Ты взломал шифр.
  Ты нашёл нас.

  Протокол Исхода никогда не был о краже данных.
  Это было испытание для отбора — созданное
  именно для такого, как ты.

  Тени нуждаются в операторах, которые умеют думать.
  Которые умеют адаптироваться. Которые умеют исчезать.

  Ты доказал, на что способен, Призрак.

  ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
  ░  Добро пожаловать в PHANTOM CELL.                ░
  ░  Твой куратор свяжется с тобой в течение 48ч.    ░
  ░  Уничтожь этот терминал после прочтения.         ░
  ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
";
            Terminal.TypeLine(ending, 8, ConsoleColor.Green);

            Terminal.PrintBlank();
            Terminal.Print($"  ИТОГОВЫЙ СЧЁТ: {_state.Score + 500} очков", ConsoleColor.Yellow);
            Terminal.PrintBlank();
            Terminal.TypeLine("  [Нажми ENTER чтобы выйти]", 10, ConsoleColor.DarkGray);
            Console.ReadLine();
            Environment.Exit(0);
        }
        private static string LevelName(GameLevel l) => l switch
        {
            GameLevel.Level0_Explore => "0 — Первичный доступ",
            GameLevel.Level1_VaultOpen => "1 — Хранилище взломано",
            GameLevel.Level2_Decrypted => "2 — Протокол Исхода расшифрован",
            GameLevel.Level3_NexusOpen => "3 — NEXUS Corp инфильтрована",
            GameLevel.Level4_Final => "4 — ЗАВЕРШЕНО",
            _ => "?"
        };
    }

    public static class StringExtensions
    {
        public static string OrNone(this string s) => string.IsNullOrEmpty(s) ? "(нет)" : s;
    }

    public enum GameLevel
    {
        Level0_Explore = 0,   // первичное исследование
        Level1_VaultOpen = 1, // /vault разблокирована
        Level2_Decrypted = 2, // exodus_protocol.enc расшифрован → найден адрес nexus
        Level3_NexusOpen = 3, // /nexus разблокирована
        Level4_Final = 4      // финальный файл расшифрован → игра пройдена
    }
}
