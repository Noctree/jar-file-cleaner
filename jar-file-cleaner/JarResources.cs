namespace JarFileCleaner;

public class JarResources
{
    public string ResourcesPath { get; set; }

    public JarResources(string path)
    {
        ResourcesPath = path;
    }

    public bool IsResource(string relativePath)
    {
        var absolutePath = Path.Combine(ResourcesPath, relativePath);
        return File.Exists(absolutePath) || Directory.Exists(absolutePath);
    }
}