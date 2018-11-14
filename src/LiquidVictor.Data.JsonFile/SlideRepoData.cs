using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using LiquidVictor.Extensions;

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
        public string Presenter { get; set; }
        public string[] SlideIds { get; set; }

        public Entities.SlideDeck AsEntity(IOrderedEnumerable<KeyValuePair<int, Entities.Slide>> slides)
        {
            return new Entities.SlideDeck(Guid.Parse(this.Id), this.Title, this.SubTitle, this.Presenter, slides);
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
        public string PrimaryImage { get; set; }

        public string GetContent()
        {
            return this.ContentText.NormalizeWhiteSpace();
        }

        public Enumerations.Layout GetLayout()
        {
            return (Enumerations.Layout)Enum.Parse(typeof(Enumerations.Layout), this.Layout);
        }

        public Entities.PresentationImage GetPrimaryImage()
        {
            return null;
        }
    }

}
