namespace LiquidVictor.Data.JsonFileSystem
{
    internal class Slide
    {
        public string Title { get; set; }
        public string Layout { get; set; }
        public string TransitionIn { get; set; }
        public string TransitionOut { get; set; }
        public string Notes { get; set; }
        public string BackgroundContent { get; set; }
        public bool NeverFullScreen { get; set; }
        public string[] ContentItemIds { get; set; }
    }
}
