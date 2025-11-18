using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;

namespace LiquidVictor.Data.Postgres;

[Table("slidedeck")]
sealed internal class SlideDeck : EntityBase
{
    // const Transition _defaultTransition = Transition.Slide;

    private string _title = string.Empty;
    private string _subTitle = string.Empty;
    private AspectRatio _aspectRatio;
    private string _presenter = string.Empty;
    private string _themeName = string.Empty;
    private Uri _slideDeckUri = new Uri("about:blank"); // TODO: Add to dataset


    [Column("title"), MaxLength(200)]
    public string Title
    {
        get => _title;
        set => this.CompareAndUpdate(ref _title, value);
    }

    [Column("subtitle"), MaxLength(500)]
    public string SubTitle
    {
        get => _subTitle;
        set => this.CompareAndUpdate(ref _subTitle, value);
    }

    [Column("presenter"), MaxLength(200)]
    public string Presenter
    {
        get => _presenter;
        set => this.CompareAndUpdate(ref _presenter, value);
    }

    [Column("themename"), MaxLength(50)]
    public string ThemeName
    {
        get => _themeName;
        set => this.CompareAndUpdate(ref _themeName, value);
    }

    [Column("aspectratio")]
    public AspectRatio AspectRatio
    {
        get => _aspectRatio;
        set => this.CompareAndUpdate(ref _aspectRatio, value);
    }

    [Column("slidedeckuri")]
    public Uri SlideDeckUri
    {
        get => _slideDeckUri;
        set => this.CompareAndUpdate(ref _slideDeckUri, value);
    }

    // public ICollection<SlideDeckSlide> SlideDeckSlides { get; set; }
    public IOrderedEnumerable<IncludeBlock> Includes { get; } = Enumerable.Empty<IncludeBlock>().OrderBy(i => 0);


    //internal Entities.SlideDeck AsEntity()
    //{
    //    // TODO: Respect the Transition value from the data store
    //    return new Entities.SlideDeck(this.Id, this.Title, this.SubTitle, this.Presenter, this.ThemeName, this.SlideDeckUri, "Printable Version", _defaultTransition, this.AspectRatio, this.Includes);
    //}

    internal void Update(Context context, Entities.SlideDeck slideDeck)
    {
        throw new NotImplementedException();

        //this.FromEntity(slideDeck);

        //// Update any Slides that are still being used
        //// and make a list of those that need to be deleted
        //var removalSlideDeckSlides = new List<SlideDeckSlide>();
        //foreach (var storageSlideDeckSlide in this.SlideDeckSlides)
        //{
        //    var slideExistsInUpdate = slideDeck.Slides.Any(s => 
        //        s.Value.Id == storageSlideDeckSlide.SlideId
        //        && s.Key == storageSlideDeckSlide.SortOrder);

        //    if (!slideExistsInUpdate)
        //        removalSlideDeckSlides.Add(storageSlideDeckSlide);
        //    else
        //    {
        //        var slide = slideDeck.Slides.SingleOrDefault(s =>
        //            s.Value.Id == storageSlideDeckSlide.SlideId
        //            && s.Key == storageSlideDeckSlide.SortOrder);

        //        storageSlideDeckSlide.Slide.FromEntity(slide.Value);
        //    }
        //}

        //// Delete the associations for slides no longer used in this deck
        //foreach (var storageSlideDeckSlide in removalSlideDeckSlides)
        //{
        //    this.SlideDeckSlides.Remove(storageSlideDeckSlide);
        //}

        //// Add any new slides/associations
        //foreach (var slide in slideDeck.Slides)
        //{
        //    var associationExists = this.SlideDeckSlides.Any(sds =>
        //        sds.SlideId == slide.Value.Id
        //        && sds.SortOrder == slide.Key);

        //    if (!associationExists)
        //    {
        //        // Handle slide already exists but no association
        //        Slide storageSlide = 
        //            context.Slides.SingleOrDefault(s => s.Id == slide.Value.Id) 
        //            ?? new Slide();
        //        storageSlide.FromEntity(slide.Value);

        //        var storageSlideDeckSlide = new SlideDeckSlide()
        //        {
        //            Id = Guid.NewGuid(),
        //            LastModifiedDate = DateTime.UtcNow,
        //            Slide = storageSlide,
        //            SortOrder = slide.Key
        //        };

        //        this.SlideDeckSlides.Add(storageSlideDeckSlide);
        //    }
        //}
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
