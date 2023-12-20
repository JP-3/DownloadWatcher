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
        Thread.Sleep(2000);
        Dictionary<string, string> data = new Dictionary<string, string>();
   
        foreach (var row in File.ReadAllLines(@"C:\\git\key.txt"))
        {
            data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
        }

        File.Delete(@$"{data[PropertiesEnum.DownloadsPath.ToString()]}\{e.Name}");

        if (e.Name.ToLower() == "restarttv.jpg")
        {
            File.Delete(@$"{data[PropertiesEnum.DownloadsPath.ToString()]}\{e.Name}");
            StartProcess(data[PropertiesEnum.RestartTV.ToString()], string.Empty, true);
        }
        else if (e.Name.ToLower() == "scanfiles.jpg")
        {
            StartProcess(data[PropertiesEnum.TVEpisodeChecker.ToString()], string.Empty, false);
        }
        else if (e.Name.ToLower() == "checkprocess.jpg")
        {
            CheckProcessIsRunning("FileMover", data[PropertiesEnum.FileMover.ToString()]);
            CheckProcessIsRunning("qbittorrent", data[PropertiesEnum.QBit.ToString()]);
        }
        else if (e.Name.ToLower() == "screenshot.jpg")
        {
            string imageLocation = @"C:\git\ScreenShot.jpg";
            StartProcess(data[PropertiesEnum.NARK.ToString()], $@"savescreenshot {imageLocation}", false);
            Thread.Sleep(1000);
            email.SendEmail("Screenshot", string.Empty, imageLocation);
            File.Delete(imageLocation);
        }
        try
        {
            File.Delete(@$"{data[PropertiesEnum.DownloadsPath.ToString()]}\{e.Name}");
        }
        catch (Exception){}
     
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
    else
    {
        email.SendEmail($"{process} is running");
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

static void StartProcess(string processPath, string startInfo, bool wait)
{
    var processStartInfo = new ProcessStartInfo(processPath);

    processStartInfo.CreateNoWindow = true;
    processStartInfo.UseShellExecute = false;
    processStartInfo.Arguments = startInfo;
    using var process = new Process();
    using (process)
    {


        process.StartInfo = processStartInfo;
        process.Start();

        if (wait)
        {
            process.WaitForExit();
        }
    }
}