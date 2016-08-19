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

        #region Constants

        const long _defaultTimeout = 30000;

        #endregion

        #region Constructors

        public MockConfigRepository(int sourceCount)
        {
            CreateSources(BuildTimeouts(sourceCount));
        }

        public MockConfigRepository(IEnumerable<long> timeouts)
        {
            CreateSources(timeouts);
        }

        #endregion

        #region Mock Information Methods

        Configuration _config = new Configuration();

        public bool GetConfigCalled { get; private set; }

        private void CreateSources(IEnumerable<long> timeouts)
        {
            foreach (var timeout in timeouts)
            {
                _config.AddSource(new Source(timeout));
            }
        }

        #endregion

        #region IConfigRepository Methods

        public Configuration GetConfig()
        {
            this.GetConfigCalled = true;
            return _config;
        }

        #endregion

        #region Private Helper Methods

        private static IEnumerable<long> BuildTimeouts(int sourceCount)
        {
            var timeouts = new List<long>();
            for (int i = 0; i < sourceCount; i++)
                timeouts.Add(_defaultTimeout);
            return timeouts;
        }

        #endregion
    }
}
