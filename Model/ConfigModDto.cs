using System.Collections.Generic;

namespace StalkerModdingHelper.Model;

public class ConfigModDto
{
    public string ModName { get; set; }
    public string ModPath { get; set; }
    public IList<string> SkipExtensions { get; set; } = new List<string>();
}