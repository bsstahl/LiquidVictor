using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Management.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var client = new LV.Publication.Management.Client();
            client.Process();
        }
    }
}
