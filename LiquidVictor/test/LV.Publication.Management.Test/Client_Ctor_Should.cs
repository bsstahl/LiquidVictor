using Microsoft.Extensions.Logging;
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
        public static void ExecutesSuccessfullyWithNullLogger()
        {
            var target = new Client(null);
            target.Process();
        }

        [Fact]
        public static void ExecutesSuccessfullyWithLogger()
        {
            var target = new Client("TestLogs".CreateLogger());
            target.Process();
        }
    }
}
