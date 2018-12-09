using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using LiquidVictor.Extensions;
using LiquidVictor.Enumerations;

namespace LiquidVictor.Data.JsonFile
{

    public class SlideRepoData
    {
        public Slidedeck[] slideDecks { get; set; }
        public Slide[] slides { get; set; }
    }

    public class Slidedeck
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string ThemeName { get; set; }
        public string Presenter { get; set; }
        public string AspectRatio { get; set; }

        public string[] SlideIds { get; set; }

        public Entities.SlideDeck AsEntity(IOrderedEnumerable<KeyValuePair<int, Entities.Slide>> slides)
        {
            return new Entities.SlideDeck(Guid.Parse(this.Id), this.Title,
                this.SubTitle, this.Presenter, this.ThemeName, (AspectRatio)Enum.Parse(typeof(AspectRatio), this.AspectRatio), slides);
        }

        public int GetOrderIndex(string slideId)
        {
            int i = 0;
            bool found = false;
            while ((!found) && (i < this.SlideIds.Count()))
            {
                found = this.SlideIds[i].ToLower() == slideId.ToLower();
                if (!found)
                    i++;
            }

            if (!found)
                throw new InvalidOperationException("Slide ID not found");

            return (i * 10);
        }
    }

    public class Slide
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string[] ContentText { get; set; }
        public string Layout { get; set; }

        public string TransitionIn { get; set; }
        public string TransitionOut { get; set; }

        public PresentationImage PrimaryImage { get; set; }

        public string GetContent()
        {
            return string.Join("\r\n", this.ContentText);
        }

        public Entities.ContentItem GetPrimaryContent()
        {
            return new Entities.ContentItem()
            {
                Content = this.ContentText[0].AsByteArray(),
                ContentType = "text/markdown",
                Id = Guid.NewGuid()
            };
        }

        public Entities.ContentItem GetSecondaryContent()
        {
            Entities.ContentItem result = null;
            if (this.ContentText.Count() > 1)
                result = new Entities.ContentItem()
                {
                    Content = this.ContentText[1].AsByteArray(),
                    ContentType = "text/markdown",
                    Id = Guid.NewGuid()
                };
            return result;
        }

        public Enumerations.Layout GetLayout()
        {
            return (Enumerations.Layout)Enum.Parse(typeof(Enumerations.Layout), this.Layout);
        }

        public Transition GetTransitionIn()
        {
            return GetTransition(this.TransitionIn);
        }

        public Transition GetTransitionOut()
        {
            return GetTransition(this.TransitionOut);
        }

        private static Transition GetTransition(string transition)
        {
            Transition result = Transition.PresentationDefault;
            if (!string.IsNullOrWhiteSpace(transition))
                result = (Transition)Enum.Parse(typeof(Transition), transition);
            return result;
        }

        public Entities.ContentItem GetPrimaryImage()
        {
            Entities.ContentItem result = null;

            if (this.PrimaryImage != null)
                result = new Entities.ContentItem()
                {
                    FileName = this.PrimaryImage.Name,
                    ContentType = this.PrimaryImage.ImageFormat,
                    Content = Convert.FromBase64String(this.PrimaryImage.Content)
                };

            return result;
        }
    }

    public class PresentationImage
    {
        public string ImageFormat { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }

}
