using System.Diagnostics;
using MyEmails;

FileSystemWatcher watcher = new FileSystemWatcher();
watcher.Path = @"C:\Users\sneaker\Downloads";

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
while (true) { System.Threading.Thread.Sleep(60000); } //infinite loop

static void OnChanged(object source, FileSystemEventArgs e)
{
    Email email = new Email();
    
    if (e.Name == "RestartTeamViewer.jpg")
    {
        email.SendEmail($"Starting {e.Name}");
        var processStartInfo = new ProcessStartInfo(@"C:\Plex\RestartTeamViewer.bat");
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
        var processStartInfo = new ProcessStartInfo(@"C:\Plex\TVEpisodeChecker\TVEpisodeChecker\bin\Debug\net6.0\TVEpisodeChecker.exe");
        processStartInfo.CreateNoWindow = true;
        processStartInfo.UseShellExecute = false;
        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        process.WaitForExit();
        File.Delete(@"C:\Users\sneaker\Downloads\ScanFiles.jpg");
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
            File.Delete(@"C:\Users\sneaker\Downloads\CheckProcess.jpg");
        }
    }
}