using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StalkerModdingHelper
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await new Helper().Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }

    public class Helper
    {
        public async Task Run()
        {
            var config = Static.ReadConfig();
            Static.ValidatePaths(config);
            
            var modDirectories = Static.GetModDirectories(config);
            var copyModTasks = modDirectories.Select(modDirectory => Static.ProcessModDirectory(config, modDirectory));
            await Task.WhenAll(copyModTasks);

            if (Static.IsAutoRunEnabled(config) == false)
                return;

            if (Static.IsStalkerRunning(config))
            {
                Static.CreateTriggerScript(config);
                Static.CreateTriggerFile(config);
            }
            else
            {
                Static.CreateTriggerScript(config);
                Static.StartStalker(config);
            }
        }
    }
}