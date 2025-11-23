using LiquidVictor.Entities;

namespace LiquidVictor.Data.Postgres;

public class SlideDeckReadRepository : Interfaces.ISlideDeckReadRepository, IDisposable
{
    private readonly Context _context;
    private Boolean disposedValue;

    public SlideDeckReadRepository()
    {
        _context = new Context();
    }

    public IEnumerable<Entities.SlideDeck> GetSlideDecks()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();

        // TODO: Throw custom error if not found

        //return _context.SlideDecks
        //    .Include(sd => sd.SlideDeckSlides)
        //    .ThenInclude(sds => sds.Slide)
        //    .ThenInclude(s => s.SlideContentItems)
        //    .ThenInclude(sci => sci.ContentItem)
        //    .Single(sd => sd.Id == id)
        //    .AsEntity();
    }

    public IEnumerable<Entities.Slide> GetSlides()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Entities.ContentItem> GetContentItems()
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

    protected virtual void Dispose(Boolean disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
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

    // TODO: Add include blocks to Postgres Repository
    public IEnumerable<Guid> GetIncludeBlockIds() => throw new NotImplementedException();
    public IEnumerable<IncludeBlock> GetIncludeBlocks() => throw new NotImplementedException();
    public IncludeBlock GetIncludeBlock(Guid id) => throw new NotImplementedException();
}
