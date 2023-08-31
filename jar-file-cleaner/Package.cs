namespace JarFileCleaner;

public class Package
{
    private static readonly char[] Delimiter = { '/', '\\' };
    public string FullPath { get; }
    public IReadOnlyList<string> Parts { get; }

    public Package(string name)
    {
        FullPath = name;
        Parts = name.Split('.');
    }

    public bool IsPathToPackage(string path)
    {
        var pathParts = path.Split(Delimiter);
        for (var i = 0; i < pathParts.Length && i < Parts.Count; i++)
        {
            var thisPath = Parts[i];
            var otherPath = pathParts[i];
            if (!thisPath.Equals(otherPath, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }
}