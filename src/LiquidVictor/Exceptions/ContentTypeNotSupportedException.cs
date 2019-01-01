using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Exceptions
{
    public class ContentTypeNotSupportedException: Exception
    {
        public string ContentType { get; set; }

        public ContentTypeNotSupportedException(string contentType)
            :base($"Content type '{contentType}' is not currently supported")
        {
            this.ContentType = contentType;
        }
    }
}
