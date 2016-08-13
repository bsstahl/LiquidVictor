using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Entities
{
    public class Configuration
    {
        public List<Source> _sources;

        public IEnumerable<Source> Sources
        {
            get { return _sources; }
        }


        public Configuration()
        {
            _sources = new List<Source>();
        }

        internal void AddSource(Source source)
        {
            _sources.Add(source);
        }
    }
}
