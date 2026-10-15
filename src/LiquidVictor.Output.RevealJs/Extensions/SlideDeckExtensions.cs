using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using LiquidVictor.Extensions;
using QRCoder;

namespace LiquidVictor.Output.RevealJs.Extensions;

public static class SlideDeckExtensions
{
    const int _qrCodeImageSize = 800;
    static readonly byte[] _pngSignature = [0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a];

    public static Slide CreateTitleSlide(this SlideDeck slideDeck)
    {
        ArgumentNullException.ThrowIfNull(slideDeck);

        var titleSlide = new Slide()
        {
            Id = Guid.NewGuid(),
            Title = slideDeck.Title,
            Layout = Enumerations.Layout.Title
        };

        titleSlide.ContentItems.Add(
            new KeyValuePair<int, ContentItem>(1,
            new ContentItem()
            {
                Content = slideDeck.SubTitle.AsByteArray(),
                ContentType = "text/plain",
                Id = Guid.NewGuid()
            }));

        titleSlide.ContentItems.Add(
            new KeyValuePair<int, ContentItem>(2,
            new ContentItem()
            {
                Content = slideDeck.Presenter.AsByteArray(),
                ContentType = "text/plain",
                Id = Guid.NewGuid()
            }));

        string url = slideDeck.SlideDeckUrl?.ToString() ?? "about:blank";
        titleSlide.ContentItems.Add(
            new KeyValuePair<int, ContentItem>(3,
            new ContentItem()
            {
                Content = url.AsByteArray(),
                ContentType = "text/plain",
                Id = Guid.NewGuid()
            }));

        string linkText = slideDeck.PrintLinkText ?? " ";
        titleSlide.ContentItems.Add(
            new KeyValuePair<int, ContentItem>(3,
            new ContentItem()
            {
                Content = linkText.AsByteArray(),
                ContentType = "text/plain",
                Id = Guid.NewGuid()
            }));

        return titleSlide;
    }

    public static Slide CreateResourcesSlide(this SlideDeck slideDeck)
    {
        ArgumentNullException.ThrowIfNull(slideDeck);

        var resourcesSlide = new Slide()
        {
            Id = Guid.NewGuid(),
            Title = "Resources",
            Layout = Enumerations.Layout.ImageRight
        };

        bool hasTypedResources = slideDeck.Resources.Any(resource => resource.Type != "Other");
        var content = new StringBuilder();
        foreach (var group in slideDeck.Resources.GroupBy(resource => resource.Type))
        {
            if (hasTypedResources)
            {
                if (content.Length > 0)
                    content.AppendLine();
                content.Append("**").Append(System.Net.WebUtility.HtmlEncode(group.Key)).AppendLine("**");
            }

            foreach (var resource in group)
            {
                content.Append("- <a href=\"")
                    .Append(System.Net.WebUtility.HtmlEncode(resource.Url))
                    .Append("\">")
                    .Append(System.Net.WebUtility.HtmlEncode(resource.Name))
                    .AppendLine("</a>");
            }
        }

        resourcesSlide.ContentItems.Add(new KeyValuePair<int, ContentItem>(0, new ContentItem
        {
            Id = Guid.NewGuid(),
            Content = content.ToString().AsByteArray(),
            ContentType = "text/markdown"
        }));

        if (slideDeck.SlideDeckUrl is not null)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(slideDeck.SlideDeckUrl.ToString(), QRCodeGenerator.ECCLevel.Q);
            resourcesSlide.ContentItems.Add(new KeyValuePair<int, ContentItem>(1, new ContentItem
            {
                Id = Guid.NewGuid(),
                Content = CreateQrCodePng(qrCodeData),
                ContentType = "image/png",
                FileName = "resources-qr-code.png"
            }));
        }

        return resourcesSlide;
    }

    private static byte[] CreateQrCodePng(QRCodeData qrCodeData)
    {
        var modules = qrCodeData.ModuleMatrix;
        int bytesPerRow = _qrCodeImageSize + 1;
        var scanlines = new byte[bytesPerRow * _qrCodeImageSize];

        for (int y = 0; y < _qrCodeImageSize; y++)
        {
            int rowOffset = y * bytesPerRow;
            scanlines[rowOffset] = 0;
            Array.Fill(scanlines, byte.MaxValue, rowOffset + 1, _qrCodeImageSize);
            var moduleRow = modules[y * modules.Count / _qrCodeImageSize];

            for (int x = 0; x < _qrCodeImageSize; x++)
                if (!moduleRow[x * modules.Count / _qrCodeImageSize])
                    scanlines[rowOffset + x + 1] = 0;
        }

        using var compressedScanlines = new MemoryStream();
        using (var compressor = new ZLibStream(compressedScanlines, CompressionLevel.Optimal, leaveOpen: true))
            compressor.Write(scanlines);

        using var png = new MemoryStream();
        png.Write(_pngSignature);

        var header = new byte[13];
        BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(0, 4), _qrCodeImageSize);
        BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(4, 4), _qrCodeImageSize);
        header[8] = 8;
        header[9] = 0;
        WritePngChunk(png, "IHDR", header);
        WritePngChunk(png, "IDAT", compressedScanlines.ToArray());
        WritePngChunk(png, "IEND", []);

        return png.ToArray();
    }

    private static void WritePngChunk(Stream png, string type, ReadOnlySpan<byte> data)
    {
        var typeBytes = Encoding.ASCII.GetBytes(type);
        Span<byte> chunkLength = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(chunkLength, (uint)data.Length);
        png.Write(chunkLength);
        png.Write(typeBytes);
        png.Write(data);

        uint crc = UpdateCrc32(uint.MaxValue, typeBytes);
        crc = ~UpdateCrc32(crc, data);
        Span<byte> checksum = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(checksum, crc);
        png.Write(checksum);
    }

    private static uint UpdateCrc32(uint crc, ReadOnlySpan<byte> bytes)
    {
        foreach (byte value in bytes)
        {
            crc ^= value;
            for (int bit = 0; bit < 8; bit++)
                crc = (crc & 1) == 0 ? crc >> 1 : (crc >> 1) ^ 0xedb88320;
        }

        return crc;
    }

    public static (int, int) GetPresentationSize(this SlideDeck deck)
    {
        ArgumentNullException.ThrowIfNull(deck);

        int width, height;
        switch (deck.AspectRatio)
        {
            case Enumerations.AspectRatio.Widescreen: // 16:9
                width = 1920;
                height = 1080;
                break;
            case Enumerations.AspectRatio.Standard: // 4:3
                width = 1024;
                height = 768;
                break;
            default:
                throw new NotSupportedException($"Invalid Aspect Ratio {deck.AspectRatio}");
        }

        return (width, height);
    }
}
