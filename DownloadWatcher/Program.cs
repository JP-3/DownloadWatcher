using DownloadWatcher;
using MyEmails;
using System.Diagnostics;

Dictionary<string, string> data = new Dictionary<string, string>();
foreach (var row in File.ReadAllLines(@"C:\\git\key.txt"))
{
    data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
}

FileSystemWatcher watcher = new FileSystemWatcher();
watcher.Path = data[PropertiesEnum.DownloadsPath.ToString()];

// Watch for all changes specified in the NotifyFilters  
//enumeration.  
watcher.NotifyFilter = NotifyFilters.Attributes |
NotifyFilters.CreationTime |
NotifyFilters.DirectoryName |
NotifyFilters.FileName |
NotifyFilters.LastAccess |
NotifyFilters.LastWrite |
NotifyFilters.Security |
NotifyFilters.Size;
watcher.Filter = "*.jpg";

// Add event handlers.  
watcher.Created += new FileSystemEventHandler(OnChanged);
watcher.IncludeSubdirectories = false;
watcher.EnableRaisingEvents = true;
while (true) { Thread.Sleep(60000); } //infinite loop

static void OnChanged(object source, FileSystemEventArgs e)
{
    Email email = new Email();

    Dictionary<string, string> data = new Dictionary<string, string>();
    foreach (var row in File.ReadAllLines(@"C:\\git\key.txt"))
    {
        data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
    }

    if (e.Name == "RestartTV.jpg")
    {
        email.SendEmail($"Starting {e.Name}");
        var processStartInfo = new ProcessStartInfo(data[PropertiesEnum.RestartTV.ToString()]);
        processStartInfo.CreateNoWindow = true;
        processStartInfo.UseShellExecute = false;
        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        process.WaitForExit();
        email.SendEmail($"Finished {e.Name}");
    }
    else if (e.Name == "ScanFiles.jpg")
    {
        email.SendEmail($"Starting {e.Name}");
        var processStartInfo = new ProcessStartInfo(data[PropertiesEnum.TVEpisodeChecker.ToString()]);
        processStartInfo.CreateNoWindow = true;
        processStartInfo.UseShellExecute = false;
        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        process.WaitForExit();
        File.Delete(@$"{data[PropertiesEnum.DownloadsPath.ToString()]}\ScanFiles.jpg");
        email.SendEmail($"Finished {e.Name}");
    }
    else if (e.Name == "CheckProcess.jpg")
    {
        Process[] processlist = Process.GetProcesses();
        foreach (Process theprocess in processlist)
        {
            if (theprocess.ProcessName == "FileMover")
            {
                email.SendEmail($"FileMover UP");
            }
            File.Delete(@$"{data[PropertiesEnum.DownloadsPath.ToString()]}\CheckProcess.jpg");
        }
    }
}