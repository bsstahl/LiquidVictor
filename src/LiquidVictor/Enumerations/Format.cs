namespace LiquidVictor.Enumerations;

public enum Format
{
    LightningTalk = 0,  // 15 min or less
    ShortSession = 1,   // Between 15-45 min
    Session = 2,        // Roughly 1 hour (45 min - 75 min)
    Talk = 3,           // Roughly 1.5 hours (75 min - 2 hours)
    ShortWorkshop = 4,  // 2-4 hours or so
    Workshop = 5,       // 4-16 hours
    LongWorkshop = 6,   // 3 days or more
    Keynote = 7,        // Keynote addresses
    Reference = 8       // Website or other reference documentation
}
