using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace LiquidVictor.Data.Postgres
{
    internal class Context : DbContext
    {
        string _connectionString;
        public Context(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal DbSet<SlideDeck> SlideDecks { get; set; }
        internal DbSet<Slide> Slides { get; set; }
        internal DbSet<ContentItem> ContentItems { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(_connectionString)
                .EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity<SlideContentItem>()
                    .HasAlternateKey(sci => new { sci.SlideId, sci.ContentItemId, sci.SortOrder })
                    .HasName("UX_slideid_slidecontentitd_sortorder");

                modelBuilder.Entity<SlideDeckSlide>()
                    .HasAlternateKey(sds => new { sds.SlideDeckId, sds.SlideId, sds.SortOrder })
                    .HasName("UX_slidedeckid_slideid_sortorder");

                modelBuilder.Entity(entityType.ClrType)
                    .Property("CreateDate")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

            }
        }

        internal void UpdateSlideDeck(Entities.SlideDeck slideDeck)
        {
            var storageSlideDeck = this.SlideDecks
                .Include(sd => sd.SlideDeckSlides)
                .ThenInclude(sds => sds.Slide)
                .ThenInclude(s => s.SlideContentItems)
                .ThenInclude(sci => sci.ContentItem)
                .SingleOrDefault(s => s.Id == slideDeck.Id);

            if (storageSlideDeck == null)
            {
                storageSlideDeck = new SlideDeck();
                this.SlideDecks.Add(storageSlideDeck);
            }

            storageSlideDeck.Update(this, slideDeck);
        }
    }
}
