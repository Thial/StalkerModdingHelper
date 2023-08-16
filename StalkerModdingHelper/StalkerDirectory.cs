namespace StalkerModdingHelper
{
    public class StalkerDirectory
    {
        public StalkerDirectory(string path, string[] files)
        {
            Path = path;
            Files = files;
        }
        
        public string Path { get; }
        public string[] Files { get; }
    }
}