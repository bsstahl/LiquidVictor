using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LV.Publication.Entities;

namespace LV.Publication.Management
{
    public class SourceProcessorCollection : List<ISourceProcessor>
    {
        ISourceProcessorFactory _sourceProcessorFactory;

        public int ActiveProcessorCount { get; private set; }


        public SourceProcessorCollection(ISourceProcessorFactory sourceProcessorFactory, IEnumerable<Source> sources)
        {
            _sourceProcessorFactory = sourceProcessorFactory;
            Load(sources);
        }

        private void Load(IEnumerable<Source> sources)
        {
            foreach (var source in sources)
                this.Add(_sourceProcessorFactory.GetSource());
        }

        internal void Start()
        {
            foreach (var source in this)
            {
                source.Start();
                this.ActiveProcessorCount++;
            }
        }

        internal void Stop()
        {
            foreach (var source in this)
            {
                source.Stop();
                this.ActiveProcessorCount--;
            }
        }
    }
}
