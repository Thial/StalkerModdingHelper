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
            
            Console.WriteLine("Done.");

            if (Static.IsAutoRunEnabled(config) == false)
                return;

            if (Static.IsStalkerRunning(config))
            {
                Console.WriteLine("Reloading S.T.A.L.K.E.R.");
                Static.CreateTriggerScript(config);
                Static.CreateTriggerFile(config);
                Static.FocusStalkerWindow(config);
            }
            else
            {
                Console.WriteLine("Launching S.T.A.L.K.E.R.");
                Static.CreateTriggerScript(config);
                Static.StartStalker(config);
                Static.FocusStalkerWindow(config);
            }
        }
    }
}