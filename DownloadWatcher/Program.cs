using DownloadWatcher;
using MyEmails;
using System.Diagnostics;

Dictionary<string, string> data = new Dictionary<string, string>();
foreach (var row in File.ReadAllLines(@"C:\\git\key.txt"))
{
    data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
}
CheckProcessIsRunning("FileMover", data[PropertiesEnum.FileMover.ToString()]);
CheckProcessIsRunning("qbittorrent", data[PropertiesEnum.QBit.ToString()]);
FileSystemWatcher watcher = new FileSystemWatcher();
watcher.Path = data[PropertiesEnum.DownloadsPath.ToString()];
watcher.NotifyFilter = NotifyFilters.FileName;
watcher.Filter = "*.jpg";

// Add event handlers.  
watcher.Created += new FileSystemEventHandler(OnChanged);
watcher.IncludeSubdirectories = false;
watcher.EnableRaisingEvents = true;
while (true) { Thread.Sleep(2000); } //infinite loop

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
            StartProcess(data[PropertiesEnum.RestartTV.ToString()], string.Empty, true);
        }
        else if (e.Name.ToLower() == "scanfiles.jpg")
        {
            File.Delete(@$"{data[PropertiesEnum.DownloadsPath.ToString()]}\ScanFiles.jpg");
            StartProcess(data[PropertiesEnum.TVEpisodeChecker.ToString()], string.Empty, true);
        }
        else if (e.Name.ToLower() == "checkprocess.jpg")
        {
            File.Delete(@$"{data[PropertiesEnum.DownloadsPath.ToString()]}\CheckProcess.jpg");
            CheckProcessIsRunning("FileMover", data[PropertiesEnum.FileMover.ToString()]);
            CheckProcessIsRunning("qbittorrent", data[PropertiesEnum.QBit.ToString()]);
        }
        else if (e.Name.ToLower().Contains(".createtvshow"))
        {
            File.Delete(@$"{data[PropertiesEnum.DownloadsPath.ToString()]}\{e.Name}");
            Directory.CreateDirectory(@$"{data[PropertiesEnum.TV.ToString()]}\{e.Name.Remove(e.Name.IndexOf('.'))}");
        }
        else if (e.Name.ToLower() == "screenshot.jpg")
        {
            string imageLocation = @"C:\git\ScreenShot.jpg";
            StartProcess(data[PropertiesEnum.TV.ToString()], $@"savescreenshot {imageLocation}", true);
            Thread.Sleep(1000);
            email.SendEmail("Screenshot", string.Empty, imageLocation);
        }
    }
    catch (Exception ex)
    {
        email.SendEmail($"DownloadWatcher Failed", ex.ToString());
    }
}

static void CheckProcessIsRunning(string process, string location)
{
    Email email = new Email();
    if (!ProcessRunning(process))
    {
        email.SendEmail($"{process} Down Restarting");
        StartProcess(location, string.Empty, false);
        Thread.Sleep(1000);
        if (ProcessRunning(process))
        {
            email.SendEmail($"{process} UP");
        }
        else
        {
            email.SendEmail($"{process} Still Down Check on it");
        }
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
            Console.WriteLine(theprocess.ProcessName);
        }
    }
    return false;
}

static void StartProcess(string processPath, string startInfo, bool wait)
{
    var processStartInfo = new ProcessStartInfo(processPath);
    processStartInfo.CreateNoWindow = true;
    processStartInfo.UseShellExecute = false;
    processStartInfo.Arguments = startInfo;
    using var process = new Process();
    process.StartInfo = processStartInfo;
    process.Start();

    if (wait)
    {
        process.WaitForExit();
    }
}