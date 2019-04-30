using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using LiquidVictor.Enumerations;

namespace LiquidVictor.Data.Postgres
{
    [Table("slidedeck")]
    internal class SlideDeck : EntityBase
    {
        const Transition _defaultTransition = Transition.Slide;

        private string _title;
        private string _subTitle;
        private AspectRatio _aspectRatio;
        private string _presenter;
        private string _themeName;

        [Column("title"), MaxLength(200)]
        public string Title
        {
            get => _title;
            set => CompareAndUpdate(ref _title, value);
        }

        [Column("subtitle"), MaxLength(500)]
        public string SubTitle
        {
            get => _subTitle;
            set => CompareAndUpdate(ref _subTitle, value);
        }

        [Column("presenter"), MaxLength(200)]
        public string Presenter
        {
            get => _presenter;
            set => CompareAndUpdate(ref _presenter, value);
        }

        [Column("themename"), MaxLength(50)]
        public string ThemeName
        {
            get => _themeName;
            set => CompareAndUpdate(ref _themeName, value);
        }

        [Column("aspectratio")]
        public AspectRatio AspectRatio
        {
            get => _aspectRatio;
            set => CompareAndUpdate(ref _aspectRatio, value);
        }

        public ICollection<SlideDeckSlide> SlideDeckSlides { get; set; }


        internal Entities.SlideDeck AsEntity()
        {
            // TODO: Respect the Transition value from the data store
            var slides = this.SlideDeckSlides.AsEntities();
            return new Entities.SlideDeck(this.Id, this.Title, this.SubTitle, this.Presenter, this.ThemeName, "Printable Version", _defaultTransition, this.AspectRatio, slides);
        }

        internal void Update(Context context, Entities.SlideDeck slideDeck)
        {
            this.FromEntity(slideDeck);

            // Update any Slides that are still being used
            // and make a list of those that need to be deleted
            var removalSlideDeckSlides = new List<SlideDeckSlide>();
            foreach (var storageSlideDeckSlide in this.SlideDeckSlides)
            {
                var slideExistsInUpdate = slideDeck.Slides.Any(s => 
                    s.Value.Id == storageSlideDeckSlide.SlideId
                    && s.Key == storageSlideDeckSlide.SortOrder);

                if (!slideExistsInUpdate)
                    removalSlideDeckSlides.Add(storageSlideDeckSlide);
                else
                {
                    var slide = slideDeck.Slides.SingleOrDefault(s =>
                        s.Value.Id == storageSlideDeckSlide.SlideId
                        && s.Key == storageSlideDeckSlide.SortOrder);

                    storageSlideDeckSlide.Slide.FromEntity(slide.Value);
                }
            }

            // Delete the associations for slides no longer used in this deck
            foreach (var storageSlideDeckSlide in removalSlideDeckSlides)
            {
                this.SlideDeckSlides.Remove(storageSlideDeckSlide);
            }

            // Add any new slides/associations
            foreach (var slide in slideDeck.Slides)
            {
                var associationExists = this.SlideDeckSlides.Any(sds =>
                    sds.SlideId == slide.Value.Id
                    && sds.SortOrder == slide.Key);

                if (!associationExists)
                {
                    // Handle slide already exists but no association
                    Slide storageSlide = context.Slides.SingleOrDefault(s => s.Id == slide.Value.Id);
                    if (storageSlide == null)
                        storageSlide = new Slide();

                    storageSlide.FromEntity(slide.Value);

                    var storageSlideDeckSlide = new SlideDeckSlide()
                    {
                        Id = Guid.NewGuid(),
                        LastModifiedDate = DateTime.UtcNow,
                        Slide = storageSlide,
                        SortOrder = slide.Key
                    };

                    this.SlideDeckSlides.Add(storageSlideDeckSlide);
                }
            }
        }

        internal void FromEntity(Entities.SlideDeck slideDeck)
        {
            this.Id = slideDeck.Id;
            this.AspectRatio = slideDeck.AspectRatio;
            this.Presenter = slideDeck.Presenter;
            this.SubTitle = slideDeck.SubTitle;
            this.ThemeName = slideDeck.ThemeName;
            this.Title = slideDeck.Title;
        }

    }
}
