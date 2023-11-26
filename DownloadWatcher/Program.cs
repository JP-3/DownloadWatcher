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
    try
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        foreach (var row in File.ReadAllLines(@"C:\\git\key.txt"))
        {
            data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
        }

        if (e.Name.ToLower() == "restarttv.jpg")
        {
            email.SendEmail($"Starting {e.Name}");
            StartProcess(data[PropertiesEnum.RestartTV.ToString()], true);
            email.SendEmail($"Finished {e.Name}");
        }
        else if (e.Name.ToLower() == "scanfiles.jpg")
        {
            email.SendEmail($"Starting {e.Name}");
            StartProcess(data[PropertiesEnum.TVEpisodeChecker.ToString()], true);
            File.Delete(@$"{data[PropertiesEnum.DownloadsPath.ToString()]}\ScanFiles.jpg");
            email.SendEmail($"Finished {e.Name}");
        }
        else if (e.Name.ToLower() == "checkprocess.jpg")
        {
            if (ProcessRunning("FileMover"))
            {
                email.SendEmail($"FileMover UP");
            }
            else
            {
                email.SendEmail($"FileMover Down Restarting");
                StartProcess(data[PropertiesEnum.FileMover.ToString()], false);
                Thread.Sleep(1000);
                if (ProcessRunning("FileMover"))
                {
                    email.SendEmail($"FileMover UP");
                }
                else
                {
                    email.SendEmail($"FileMover Still Down Check on it");
                }
            }
            File.Delete(@$"{data[PropertiesEnum.DownloadsPath.ToString()]}\CheckProcess.jpg");
        }
    }
    catch (Exception ex)
    {
        email.SendEmail($"DownloadWatcher Failed", ex.ToString());
    }
}
static bool ProcessRunning(string process)
{
    Process[] processlist = Process.GetProcesses();
    foreach (Process theprocess in processlist)
    {
        if (theprocess.ProcessName == process)
        {
            return true;
        }
    }
    return false;
}

static void StartProcess(string processPath, bool wait)
{
    var processStartInfo = new ProcessStartInfo(processPath);
    processStartInfo.CreateNoWindow = true;
    processStartInfo.UseShellExecute = false;
    using var process = new Process();
    process.StartInfo = processStartInfo;
    process.Start();

    if (wait)
    {
        process.WaitForExit();
    }
}