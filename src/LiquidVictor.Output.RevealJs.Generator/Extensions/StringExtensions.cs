namespace LiquidVictor.Output.RevealJs.Generator.Extensions;

internal static class StringExtensions
{
    internal static void CopyFolder(this string sourcePath, string targetPath)
    {
        if (System.IO.Directory.Exists(sourcePath))
        {
            if (!System.IO.Directory.Exists(targetPath))
                System.IO.Directory.CreateDirectory(targetPath);

            // Copy files
            var sourceFiles = System.IO.Directory.GetFiles(sourcePath);
            foreach (var sourceFile in sourceFiles)
            {
                var fileName = System.IO.Path.GetFileName(sourceFile);
                var destFile = System.IO.Path.Combine(targetPath, fileName);

                System.IO.File.Copy(sourceFile, destFile, true);
            }

            // Copy folders
            var sourceFolders = System.IO.Directory.GetDirectories(sourcePath);
            foreach (var sourceFolder in sourceFolders)
            {
                string childFolder = sourceFolder.Split(System.IO.Path.DirectorySeparatorChar).Last();
                string folderTargetPath = Path.Combine(targetPath, childFolder);
                sourceFolder.CopyFolder(folderTargetPath);
            }
        }
    }

}
