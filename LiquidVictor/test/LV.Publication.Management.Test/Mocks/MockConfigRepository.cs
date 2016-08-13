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
        #region Mock Information Methods

        Configuration _config = new Configuration();

        public bool GetConfigCalled { get; private set; }

        public void CreateSources(int sourceCount)
        {
            for (int i = 0; i < sourceCount; i++)
            {
                _config.AddSource(new Source());
            }
        }

        #endregion

        #region IConfigRepository Methods

        public Configuration GetConfig()
        {
            this.GetConfigCalled = true;
            return new Configuration();
        }

        #endregion
    }
}
