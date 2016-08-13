using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Management.Test.Mocks
{
    public class MockSourceProcessorFactory: ISourceProcessorFactory
    {
        public int TimesCreateCalled { get; private set; }


    }
}
