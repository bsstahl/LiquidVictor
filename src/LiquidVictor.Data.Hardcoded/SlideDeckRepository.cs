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
                        Title = "Content Slide 1",
                        ContentText = "# This is the 1st slide Header\r\n* Bullet Point 1\r\n* Bullet Point 2"
                    } },

                { 20, new Slide()
                    {
                        Title = "Content Slide 2",
                        ContentText = "# This is the 2nd slide Header\r\n* Bullet Point 1\r\n* Bullet Point 2"
                    } }
            };

            return new SlideDeck(id, "Test Presentation", "A Test of What Liquid Victor Can Do", slides);
        }
    }
}
