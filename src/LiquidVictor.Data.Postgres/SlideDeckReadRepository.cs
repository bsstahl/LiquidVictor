using System;
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
