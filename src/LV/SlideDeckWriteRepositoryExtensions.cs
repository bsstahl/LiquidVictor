using LiquidVictor.Interfaces;
using System;

namespace LV
{

    internal static class SlideDeckWriteRepositoryExtensions
    {
        internal static void WriteContentItem(this ISlideDeckWriteRepository writeRepo, Guid contentItemId,
            string contentType, string contentItemFileName, string contentItemTitle, byte[] content)
        {
            writeRepo.SaveContentItem(new LiquidVictor.Entities.ContentItem()
            {
                Id = contentItemId,
                ContentType = contentType,
                FileName = contentItemFileName,
                Title = contentItemTitle,
                Content = content
            });
        }


    }
}
