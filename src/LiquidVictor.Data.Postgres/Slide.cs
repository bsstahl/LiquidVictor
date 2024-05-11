using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace LiquidVictor.Data.Postgres
{
    [Table("slides")]
    internal class Slide : EntityBase
    {
        [Column("title"), Required]
        public string Title { get; set; }

        [Column("layout"), Required]
        public Layout Layout { get; set; }

        [Column("transitionin")]
        public Transition TransitionIn { get; set; }

        [Column("transitionout")]
        public Transition TransitionOut { get; set; }

        [Column("notes")]
        public string Notes { get; set; }

        public ICollection<SlideContentItem> SlideContentItems { get; set; }


        internal Entities.Slide AsEntity()
        {
            var slide =  new Entities.Slide()
            {
                Id = this.Id,
                Layout = this.Layout,
                Notes = this.Notes,
                Title = this.Title,
                TransitionIn = this.TransitionIn,
                TransitionOut = this.TransitionOut
            };

            this.SlideContentItems.AsEntities().ToList()
                .ForEach(ci => slide.ContentItems.Add(ci));

            return slide;
        }

        internal void FromEntity(Entities.Slide slide)
        {
            this.Id = slide.Id;
            this.LastModifiedDate = DateTime.UtcNow;
            this.Title = slide.Title;
            this.Layout = slide.Layout;
            this.TransitionIn = slide.TransitionIn;
            this.TransitionOut = slide.TransitionOut;
            this.Notes = slide.Notes;

            if (this.SlideContentItems == null)
                this.SlideContentItems = new List<SlideContentItem>();

            // Update any Content Items that are still being used
            // and make a list of those that need to be deleted
            var removalSlideContentItems = new List<SlideContentItem>();
            foreach (var storageSlideContentItem in this.SlideContentItems)
            {
                var existsInUpdate = slide.ContentItems.Any(s =>
                    s.Value != null
                    && s.Value.Id == storageSlideContentItem.ContentItemId
                    && s.Key == storageSlideContentItem.SortOrder);

                if (!existsInUpdate)
                    removalSlideContentItems.Add(storageSlideContentItem);
                else
                {
                    var contentItem = slide.ContentItems.SingleOrDefault(s =>
                        s.Value.Id == storageSlideContentItem.ContentItemId
                        && s.Key == storageSlideContentItem.SortOrder);

                    storageSlideContentItem.ContentItem.FromEntity(contentItem.Value);
                }
            }

            // Delete the associations for slides no longer used in this deck
            foreach (var storageSlideDeckSlide in removalSlideContentItems)
            {
                this.SlideContentItems.Remove(storageSlideDeckSlide);
            }

            // Add any new Content Items/associations
            foreach (var contentItem in slide.ContentItems)
            {
                if (contentItem.Value != null)
                {
                    var associationExists = this.SlideContentItems.Any(sci =>
                        sci.ContentItemId == contentItem.Value.Id
                        && sci.SortOrder == contentItem.Key);

                    if (!associationExists)
                    {
                        var storageContentItem = new ContentItem();
                        storageContentItem.FromEntity(contentItem.Value);

                        var storageSlideContentItem = new SlideContentItem()
                        {
                            Id = Guid.NewGuid(),
                            LastModifiedDate = DateTime.UtcNow,
                            ContentItem = storageContentItem,
                            SortOrder = contentItem.Key
                        };

                        this.SlideContentItems.Add(storageSlideContentItem);
                    }
                }
            }
        }
    }
}
