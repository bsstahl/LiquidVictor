using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LiquidVictor.Data.Postgres
{
    public class ReadRepository : Interfaces.ISlideDeckRepository
    {
        Context _context;
        public ReadRepository()
        {
            _context = new Context();
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
