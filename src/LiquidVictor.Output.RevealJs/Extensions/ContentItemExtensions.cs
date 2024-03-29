﻿using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiquidVictor.Output.RevealJs.Extensions
{
    public static class ContentItemExtensions
    {
        public static string AsStartContentItemSlideSection(this ContentItem item)
        {
            var result = "<section ";

            if (!string.IsNullOrWhiteSpace(item.Alignment))
                result += $"class=\"content-{item.Alignment}\"";
            
            result += ">";
            return result;
        }

        public static void AddFromSlide(this List<ContentItem> images, Slide slideValue)
        {
            if (slideValue.BackgroundContent != null)
                images.AddIfNotPresent(slideValue.BackgroundContent);

            // Add additional content item images to images collection
            foreach (var contentItem in slideValue.ContentItems)
                if (contentItem.Value.IsImage())
                    images.AddIfNotPresent(contentItem.Value);
        }

        public static void AddIfNotPresent(this List<ContentItem> items, ContentItem item)
        {
            if (!items.Any(i => i.Id == item.Id))
                items.Add(item);
        }

        public static string RelativePathToImage(this ContentItem item)
        {
            return $"img/{item.Id.ToString()}{System.IO.Path.GetExtension(item.FileName)}";
        }

        public static bool IsText(this ContentItem contentItem)
        {
            return contentItem.ContentType.ToLower().StartsWith("text");
        }

        public static bool IsImage(this ContentItem contentItem)
        {
            return contentItem.ContentType.ToLower().StartsWith("image");
        }

    }
}
