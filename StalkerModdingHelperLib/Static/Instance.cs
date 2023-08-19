using System.Diagnostics;
using System.IO;
using System.Linq;
using StalkerModdingHelperLib.Enums;

namespace StalkerModdingHelperLib.Static;

public class Instance
{
    public static Process GetProcess(string processName)
    {
        var processlist = Process.GetProcesses();
        return processlist.FirstOrDefault(p => p.ProcessName == processName);
    }
    
    public static bool IsRunning(string executablePath)
    {
        var fileName = Path.GetFileName(executablePath);
        var name = fileName.TrimEnd(".exe");
        var instanceProcess = GetProcess(name);
        return instanceProcess != null;
    }
    
    public static void Start(string executablePath, InstanceType instanceType, string saveName)
    {
        var instanceProcess = new Process();
        instanceProcess.StartInfo.FileName = executablePath;
        var arguments = instanceType switch
        {
            InstanceType.Stalker => $"-dbg -nocache -cls -start server({saveName}/single/alife/load) client(localhost)",
            InstanceType.ModOrganizer => $""
        };
        instanceProcess.StartInfo.Arguments = arguments;
        instanceProcess.Start();
    }
}