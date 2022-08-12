using System;
using System.Collections.Generic;
using System.Linq;
using LiquidVictor.Entities;
using Newtonsoft.Json;

namespace LiquidVictor.Data.JsonFile
{
    public class SlideDeckRepository : Interfaces.ISlideDeckReadRepository, Interfaces.ISlideDeckWriteRepository
    {
        readonly string _dataPath;
        public SlideDeckRepository(string dataPath)
        {
            _dataPath = dataPath;
        }

        public IEnumerable<SlideDeck> GetSlideDecks()
        {
            throw new NotImplementedException();
        }

        public SlideDeck GetSlideDeck(Guid id)
        {
            var fullPath = System.IO.Path.Combine(_dataPath, $"{id}.json");
            var json = System.IO.File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<SlideDeck>(json);
        }

        public void SaveSlideDeck(SlideDeck slideDeck)
        {
            var fullPath = System.IO.Path.Combine(_dataPath, $"{slideDeck.Id.ToString()}.json");
            var json = JsonConvert.SerializeObject(slideDeck);
            System.IO.File.WriteAllText(fullPath, json);
        }

        public IEnumerable<Guid> GetSlideDeckIds()
        {
            throw new NotImplementedException();
        }

        public void SaveSlide(Slide slide)
        {
            // TODO: Refactor from SaveSlideDeck
            throw new NotImplementedException();
        }

        public void SaveContentItem(ContentItem contentItem)
        {
            // TODO: Refactor from SaveSlideDeck
            throw new NotImplementedException();
        }

        public Slide GetSlide(Guid id)
        {
            // TODO: Refactor from GetSlideDeck
            throw new NotImplementedException();
        }

        public ContentItem GetContentItem(Guid id)
        {
            // TODO: Refactor from GetSlideDeck
            throw new NotImplementedException();
        }

        public IEnumerable<Slide> GetSlides()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentItem> GetContentItems()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> GetSlideIds()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> GetContentItemIds()
        {
            throw new NotImplementedException();
        }
    }
}
