using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LV.Publication.Management.Test
{
    public class Client_Ctor_Should
    {
        public Client_Ctor_Should()
        {
        }

        [Fact]
        public static void ExecuteSuccessfully()
        {
            var target = new Client();
            target.Process();
        }
    }
}
