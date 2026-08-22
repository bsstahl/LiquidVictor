using LiquidVictor.Entities;
using LiquidVictor.Enumerations;

namespace LiquidVictor.Data.Hardcoded
{
    public class SlideDeckRepository : Interfaces.ISlideDeckReadRepository
    {
        public IEnumerable<Entities.SlideDeck> GetSlideDecks()
        {
            throw new NotImplementedException();
        }

        public ContentItem GetContentItem(Guid id)
        {
            // TODO: Refactor from GetSlideDeck
            throw new NotImplementedException();
        }

        public Slide GetSlide(Guid id)
        {
            // TODO: Refactor from GetSlideDeck
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> GetSlideDeckIds()
        {
            throw new NotImplementedException();
        }

        public SlideDeck GetSlideDeck(Guid id)
        {
            var includeBlocks = new List<IncludeBlock>();
            if (id == Guid.Parse("E0B187D2-C9B7-4635-8FE5-0CA21BC5007F"))
            {
                includeBlocks.Add(10, "Full Screen Slide", Layout.FullPage,
                    "* Bullet Point 1\r\n* Bullet Point 2", "text/markdown");

                includeBlocks.Add(15, "Paragraph Slide", Layout.FullPageFragments,
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce facilisis consectetur dui ac ultrices. Vivamus et enim erat. Vestibulum vel lorem non diam ornare elementum sit amet sit amet diam. Nam in accumsan dui, sed aliquam orci. Duis ac ullamcorper lacus, eget mollis magna. Pellentesque cursus leo sem, luctus pharetra mauris aliquam sit amet. Pellentesque bibendum, erat at aliquet imperdiet, metus diam rhoncus ex, dictum elementum augue elit et ipsum. In fermentum consequat velit, non laoreet lacus. Ut feugiat nulla quis sapien consectetur, sed consectetur ante porttitor. Vestibulum tincidunt placerat nisl et efficitur. Pellentesque sodales auctor lorem lacinia malesuada. Duis vestibulum massa vel vulputate commodo.",
                    "text/plain");

                includeBlocks.Add(20, "Image-Right Slide", Layout.ImageRight,
                    "* Bullet Point 3\r\n* Bullet Point 4", "text/markdown",
                    "Angles", @"Angles2.svg",
                    "image/svg");

                includeBlocks.Add(30, "Image-Left Slide", Layout.ImageLeft,
                    "* Bullet Point 5\r\n* Bullet Point 6\r\n* Bullet Point 7\r\n* Bullet Point 8\r\n* Bullet Point 9\r\n* Bullet Point 10",
                    "text/markdown", "System States",
                    @"System States.png",
                    "image/png");
            }
            else if (id == Guid.Parse("728f2f58-9ee6-4e5a-9281-26d094b7a68a"))
            {
                var slideGroup1 = new List<Slide>
                {
                    {
                        10,
                        "Paragraph Slide",
                        Layout.FullPageFragments,
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce facilisis consectetur dui ac ultrices. Vivamus et enim erat. Vestibulum vel lorem non diam ornare elementum sit amet sit amet diam. Nam in accumsan dui, sed aliquam orci. Duis ac ullamcorper lacus, eget mollis magna. Pellentesque cursus leo sem, luctus pharetra mauris aliquam sit amet. Pellentesque bibendum, erat at aliquet imperdiet, metus diam rhoncus ex, dictum elementum augue elit et ipsum. In fermentum consequat velit, non laoreet lacus. Ut feugiat nulla quis sapien consectetur, sed consectetur ante porttitor. Vestibulum tincidunt placerat nisl et efficitur. Pellentesque sodales auctor lorem lacinia malesuada. Duis vestibulum massa vel vulputate commodo.",
                        "text/plain"
                    },
                    {
                        15,
                        "Full Screen Slide",
                        Layout.FullPage,
                        "* Bullet Point 1\r\n* Bullet Point 2",
                        "text/markdown"
                    }
                };

                var slideGroup2 = new List<Slide>
                {
                    {
                        20,
                        "Image-Left Slide",
                        Layout.ImageLeft,
                        "* Bullet Point 5\r\n* Bullet Point 6\r\n* Bullet Point 7\r\n* Bullet Point 8\r\n* Bullet Point 9\r\n* Bullet Point 10",
                        "text/markdown",
                        "System States",
                        @"System States.png",
                        "image/png"
                    },
                    {
                        30,
                        "Image-Right Slide",
                        Layout.ImageRight,
                        "* Bullet Point 3\r\n* Bullet Point 4",
                        "text/markdown",
                        "Angles",
                        @"Angles2.svg",
                        "image/svg"
                    }
                };

                includeBlocks.Add(new IncludeBlock(slideGroup1.OrderBy(i => slideGroup1.IndexOf(i))));
                includeBlocks.Add(new IncludeBlock(slideGroup2.OrderBy(i => slideGroup2.IndexOf(i))));
            }
            else if (id == Guid.Parse("bcf43e23-4771-4862-baf7-9bccb9c096c5"))
            {
                var slide1 = new List<Slide>
                {
                    {
                    10,
                    "Image-Right Slide",
                    Layout.ImageRight,
                    "* Bullet Point 3\r\n* Bullet Point 4",
                    "text/markdown",
                    "Angles",
                    @"Angles2.svg",
                    "image/svg"
                    } 
                };

                var slideGroup1 = new List<Slide>
                {
                    {
                        15,
                        "Full Screen Slide",
                        Layout.FullPage,
                        "* Bullet Point 1\r\n* Bullet Point 2",
                        "text/markdown"
                    },
                    {
                        20,
                        "Image-Left Slide",
                        Layout.ImageLeft,
                        "* Bullet Point 5\r\n* Bullet Point 6\r\n* Bullet Point 7\r\n* Bullet Point 8\r\n* Bullet Point 9\r\n* Bullet Point 10",
                        "text/markdown",
                        "System States",
                        @"System States.png",
                        "image/png"
                    }
                };

                var slide2 = new List<Slide>
                {
                    {
                    30,
                    "Paragraph Slide",
                    Layout.FullPageFragments,
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce facilisis consectetur dui ac ultrices. Vivamus et enim erat. Vestibulum vel lorem non diam ornare elementum sit amet sit amet diam. Nam in accumsan dui, sed aliquam orci. Duis ac ullamcorper lacus, eget mollis magna. Pellentesque cursus leo sem, luctus pharetra mauris aliquam sit amet. Pellentesque bibendum, erat at aliquet imperdiet, metus diam rhoncus ex, dictum elementum augue elit et ipsum. In fermentum consequat velit, non laoreet lacus. Ut feugiat nulla quis sapien consectetur, sed consectetur ante porttitor. Vestibulum tincidunt placerat nisl et efficitur. Pellentesque sodales auctor lorem lacinia malesuada. Duis vestibulum massa vel vulputate commodo.",
                    "text/plain"
                    }
                };

                includeBlocks.Add(new IncludeBlock(slide1.Single()));
                includeBlocks.Add(new IncludeBlock(slideGroup1.OrderBy(i => slideGroup1.IndexOf(i))));
                includeBlocks.Add(new IncludeBlock(slide2.Single()));
            }
            else
                throw new Exceptions.SlideDeckNotFoundException(id, this.GetType().FullName ?? string.Empty);

            return new SlideDeck(id, "Demo Presentation", "A Liquid Victor Demonstration", "Joe Presenter (@joep)", "Printable Version", includeBlocks.OrderBy(i => includeBlocks.IndexOf(i)));
        }

        public IEnumerable<Slide> GetSlides()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentItem> GetContentItems()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> GetSlideIds()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> GetContentItemIds()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> GetIncludeBlockIds() => throw new NotImplementedException();
        public IEnumerable<IncludeBlock> GetIncludeBlocks() => throw new NotImplementedException();
        public IncludeBlock GetIncludeBlock(Guid id) => throw new NotImplementedException();
    }
}