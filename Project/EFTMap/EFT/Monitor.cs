using Microsoft.Win32;

using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;

namespace EFTMap
{
    internal partial class Monitor
    {
        private string? logFilePath;
        private long lastLineCount = 0;

        private bool foundGamePooled = false;
        private bool foundCollectedMemory = false;

        private System.Timers.Timer? timer;

        #region Map List
        private readonly Dictionary<string, string> rcidMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "sandbox_high", "ground-zero" },

            { "factory_day", "factory" },
            { "factory4_day", "factory" },
            { "factory_night", "factory" },
            { "factory4_night", "factory" },

            { "lighthouse", "lighthouse" },

            { "shoreline", "shoreline" },

            { "woods", "woods" },

            { "bigmap", "customs" },

            { "city", "streets" },
            { "tarkovstreets", "streets" },

            { "laboratory", "lab" },

            { "rezerv_base", "reserve" },
            { "rezervbase", "reserve" },

            { "shopping_mall", "interchange" },

            { "Labyrinth", "Labyrinth" },
        };
        #endregion


        [GeneratedRegex(@"rcid:([^.]+)\.scenespreset")]
        private static partial Regex Regex_ScenesPreset();

        public event Action<GameState>? OnGameStateChanged;
        public event Action<string>? OnLog;
        public event Action<string>? OnMapChanged;

        public void StartMonitoring()
        {
            string? installPath = GetTarkovInstallPath();
            if (string.IsNullOrEmpty(installPath)) return;

            string logsPath = Path.Combine(installPath, "Logs");
            string? latestLogFolder = GetLatestLogFolderPath(logsPath);
            if (string.IsNullOrEmpty(latestLogFolder)) return;

            string? logFile = Directory.GetFiles(latestLogFolder, "*application.log*").FirstOrDefault();
            if (string.IsNullOrEmpty(logFile)) return;

            #if DEBUG
            if (!File.Exists(logFile))
            {
                Debug.WriteLine($"로그 파일이 없음: {logFile}");
                return;
            }
            #endif

            this.logFilePath = logFile;
            lastLineCount = GetSafeLineCount(logFile);

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = true;
            timer.Start();
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (logFilePath == null) return;

            try
            {
                List<string> newLines = [];

                #region 라인 확인
                using (FileStream fs = new(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new(fs))
                {
                    int currentLine = 0;
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine() ?? "";
                        if (currentLine >= lastLineCount)
                            newLines.Add(line);
                        currentLine++;
                    }

                    lastLineCount = currentLine;
                }
                #endregion

                foreach (var line in newLines)
                {
                    OnLog?.Invoke(line);

                    if (line.Contains("scene preset path:", StringComparison.OrdinalIgnoreCase))
                    {
                        var match = Regex_ScenesPreset().Match(line.ToLower());
                        if (match.Success)
                        {
                            string rcid = match.Groups[1].Value;
                            string mapName = rcidMap.TryGetValue(rcid, out var mapped) ? mapped : rcid;
                            OnMapChanged?.Invoke(mapName);
                        }
                    }


                    if (!foundGamePooled && line.Contains("Info|application|GamePooled"))
                        foundGamePooled = true;

                    if (!foundCollectedMemory && line.Contains("Debug|application|collectedMemoryGB"))
                        foundCollectedMemory = true;

                    if (line.Contains("application|SelectProfile ProfileId:"))
                    {
                        ResetFlags();
                        OnGameStateChanged?.Invoke(GameState.End);
                    }

                    if (foundGamePooled && foundCollectedMemory)
                    {
                        ResetFlags();
                        OnGameStateChanged?.Invoke(GameState.Start);
                    }
                }
            }
            //catch (IOException ex)
            catch (IOException)
            {
                //Debug.WriteLine("파일 접근 오류: " + ex.Message);
                OnGameStateChanged?.Invoke(GameState.Error);
            }
        }

        private static int GetSafeLineCount(string path)
        {
            int lineCount = 0;
            try
            {
                using FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader sr = new(fs);
                while (sr.ReadLine() != null)
                {
                    lineCount++;
                }
            }
            //catch (IOException ex)
            catch (IOException)
            {
                //Debug.WriteLine("파일 줄 수 계산 중 오류: " + ex.Message);
            }

            return lineCount;
        }



        private void ResetFlags()
        {
            foundGamePooled = false;
            foundCollectedMemory = false;
        }

        public enum GameState
        {
            Start,
            End,
            Error
        }

        internal static string? GetTarkovInstallPath()
        {
            string reg = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\EscapeFromTarkov";
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(reg);
            if (key != null)
            {
                object? value = key.GetValue("InstallLocation");
                if (value != null)
                    return value.ToString();
            }
            return null;
        }

        internal static string? GetLatestLogFolderPath(string path)
        {
            if (!Directory.Exists(path)) return null;

            DirectoryInfo? latest = new DirectoryInfo(path)
                .GetDirectories()
                .OrderByDescending(dir => dir.LastWriteTime)
                .FirstOrDefault();

            return latest?.FullName;
        }
    }
}
