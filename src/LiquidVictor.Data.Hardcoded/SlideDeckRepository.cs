using System;
using System.Collections.Generic;
using System.Linq;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;

namespace LiquidVictor.Data.Hardcoded
{
    public class SlideDeckRepository : Interfaces.ISlideDeckReadRepository
    {
        public SlideDeck GetSlideDeck(Guid id)
        {
            var slides = new List<KeyValuePair<int, Slide>>();
            if (id == Guid.Parse("E0B187D2-C9B7-4635-8FE5-0CA21BC5007F"))
            {
                slides.Add(10, "Full Screen Slide", Layout.FullPage,
                    "* Bullet Point 1\r\n* Bullet Point 2", "text/markdown");

                slides.Add(15, "Paragraph Slide", Layout.FullPage, 
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce facilisis consectetur dui ac ultrices. Vivamus et enim erat. Vestibulum vel lorem non diam ornare elementum sit amet sit amet diam. Nam in accumsan dui, sed aliquam orci. Duis ac ullamcorper lacus, eget mollis magna. Pellentesque cursus leo sem, luctus pharetra mauris aliquam sit amet. Pellentesque bibendum, erat at aliquet imperdiet, metus diam rhoncus ex, dictum elementum augue elit et ipsum. In fermentum consequat velit, non laoreet lacus. Ut feugiat nulla quis sapien consectetur, sed consectetur ante porttitor. Vestibulum tincidunt placerat nisl et efficitur. Pellentesque sodales auctor lorem lacinia malesuada. Duis vestibulum massa vel vulputate commodo.",
                    "text/plain");

                slides.Add(20, "Image-Right Slide", Layout.ImageRight,
                    "* Bullet Point 3\r\n* Bullet Point 4", "text/markdown",
                    "Canis Latrans", @"..\..\..\..\..\Media\CanisLatrans_small.jpg",
                    "image/jpeg");

                slides.Add(30, "Image-Left Slide", Layout.ImageLeft,
                    "* Bullet Point 5\r\n* Bullet Point 6\r\n* Bullet Point 7\r\n* Bullet Point 8\r\n* Bullet Point 9\r\n* Bullet Point 10",
                    "text/markdown", "Carl Sagan", 
                    @"..\..\..\..\..\Media\Carl-Sagan-portrait-590x295.png",
                    "image/png");
            }
            else
                throw new Exceptions.SlideDeckNotFoundException(id, this.GetType().FullName);

            return new SlideDeck(id, "Demo Presentation", "A Liquid Victor Demonstration", "Joe Presenter (@joep)", "Printable Version", slides);
        }
    }
}