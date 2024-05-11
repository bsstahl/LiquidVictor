using System;

namespace LiquidVictor.Exceptions
{
    public class SlideDeckNotFoundException : Exception
    {
        public SlideDeckNotFoundException()
        { }

        public SlideDeckNotFoundException(Guid slideDeckId, string searchLocation)
            : this(CreateMessage(slideDeckId, searchLocation, null))
        { }

        public SlideDeckNotFoundException(Guid slideDeckId, string searchLocation, Exception innerException)
            : base(CreateMessage(slideDeckId, searchLocation, innerException), innerException)
        {
            this.SlideDeckId = slideDeckId;
            this.SearchLocation = searchLocation;
        }

        public SlideDeckNotFoundException(string message) 
            : base(message)
        { }

        public SlideDeckNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        { }

        public Guid SlideDeckId { get; set; }
        public string SearchLocation { get; set; } = string.Empty;

        private static string CreateMessage(Guid slideDeckId, string searchLocation, Exception? innerException)
        {
            string result = $"Unable to find slide deck with ID {slideDeckId}";
            if (!string.IsNullOrWhiteSpace(searchLocation))
                result += $" in {searchLocation}'";
            if (innerException is not null)
                result += $" due to {innerException}";

            return result;
        }

    }
}
