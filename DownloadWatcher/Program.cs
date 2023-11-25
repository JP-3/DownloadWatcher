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
    email.SendEmail($"Starting {e.Name}");
    if (e.Name == "RestartTeamViewer.jpg")
    {
        var processStartInfo = new ProcessStartInfo(@"C:\Plex\RestartTeamViewer.bat");
        processStartInfo.CreateNoWindow = true;
        processStartInfo.UseShellExecute = false;
        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        process.WaitForExit();
    }
    else if (e.Name == "ScanFiles.jpg")
    {
        var processStartInfo = new ProcessStartInfo(@"C:\Plex\TVEpisodeChecker\TVEpisodeChecker\bin\Debug\net6.0\TVEpisodeChecker.exe");
        processStartInfo.CreateNoWindow = true;
        processStartInfo.UseShellExecute = false;
        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        process.WaitForExit();
        File.Delete(@"C:\Users\sneaker\Downloads\ScanFiles.jpg");
    }
    email.SendEmail($"Finished {e.Name}");
}