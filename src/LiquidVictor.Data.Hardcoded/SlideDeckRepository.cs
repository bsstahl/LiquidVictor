using System;
using System.Collections.Generic;
using LiquidVictor.Entities;

namespace LiquidVictor.Data.Hardcoded
{
    public class SlideDeckRepository : Interfaces.ISlideDeckRepository
    {
        public SlideDeck GetSlideDeck(Guid id)
        {
            var slides = new SortedList<int, Slide>()
            {
                { 10, new Slide()
                    {
                        Title = "Full Screen Slide",
                        ContentText = "* Bullet Point 1\r\n* Bullet Point 2",
                        Layout = Enumerations.Layout.FullPage
                    } },

                { 15, new Slide()
                    {
                        Title = "Paragraph Slide",
                        ContentText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce facilisis consectetur dui ac ultrices. Vivamus et enim erat. Vestibulum vel lorem non diam ornare elementum sit amet sit amet diam. Nam in accumsan dui, sed aliquam orci. Duis ac ullamcorper lacus, eget mollis magna. Pellentesque cursus leo sem, luctus pharetra mauris aliquam sit amet. Pellentesque bibendum, erat at aliquet imperdiet, metus diam rhoncus ex, dictum elementum augue elit et ipsum. In fermentum consequat velit, non laoreet lacus. Ut feugiat nulla quis sapien consectetur, sed consectetur ante porttitor. Vestibulum tincidunt placerat nisl et efficitur. Pellentesque sodales auctor lorem lacinia malesuada. Duis vestibulum massa vel vulputate commodo.",
                        Layout = Enumerations.Layout.FullPage
                    } },

                { 20, new Slide()
                    {
                        Title = "Image-Right Slide",
                        ContentText = "* Bullet Point 3\r\n* Bullet Point 4",
                        PrimaryImage  = new Entities.PresentationImage()
                            {
                                Name = @"Canis Latrans",
                                Content = System.IO.File.ReadAllBytes(@"..\..\..\..\..\Media\canis latrans.jpg"),
                                ImageFormat = "image/jpeg"
                            },
                        Layout = Enumerations.Layout.ImageRight
                    } },
                { 30, new Slide()
                    {
                        Title = "Image-Left Slide",
                        ContentText = "* Bullet Point 5\r\n* Bullet Point 6\r\n* Bullet Point 7\r\n* Bullet Point 8\r\n* Bullet Point 9\r\n* Bullet Point 10",
                        PrimaryImage  = new Entities.PresentationImage()
                            {
                                Name = @"Carl Sagan",
                                Content = System.IO.File.ReadAllBytes(@"..\..\..\..\..\Media\Carl-Sagan-portrait-590x295.png"),
                                ImageFormat = "image/png"
                            },
                        Layout = Enumerations.Layout.ImageLeft
                    } }
            };

            return new SlideDeck(id, "Demo Presentation", "A Liquid Victor Demonstration", "Joe Presenter (@joep)", slides);
        }
    }
}
