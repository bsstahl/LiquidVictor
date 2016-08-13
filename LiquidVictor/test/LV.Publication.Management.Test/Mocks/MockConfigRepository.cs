using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LV.Publication.Entities;

namespace LV.Publication.Management.Test.Mocks
{
    public class MockConfigRepository : IConfigRepository
    {
        public bool GetConfigCalled { get; private set; }

        public Configuration GetConfig()
        {
            this.GetConfigCalled = true;
            return new Configuration();
        }
    }
}
