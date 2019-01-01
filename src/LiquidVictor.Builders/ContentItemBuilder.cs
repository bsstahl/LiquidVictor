using System;
using System.Collections.Generic;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Extensions;

namespace LiquidVictor.Builders
{
    public class ContentItemBuilder: ContentItem
    {
        public ContentItem Build()
        {
            return this;
        }

        public new ContentItemBuilder Id(Guid id)
        {
            base.Id = id;
            return this;
        }

        public ContentItemBuilder AssignId()
        {
            return this.Id(Guid.NewGuid());
        }

        public new ContentItemBuilder Title(string title)
        {
            base.Title = title;
            return this;
        }

        public new ContentItemBuilder ContentType(string contentType)
        {
            base.ContentType = contentType;
            return this;
        }

        public new ContentItemBuilder FileName(string fileName)
        {
            base.FileName = fileName;
            return this;
        }

        public new ContentItemBuilder Content(byte[] content)
        {
            base.Content = content;
            return this;
        }

        public new ContentItemBuilder Content(string textContent)
        {
            base.Content = textContent.AsByteArray();
            return this;
        }

        public ContentItemBuilder CreateHTML(string htmlText)
        {
            return this.AssignId()
                .ContentType("text/html")
                .Content(htmlText);
        }

        public ContentItemBuilder CreateMarkdown(string markdown)
        {
            return this.AssignId()
                .ContentType("text/markdown")
                .Content(markdown);
        }

        public ContentItemBuilder CreatePlainText(string plainText)
        {
            return this.AssignId()
                .ContentType("text/plain")
                .Content(plainText);
        }

        public ContentItemBuilder LoadImage(string imagePath)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public ContentItemBuilder CreateJpeg(byte[] image)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public ContentItemBuilder CreateJpeg(string base64EncodedImage)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public ContentItemBuilder CreatePng(byte[] image)
        {
            return this.AssignId()
                .ContentType("image/png")
                .Content(image);
        }

        public ContentItemBuilder CreatePng(string base64EncodedImage)
        {
            return this.CreatePng(base64EncodedImage.FromBase64String());
        }

    }
}
