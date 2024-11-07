using CrimsonLog.Structs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CrimsonLog.Systems;

public static class Logger
{
    public static Task<string> CreateDirectory(string folderName)
    {
        if (!Directory.Exists($"CrimsonLogs/{folderName}"))
        {
            Directory.CreateDirectory($"CrimsonLogs/{folderName}");
        }

        return Task.FromResult($"CrimsonLogs/{folderName}");
    }

    public static async void Record(string folderName, string recordName, string message)
    { 
        string path = await CreateDirectory(folderName);

        string fileName = $"{path}/{recordName}_{DateTime.Now:yyyy-MM-dd}";

        if (!File.Exists(fileName))
        {
            File.Create(fileName).Dispose();
            DeleteOldLogs(path, recordName);
        }

        File.AppendAllText(fileName, $"{Time()} | {message}");
    }

    public static string Time()
    {
        return $"{DateTime.Now:HH:mm.ss}";
    }

    public static void DeleteOldLogs(string folderPath, string prefix)
    {
        int daysAgo = Settings.DaysToKeep.Value;

        if (daysAgo < 0) return;

        string[] files = Directory.GetFiles(folderPath, $"{prefix}*");
        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            if ((DateTime.Now - fileInfo.CreationTime).Days > daysAgo)
            {
                File.Delete(file);
            }
        }
    }
}
