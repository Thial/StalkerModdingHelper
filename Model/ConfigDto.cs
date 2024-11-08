namespace StalkerModdingHelper.Model;

public class ConfigDto
{
    public LaunchType LaunchType { get; set; }
    public string LaunchPath { get; set; }
    public string SaveName { get; set; }
    public bool AutoRun { get; set; }
    public string ExecutableName { get; set; }
    public string ProfileName { get; set; }
    public IList<ConfigModDto> Mods { get; set; } = new List<ConfigModDto>();
}