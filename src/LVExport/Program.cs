using LiquidVictor.Extensions;

namespace LVExport;

internal class Program
{
    static void Main(string[] _)
    {
        // HACK: Remove hardcoding of dependencies

        var sourcePath = "C:\\s\\r\\LiquidVictorDatabases\\JsonFileSystem\\BSStahl";
        var targetPath = "C:\\s\\r\\LiquidVictorDatabases\\YamlFile\\BSStahl";

        var source = new LiquidVictor.Data.JsonFileSystem.SlideDeckReadRepository(sourcePath);
        var target = new LiquidVictor.Data.YamlFile.SlideDeckWriteRepository(targetPath);

        var slideDecks = source.GetSlideDecks();
        foreach (var slideDeck in slideDecks)
        {
            target.SaveSlideDeck(slideDeck);

            var fullTitle = $"{slideDeck.Title} {slideDeck.SubTitle}".Trim();
            var filename = $"{fullTitle}-{slideDeck.Format}.yaml".Clean();
            Console.WriteLine(filename);
        }
    }
}
