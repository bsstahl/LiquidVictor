using System.Globalization;

namespace LVImport;

internal static class ImportArguments
{
    private const string _sourceRepoTypePrefix = "-SOURCEREPOTYPE:";
    private const string _sourceRepoPathPrefix = "-SOURCEREPOPATH:";
    private const string _slidesPrefix = "-SLIDES:";
    private const string _titlePrefix = "-TITLE:";

    internal static ImportOptions Parse(IReadOnlyList<string> args)
    {
        ArgumentNullException.ThrowIfNull(args);

        var options = new ImportOptions();
        foreach (var arg in args)
        {
            var upperArg = arg.ToUpperInvariant();
            if (upperArg is "--HELP" or "-H" or "-?" or "/?")
                options.ShowHelp = true;
            else if (upperArg == "--SKIPOUTPUT")
                options.SkipOutput = true;
            else if (upperArg == "--VERBOSE")
                options.Verbose = true;
            else if (upperArg.StartsWith(_sourceRepoTypePrefix, StringComparison.Ordinal))
                options.SourceRepoType = arg[_sourceRepoTypePrefix.Length..].Trim();
            else if (upperArg.StartsWith(_sourceRepoPathPrefix, StringComparison.Ordinal))
                options.SourceRepoPath = arg[_sourceRepoPathPrefix.Length..].Trim();
            else if (upperArg.StartsWith(_slidesPrefix, StringComparison.Ordinal))
                options.SlideNumbers = ParseSlideNumbers(arg[_slidesPrefix.Length..]);
            else if (upperArg.StartsWith(_titlePrefix, StringComparison.Ordinal))
                options.Title = arg[_titlePrefix.Length..].Trim();
            else if (arg.StartsWith('-'))
                throw new ArgumentException($"Unknown parameter '{arg}'", nameof(args));
            else if (string.IsNullOrWhiteSpace(options.PowerpointPath))
                options.PowerpointPath = arg.Trim();
            else
                throw new ArgumentException($"Only one PowerPoint file may be specified but '{options.PowerpointPath}' and '{arg}' were both provided", nameof(args));
        }

        if (!options.ShowHelp)
            Validate(options);

        return options;
    }

    internal static IReadOnlyCollection<int> ParseSlideNumbers(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var result = new SortedSet<int>();
        var entries = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var entry in entries)
        {
            var bounds = entry.Split('-', StringSplitOptions.TrimEntries);
            if (bounds.Length > 2)
                throw new ArgumentException($"Invalid slide range '{entry}'", nameof(value));

            var first = ParseSlideNumber(bounds[0], entry);
            var last = bounds.Length == 2 ? ParseSlideNumber(bounds[1], entry) : first;
            if (last < first)
                throw new ArgumentException($"Invalid slide range '{entry}'; the range must be ascending", nameof(value));

            for (var i = first; i <= last; i++)
                result.Add(i);
        }

        return result.Count == 0
            ? throw new ArgumentException("At least one slide number must be specified with -Slides", nameof(value))
            : result;
    }

    private static int ParseSlideNumber(string value, string entry)
    {
        return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var number) && number > 0
            ? number
            : throw new ArgumentException($"Invalid slide number in '{entry}'; slide numbers must be positive integers", nameof(value));
    }

    private static void Validate(ImportOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.PowerpointPath))
            throw new ArgumentException("The path to a PowerPoint (.pptx) file must be specified");

        if (!WriteRepositoryFactory.IsSupported(options.SourceRepoType))
            throw new ArgumentException($"Invalid Source Repository Type '{options.SourceRepoType}'");

        if (!options.SkipOutput && string.IsNullOrWhiteSpace(options.SourceRepoPath))
            throw new ArgumentException("A target repository must be specified using -SourceRepoPath (or use --SkipOutput)");
    }
}
