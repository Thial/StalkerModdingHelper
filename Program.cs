using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StalkerModdingHelper
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            await new Helper().Run();
        }
    }

    public class Helper
    {
        public async Task Run()
        {
            var config = Static.ReadConfig();
            Static.ValidatePaths(config);
            
            var modDirectories = Static.GetModDirectories(config);

            try
            {
                var copyModTasks = modDirectories.Select(modDirectory => Static.ProcessModDirectory(config, modDirectory));
                await Task.WhenAll(copyModTasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }
}