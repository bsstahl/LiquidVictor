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

        public ICollection<KeyValuePair<int, ContentItem>> ContentItems { get; set; }

    }
}
