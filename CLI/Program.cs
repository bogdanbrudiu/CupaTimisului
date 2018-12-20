using CommandLine;
using CommandLine.Text;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contest;

namespace CLI
{
    public class Program
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private static readonly HeadingInfo _headingInfo = new HeadingInfo("Cabrillo", "0.1");


      




        static void Main(string[] args)
        {

            var options = new Options();
            var results = Parser.Default.ParseArguments<Options>(args);
           
            if (string.IsNullOrEmpty(options.InputFolder) && string.IsNullOrEmpty(options.InputFile))
            {
                HelpText.AutoBuild(results);
            }
            switch (options.VerboseLevel)
            {
                case 0:
                    ((log4net.Repository.Hierarchy.Logger)log.Logger).Level = log4net.Core.Level.Error;
                    break;
                case 1:
                    ((log4net.Repository.Hierarchy.Logger)log.Logger).Level = log4net.Core.Level.Warn;
                    break;
                case 2:
                    ((log4net.Repository.Hierarchy.Logger)log.Logger).Level = log4net.Core.Level.All;
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(options.InputFolder))
            {
                Contest.Program.ProcessFolder(options);
            }
            if (!string.IsNullOrEmpty(options.InputFile))
            {
                List<Cabrillo> logList = new List<Cabrillo>();
                logList.Add(Contest.Program.Parse(new FileInfo(options.InputFile), options.IgnoreStartOfLogTag));
                if (options.CupaTimisului)
                {
                    Contest.Program.ParseQSO(logList, options.IgnoreDateTimeValidation, Contest.Program.ScorCupaTimisului);
                }
                if (options.ZiuaTelecomunicatiilor)
                {
                    Contest.Program.ParseQSO(logList, options.IgnoreDateTimeValidation, Contest.Program.ScorZT);
                }
                List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
                //startendPairs.Add(Tuple.Create(etapa1Start, etapa1End));
                //startendPairs.Add(Tuple.Create(etapa2Start, etapa2End));

                startendPairs.Add(Tuple.Create(options.Etapa1Start, options.Etapa1End));
                startendPairs.Add(Tuple.Create(options.Etapa2Start, options.Etapa2End));
                List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, options.IgnoreDateTimeValidation);
                foreach (List<QSO> etapa in etape)
                {
                    Cabrillo.CheckOneQSO(etapa);
                }

                foreach (var q in logList[0].QSOs)
                {
                    Cabrillo.ParseFrequency(q, options.IgnoreModeFrequencyValidation);
                }
                if (options.CupaTimisului)
                {
                    Cabrillo.ModeChangeCheck(logList[0]);
                }
            }
            Console.ReadKey();
        }

       

    }
}
