using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LiquidVictor.Data.Postgres
{
    public class SlideDeckReadRepository : Interfaces.ISlideDeckReadRepository
    {
        Context _context;

        public SlideDeckReadRepository()
        {
            _context = new Context();
        }

        public SlideDeckReadRepository(string connectionString)
        {
            _context = new Context(connectionString);
        }

        public Entities.ContentItem GetContentItem(Guid id)
        {
            // TODO: Refactor from GetSlideDeck
            throw new NotImplementedException();
        }

        public Entities.Slide GetSlide(Guid id)
        {
            // TODO: Refactor from GetSlideDeck
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> GetSlideDeckIds()
        {
            throw new NotImplementedException();
        }

        public Entities.SlideDeck GetSlideDeck(Guid id)
        {
            // TODO: Throw custom error if not found

            return _context.SlideDecks
                .Include(sd => sd.SlideDeckSlides)
                .ThenInclude(sds => sds.Slide)
                .ThenInclude(s => s.SlideContentItems)
                .ThenInclude(sci => sci.ContentItem)
                .Single(sd => sd.Id == id)
                .AsEntity();
        }
    }
}
