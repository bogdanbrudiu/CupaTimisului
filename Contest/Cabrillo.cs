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

namespace Contest
{
    public class QSO 
    {
        
        public QSO() 
        {
            InvalidResons = new ArrayList();
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
        public int Score 
        {
            get
            {
                if (Valid && PairQSO!=null)
                {

                    if (CallSign2.ToLower()=="yp10kqt")//joker
                    {
                        return 10;
                    }
                    if (County1 == County2 && County1.ToLower() == "tm")//tm-tm
                    {
                        return 1;
                    }
                    if (County1 != County2 &&  County2.ToLower() == "tm")//yo-tm
                    {
                        return 4;
                    }
                    if (County1 != County2 && County1.ToLower() == "tm")//tm-yo
                    {
                        return 4;// 2;
                    }
                    if (County1 != County2 && County2.ToLower() != "tm")//yo-yo
                    {
                        return 2;
                    }
                    if (County1 == County2 && County1.ToLower() != "tm")//acelasi judet
                    {
                        return 1;
                    }
                }
                return 0;
            }
        }
        public int Etapa { get; set; }
    }

    public class Cabrillo
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Program));
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
        
         public void ParseQSO(bool ignoreDateTimeValidation)
         {
             ParseQSO(ignoreDateTimeValidation, new DateTime(2014, 12, 14, 00, 00, 00));
         }
         public void ParseQSO(bool ignoreDateTimeValidation, DateTime dtignoreDateTimeValidation)
        {
            foreach (string rowqso in this.ROWQSOs)
            {

                QSO qso = new QSO();
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
                                    && qso.CallSign1 == qso_.CallSign2 
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


}
