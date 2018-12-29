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

        public void SaveSlideDeck(Entities.SlideDeck slideDeck)
        {
            _context.UpdateSlideDeck(slideDeck);
            _context.SaveChanges(true);
        }

    }
}
