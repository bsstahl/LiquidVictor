using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Exceptions
{
    public class SlideDeckNotFoundException : Exception
    {
        public SlideDeckNotFoundException() { }

        public SlideDeckNotFoundException(Guid slideDeckId, string searchLocation)
            : base(CreateMessage(slideDeckId, searchLocation, null))
        {
            this.SlideDeckId = slideDeckId;
            this.SearchLocation = searchLocation;
        }

        public SlideDeckNotFoundException(Guid slideDeckId, string searchLocation, Exception innerException)
            : base(CreateMessage(slideDeckId, searchLocation, innerException), innerException)
        {
            this.SlideDeckId = slideDeckId;
            this.SearchLocation = searchLocation;
        }

        public Guid SlideDeckId { get; set; }
        public string SearchLocation { get; set; }

        private static string CreateMessage(Guid slideDeckId, string searchLocation, Exception innerException)
        {
            string result = $"Unable to find slide deck with ID {slideDeckId.ToString()}";
            if (!string.IsNullOrWhiteSpace(searchLocation))
                result += $" in {searchLocation}'";
            if (innerException != null)
                result += $" due to {innerException.ToString()}";

            return result;
        }

    }
}
