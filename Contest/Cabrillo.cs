using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using log4net;
using System.Diagnostics;
using CommandLine;

namespace Contest
{
   
    public sealed class Options
    {
        #region Standard Option Attribute

        [Option("CupaTimisului", Default = true,
         HelpText = "CupaTimisului")]
        public bool CupaTimisului { get; set; }

        [Option("ZiuaTelecomunicatiilor", Default = false,
         HelpText = "Ziua Telecomunicatiilor")]
        public bool ZiuaTelecomunicatiilor { get; set; }


        [Option('c', "check", Default = "",
                HelpText = "Input file with data to process.")]
        public string InputFile { get; set; }

        [Option('e', "evaluate", Default = "",
                    HelpText = "Folder with files to evaluate")]
        public string InputFolder { get; set; }

        [Option('f', "ignoreModeFrequencyValidation", Default = false,
                  HelpText = "Ignore Mode Frequency Validation")]
        public bool IgnoreModeFrequencyValidation { get; set; }

        [Option('t', "ignoreDateTimeValidation", Default = false,
          HelpText = "Ignore Date and Hour Validation, only minutes are taked into account")]
        public bool IgnoreDateTimeValidation { get; set; }

        [Option('i', "ignoreCallsigns", Default = "",
            HelpText = "Ignore Callsigns fro still waiting")]
        public string IgnoreCallsigns { get; set; }


        [Option('l', "ignoreStartOfLogTag", Default = false,
          HelpText = "Ignore StartOfLog Tag")]
        public bool IgnoreStartOfLogTag { get; set; }

        [Option('v', "verbose", Default = 2,
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


    }
    public class QSO 
    {
        
        public QSO(CalculateScore myScore) 
        {
            InvalidResons = new ArrayList();
            this.myScore = myScore;
        }
        
        public bool Valid 
        {
            get { return InvalidResons.Count == 0; }
        }
        public void IsInvalid(string txt) 
        {
            InvalidResons.Add(txt);
        }
        public ArrayList InvalidResons { get; set; }
        public string Frequency { get; set; }
        public string Mode { get; set; }
        public DateTime DateTime { get; set; }
        public string CallSign1 { get; set; }
        public string RST1 { get; set; }
        public string Exchange1 { get; set; }
        public string County1 { get; set; }
        public string CallSign2 { get; set; }
        public string RST2 { get; set; }
        public string Exchange2 { get; set; }
        public string County2 { get; set; }
        public QSO PairQSO { get; set; }
        public Cabrillo Log { get; set; }
        public string ROWQSO { get; set; }
        public delegate int CalculateScore(QSO qso);
        private CalculateScore myScore;
        public int Score 
        {
            get
            {
                if (Valid && PairQSO!=null)
                {

                    return myScore(this);
                }
                return 0;
            }
        }
        
      

        public int Etapa { get; set; }
    }

    public class Cabrillo
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Cabrillo));
        public static string START_OF_LOG = "START-OF-LOG";
        public static string END_OF_LOG = "END-OF-LOG";
        public static string CALLSIGN = "CALLSIGN";
        public static string CATEGORY_ASSISTED = "CATEGORY-ASSISTED";
        public static string CATEGORY_BAND = "CATEGORY-BAND";
        public static string CATEGORY_MODE = "CATEGORY-MODE";
        public static string CATEGORY_OPERATOR = "CATEGORY-OPERATOR";
        public static string CATEGORY_POWER = "CATEGORY-POWER";
        public static string CATEGORY_STATION = "CATEGORY-STATION";
        public static string CATEGORY_TIME = "CATEGORY-TIME";
        public static string CATEGORY_TRANSMITTER = "CATEGORY-TRANSMITTER";
        public static string CATEGORY_OVERLAY = "CATEGORY-OVERLAY";
        public static string CLAIMED_SCORE = "CLAIMED-SCORE";
        public static string CLUB = "CLUB";
        public static string CONTEST = "CONTEST";
        public static string CREATED_BY = "CREATED-BY";
        public static string EMAIL = "EMAIL";
        public static string LOCATION = "LOCATION";
        public static string NAME = "NAME";
        public static string ADDRESS = "ADDRESS";
        public static string ADDRESS_CITY = "ADDRESS-CITY";
        public static string ADDRESS_STATE_PROVINCE = "ADDRESS-STATE-PROVINCE";
        public static string ADDRESS_POSTALCODE = "ADDRESS-POSTALCODE";
        public static string ADDRESS_COUNTRY = "ADDRESS-COUNTRY";
        public static string OPERATORS = "OPERATORS";
        public static string OFFTIME = "OFFTIME";
        public static string SOAPBOX = "SOAPBOX";
        public static string QSO = "QSO";
        public static string DEBUG = "DEBUG";


        public Cabrillo() 
        {
            InvalidLines = new List<string>();
            ROWQSOs = new List<string>();
            QSOs = new List<QSO>();
            Etape = new List<List<QSO>>();
            Address = new List<string>();
            SoapBox = new List<string>();
            Valid = true;
        }

        private bool valid = true;
        public bool Valid 
        {
            get 
            {
                return valid;
            }
            set
            {
                valid=value;
                if(!valid)
                {
                    foreach(QSO qso in QSOs)
                    {
                        qso.IsInvalid("Log is invalid");
                        if (qso.PairQSO != null) 
                        {
                            qso.PairQSO.IsInvalid("Log is invalid");
                        }
                    }
                }
            }
        }
        public string FileName { get; set; }
        private string callsign;
        public string CallSign 
        {
            get 
            {
                return callsign;
            }
            set 
            {
                if (value != null)
                {
                    callsign = ((string)value).Trim();
                }
                else 
                {
                    callsign = null;
                }
            } 
        }
        public string CategoryAssisted { get; set; }
        public string CategoryBand { get; set; }
        public string CategoryMode { get; set; }
        public string CategoryOperator { get; set; }
        public string CategoryPower { get; set; }
        public string CategoryStation { get; set; }
        public string CategoryTime { get; set; }
        public string CategoryTransmiter { get; set; }
        public string CategoryOverlay { get; set; }
        public string ClaimedScore { get; set; }
        public string Club { get; set; }
        public string Contest { get; set; }
        public string CreatedBy { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public IList<string> Address { get; set; }
        public string AddressCity { get; set; }
        public string AddressStateProvince { get; set; }
        public string AddressPostalCode { get; set; }
        public string AddressCountry { get; set; }
        public string Operators { get; set; }
        public string OffTime { get; set; }
        public IList<string> SoapBox { get; set; }
        public List<string> ROWQSOs { get; set; }
        public List<string> InvalidLines { get; set; }
        public List<QSO> QSOs { get; set; }
        public List<List<QSO>> Etape { get; set; }
        public string Debug { get; set; }

        public List<int> Multiplicator
        {
            get 
            {
                List<int> result = new List<int>();
                foreach (List<QSO> etapa in Etape)
                {
                    ArrayList multi=new ArrayList();
                    ArrayList multitm = new ArrayList();
                    foreach (var item in etapa.Where(qso => qso.PairQSO != null && qso.Valid && qso.County2 != qso.County1 && qso.County2.ToLower() != "tm"))
	                {
                        if (!multi.Contains(item.County2)) 
                        {
                            multi.Add(item.County2);
                        }
	                }
                    foreach (var item in etapa.Where(qso => qso.PairQSO != null && qso.Valid && qso.County2 != qso.County1 && qso.County2.ToLower() == "tm"))
                    {
                        if (!multi.Contains(item.CallSign2))
                        {
                            multitm.Add(item.CallSign2);
                        }
                    }
                    result.Add(multi.Count + multitm.Count);
                }
                return result;
            }
        }

        public static Cabrillo Parse(string fullName, Stream fs, bool ignoreStartOfLogTag)
        {


            StreamReader r = new StreamReader(fs);
            Cabrillo c = new Cabrillo();
            c.FileName = fullName;
            bool started = false;

            if (ignoreStartOfLogTag)
            {
                started = true;
            }

            string line;
            while ((line = r.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                if (line.StartsWith(Cabrillo.END_OF_LOG))
                    break;
                if (line.StartsWith(Cabrillo.START_OF_LOG))
                {
                    started = true;
                    continue;
                }
                if (!started)
                {
                    continue;
                }

                if (line.StartsWith(Cabrillo.QSO))
                {
                    c.ROWQSOs.Add(line.Substring(Cabrillo.QSO.Length + 1));
                    continue;
                }
                if (line.StartsWith(Cabrillo.CALLSIGN))
                {
                    c.CallSign = line.Substring(Cabrillo.CALLSIGN.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CATEGORY_ASSISTED))
                {
                    c.CategoryAssisted = line.Substring(Cabrillo.CATEGORY_ASSISTED.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CATEGORY_BAND))
                {
                    c.CategoryBand = line.Substring(Cabrillo.CATEGORY_BAND.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CATEGORY_MODE))
                {
                    c.CategoryMode = line.Substring(Cabrillo.CATEGORY_MODE.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CATEGORY_OPERATOR))
                {
                    c.CategoryOperator = line.Substring(Cabrillo.CATEGORY_OPERATOR.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CATEGORY_POWER))
                {
                    c.CategoryPower = line.Substring(Cabrillo.CATEGORY_POWER.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CATEGORY_STATION))
                {
                    c.CategoryStation = line.Substring(Cabrillo.CATEGORY_STATION.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CATEGORY_TIME))
                {
                    c.CategoryTime = line.Substring(Cabrillo.CATEGORY_TIME.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CATEGORY_TRANSMITTER))
                {
                    c.CategoryTransmiter = line.Substring(Cabrillo.CATEGORY_TRANSMITTER.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CATEGORY_OVERLAY))
                {
                    c.CategoryOverlay = line.Substring(Cabrillo.CATEGORY_OVERLAY.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CLAIMED_SCORE))
                {
                    c.ClaimedScore = line.Substring(Cabrillo.CLAIMED_SCORE.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CLUB))
                {
                    c.Club = line.Substring(Cabrillo.CLUB.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CONTEST))
                {
                    c.Contest = line.Substring(Cabrillo.CONTEST.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.CREATED_BY))
                {
                    c.CreatedBy = line.Substring(Cabrillo.CREATED_BY.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.EMAIL))
                {
                    c.Email = line.Substring(Cabrillo.EMAIL.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.LOCATION))
                {
                    c.Location = line.Substring(Cabrillo.LOCATION.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.NAME))
                {
                    c.Name = line.Substring(Cabrillo.NAME.Length + 1);
                    continue;
                }
              
                if (line.StartsWith(Cabrillo.ADDRESS_CITY))
                {
                    c.AddressCity = line.Substring(Cabrillo.ADDRESS_CITY.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.ADDRESS_STATE_PROVINCE))
                {
                    c.AddressStateProvince = line.Substring(Cabrillo.ADDRESS_STATE_PROVINCE.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.ADDRESS_POSTALCODE))
                {
                    c.AddressPostalCode = line.Substring(Cabrillo.ADDRESS_POSTALCODE.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.ADDRESS_COUNTRY))
                {
                    c.AddressCountry = line.Substring(Cabrillo.ADDRESS_COUNTRY.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.ADDRESS))
                {
                    c.Address.Add(line.Substring(Cabrillo.ADDRESS.Length + 1));
                    continue;
                }
                if (line.StartsWith(Cabrillo.OPERATORS))
                {
                    c.Operators = line.Substring(Cabrillo.OPERATORS.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.OFFTIME))
                {
                    c.OffTime = line.Substring(Cabrillo.OFFTIME.Length + 1);
                    continue;
                }
                if (line.StartsWith(Cabrillo.SOAPBOX))
                {
                    c.SoapBox.Add(line.Substring(Cabrillo.SOAPBOX.Length + 1));
                    continue;
                }
                if (line.StartsWith(Cabrillo.DEBUG))
                {
                    c.Debug = line.Substring(Cabrillo.DEBUG.Length + 1);
                    continue;
                }
                c.InvalidLines.Add(line);
            }
            return c;
        }
        
         public void ParseQSO(bool ignoreDateTimeValidation, QSO.CalculateScore myScore)
         {
             ParseQSO(ignoreDateTimeValidation,myScore, new DateTime(2015, 12, 20, 00, 00, 00));
         }
         public void ParseQSO(bool ignoreDateTimeValidation, QSO.CalculateScore myScore, DateTime dtignoreDateTimeValidation)
        {
            foreach (string rowqso in this.ROWQSOs)
            {

                QSO qso = new QSO(myScore);
                qso.Log = this;
                qso.ROWQSO = rowqso;
                Regex emailregex = new Regex(@"\s*(?<Frequency>[^\s]*)\s*(?<Mode>[^\s]*)\s*(?<Date>\d*[-,.]\d*[-,.]\d*)\s*(?<HH>\d\d).?(?<MM>\d\d)\s*(?<CallSign1>[^\s]*)\s*(?<RST1>[\d]*)\s*(?<Exchange1>[\d]*)\s?(?<County1>[^\s]*)\s*(?<CallSign2>[^\s]*)\s*(?<RST2>[\d]*)\s*(?<Exchange2>[\d]*)\s?(?<County2>[^\s]*)");
                Match matches = emailregex.Match(rowqso);
                qso.Frequency = matches.Groups["Frequency"].Value;
                qso.Mode = matches.Groups["Mode"].Value;
                DateTime dt = new DateTime();
                if (ignoreDateTimeValidation) 
                {
                    if (!DateTime.TryParseExact(dtignoreDateTimeValidation.ToString("yyyy-MM-dd") + " " + matches.Groups["HH"].Value + matches.Groups["MM"].Value, "yyyy-MM-dd HHmm", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dt))
                    {
                        log.Error("Invalid DateTime: " + this.FileName + " ->" + matches.Groups["Date"].Value + " " + matches.Groups["HH"].Value + matches.Groups["MM"].Value + "<-");
                        qso.IsInvalid("Invalid DateTime: ->" + matches.Groups["Date"].Value + " " + matches.Groups["HH"].Value + matches.Groups["MM"].Value + "<-");
                    }
                }
                else
                {
                    if (!DateTime.TryParseExact(matches.Groups["Date"].Value + " " + matches.Groups["HH"].Value + matches.Groups["MM"].Value, "yyyy-MM-dd HHmm", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dt))
                    {
                        log.Error("Invalid DateTime: " + this.FileName + " ->" + matches.Groups["Date"].Value + " " + matches.Groups["HH"].Value + matches.Groups["MM"].Value + "<-");
                        qso.IsInvalid("Invalid DateTime: ->" + matches.Groups["Date"].Value + " " + matches.Groups["HH"].Value + matches.Groups["MM"].Value + "<-");
                    }
                }
             
                qso.CallSign1 = matches.Groups["CallSign1"].Value;
                if (string.IsNullOrEmpty(qso.CallSign1))
                {
                    log.Error("Invalid CallSign1: " + this.FileName + " ->" + matches.Groups["CallSign1"].Value + "<-");
                    qso.IsInvalid("Invalid CallSign1: ->" + matches.Groups["CallSign1"].Value + "<-");
                }

                qso.CallSign2 = matches.Groups["CallSign2"].Value;
                if (string.IsNullOrEmpty(qso.CallSign2))
                {
                    log.Error("Invalid CallSign2: " + this.FileName + " ->" + matches.Groups["CallSign2"].Value + "<-");
                    qso.IsInvalid("Invalid CallSign2: ->" + matches.Groups["CallSign2"].Value + "<-");
                }


                qso.RST1 = matches.Groups["RST1"].Value;
                if (string.IsNullOrEmpty(qso.RST1))
                {
                    log.Error("Invalid RST1: " + this.FileName + " ->" + matches.Groups["RST1"].Value + "<-");
                    qso.IsInvalid("Invalid RST1: ->" + matches.Groups["RST1"].Value + "<-");
                }

                qso.RST2 = matches.Groups["RST2"].Value;
                if (string.IsNullOrEmpty(qso.RST2))
                {
                    log.Error("Invalid RST2: " + this.FileName + " ->" + matches.Groups["RST2"].Value + "<-");
                    qso.IsInvalid("Invalid RST2: ->" + matches.Groups["RST2"].Value + "<-");
                }

                qso.Exchange1 = matches.Groups["Exchange1"].Value;
                if (string.IsNullOrEmpty(qso.Exchange1))
                {
                    log.Error("Invalid Exchange1: " + this.FileName + " ->" + matches.Groups["Exchange1"].Value + "<-");
                    qso.IsInvalid("Invalid Exchange1: ->" + matches.Groups["Exchange1"].Value + "<-");
                }


                qso.Exchange2 = matches.Groups["Exchange2"].Value;
                if (string.IsNullOrEmpty(qso.Exchange2))
                {
                    log.Error("Invalid Exchange2: " + this.FileName + " ->" + matches.Groups["Exchange2"].Value + "<-");
                    qso.IsInvalid("Invalid Exchange2: ->" + matches.Groups["Exchange2"].Value + "<-");
                }

                qso.County1 = matches.Groups["County1"].Value;
                if (string.IsNullOrEmpty(qso.County1))
                {
                    log.Error("Invalid County1: " + this.FileName + " ->" + matches.Groups["County1"].Value + "<-");
                    qso.IsInvalid("Invalid County1: ->" + matches.Groups["County1"].Value + "<-");
                }


                qso.County2 = matches.Groups["County2"].Value;
                if (string.IsNullOrEmpty(qso.County2))
                {
                    log.Error("Invalid County2: " + this.FileName + " ->" + matches.Groups["County2"].Value + "<-");
                    qso.IsInvalid("Invalid County2: ->" + matches.Groups["County2"].Value + "<-");
                }
                

                qso.DateTime = dt;
                this.QSOs.Add(qso);
            }
        }

        public static List<List<QSO>> ProcessQSOs(List<Cabrillo> logList, List<Tuple<DateTime, DateTime>> startendPairs, bool ignoreDateTimeValidation)
        {
            List<List<QSO>> etape = new List<List<QSO>>();
            foreach (var item in startendPairs)
            {
                etape.Add(new List<QSO>());
            }




            foreach (var cabrilloLog in logList)
            {
                foreach (var item in startendPairs)
                {
                    cabrilloLog.Etape.Add(new List<QSO>());
                }
                foreach (var qso in cabrilloLog.QSOs)
                {
                    int index = 0;
                    bool sw = false;
                    foreach (Tuple<DateTime, DateTime> etapa in startendPairs)
                    {
                        if ((qso.DateTime >= etapa.Item1 && qso.DateTime < etapa.Item2) || (ignoreDateTimeValidation && qso.DateTime.AddHours(-2) >= etapa.Item1 && qso.DateTime.AddHours(-2) < etapa.Item2))
                        {
                            qso.Etapa = index;
                            etape[index].Add(qso);
                            cabrilloLog.Etape[index].Add(qso);
                            sw = true;
                            break;
                        }
                        index++;
                    }
                    if (!sw)
                    {
                        log.Error("Invalid QSOs DateTime out of bounds:" + cabrilloLog.FileName + " ->" + qso.DateTime + "<-");

                        string between = "";
                        foreach (Tuple<DateTime, DateTime> etapa in startendPairs)
                        {
                            between += etapa.Item1 + "-" + etapa.Item2 + " ";
                        }
                        qso.IsInvalid("Invalid QSOs DateTime out of bounds: ->" + qso.DateTime + "<- should be between " + between);
                    }

                }
            }






            foreach (var cabrilloLog in logList)
            {
                foreach (var item in startendPairs)
                {
                    cabrilloLog.Etape.Add(new List<QSO>());
                }
                foreach (var qso in cabrilloLog.QSOs)
                {
                  



                    foreach (var cabrilloLog_ in logList)
                    {
                        if (cabrilloLog_.CallSign == qso.CallSign2)
                        {
                            foreach (var qso_ in cabrilloLog_.QSOs)
                            {
                               
                                if (qso.Etapa==qso_.Etapa 
                                    && qso.PairQSO == null 
                                    && qso_.PairQSO == null 
                                    && qso.CallSign1.ToUpper() == qso_.CallSign2.ToUpper()
                                    && qso.Mode == qso_.Mode
                                    && Math.Abs((qso.DateTime - qso_.DateTime).TotalMinutes) < 5
                                    )
                                {
                                    qso.PairQSO = qso_;
                                    qso_.PairQSO = qso;
                                    if (qso.RST1 != qso_.RST2 ||
                                        qso.Exchange1.TrimStart(new char[] { '0' }) != qso_.Exchange2.TrimStart(new char[] { '0' }) 
                                        )
                                    {
                                        log.Error("RST or Exchange are not correct: " + qso_.Log.FileName + " - " + qso.Log.FileName + " ->" + qso_.ROWQSO + " - " + qso.ROWQSO + "<-");
                                        qso_.IsInvalid("RST or Exchange are not correct: ->" + qso_.RST2 + " - " + qso.RST1 + " " + qso_.Exchange2 + " - " + qso.Exchange1 + "<-");
                                    }
                                    if (qso.RST2 != qso_.RST1 ||
                                        qso.Exchange2.TrimStart(new char[] { '0' }) != qso_.Exchange1.TrimStart(new char[] { '0' })
                                     )
                                    {
                                        log.Error("RST or Exchange are not correct: " + qso.Log.FileName + " - " + qso_.Log.FileName + " ->" + qso.ROWQSO + " - " + qso_.ROWQSO + "<-");
                                        qso.IsInvalid("RST or Exchange are not correct: ->" + qso.RST2 + " - " + qso_.RST1 + " " + qso.Exchange2 + " - " + qso_.Exchange1 + "<-");
                                    }

                                    if (qso.County1.ToUpper() != qso_.County2.ToUpper())
                                    {
                                        log.Error("County is not correct: " + qso_.Log.FileName + " - " + qso.Log.FileName + " ->" + qso_.ROWQSO + " - " + qso.ROWQSO + "<-");
                                        qso_.IsInvalid("County is not correct: ->" + qso_.County2 + " - " + qso.County1+ "<-");
                                    }
                                    if (qso.County2.ToUpper() != qso_.County1.ToUpper())
                                    {
                                        log.Error("County is not correct: " + qso.Log.FileName + " - " + qso_.Log.FileName + " ->" + qso.ROWQSO + " - " + qso_.ROWQSO + "<-");
                                        qso.IsInvalid("County is not correct: ->" + qso.County2 + " - " + qso_.County1+"<-");
                                    }


                                    if (!ignoreDateTimeValidation)
                                    {
                                        if (qso.DateTime.Subtract(qso_.DateTime).Duration().CompareTo(new TimeSpan(0, 5, 0)) == 1)
                                        {
                                            log.Error("More then 5 min difference: " + qso.Log.FileName + " - " + qso_.Log.FileName + " ->" + qso.ROWQSO + " - " + qso_.ROWQSO + "<-");
                                            qso_.IsInvalid("More then 5 min difference: ->" + qso_.DateTime + " - " + qso.DateTime + "<-");
                                            qso.IsInvalid("More then 5 min difference: ->" + qso.DateTime + " - " + qso_.DateTime + "<-");
                                        }
                                    }
                                    else
                                    {
                                        if (Math.Abs( qso.DateTime.Minute-qso_.DateTime.Minute) > 5)
                                        {
                                            if ((qso.DateTime.Minute < qso_.DateTime.Minute) ? Math.Abs(qso.DateTime.Minute+60 - qso_.DateTime.Minute) > 5 : Math.Abs(qso.DateTime.Minute - qso_.DateTime.Minute-60) > 5)
                                            {
                                            log.Error("More then 5 min difference: " + qso.Log.FileName + " - " + qso_.Log.FileName + " ->" + qso.ROWQSO + " - " + qso_.ROWQSO + "<-");
                                            qso_.IsInvalid("More then 5 min difference: ->" + qso_.DateTime + " - " + qso.DateTime + "<-");
                                            qso.IsInvalid("More then 5 min difference: ->" + qso.DateTime + " - " + qso_.DateTime + "<-");
                                            }
                                        }
                                    }
                                    break;
                                }

                            }
                        }
                    }
                   
                }
            }





          


            return etape;
        }

        public static void CheckOneQSO(List<QSO> etapa1)
        {
            List<string> pairs = new List<string>();
            foreach (QSO qso in etapa1)
            {
                var signature = qso.CallSign1 + "-" + qso.CallSign2 + "-" + qso.Mode;
                if (qso.PairQSO != null)
                {
                    if (!pairs.Contains(signature))
                    {
                        pairs.Add(signature);
                    }
                    else
                    {
                        log.Error("Invalid QSOs more then once:" + qso.Log.FileName + " ->" + signature + "<-");
                        qso.IsInvalid("Invalid QSOs more then once: " + signature);
                    }
                }
            }
        }

        public static void ParseFrequency(QSO qso, bool ignoreModeFrequencyValidation)
        {
            double freq;
            if (!Double.TryParse(qso.Frequency, out freq))
            {
                log.Error("Invalid Frequency :" + qso.Log.FileName + " ->" + qso.Frequency + "<-");
                qso.IsInvalid("Invalid Frequency: ->" + qso.Frequency+"<-");
            }
            else
            {
                if (!ignoreModeFrequencyValidation)
                {
                    if (qso.Mode == "CW")
                    {

                        if (freq < 3510 || freq > 3560)
                        {
                            log.Error("Mode Frequency Violation:" + qso.Log.FileName + " ->" + qso.Mode + " " + qso.Frequency + "<-");
                            qso.IsInvalid("Mode Frequency Violation: ->" + qso.Mode + " " + qso.Frequency + "<- for CW should be 3510><3560");
                        }
                    }
                    else
                    {
                        if (qso.Mode == "SSB" || qso.Mode == "PH")
                        {
                            if (freq < 3675 || freq > 3775)
                            {
                                log.Error("Mode Frequency Violation:" + qso.Log.FileName + " ->" + qso.Mode + " " + qso.Frequency + "<-");
                                qso.IsInvalid("Mode Frequency Violation: ->" + qso.Mode + " " + qso.Frequency + "<- for PH/SSB should be 3675><3775");
                            }
                        }
                        else
                        {
                            log.Error("Mode Violation:" + qso.Log.FileName + " ->" + qso.Mode + "<-");
                            qso.IsInvalid("Mode Violation: ->" + qso.Mode + "<- should be CW PH or SSB");
                        }
                    }
                }
            }
        }

        public static void ModeChangeCheck(Cabrillo cabrilloLog)
        {
            string lastmode = "";
            DateTime lastmodeTime = DateTime.MinValue;

            string beforelastmode = "";
            DateTime beforelastmodeTime = DateTime.MinValue;

            foreach (var qso in cabrilloLog.QSOs.OrderBy(x => x.DateTime))
            {
                if (lastmode != qso.Mode)
                {

                    if (beforelastmodeTime.AddMinutes(5) > qso.DateTime)
                    {
                        log.Error("Mode Change Violation:" + cabrilloLog.FileName + " ->" + qso.DateTime + " " + qso.Mode + "<-");
                        qso.IsInvalid("Mode Change Violation: ->" + qso.DateTime + " " + qso.Mode + "<-");
                    }
                    else
                    {
                        beforelastmode = lastmode;
                        beforelastmodeTime=lastmodeTime;

                        lastmode = qso.Mode;
                        lastmodeTime = qso.DateTime;
                    }
                }
                else
                {
                    lastmodeTime = qso.DateTime;
                }
            }
        }


    }

    public static class Program {
        public static string UploadPath
        {
            get
            {
                return System.IO.Path.Combine("Uploads/", ContestDate.Year.ToString());
            }
        }
        public static DateTime ContestDate
        {
            get {
                DateTime myDate = new DateTime(DateTime.Today.Month<12?DateTime.Today.Year-1: DateTime.Today.Year, 12,17);
                int offset = myDate.DayOfWeek - DayOfWeek.Sunday;
                return myDate.AddDays(offset <3?- offset:7-offset);
            }
        }

        public static int ScorCupaTimisului(QSO qso)
        {
            if (qso.CallSign2.ToLower() == "YP1989TM")//joker
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


        static readonly ILog log = LogManager.GetLogger(typeof(Program));
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
                        if (existing != null && new FileInfo(existing.FileName).CreationTime < subFile.CreationTime)
                        {
                            logList.Remove(existing);
                            existing = null;

                        }

                        if (existing == null)
                        {
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

            if (options.CupaTimisului)
            {
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
                                if (options.CupaTimisului)
                                {
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
                string[] ignorelist = options.IgnoreCallsigns.Replace(" ", "").Split(',');

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

        public static void ParseQSO(List<Cabrillo> list, bool ignoreDateTimeValidation, QSO.CalculateScore myScore)
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
