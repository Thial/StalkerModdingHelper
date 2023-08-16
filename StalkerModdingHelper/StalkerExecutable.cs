using System.IO;

namespace StalkerModdingHelper;

public class StalkerExecutable
{
    public StalkerExecutable(string path, string name)
    {
        Path = path;
        Name = name;
    }
    
    public string Path { get; }
    public string Name { get; }
}