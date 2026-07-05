using System;
using System.Collections.Generic;
using System.Text;

namespace Hacker_Tracer
{
    public class GameState
    {

        public GameLevel Level { get; set; } = GameLevel.Level0_Explore;
        public int Score { get; set; } = 0;
        public string CurrentPath { get; set; } = "~";
        public List<string> UnlockedDirs { get; set; } = new();
        public List<string> DecryptedFiles { get; set; } = new();
        public List<string> UnlockedKeys { get; set; } = new();
        public List<string> ScannedHosts { get; set; } = new();
        public List<string> ConnectedHosts { get; set; } = new();

        public bool HasKey(string key) =>
        UnlockedKeys.Contains(key, StringComparer.OrdinalIgnoreCase);

        public void AddKey(string key)
        {
            if (!HasKey(key))
            {
                UnlockedKeys.Add(key);
                Score += 50;
            }
        }

        public void UnlockDir(string dirName)
        {
            if (!UnlockedDirs.Contains(dirName))
            {
                UnlockedDirs.Add(dirName);
                Score += 100;
            }
        }

        public void MarkDecrypted(string fileName)
        {
            if (!DecryptedFiles.Contains(fileName))
            {
                DecryptedFiles.Add(fileName);
                Score += 200;
            }
        }

        public bool IsDirUnlocked(string dirName) => UnlockedDirs.Contains(dirName);

        public bool IsFileDecrypted(string fileName) => DecryptedFiles.Contains(fileName);
    }
}
