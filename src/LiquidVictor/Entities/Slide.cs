using LiquidVictor.Enumerations;
using LiquidVictor.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Entities
{
    public class Slide
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public Layout Layout { get; set; }

        public Transition TransitionIn { get; set; }
        public Transition TransitionOut { get; set; }

        public string Notes { get; set; }

        public IEnumerable<KeyValuePair<int, ContentItem>> ContentItems { get; set; }

        //// TODO: Remove this
        //public string PrimaryContent
        //{
        //    get
        //    {
        //        return this.ContentItems
        //            .Where(c => c.Value.ContentType.ToLower().StartsWith("text"))
        //            .OrderBy(c => c.Key)
        //            .First().Value
        //            .Content.AsString();
        //    }
        //}

        //// TODO: Remove this
        //public string SecondaryContent
        //{
        //    get
        //    {
        //        return this.ContentItems
        //            .Where(c => c.Value.ContentType.ToLower().StartsWith("text"))
        //            .OrderBy(c => c.Key)
        //            .Skip(1).First().Value
        //            .Content.AsString();
        //    }
        //}


        //// TODO: Remove this
        //public ContentItem PrimaryImage
        //{
        //    get
        //    {
        //        return this.ContentItems
        //            .Where(c => c.Value.ContentType.ToLower().StartsWith("image"))
        //            .OrderBy(c => c.Key)
        //            .First().Value;
        //    }
        //}

        // public string SecondaryContent { get; set; }
        // public ContentItem SecondaryImage { get; set; }
    }
}
