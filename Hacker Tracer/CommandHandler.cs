using System;
using System.Collections.Generic;
using System.Text;

namespace Hacker_Tracer
{
    internal class CommandHandler
    {
    

        public bool Handle(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return true;

            var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLower();
            var arg = parts.Length > 1 ? parts[1] : "";

            switch (cmd)
            {
                case "help": break;
                case "ls":
                case "dir": break;
                case "cd": break;
                case "cat":
                case "open":
                case "read": break;
                case "decrypt": ; break;
                case "hack":; break;
                case "scan":; break;
                case "connect":; break;
                case "status":; break;
                case "clear":
                case "cls": Console.Clear(); break;
                case "exit":
                case "quit":
                default:
                    Terminal.Error($"Неизвестная команда: «{cmd}». Введи  help  для списка команд.");
                    break;
            }
            return true;
        }
    }
    }
