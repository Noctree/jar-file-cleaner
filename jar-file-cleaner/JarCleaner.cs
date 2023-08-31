using System.IO.Compression;
using static JarFileCleaner.Logger;

namespace JarFileCleaner;

public static class JarCleaner
{
    private static readonly string[] ExcludedDirectories = { "META-INF" };
    
    public static bool TryUnpack(string input, out string output)
    {
        output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(output);
        try
        {
            Info("Unpacking jar...");
            ZipFile.ExtractToDirectory(input, output);
        }
        catch (Exception ex)
        {
            Error($"Failed to unpack JAR file, reason: {ex.Message}");
            Directory.Delete(output, true);
            return false;
        }

        return true;
    }

    public static bool TryRepack(string input, out string output)
    {
        output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()) + ".jar";
        try
        {
            Info("Repacking jar...");
            ZipFile.CreateFromDirectory(input, output);
        }
        catch (Exception ex)
        {
            Error($"Failed to repack JAR file, reason: {ex.Message}");
            return false;
        }

        return true;
    }

    public static void Clean(string root, ICollection<Package> packagesToPreserve, JarResources jarResources)
    {
        Info("Cleaning Jar...");
        var stack = new Stack<string>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            ProcessEntry(root, packagesToPreserve, jarResources, current, stack);
        }
    }

    private static void ProcessEntry(string root,
        ICollection<Package> packagesToPreserve,
        JarResources jarResources,
        string current,
        Stack<string> stack)
    {
        foreach (var entry in Directory.GetFileSystemEntries(current))
        {
            var relativeEntry = Path.GetRelativePath(root, entry);
            if (ExcludedDirectories.Contains(relativeEntry))
            {
                continue;
            }

            if (packagesToPreserve.Any(p => p.IsPathToPackage(relativeEntry)))
            {
                if (Directory.Exists(entry))
                    stack.Push(entry);
                continue;
            }

            if (jarResources.IsResource(relativeEntry))
                continue;
            DeleteEntry(entry);
        }
    }

    private static void DeleteEntry(string path)
    {
        Info($"Deleting {path}...");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Directory.Delete(path, true);
        }
    }
}