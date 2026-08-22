using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiquidVictor.Interfaces;

namespace LiquidVictor.Data.Postgres
{
    public class SlideDeckWriteRepository : ISlideDeckWriteRepository, IDisposable
    {
        private readonly Context _context;
        private Boolean disposedValue;

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

        protected virtual void Dispose(Boolean disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    if (_context is not null)
                        _context.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
