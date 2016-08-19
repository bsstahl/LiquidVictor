using LV.Publication.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Management.Console
{
    public class Program
    {
        const string _logCategory = "LV.Publication.Management.Console";

        public static void Main(string[] args)
        {
            int executionMs;
            if (int.TryParse(args[0], out executionMs))
            {
                var p = new Program();
                p.Process(executionMs);
            }
        }

        public void Process(int executionMs)
        {
            var loggerFactory = new LoggerFactory().AddDebug();
            var logger = loggerFactory.CreateLogger(_logCategory);
            IConfigRepository configRepo = null;
            ISourceProcessorFactory sourceProcessorFactory = null;

            var client = new Client(logger, configRepo, sourceProcessorFactory);
            client.Start();
            Task.Delay(executionMs);
            client.Stop();
        }
    }
}
