using System;
using System.Collections.Generic;
using System.Text;

namespace Hacker_Tracer;


public enum FileType { Text, Encrypted, Executable, Directory }
public class VirtualFile {

    public string Name { get; set; } = "";
    public string EncryptedContent { get; set; } = "";
    public string Content { get; set; } = "";
    public FileType Type { get; set; }
    public bool IsHidden { get; set; }

}

public static class FileSystemBuilder
{
    public static VirtualDirectory BuildRoot()
    {
        var root = new VirtualDirectory { Name = "~" };

        // уровень 0
        root.Files.Add(new VirtualFile
        {
            Name = "README.txt",
            Type = FileType.Text,
            Content =
               "GHOSTNET ТЕРМИНАЛ v2.7\n" +
               "----------------------\n" +
               "Ты находишься внутри неизвестной системы.\n" +
               "Твоя миссия: найти Протокол Исхода.\n" +
               "Удачи. Она тебе понадобится.\n\n" +
               "ПОДСКАЗКА: начни с команды  scan  или  ls"
        });

        root.Files.Add(new VirtualFile
        {
            Name = "motd.txt",
            Type = FileType.Text,
            Content =
                "Сообщение дня:\n" +
                "«Чем тише ты становишься, тем больше способен услышать.»\n" +
                "                                         — Kali Linux\n\n" +
                "ВНИМАНИЕ: Несанкционированный доступ запрещён.\n" +
                "(Не правда ли, забавно?)"
        });

        var logs = new VirtualDirectory { Name = "logs" };

        logs.Files.Add(new VirtualFile
        {
            Name = "access.log",
            Type = FileType.Text,
            Content =
                "[2024-03-01 02:17:44] ВХОД   admin      192.168.1.1   OK\n" +
                "[2024-03-01 02:18:03] ВХОД   ???        10.0.0.99     OK\n" +
                "[2024-03-01 02:18:17] ЗАПУСК shadow.exe              OK\n" +
                "[2024-03-01 02:19:01] ЗАГРУЗКА exodus_protocol.enc   OK\n" +
                "[2024-03-01 02:20:55] ВЫХОД  admin                   OK\n\n" +
                "Интересно... кто-то загрузил зашифрованный файл.\n" +
                "Попробуй посмотреть в /vault"
        });

        logs.Files.Add(new VirtualFile
        {
            Name = "error.log",
            Type = FileType.Text,
            Content =
                "[2024-03-01 02:18:10] ОШИБКА  Обнаружен обход аутентификации\n" +
                "[2024-03-01 02:18:10] ОШИБКА  Система противодействия вторжениям offline\n" +
                "[2024-03-01 02:19:45] ВНИМАНИЕ Файл exodus_protocol.enc зашифрован CAESAR-13\n" +
                "[2024-03-01 02:19:45] ВНИМАНИЕ Ключ хранится в /sys/kernel.cfg  (ключ доступа: r00tK3y)\n"
        });

        var sys = new VirtualDirectory { Name = "sys" };

        sys.Files.Add(new VirtualFile
        {
            Name = "sysinfo.txt",
            Type = FileType.Text,
            Content =
                "ОС:        GhostNet 0.9.1-anonymous\n" +
                "Ядро:      darkcore-4.20\n" +
                "Аптайм:    13 дней, 7 часов\n" +
                "Пользователи: 1 активный (ты)\n" +
                "Архитектура: x86_64"
        });

        sys.Files.Add(new VirtualFile
        {
            Name = "kernel.cfg",
            Type = FileType.Text,
            Content =
                "# Конфигурация ядра GhostNet\n" +
                "debug_mode=0\n" +
                "safe_mode=0\n" +
                "vault_unlock_key=r00tK3y\n" +
                "# Этот ключ открывает доступ к /vault\n" +
                "# НЕ передавай этот файл никому."
        });

        root.SubDirs.Add(sys);

        root.SubDirs.Add(logs);

        return root;
    }
}
    public class VirtualDirectory
{
    public string Name { get; set; } = "";
    public bool IsLocked { get; set; }
    public bool IsHidden { get; set; }
    public string RequiredKey { get; set; } = "";
    public List<VirtualFile> Files { get; set; } = new();
    public List<VirtualDirectory> SubDirs { get; set; } = new();
}
