using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiquidVictor.Data.Postgres
{
    public class SlideDeckWriteRepository
    {
        public void SaveSlideDeck(Entities.SlideDeck slideDeck)
        {
            var context = new Context();
            context.UpdateSlideDeck(slideDeck);
            context.SaveChanges(true);
        }

    }
}
