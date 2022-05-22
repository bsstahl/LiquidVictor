using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Interfaces;

namespace LiquidVictor.Data.Postgres
{
    public class SlideDeckWriteRepository : ISlideDeckWriteRepository
    {
        Context _context;

        public SlideDeckWriteRepository(string connectionString)
        {
            _context = new Context(connectionString);
        }

        public void SaveContentItem(Entities.ContentItem contentItem)
        {
            // TODO: Refactor out of SaveSlideDeck
            throw new NotImplementedException();
        }

        public void SaveSlide(Entities.Slide slide)
        {
            // TODO: Refactor out of SaveSlideDeck
            throw new NotImplementedException();
        }

        public void SaveSlideDeck(Entities.SlideDeck slideDeck)
        {
            _context.UpdateSlideDeck(slideDeck);
            _context.SaveChanges(true);
        }

    }
}
