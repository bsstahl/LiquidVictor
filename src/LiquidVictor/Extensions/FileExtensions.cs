using System.Globalization;

namespace LiquidVictor.Extensions;

public static class FileExtensions
{

    public static string GetContentType(this string sourceFilePath)
    {
        string result = string.Empty;
        string cleanExtension = System.IO.Path.GetExtension(sourceFilePath).ToLower(CultureInfo.CurrentCulture);
        switch (cleanExtension)
        {
            case ".md":
                result = "text/markdown";
                break;
            case ".png":
                result = "image/png";
                break;
            case ".jpg":
            case ".jfif":
            case ".jpeg":
                result = "image/jpg";
                break;
            case ".gif":
                result = "image/gif";
                break;
        }

        return result;
    }

}
