using System;
using Markdig;
using Xunit;
using LiquidVictor.Entities;
using LiquidVictor.Test.Extensions;
using System.Xml.Linq;
using LiquidVictor.Builders;
using System.Diagnostics;
using TestHelperExtensions;

namespace LiquidVictor.Output.RevealJs.Layout.FullPage.Test
{
    public class Engine_Layout_Should
    {
        [Fact]
        public void NotFailIfContentItemCollectionIsNull()
        {
            var target = new Engine();
            var slide = new SlideBuilder()
                .AssignId()
                .Build();
            var actual = target.Layout(slide);
            Assert.True(slide.ContentItems == null, "Invalid test if the ContentItems collection is not null");
        }

        [Fact]
        public void NotFailIfContentItemCollectionIsEmpty()
        {
            var target = new Engine();
            var slide = new SlideBuilder()
                .AssignId()
                .WithEmptyContentItemsCollection()
                .Build();
            var actual = target.Layout(slide);
        }

        [Fact]
        public void ReturnAWellFormedHTMLSectionTag()
        {
            var target = new Engine();
            var slide = new SlideBuilder()
                .AssignId()
                .WithEmptyContentItemsCollection()
                .Build();
            var actual = target.Layout(slide);
            var section = XElement.Parse(actual);
            Assert.Equal("section", section.Name.LocalName);
        }

        [Fact]
        public void ContainTheTitleIfOneIsSpecified()
        {
            string expected = string.Empty.GetRandom();
            var target = new Engine();
            var slide = new SlideBuilder()
                .AssignId()
                .Title(expected)
                .Build();
            var actual = target.Layout(slide);
            Assert.Contains(expected, actual);
        }

        [Fact]
        public void NotContainAnH1IfNoTitleIsSpecified()
        {
            var target = new Engine();
            var slide = new SlideBuilder()
                .AssignId()
                .Build();
            var actual = target.Layout(slide);
            Assert.True(string.IsNullOrEmpty(slide.Title), "Invalid test if Title is not null or empty");
            Assert.DoesNotContain("<h1>", actual.ToLower());
        }

        [Fact]
        public void NotContainAnH1IfTheTitleIsJustWhitespace()
        {
            var target = new Engine();
            var slide = new SlideBuilder()
                .AssignId()
                .Title(" \t ")
                .Build();
            var actual = target.Layout(slide);
            Assert.False(string.IsNullOrEmpty(slide.Title), "Invalid test if string is null or empty (should contain whitespace)");
            Assert.DoesNotContain("<h1>", actual.ToLower());
        }

        [Fact]
        public void ContainTheContentTextIfTheOnlyContentItemIsText()
        {
            string expected = string.Empty.GetRandom();

            var target = new Engine();

            var contentItem = new ContentItemBuilder()
                .CreatePlainText(expected)
                .Build();

            var slide = new SlideBuilder()
                .AssignId()
                .AddContentItem(10, contentItem)
                .Build();

            var actual = target.Layout(slide);
            Assert.Contains(expected, actual);
        }

        [Fact]
        public void ContainTheContentTextIfTheFirstContentItemIsText()
        {
            string expected = string.Empty.GetRandom();

            var target = new Engine();

            var textContentItem = new ContentItemBuilder()
                .CreatePlainText(expected)
                .Build();

            var imageContentItem = new ContentItemBuilder()
                .CreatePng(string.Empty.TinyBase64EncodedPng())
                .Build();

            var anotherContentItem = new ContentItemBuilder()
                .CreateMarkdown($"## {string.Empty.GetRandom()}")
                .Build();

            var slide = new SlideBuilder()
                .AssignId()
                .AddContentItem(10, textContentItem)
                .AddContentItem(20, imageContentItem)
                .AddContentItem(30, anotherContentItem)
                .Build();

            var actual = target.Layout(slide);
            Assert.Contains(expected, actual);
        }

        [Fact]
        public void ContainTheContentImageIfTheOnlyContentItemIsAnImage()
        {
            string expected = string.Empty.TinyBase64EncodedPng();

            var target = new Engine();

            var contentItem = new ContentItemBuilder()
                .CreatePng(expected)
                .Build();

            var slide = new SlideBuilder()
                .AssignId()
                .AddContentItem(10, contentItem)
                .Build();

            var actual = target.Layout(slide);
            Assert.Contains(expected, actual);
        }

        [Fact]
        public void ContainTheContentImageIfTheFirstContentItemIsAnImage()
        {
            string expected = string.Empty.TinyBase64EncodedPng();

            var target = new Engine();

            var textContentItem = new ContentItemBuilder()
                .CreatePlainText(expected)
                .Build();

            var imageContentItem = new ContentItemBuilder()
                .CreatePng(expected)
                .Build();

            var anotherContentItem = new ContentItemBuilder()
                .CreateMarkdown($"## {string.Empty.GetRandom()}")
                .Build();

            var slide = new SlideBuilder()
                .AssignId()
                .AddContentItem(10, textContentItem)
                .AddContentItem(5, imageContentItem)
                .AddContentItem(30, anotherContentItem)
                .Build();

            var actual = target.Layout(slide);
            Assert.Contains(expected, actual);
        }

        [Fact]
        public void ThrowContentTypeNotSupportedIfContentTypeIsNotImageOrText()
        {
            var target = new Engine();

            var contentItem = new ContentItemBuilder()
                .ContentType(string.Empty.GetRandom())
                .Content(string.Empty.GetRandom())
                .Build();

            var slide = new SlideBuilder()
                .AssignId()
                .AddContentItem(10, contentItem)
                .Build();

            Assert.Throws<Exceptions.ContentTypeNotSupportedException>(() => target.Layout(slide));
        }
    }
}