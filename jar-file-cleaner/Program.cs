using System.Reflection;
using System.Text.RegularExpressions;
using CommandLine;
using static JarFileCleaner.Logger;

[assembly:AssemblyCopyright("Copyright © Matyáš Holub 2023")]

namespace JarFileCleaner;

public static class Program
{
    const string PackagesRegex = """^([a-zA-Z_][a-zA-Z0-9_]*(\.[a-zA-Z_][a-zA-Z0-9_]*)*)(,[a-zA-Z_][a-zA-Z0-9_]*(\.[a-zA-Z_][a-zA-Z0-9_]*)*)*$""";
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments(static () => new Options(), args).WithParsed(Execute);
    }

    private static void Execute(Options opts)
    {
        Verbose = opts.Verbose;
        Info($"Settings - {opts}");
        if (!ValidateSettings(opts))
            return;

        var packages = opts
            .Packages
            .Split(opts.PackageListDelimiter, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(p => new Package(p))
            .ToList();
        var resources = new JarResources(opts.ResourcesFolder!);
        if (!JarCleaner.TryUnpack(opts.Input, out var unpackedJar))
        {
            return;
        }
        JarCleaner.Clean(unpackedJar, packages, resources);
        if (!JarCleaner.TryRepack(unpackedJar, out var repackedJar))
        {
            return;
        }

        Info("Moving JAR to destination...");
        var destination = string.IsNullOrEmpty(opts.Output) ? opts.Input : opts.Output;
        File.Move(repackedJar, destination, true);
        Info("Cleaning temporary files...");
        Directory.Delete(unpackedJar, true);
        Info($"Output JAR saved to {destination}");
        Info("Done!");
        if (opts.Pause)
        {
            Console.WriteLine("Paused! Press enter to quit");
            Console.ReadLine();
        }
    }

    private static bool ValidateSettings(Options opts)
    {
        opts.Input = opts.Input.Replace("\"", string.Empty, StringComparison.Ordinal);
        opts.ResourcesFolder = opts.ResourcesFolder?.Replace("\"", string.Empty, StringComparison.Ordinal);
        if (!File.Exists(opts.Input))
        {
            Error($"Input file does not exist - {opts.Input}");
            return false;
        }
        if (opts.ResourcesFolder is not null && !Directory.Exists(opts.ResourcesFolder))
        {
            Error($"Jar Resources folder does not exist - {opts.ResourcesFolder}");
            return false;
        }
        if (!Regex.IsMatch(opts.Packages, PackagesRegex))
        {
            Error($"Invalid package list - {opts.Packages}");
            return false;
        }

        opts.Output = Path.ChangeExtension(opts.Output, ".jar");
        return true;
    }
    public record Options
    {
        public char PackageListDelimiter { get; } = ',';
        
        [Option('i', "input", HelpText = "The input JAR file", Required = true)]
        public string Input { get; set; }

        [Option('r', "resources",
            HelpText = "Folder containing the resources, any files contained in this folder won't be removed")]
        public string? ResourcesFolder { get; set; } = null;
        
        [Option('p', "packages", HelpText = "List of packages to preserve, separated by ','", Required = true)]
        public string Packages { get; set; }

        [Option('v', "verbose", HelpText = "More output")]
        public bool Verbose { get; set; } = false;

        [Option('o', "output", HelpText = "The output JAR file, overwrites the original if unspecified")]
        public string? Output { get; set; } = null;
        
        [Option("pause", HelpText = "Pauses execution before end for debugging")]
        public bool Pause { get; set; }
    }
}