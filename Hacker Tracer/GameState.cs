using System;
using System.Collections.Generic;
using System.Text;

namespace Hacker_Tracer
{
    public class GameState
    {

        public string CurrentPath { get; set; } = "~";
        public List<string> UnlockedDirs { get; set; } = new();
        public List<string> DecryptedFiles { get; set; } = new();



        public bool IsDirUnlocked(string dirName) => UnlockedDirs.Contains(dirName);

        public bool IsFileDecrypted(string fileName) => DecryptedFiles.Contains(fileName);
    }
}
