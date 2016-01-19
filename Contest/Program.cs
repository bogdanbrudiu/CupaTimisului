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


        public sealed class Options {
            #region Standard Option Attribute

            [Option("CupaTimisului", DefaultValue = true,
             HelpText = "CupaTimisului", MutuallyExclusiveSet = "ZiuaTelecomunicatiilor")]
            public bool CupaTimisului { get; set; }

            [Option("ZiuaTelecomunicatiilor", DefaultValue = false,
             HelpText = "Ziua Telecomunicatiilor", MutuallyExclusiveSet = "CupaTimisului")]
            public bool ZiuaTelecomunicatiilor { get; set; }


            [Option('c', "check", DefaultValue = "",
                    HelpText = "Input file with data to process.", MutuallyExclusiveSet = "InputFolder")]
            public string InputFile { get; set; }

            [Option('e', "evaluate", DefaultValue = "",
                        HelpText = "Folder with files to evaluate", MutuallyExclusiveSet = "InputFile")]
            public string InputFolder { get; set; }

            [Option('f', "ignoreModeFrequencyValidation", DefaultValue = false,
                      HelpText = "Ignore Mode Frequency Validation")]
            public bool IgnoreModeFrequencyValidation { get; set; }

            [Option('t', "ignoreDateTimeValidation", DefaultValue = false,
              HelpText = "Ignore Date and Hour Validation, only minutes are taked into account")]
            public bool IgnoreDateTimeValidation { get; set; }

            [Option('i', "ignoreCallsigns", DefaultValue = "",
                HelpText = "Ignore Callsigns fro still waiting")]
            public string IgnoreCallsigns { get; set; }


            [Option('l', "ignoreStartOfLogTag", DefaultValue = false,
              HelpText = "Ignore StartOfLog Tag")]
            public bool IgnoreStartOfLogTag { get; set; }

            [Option('v', "verbose", DefaultValue = 2,
                    HelpText = "Verbose level. Range: from 0 to 2.")]
            public int? VerboseLevel { get; set; }

            [Option("etapa1Start",
           HelpText = "etapa1Start")]
            public DateTime Etapa1Start { get; set; }
            [Option("etapa1End",
          HelpText = "etapa1End")]
            public DateTime Etapa1End { get; set; }

            [Option("etapa2Start", HelpText = "etapa2Start")]
            public DateTime Etapa2Start { get; set; }
            [Option("etapa2End", HelpText = "etapa2End")]
            public DateTime Etapa2End { get; set; }
            #endregion

            #region Specialized Option Attribute

            [HelpOption(
                    HelpText = "Dispaly this help screen.")]
            public string GetUsage()
            {
                var help = new HelpText(Program._headingInfo);
                help.AdditionalNewLineAfterOption = true;
                help.Copyright = new CopyrightInfo("Bogdan-Ioan BRUDIU", 2011, 2015);
                try
                {
                    HelpText.DefaultParsingErrorsHandler(this, help);
                }
                catch { }
                help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
                help.AddPreOptionsLine("the MIT License <http://www.opensource.org/licenses/mit-license.php>.");
                help.AddPreOptionsLine("Usage: SampleApp -cfile.cbr");
                help.AddPreOptionsLine("       SampleApp -ed:\\Test -v0");
                help.AddOptions(this);

                return help;
            }

            [ParserState]
            public IParserState LastParserState { get; set; }

            #endregion
        }


        public static int ScorCupaTimisului(QSO qso)
        {
            if (qso.CallSign2.ToLower() == "yo2kqt")//joker
            {
                return 10;
            }
            if (qso.County1 == qso.County2 && qso.County1.ToLower() == "tm")//tm-tm
            {
                return 1;
            }
            if (qso.County1 != qso.County2 && qso.County2.ToLower() == "tm")//yo-tm
            {
                return 4;
            }
            if (qso.County1 != qso.County2 && qso.County1.ToLower() == "tm")//tm-yo
            {
                return 4;// 2;
            }
            if (qso.County1 != qso.County2 && qso.County2.ToLower() != "tm")//yo-yo
            {
                return 2;
            }
            if (qso.County1 == qso.County2 && qso.County1.ToLower() != "tm")//acelasi judet
            {
                return 1;
            }
            return 0;
        }

        public static int ScorZT(QSO qso)
        {
          
            if (qso.County1 == qso.County2 && qso.County1.ToLower() == "tc")//tc-tc
            {
                return 4;
            }
            if (qso.County1 != qso.County2 && qso.County2.ToLower() == "tc")//yo-tc
            {
                return 4;
            }
            if (qso.County1 != qso.County2 && qso.County1.ToLower() == "tc")//tc-yo
            {
                return 2;
            }
            if (qso.County1 != qso.County2 && qso.County2.ToLower() != "tc")//yo-yo
            {
                return 2;
            }
            if (qso.County1 == qso.County2 && qso.County1.ToLower() != "tc")//acelasi judet
            {
                return 2;
            }
            return 0;
        }
        //public static DateTime etapa1Start = new DateTime(2015, 12, 20, 14, 00, 00);
        //public static DateTime etapa1End = new DateTime(2015, 12, 20, 15, 00, 00);

        //public static DateTime etapa2Start = new DateTime(2015, 12, 20, 15, 00, 00);
        //public static DateTime etapa2End = new DateTime(2015, 12, 20, 16, 00, 00);



        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

          


            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
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
                ProcessFolder(options);
            }
            if (!string.IsNullOrEmpty(options.InputFile)) 
            {
                List<Cabrillo> logList = new List<Cabrillo>();
                logList.Add(Parse(new FileInfo(options.InputFile), options.IgnoreStartOfLogTag ));
                if (options.CupaTimisului)
                {
                    ParseQSO(logList, options.IgnoreDateTimeValidation, ScorCupaTimisului);
                }
                if (options.ZiuaTelecomunicatiilor)
                {
                    ParseQSO(logList, options.IgnoreDateTimeValidation, ScorZT);
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

        public static void ProcessFolder(Options options)
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
                        Cabrillo newlog = Parse(subFile, options.IgnoreStartOfLogTag);
                        Cabrillo existing = logList.Find(x => x.CallSign.Equals(newlog.CallSign));
                        if (existing!=null && new FileInfo(existing.FileName).CreationTime < subFile.CreationTime) {
                            logList.Remove(existing);
                            existing = null;

                        }

                        if (existing == null) {
                            logList.Add(newlog);
                        }
                        
                    }
                }
            }
            if (options.CupaTimisului)
            {
                ParseQSO(logList, options.IgnoreDateTimeValidation, ScorCupaTimisului);
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                ParseQSO(logList, options.IgnoreDateTimeValidation, ScorZT);
            }
            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            //startendPairs.Add(Tuple.Create(etapa1Start, etapa1End));
            //startendPairs.Add(Tuple.Create(etapa2Start, etapa2End));

            startendPairs.Add(Tuple.Create(options.Etapa1Start, options.Etapa1End));
            startendPairs.Add(Tuple.Create(options.Etapa2Start, options.Etapa2End));
            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, options.IgnoreDateTimeValidation);


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

            if (options.CupaTimisului) {
                //Schimbarea modului de lucru se poate face la un interval de cel putin 5 minute.
                foreach (var cabrilloLog in logList)
                {
                    log.Info("Check ModeChange QSOs:" + cabrilloLog.FileName);
                    Cabrillo.ModeChangeCheck(cabrilloLog);
                    log.Info("Done Checking ModeChange QSOs:" + cabrilloLog.FileName);
                }
            }
            if (options.CupaTimisului)
            {
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
            var totalhtml = new FileInfo(Path.Combine(resultDi.FullName, "total.cshtml"));
            Hashtable callsigns = new Hashtable();
            Hashtable callsignsnr = new Hashtable();


            foreach (var cabrilloLog in logList)
            {
                callsigns[cabrilloLog.CallSign.ToUpper()] = true;

                foreach (QSO qso in cabrilloLog.QSOs)
                {
                    if (!callsigns.ContainsKey(qso.CallSign2.ToUpper()))
                    {
                        callsigns[qso.CallSign2.ToUpper()] = false;
                    }

                    if (!callsignsnr.ContainsKey(qso.CallSign2.ToUpper()))
                    {
                        callsignsnr[qso.CallSign2.ToUpper()] = 1;
                    }
                    else
                    {
                        callsignsnr[qso.CallSign2.ToUpper()] = (int)callsignsnr[qso.CallSign2.ToUpper()] + 1;
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
                            if (qso.CallSign2.ToUpper() == callsign)
                            {
                                //add fake qso
                                if (options.CupaTimisului) { 
                                    qso.PairQSO = new QSO(ScorCupaTimisului) { CallSign1 = callsign, CallSign2 = qso.CallSign1.ToUpper(), RST1 = qso.RST2, RST2 = qso.RST1, County1 = qso.County2, County2 = qso.County1, Frequency = "No Log submited", Mode = qso.Mode };
                                }
                                if (options.ZiuaTelecomunicatiilor)
                                {
                                    qso.PairQSO = new QSO(ScorZT) { CallSign1 = callsign, CallSign2 = qso.CallSign1.ToUpper(), RST1 = qso.RST2, RST2 = qso.RST1, County1 = qso.County2, County2 = qso.County1, Frequency = "No Log submited", Mode = qso.Mode };
                                }
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
                string[] ignorelist=options.IgnoreCallsigns.Replace(" ", "").Split(',');
                
                foreach (string callsign in callsigns.Keys)
                {
                    if (!(bool)callsigns[callsign] && !ignorelist.Contains(callsign))
                    {
                        totalwriter.WriteLine(callsign + " " + callsignsnr[callsign]);
                    }
                }
            }

            using (Stream totalstream = totalhtml.OpenWrite())
            using (StreamWriter totalwriter = new StreamWriter(totalstream))
            {
                totalwriter.WriteLine("<p>Loguri primite pana la  ora  UTC:" + DateTime.Now.ToUniversalTime() + "</p>");

                totalwriter.WriteLine("<table><tr><th>Indicativ</th><th>Numar legaturi</th></tr>");
                foreach (var cabrilloLog in logList)
                {
                    totalwriter.WriteLine("<tr><td>" + cabrilloLog.CallSign + "</td><td>" + cabrilloLog.ROWQSOs.Count + "</td></tr>");
                }
                totalwriter.WriteLine("</table>");
                totalwriter.WriteLine("Asteptam in continuare loguri de la:");
                totalwriter.WriteLine("<table><tr><th>Indicativ</th><th>Numar legaturi in care apare indicativul</th></tr>");
                string[] ignorelist = options.IgnoreCallsigns.Replace(" ", "").Split(',');
                foreach (string callsign in callsigns.Keys)
                {
                    if (!(bool)callsigns[callsign] && !ignorelist.Contains(callsign))
                    {
                        totalwriter.WriteLine("<tr><td>" + callsign + "</td><td>" + callsignsnr[callsign] + "</td></tr>");
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

        public static void ParseQSO(List<Cabrillo> list, bool ignoreDateTimeValidation, QSO.CalculateScore myScore )
        {
            foreach (Cabrillo cabrilloLog in list)
            {
                log.Info("Parsing QSO:" + cabrilloLog.FileName);
                cabrilloLog.ParseQSO(ignoreDateTimeValidation, myScore);
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
