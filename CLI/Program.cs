using CommandLine;
using CommandLine.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contest;
using Microsoft.Extensions.Logging;


namespace CLI
{
    public class Program
    {


        private static readonly HeadingInfo _headingInfo = new HeadingInfo("Cabrillo", "0.1");


      




        static void Main(string[] args)
        {
            var options = new Options();
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("CLI.Program", LogLevel.Debug)
                    .AddConsole();

                switch (options.VerboseLevel)
                {
                    case 0:
                        builder.AddFilter("CLI.Program", LogLevel.Error);
                        break;
                    case 1:
                        builder.AddFilter("CLI.Program", LogLevel.Warning);
                        break;
                    case 2:
                        builder.AddFilter("CLI.Program", LogLevel.Trace);
                        break;
                    default:
                        break;
                }

             
            });

         
            var results = Parser.Default.ParseArguments<Options>(args);
           
            if (string.IsNullOrEmpty(options.InputFolder) && string.IsNullOrEmpty(options.InputFile))
            {
                HelpText.AutoBuild(results);
            }
         
            ILogger logger = loggerFactory.CreateLogger<Program>();
            if (!string.IsNullOrEmpty(options.InputFolder))
            {
                Contest.Program.ProcessFolder(options, logger);
            }
            if (!string.IsNullOrEmpty(options.InputFile))
            {
                List<Cabrillo> logList = new List<Cabrillo>();
                logList.Add(Contest.Program.Parse(new FileInfo(options.InputFile), options.IgnoreStartOfLogTag, logger));
                if (options.CupaTimisului)
                {
                    Contest.Program.ParseQSO(logList, options.IgnoreDateTimeValidation, Contest.Program.ScorCupaTimisului, logger);
                }
                if (options.ZiuaTelecomunicatiilor)
                {
                    Contest.Program.ParseQSO(logList, options.IgnoreDateTimeValidation, Contest.Program.ScorZT, logger);
                }
                List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
                //startendPairs.Add(Tuple.Create(etapa1Start, etapa1End));
                //startendPairs.Add(Tuple.Create(etapa2Start, etapa2End));

                startendPairs.Add(Tuple.Create(options.Etapa1Start, options.Etapa1End));
                startendPairs.Add(Tuple.Create(options.Etapa2Start, options.Etapa2End));
                List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, options.IgnoreDateTimeValidation, logger);
                foreach (List<QSO> etapa in etape)
                {
                    Cabrillo.CheckOneQSO(etapa, logger);
                }

                foreach (var q in logList[0].QSOs)
                {
                    Cabrillo.ParseFrequency(q, options.IgnoreModeFrequencyValidation, logger);
                }
                if (options.CupaTimisului)
                {
                    Cabrillo.ModeChangeCheck(logList[0], logger);
                }
            }
            Console.ReadKey();
        }

       

    }
}
