using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using log4net.Config;
using System.Globalization;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;
using log4net.Repository.Hierarchy;
using System.Collections;


namespace Contest
{
    public class Program
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private static readonly HeadingInfo _headingInfo = new HeadingInfo("Cabrillo", "0.1"); 
    

        private sealed class Options : CommandLineOptionsBase
        {
            #region Standard Option Attribute
            [Option("c", "check",
                    HelpText = "Input file with data to process.", MutuallyExclusiveSet = "InputFolder")]
            public string InputFile = String.Empty;

            [Option("e", "evaluate",
                    HelpText = "Folder with files to evaluate", MutuallyExclusiveSet = "InputFile")]
            public string InputFolder = String.Empty;

            [Option(null, "ignoreModeFrequencyValidation",
                  HelpText = "Ignore Mode Frequency Validation")]
            public bool IgnoreModeFrequencyValidation = false;

            [Option(null, "ignoreDateTimeValidation",
              HelpText = "Ignore Date and Hour Validation, only minutes are taked into account")]
            public bool ignoreDateTimeValidation = false;

          

            [Option(null, "ignoreStartOfLogTag",
              HelpText = "Ignore StartOfLog Tag")]
            public bool ignoreStartOfLogTag = false;

            [Option("v", null,
                    HelpText = "Verbose level. Range: from 0 to 2.")]
            public int? VerboseLevel = 2;

            #endregion

            #region Specialized Option Attribute

            [HelpOption(
                    HelpText = "Dispaly this help screen.")]
            public string GetUsage()
            {
                var help = new HelpText(Program._headingInfo);
                help.AdditionalNewLineAfterOption = true;
                help.Copyright = new CopyrightInfo("Bogdan-Ioan BRUDIU", 2011, 2013);
                try
                {
                    this.HandleParsingErrorsInHelp(help);
                }
                catch { }
                help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
                help.AddPreOptionsLine("the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
                help.AddPreOptionsLine("Usage: SampleApp -cfile.cbr");
                help.AddPreOptionsLine("       SampleApp -ed:\\Test -v0");
                help.AddOptions(this);

                return help;
            }

            private void HandleParsingErrorsInHelp(HelpText help)
            {
                string errors = help.RenderParsingErrorsText(this);
                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR: ", errors, Environment.NewLine));
                }
            }
            #endregion
        }
        
        public static DateTime etapa1Start = new DateTime(2013, 12, 15, 14, 00, 00);
        public static DateTime etapa1End = new DateTime(2013, 12, 15, 15, 00, 00);

        public static DateTime etapa2Start = new DateTime(2013, 12, 15, 15, 00, 00);
        public static DateTime etapa2End = new DateTime(2013, 12, 15, 16, 00, 00);

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

          


            var options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
                Environment.Exit(1);
            if (string.IsNullOrEmpty(options.InputFolder) && string.IsNullOrEmpty(options.InputFile)) 
            {
                options.GetUsage();
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
                List<Cabrillo> logList = new List<Cabrillo>();
                string folder = options.InputFolder;
                if (!folder.EndsWith("\\"))
                {
                    folder += "\\";
                }
                log.Info("Opening files from folder:" + folder);
                DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(folder));
                if (di != null)
                {
                    FileInfo[] subFiles = di.GetFiles();
                    if (subFiles.Length > 0)
                    {
                        foreach (FileInfo subFile in subFiles)
                        {
                            logList.Add(Parse(subFile, options.ignoreStartOfLogTag));
                        }
                    }
                }

                ParseQSO(logList, options.ignoreDateTimeValidation);


                List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
                startendPairs.Add(Tuple.Create(etapa1Start, etapa1End));
                startendPairs.Add(Tuple.Create(etapa2Start, etapa2End));
                List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, options.ignoreDateTimeValidation);


                List<QSO> etapa1 = etape[0];
                List<QSO> etapa2 = etape[1];


                //Cu o statie se poate lucra o singura data  in fiecare etapa, in CW sau in SSB, pe portiunea de banda rezervata modului respectiv.
                List<string> pairs = new List<string>();
                log.Info("Check OneQSO");
                Cabrillo.CheckOneQSO(etapa1);
                Cabrillo.CheckOneQSO(etapa2);
                log.Info("Done Checking OneQSO");


                foreach (var cabrilloLog in logList)
                {
                    log.Info("Check Mode QSOs:" + cabrilloLog.FileName);
                    foreach (var qso in cabrilloLog.QSOs)
                    {
                        Cabrillo.ParseFrequency(qso, options.IgnoreModeFrequencyValidation);
                    }
                    log.Info("Done Checking Mode QSOs:" + cabrilloLog.FileName);
                }


                //Schimbarea modului de lucru se poate face la un interval de cel putin 5 minute.
                foreach (var cabrilloLog in logList)
                {
                    log.Info("Check ModeChange QSOs:" + cabrilloLog.FileName);
                    Cabrillo.ModeChangeCheck(cabrilloLog);
                    log.Info("Done Checking ModeChange QSOs:" + cabrilloLog.FileName);
                }

                //In clasament vor intra doar logurile care contin minim 5 QSO – uri.
                foreach (var cabrilloLog in logList)
                {
                    log.Info("Check MinQso nr.:" + cabrilloLog.FileName);
                    if (cabrilloLog.QSOs.Count < 5)
                    {
                        log.Warn("MinQSO nr Violation:" + cabrilloLog.FileName + " ->" + cabrilloLog.QSOs.Count + "<-");
                        cabrilloLog.Valid = false;
                    }
                    log.Info("Done Checking MinQSO nr:" + cabrilloLog.FileName);
                }

                foreach (var cabrilloLog in logList)
                {
                    int total0 = (cabrilloLog.Multiplicator[0] * cabrilloLog.Etape[0].Sum(qso => qso.Score));
                    int total1 = (cabrilloLog.Multiplicator[1] * cabrilloLog.Etape[1].Sum(qso => qso.Score));
                    log.Info(cabrilloLog.FileName + "-" + cabrilloLog.CallSign + "- Etapa1:" + cabrilloLog.Multiplicator[0] + "-" + cabrilloLog.Etape[0].Sum(qso => qso.Score) + "->" + total0 + "<-" + " Etapa2:" + cabrilloLog.Multiplicator[1] + "-" + cabrilloLog.Etape[1].Sum(qso => qso.Score) + "->" + total1 + "<-");
                }

                if (Directory.Exists(Path.Combine(di.FullName, "Result")))
                {
                    Directory.Delete(Path.Combine(di.FullName, "Result"), true);
                }



                DirectoryInfo resultDi = di.CreateSubdirectory("Result");
                var total = new FileInfo(Path.Combine(resultDi.FullName, "total.txt"));

                Hashtable callsigns = new Hashtable();
                Hashtable callsignsnr = new Hashtable();


                foreach (var cabrilloLog in logList)
                {
                    callsigns[cabrilloLog.CallSign] = true;

                    foreach (QSO qso in cabrilloLog.QSOs)
                    {
                        if (!callsigns.ContainsKey(qso.CallSign2))
                        {
                            callsigns[qso.CallSign2] = false;
                        }

                        if (!callsignsnr.ContainsKey(qso.CallSign2))
                        {
                            callsignsnr[qso.CallSign2] = 1;
                        }
                        else
                        {
                            callsignsnr[qso.CallSign2] = (int)callsignsnr[qso.CallSign2] + 1;
                        }
                    }

                }

                foreach (string callsign in callsigns.Keys)
                {
                    if (!(bool)callsigns[callsign] && (int)callsignsnr[callsign] >= 5)
                    {
                        //ok so we do not have the log but we need to give points
                        foreach (var cabrilloLog in logList)
                        {
                            foreach (QSO qso in cabrilloLog.QSOs)
                            {
                                if (qso.CallSign2 == callsign)
                                {
                                    //add fake qso
                                    qso.PairQSO = new QSO() { CallSign1 = callsign, CallSign2 = qso.CallSign1, RST1 = qso.RST2, RST2 = qso.RST1, County1 = qso.County2, County2 = qso.County1, Frequency = "No Log submited", Mode = qso.Mode };
                                }
                            }
                        }
                    }
                }
                foreach (Cabrillo cabrillo in logList)
                {
                    foreach (QSO qso in cabrillo.QSOs)
                    {
                        if (qso.PairQSO == null)
                        {
                            qso.InvalidResons.Add("No Pair QSO");
                        }
                    }

                }
                using (Stream totalstream = total.OpenWrite())
                using (StreamWriter totalwriter = new StreamWriter(totalstream))
                {

                    int index = 1;

                    foreach (var cabrilloLog in logList)
                    {
                        totalwriter.WriteLine((index++) + " " + cabrilloLog.CallSign + " " + cabrilloLog.ROWQSOs.Count + " " + (cabrilloLog.Multiplicator[0] * cabrilloLog.Etape[0].Sum(qso => qso.Score) + cabrilloLog.Multiplicator[1] * cabrilloLog.Etape[1].Sum(qso => qso.Score)) + " " + cabrilloLog.Etape[0].Sum(qso => qso.Score) + " " + cabrilloLog.Multiplicator[0] + " " + cabrilloLog.Etape[1].Sum(qso => qso.Score) + " " + cabrilloLog.Multiplicator[1]);
                    }
                    totalwriter.WriteLine("No log from:");
                    foreach (string callsign in callsigns.Keys)
                    {
                        if (!(bool)callsigns[callsign])
                        {
                            totalwriter.WriteLine(callsign + " " + callsignsnr[callsign]);
                        }
                    }


                    totalwriter.WriteLine("<p>Loguri primite pana la  ora  UTC:</p>");
                    totalwriter.WriteLine("<table><tr><th>Indicativ</th><th>Numar legaturi</th></tr>");
                    foreach (var cabrilloLog in logList)
                    {
                        totalwriter.WriteLine("<tr><td>" + cabrilloLog.CallSign + "</td><td>" + cabrilloLog.ROWQSOs.Count + "</td></tr>");
                    }
                    totalwriter.WriteLine("</table>");
                    totalwriter.WriteLine("Asteptam in continuare loguri de la:");
                    totalwriter.WriteLine("<table><tr><th>Indicativ</th><th>Numar legaturi in care apare indicativul</th></tr>");
                    foreach (string callsign in callsigns.Keys)
                    {
                        if (!(bool)callsigns[callsign])
                        {
                            totalwriter.WriteLine("<tr><td>"+callsign + "</td><td>" + callsignsnr[callsign]+"</td></tr>");
                        }
                    }
                    totalwriter.WriteLine("</table>");

                }
                foreach (var cabrilloLog in logList)
                {


                    var file = new FileInfo(Path.Combine(resultDi.FullName, Path.GetFileName(cabrilloLog.FileName)));
                    if (!file.Exists) // you may not want to overwrite existing files
                    {
                        using (Stream stream = file.OpenWrite())
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine("CONTEST:" + cabrilloLog.Contest);
                            writer.WriteLine("CALLSIGN:" + cabrilloLog.CallSign);
                            writer.WriteLine("CATEGORY-ASSISTED:" + cabrilloLog.CategoryAssisted);
                            writer.WriteLine("CATEGORY-BAND:" + cabrilloLog.CategoryBand);
                            writer.WriteLine("CATEGORY-MODE:" + cabrilloLog.CategoryMode);
                            writer.WriteLine("CATEGORY-OPERATOR:" + cabrilloLog.CategoryOperator);
                            writer.WriteLine("CATEGORY-POWER:" + cabrilloLog.CategoryPower);
                            writer.WriteLine("CATEGORY-STATION:" + cabrilloLog.CategoryStation);
                            writer.WriteLine("CATEGORY-TIME:" + cabrilloLog.CategoryTime);
                            writer.WriteLine("CATEGORY-TRANSMITTER:" + cabrilloLog.CategoryTransmiter);
                            writer.WriteLine("CATEGORY-OVERLAY:" + cabrilloLog.CategoryOverlay);
                            writer.WriteLine("CLAIMED-SCORE:" + cabrilloLog.ClaimedScore);
                            writer.WriteLine("CLUB:" + cabrilloLog.Club);
                            writer.WriteLine("CREATED-BY:" + cabrilloLog.CreatedBy);
                            writer.WriteLine("OPERATORS:" + cabrilloLog.Operators);
                            writer.WriteLine("EMAIL:" + cabrilloLog.Email);
                            writer.WriteLine("LOCATION:" + cabrilloLog.Location);
                            writer.WriteLine("NAME:" + cabrilloLog.Name);
                            foreach (var addressline in cabrilloLog.Address)
                            {
                                writer.WriteLine("ADDRESS:" + addressline);
                            }
                            writer.WriteLine("ADDRESS:" + cabrilloLog.Contest);
                            writer.WriteLine("ADDRESS-CITY:" + cabrilloLog.AddressCity);
                            writer.WriteLine("ADDRESS-STATE-PROVINCE:" + cabrilloLog.AddressStateProvince);
                            writer.WriteLine("ADDRESS-POSTALCODE:" + cabrilloLog.AddressPostalCode);
                            writer.WriteLine("ADDRESS-COUNTRY:" + cabrilloLog.AddressCountry);
                            writer.WriteLine("OFFTIME:" + cabrilloLog.OffTime);
                            writer.WriteLine("DEBUG:" + cabrilloLog.Debug);
                            foreach (var soapboxline in cabrilloLog.SoapBox)
                            {
                                writer.WriteLine("SOAPBOX:" + soapboxline);
                            }
                            foreach (QSO qso in cabrilloLog.QSOs)
                            {


                                writer.WriteLine("QSO:" + qso.ROWQSO);
                                writer.WriteLine("SOAPBOX: Parsed QSO ->" + qso.Frequency + "<- ->" + qso.Mode + "<- ->" + qso.DateTime + "<- ->" + qso.CallSign1 + "<- ->" + qso.RST1 + "<- ->" + qso.Exchange1 + "<- ->" + qso.County1 + "<- ->" + qso.CallSign2 + "<- ->" + qso.RST2 + "<- ->" + qso.Exchange2 + "<- ->" + qso.County2 + "<-");
                                if (qso.PairQSO != null)
                                {
                                    writer.WriteLine("SOAPBOX:   Pair QSO " + qso.PairQSO.Frequency + "<- ->" + qso.PairQSO.Mode + "<- ->" + qso.PairQSO.DateTime + "<- ->" + qso.PairQSO.CallSign1 + "<- ->" + qso.PairQSO.RST1 + "<- ->" + qso.PairQSO.Exchange1 + "<- ->" + qso.PairQSO.County1 + "<- ->" + qso.PairQSO.CallSign2 + "<- ->" + qso.PairQSO.RST2 + "<- ->" + qso.PairQSO.Exchange2 + "<- ->" + qso.PairQSO.County2 + "<-");
                                }
                                //else 
                                //{ 
                                //    writer.WriteLine("SOAPBOX:  No Pair QSO");
                                //}

                                foreach (string reasons in qso.InvalidResons)
                                {
                                    writer.WriteLine("SOAPBOX:" + reasons);
                                }
                                writer.WriteLine("SOAPBOX: Score " + qso.Score);
                            }
                            writer.WriteLine("SOAPBOX: Multiplicator Etapa1 " + cabrilloLog.Multiplicator[0]);
                            writer.WriteLine("SOAPBOX: Multiplicator Etapa2 " + cabrilloLog.Multiplicator[1]);

                            writer.WriteLine("SOAPBOX: Scor Etapa1 " + cabrilloLog.Multiplicator[0] * cabrilloLog.Etape[0].Sum(qso => qso.Score));
                            writer.WriteLine("SOAPBOX: Scor Etapa2 " + cabrilloLog.Multiplicator[1] * cabrilloLog.Etape[1].Sum(qso => qso.Score));

                            writer.WriteLine("SOAPBOX: Scor " + (cabrilloLog.Multiplicator[0] * cabrilloLog.Etape[0].Sum(qso => qso.Score) + cabrilloLog.Multiplicator[1] * cabrilloLog.Etape[1].Sum(qso => qso.Score)));
                        }

                    }
                }




                Console.WriteLine("Done.");
            }
            if (!string.IsNullOrEmpty(options.InputFile)) 
            {
                List<Cabrillo> logList = new List<Cabrillo>();
                logList.Add(Parse(new FileInfo(options.InputFile), options.ignoreStartOfLogTag ));
                ParseQSO(logList, options.ignoreDateTimeValidation );
                List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
                startendPairs.Add(Tuple.Create(etapa1Start, etapa1End));
                startendPairs.Add(Tuple.Create(etapa2Start, etapa2End));
                List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, options.ignoreDateTimeValidation);
                foreach (List<QSO> etapa in etape)
                {
                    Cabrillo.CheckOneQSO(etapa);
                }

                foreach (var q in logList[0].QSOs)
                {
                    Cabrillo.ParseFrequency(q, options.IgnoreModeFrequencyValidation);
                }
                Cabrillo.ModeChangeCheck(logList[0]);
            }
            Console.ReadKey();
        }






        public static void ParseQSO(List<Cabrillo> list, bool ignoreDateTimeValidation)
        {
            foreach (Cabrillo cabrilloLog in list)
            {
                log.Info("Parsing QSO:" + cabrilloLog.FileName);
                cabrilloLog.ParseQSO(ignoreDateTimeValidation);
                log.Info("Done Parsing QSO:" + cabrilloLog.FileName);
            }
        }

        public static Cabrillo Parse(FileInfo subFile, bool ignoreStartOfLogTag)
        {
            Cabrillo result;
            log.Info("Opening File:" + subFile.FullName);
            FileStream fs = subFile.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            result = Parse(fs, subFile.FullName, ignoreStartOfLogTag);
            fs.Close();
            return result;
        }

        public static Cabrillo Parse(Stream fs, string fullName, bool ignoreStartOfLogTag)
        {
            log.Info("Parsing:" + fullName);
          
            Cabrillo c = Cabrillo.Parse(fullName, fs, ignoreStartOfLogTag);
            foreach (string invalidLine in c.InvalidLines)
            {
                log.Warn("Invalid tag:" + c.FileName + " ->" + invalidLine + "<-");
            }
            return c;
        }
    }
}
