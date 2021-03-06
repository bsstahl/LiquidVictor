﻿using System;
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

        public void SaveSlideDeck(SlideDeck slideDeck)
        {
            var fullPath = System.IO.Path.Combine(_dataPath, $"{slideDeck.Id.ToString()}.json");
            var json = JsonConvert.SerializeObject(slideDeck);
            System.IO.File.WriteAllText(fullPath, json);
        }

        public SlideDeck GetSlideDeck(Guid id)
        {
            var fullPath = System.IO.Path.Combine(_dataPath, $"{id}.json");
            var json = System.IO.File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<SlideDeck>(json);
        }

    }
}
