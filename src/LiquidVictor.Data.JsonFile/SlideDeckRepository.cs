using System;
using System.Collections.Generic;
using System.Linq;
using LiquidVictor.Entities;
using Newtonsoft.Json;

namespace LiquidVictor.Data.JsonFile
{
    public class SlideDeckRepository : Interfaces.ISlideDeckRepository
    {
        readonly string _dataFilePath;
        public SlideDeckRepository(string dataFilePath)
        {
            _dataFilePath = dataFilePath;
        }

        public SlideDeck GetSlideDeck(Guid id)
        {
            SlideDeck result;

            var repoJson = System.IO.File.ReadAllText(_dataFilePath);
            var repoData = JsonConvert.DeserializeObject<SlideRepoData>(repoJson);

            var slideDeck = repoData.slideDecks
                .SingleOrDefault(d => d.Id.ToLower() == id.ToString().ToLower());

            if (slideDeck == null)
                throw new Exceptions.SlideDeckNotFoundException(id, _dataFilePath);
            else
            {
                var slides = repoData.slides
                    .Where(s => slideDeck.SlideIds.Contains(s.Id))
                    .Select(s => new KeyValuePair<int, Entities.Slide>(
                        slideDeck.GetOrderIndex(s.Id), new Entities.Slide()
                        {
                            Id = Guid.Parse(s.Id),
                            ContentText = s.GetContent(),
                            Layout = s.GetLayout(),
                            Title = s.Title,
                            TransitionIn = s.GetTransitionIn(),
                            TransitionOut = s.GetTransitionOut(),
                            PrimaryImage = s.GetPrimaryImage()
                        }))
                    .OrderBy(s => s.Key);

                result = slideDeck.AsEntity(slides);
            }

            return result;
        }
    }
}
