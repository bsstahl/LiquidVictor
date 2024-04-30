namespace LiquidVictor.Enumerations;

public enum Format
{
    LightningTalk = 0,  // Less than 30 min
    Session = 1,        // Roughly 1 hour
    Talk = 2,           // Roughly 1.5 hours
    ShortWorkshop = 3,  // 2-4 hours or so
    Workshop = 4,       // 4-16 hours
    LongWorkshop = 5,   // 3 days or more
    Keynote = 6,        // Keynote addresses
    Reference = 7       // Website or other reference documentation
}
