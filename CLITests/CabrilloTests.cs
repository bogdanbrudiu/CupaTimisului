using CLI;
using Contest;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Test
{
    public class CabrilloTest
    {
        public ILogger logger;
        public CabrilloTest() {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("CLI.Program", LogLevel.Debug)
                    .AddConsole();




            });
            logger=loggerFactory.CreateLogger<CabrilloTest>();
        }
        [Test]
        public void Cabrillo_Parse()
        {
            string test = @"START-OF-LOG: 3.0
CONTEST:Cupa Timisului
CALLSIGN:YO2MKE
CATEGORY-ASSISTED:NON-ASSISTED
CATEGORY-BAND:20M
CATEGORY-MODE:CW
CATEGORY-OPERATOR:SINGLE-OP
CATEGORY-POWER:QRP
CATEGORY-STATION:K3
CATEGORY-TIME:time
CATEGORY-TRANSMITTER:ONE
CATEGORY-OVERLAY:overlay
CLAIMED-SCORE:160
CLUB:YO2KQT
CREATED-BY:YO2MKE
OPERATORS:YO2MKE
EMAIL:bogdan@brudiu.ro
INVALIDTAG1:this is an invalid tag
INVALIDTAG2:this is an invalid tag
LOCATION:KN
NAME:Bogdan BRUDIU
ADDRESS:7 Torontalului
ADDRESS:sc. A ap. 9
ADDRESS-CITY:Timisoara
ADDRESS-STATE-PROVINCE:Timis
ADDRESS-POSTALCODE:300627
ADDRESS-COUNTRY:ROMAINA
SOAPBOX:test
SOAPBOX:test
OFFTIME:offtime
DEBUG:debug
QSO: 3500 PH 2010-12-18 1400 YO2MKE    59  001 TM  YO5OED    59  001 BH
QSO: 3500 PH 2010-12-18 1401 YO2MKE    59  002 TM  YO2KAR    59  003 HD
QSO: 3500 PH 2010-12-18 1403 YO2MKE    59  003 TM  YO3KPA    59  007 BU
QSO: 3500 PH 2010-12-18 1404 YO2MKE    59  004 TM  YO2KJJ    59  007 TM
QSO: 3500 PH 2010-12-18 1405 YO2MKE    59  005 TM  YO2LLZ    59  005 TM
QSO: 3500 PH 2010-12-18 1408 YO2MKE    59  006 TM  YO7MGG    59  016 DJ
QSO: 3500 PH 2010-12-18 1409 YO2MKE    59  007 TM  YO9FL     59  015 CL
END-OF-LOG:
";
            byte[] byteArray = Encoding.ASCII.GetBytes(test);
            MemoryStream memStream = new MemoryStream(byteArray);
            Cabrillo cabrillo = Cabrillo.Parse("FileName.test", memStream, false, logger);
            Assert.That(cabrillo.Contest, Is.EqualTo("Cupa Timisului"));
            Assert.That(cabrillo.CallSign, Is.EqualTo("YO2MKE"));
            Assert.That(cabrillo.CategoryAssisted, Is.EqualTo("NON-ASSISTED"));
            Assert.That(cabrillo.CategoryBand, Is.EqualTo("20M"));
            Assert.That(cabrillo.CategoryMode, Is.EqualTo("CW"));
            Assert.That(cabrillo.CategoryOperator, Is.EqualTo("SINGLE-OP"));
            Assert.That(cabrillo.CategoryPower, Is.EqualTo("QRP"));
            Assert.That(cabrillo.CategoryStation, Is.EqualTo("K3"));
            Assert.That(cabrillo.CategoryTime, Is.EqualTo("time"));
            Assert.That(cabrillo.CategoryTransmiter, Is.EqualTo("ONE"));
            Assert.That(cabrillo.CategoryOverlay, Is.EqualTo("overlay"));
            Assert.That(cabrillo.ClaimedScore, Is.EqualTo("160"));
            Assert.That(cabrillo.Club, Is.EqualTo("YO2KQT"));
            Assert.That(cabrillo.CreatedBy, Is.EqualTo("YO2MKE"));
            Assert.That(cabrillo.Operators, Is.EqualTo("YO2MKE"));
            Assert.That(cabrillo.Email, Is.EqualTo("bogdan@brudiu.ro"));
            Assert.That(cabrillo.Location, Is.EqualTo("KN"));
            Assert.That(cabrillo.Name, Is.EqualTo("Bogdan BRUDIU"));
            Assert.That(cabrillo.Address.Count, Is.EqualTo(2));
            Assert.That(cabrillo.Address[0], Is.EqualTo("7 Torontalului"));
            Assert.That(cabrillo.Address[1], Is.EqualTo("sc. A ap. 9"));
            Assert.That(cabrillo.AddressCity, Is.EqualTo("Timisoara"));
            Assert.That(cabrillo.AddressStateProvince, Is.EqualTo("Timis"));
            Assert.That(cabrillo.AddressPostalCode, Is.EqualTo("300627"));
            Assert.That(cabrillo.AddressCountry, Is.EqualTo("ROMAINA"));
            Assert.That(cabrillo.SoapBox[0], Is.EqualTo("test"));
            Assert.That(cabrillo.OffTime, Is.EqualTo("offtime"));
            Assert.That(cabrillo.Debug, Is.EqualTo("debug"));
            Assert.That(cabrillo.InvalidLines.Count, Is.EqualTo(2));
            Assert.That(cabrillo.InvalidLines[0], Is.EqualTo("INVALIDTAG1:this is an invalid tag"));
            Assert.That(cabrillo.InvalidLines[1], Is.EqualTo("INVALIDTAG2:this is an invalid tag"));
            Assert.That(cabrillo.ROWQSOs.Count, Is.EqualTo(7));
            Assert.That(cabrillo.ROWQSOs[0], Is.EqualTo(" 3500 PH 2010-12-18 1400 YO2MKE    59  001 TM  YO5OED    59  001 BH"));
            Assert.That(cabrillo.ROWQSOs[6], Is.EqualTo(" 3500 PH 2010-12-18 1409 YO2MKE    59  007 TM  YO9FL     59  015 CL"));
        }

        [Test]
        public void Cabrillo_QSOParse()
        {
            string test = @"START-OF-LOG: 3.0
CONTEST:Cupa Timisului
CALLSIGN:YO2MKE
CATEGORY-ASSISTED:NON-ASSISTED
CATEGORY-BAND:20M
CATEGORY-MODE:CW
CATEGORY-OPERATOR:SINGLE-OP
CATEGORY-POWER:QRP
CATEGORY-STATION:K3
CATEGORY-TIME:time
CATEGORY-TRANSMITTER:ONE
CATEGORY-OVERLAY:overlay
CLAIMED-SCORE:160
CLUB:YO2KQT
CREATED-BY:YO2MKE
OPERATORS:YO2MKE
EMAIL:bogdan@brudiu.ro
INVALIDTAG1:this is an invalid tag
INVALIDTAG2:this is an invalid tag
LOCATION:KN
NAME:Bogdan BRUDIU
ADDRESS:7 Torontalului
ADDRESS:sc. A ap. 9
ADDRESS-CITY:Timisoara
ADDRESS-STATE-PROVINCE:Timis
ADDRESS-POSTALCODE:300627
ADDRESS-COUNTRY:ROMAINA
SOAPBOX:test
SOAPBOX:test
OFFTIME:offtime
DEBUG:debug
QSO: 3500 PH 2010-12-18 1400 YO2MKE    59  001 TM  YO5OED    59  001 BH
QSO: 3500 PH 2010-12-18 1401 YO2MKE    59  002 TM  YO2KAR    59  003 HD
QSO: 3500 PH 2010-12-18 1403 YO2MKE    59  003 TM  YO3KPA    59  007 BU
QSO: 3500 PH 2010-12-18 1404 YO2MKE    59  004 TM  YO2KJJ    59  007 TM
QSO: 3500 PH 2010-12-18 1405 YO2MKE    59  005 TM  YO2LLZ    59  005 TM
QSO: 3500 PH 2010-12-18 1408 YO2MKE    59  006 TM  YO7MGG    59  016 DJ
QSO: 3500 PH 2010-12-18 1409 YO2MKE    59  007 TM  YO9FL     59  015 CL
END-OF-LOG:
";
            byte[] byteArray = Encoding.ASCII.GetBytes(test);
            MemoryStream memStream = new MemoryStream(byteArray);
         
            Cabrillo cabrillo = Cabrillo.Parse("FileName.test", memStream, false, logger);
            cabrillo.ParseQSO(false, Contest.Program.ScorCupaTimisului);
            Assert.That(cabrillo.QSOs.Count, Is.EqualTo(7));
            QSO qso1 = cabrillo.QSOs[0];
            Assert.That(qso1.CallSign1, Is.EqualTo("YO2MKE"));
            Assert.That(qso1.CallSign2, Is.EqualTo("YO5OED"));
            Assert.That(qso1.County1, Is.EqualTo("TM"));
            Assert.That(qso1.County2, Is.EqualTo("BH"));
            Assert.That(qso1.DateTime.ToString("yyyy-MM-dd"), Is.EqualTo("2010-12-18"));
            Assert.That(qso1.Exchange1, Is.EqualTo("001"));
            Assert.That(qso1.Exchange2, Is.EqualTo("001"));
            Assert.That(qso1.Frequency, Is.EqualTo("3500"));
            Assert.That(qso1.Mode, Is.EqualTo("PH"));
            Assert.That(qso1.RST1, Is.EqualTo("59"));
            Assert.That(qso1.RST2, Is.EqualTo("59"));
            Assert.That(qso1.Log.FileName, Is.EqualTo("FileName.test"));
        }


        [Test]
        public void Cabrillo_QSOParseNonStandard()
        {
            string test = @"START-OF-LOG: 3.0
QSO: 3500 PH 2010-12-18 1400 YO2MKE    59  001TM  YO5OED    59  001BH
SOAPBOX: control nr and County together
QSO: 3500 PH 2010-12-18 14:01 YO2MKE    59  002 TM  YO2KAR    59  003 HD
SOAPBOX: time with separator
END-OF-LOG:
";
            byte[] byteArray = Encoding.ASCII.GetBytes(test);
            MemoryStream memStream = new MemoryStream(byteArray);
            Cabrillo cabrillo = Cabrillo.Parse("FileName.test", memStream, false, logger);
            cabrillo.ParseQSO(false, Contest.Program.ScorCupaTimisului);
            Assert.That(cabrillo.QSOs.Count, Is.EqualTo(2));
            QSO qso0 = cabrillo.QSOs[0];
            Assert.That(qso0.CallSign1, Is.EqualTo("YO2MKE"));
            Assert.That(qso0.CallSign2, Is.EqualTo("YO5OED"));
            Assert.That(qso0.County1, Is.EqualTo("TM"));
            Assert.That(qso0.County2, Is.EqualTo("BH"));
            Assert.That(qso0.DateTime.ToString("yyyy-MM-dd"), Is.EqualTo("2010-12-18"));
            Assert.That(qso0.Exchange1, Is.EqualTo("001"));
            Assert.That(qso0.Exchange2, Is.EqualTo("001"));
            Assert.That(qso0.Frequency, Is.EqualTo("3500"));
            Assert.That(qso0.Mode, Is.EqualTo("PH"));
            Assert.That(qso0.RST1, Is.EqualTo("59"));
            Assert.That(qso0.RST2, Is.EqualTo("59"));
            QSO qso1 = cabrillo.QSOs[1];
            Assert.That(qso1.CallSign1, Is.EqualTo("YO2MKE"));
            Assert.That(qso1.CallSign2, Is.EqualTo("YO2KAR"));
            Assert.That(qso1.County1, Is.EqualTo("TM"));
            Assert.That(qso1.County2, Is.EqualTo("HD"));
            Assert.That(qso1.DateTime.ToString("yyyy-MM-dd"), Is.EqualTo("2010-12-18"));
            Assert.That(qso1.Exchange1, Is.EqualTo("002"));
            Assert.That(qso1.Exchange2, Is.EqualTo("003"));
            Assert.That(qso1.Frequency, Is.EqualTo("3500"));
            Assert.That(qso1.Mode, Is.EqualTo("PH"));
            Assert.That(qso1.RST1, Is.EqualTo("59"));
            Assert.That(qso1.RST2, Is.EqualTo("59"));
            Assert.That(qso1.Log.FileName, Is.EqualTo("FileName.test"));
        }
        [Test]
        public void Cabrillo_ProcessQSOs()
        {
            string test1 = @"START-OF-LOG: 3.0
CALLSIGN:YO1
QSO: 3500 PH 2010-12-18 1400 YO1    59  001 TM  YO2    59  001 TM
QSO: 3500 CW 2010-12-18 1401 YO1    59  002 TM  YO2    59  002 TM
QSO: 3500 PH 2010-12-18 1500 YO1    59  003 TM  YO3    59  003 TM
QSO: 3500 CW 2010-12-18 1501 YO1    59  004 TM  YO3    59  004 TM
QSO: 3500 PH 2010-12-18 1601 YO1    59  005 TM  YO2    59  005 TM
END-OF-LOG:
";
            string test2 = @"START-OF-LOG: 3.0
CALLSIGN:YO2
QSO: 3500 PH 2010-12-18 1400 YO2    59  001 TM  YO1    59  001 TM
QSO: 3500 CW 2010-12-18 1401 YO2    59  002 TM  YO1    59  002 TM
END-OF-LOG:
";
            string test3 = @"START-OF-LOG: 3.0
CALLSIGN:YO3
QSO: 3500 PH 2010-12-18 1500 YO3    59  003 TM  YO1    59  003 TM
QSO: 3500 CW 2010-12-18 1501 YO3    59  004 TM  YO1    59  004 TM
END-OF-LOG:
";

            byte[] byteArray1 = Encoding.ASCII.GetBytes(test1);
            MemoryStream memStream1 = new MemoryStream(byteArray1);
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false, logger);
            cabrillo1.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            byte[] byteArray2 = Encoding.ASCII.GetBytes(test2);
            MemoryStream memStream2 = new MemoryStream(byteArray2);
            Cabrillo cabrillo2 = Cabrillo.Parse("File2.test", memStream2, false, logger);
            cabrillo2.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            byte[] byteArray3 = Encoding.ASCII.GetBytes(test3);
            MemoryStream memStream3 = new MemoryStream(byteArray3);
            Cabrillo cabrillo3 = Cabrillo.Parse("File3.test", memStream3, false, logger);
            cabrillo3.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            List<Cabrillo> logList = new List<Cabrillo>();
            logList.Add(cabrillo1);
            logList.Add(cabrillo2);
            logList.Add(cabrillo3);

            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 14, 00, 00), new DateTime(2010, 12, 18, 15, 00, 00)));
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 15, 00, 00), new DateTime(2010, 12, 18, 16, 00, 00)));
            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, false, logger);
            Assert.That(etape.Count, Is.EqualTo(2));
            Assert.That(etape[0].Count, Is.EqualTo(4));
            Assert.That(etape[1].Count, Is.EqualTo(4));
            Assert.That(logList[0].QSOs[4].InvalidResons.Count, Is.EqualTo(1));

            Assert.That(cabrillo2.QSOs[0], Is.EqualTo(cabrillo1.QSOs[0].PairQSO));
            Assert.That(cabrillo2.QSOs[1], Is.EqualTo(cabrillo1.QSOs[1].PairQSO));
            Assert.That(cabrillo3.QSOs[0], Is.EqualTo(cabrillo1.QSOs[2].PairQSO));
            Assert.That(cabrillo3.QSOs[1], Is.EqualTo(cabrillo1.QSOs[3].PairQSO));
        }

        [Test]
        public void Cabrillo_CheckOneQSOs()
        {
            string test1 = @"START-OF-LOG: 3.0
CALLSIGN:YO1
QSO: 3500 PH 2010-12-18 1400 YO1    59  001 TM  YO2    59  001 TM
QSO: 3500 CW 2010-12-18 1401 YO1    59  002 TM  YO2    59  002 TM
QSO: 3500 PH 2010-12-18 1500 YO1    59  003 TM  YO3    59  003 TM
QSO: 3500 CW 2010-12-18 1501 YO1    59  004 TM  YO3    59  004 TM
QSO: 3500 PH 2010-12-18 1601 YO1    59  005 TM  YO2    59  005 TM
QSO: 3500 CW 2010-12-18 1402 YO1    59  006 TM  YO2    59  003 TM
END-OF-LOG:
";
            string test2 = @"START-OF-LOG: 3.0
CALLSIGN:YO2
QSO: 3500 PH 2010-12-18 1400 YO2    59  001 TM  YO1    59  001 TM
QSO: 3500 CW 2010-12-18 1401 YO2    59  002 TM  YO1    59  002 TM
QSO: 3500 CW 2010-12-18 1402 YO2    59  003 TM  YO1    59  006 TM
QSO: 3500 PH 2010-12-18 1601 YO2    59  005 TM  YO1    59  005 TM
END-OF-LOG:
";
            string test3 = @"START-OF-LOG: 3.0
CALLSIGN:YO3
QSO: 3500 PH 2010-12-18 1500 YO3    59  003 TM  YO1    59  003 TM
QSO: 3500 CW 2010-12-18 1501 YO3    59  004 TM  YO1    59  004 TM
END-OF-LOG:
";

            byte[] byteArray1 = Encoding.ASCII.GetBytes(test1);
            MemoryStream memStream1 = new MemoryStream(byteArray1);
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false, logger);
            cabrillo1.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            byte[] byteArray2 = Encoding.ASCII.GetBytes(test2);
            MemoryStream memStream2 = new MemoryStream(byteArray2);
            Cabrillo cabrillo2 = Cabrillo.Parse("File2.test", memStream2, false, logger);
            cabrillo2.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            byte[] byteArray3 = Encoding.ASCII.GetBytes(test3);
            MemoryStream memStream3 = new MemoryStream(byteArray3);
            Cabrillo cabrillo3 = Cabrillo.Parse("File3.test", memStream3, false, logger);
            cabrillo3.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            List<Cabrillo> logList = new List<Cabrillo>();
            logList.Add(cabrillo1);
            logList.Add(cabrillo2);
            logList.Add(cabrillo3);

            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 14, 00, 00), new DateTime(2010, 12, 18, 15, 00, 00)));
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 15, 00, 00), new DateTime(2010, 12, 18, 16, 00, 00)));
            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, false, logger);
            Cabrillo.CheckOneQSO(etape[0], logger);
            Assert.That(logList[0].QSOs[5].InvalidResons.Count, Is.EqualTo(1));


        }

        [Test]
        public void Cabrillo_ParseFrequency()
        {
            string test1 = @"START-OF-LOG: 3.0
CALLSIGN:YO1
QSO: 3680 SSB 2010-12-18 1400 YO1    59  001 TM  YO2    59  001 TM
QSO: 3580 SSB 2010-12-18 1400 YO1    59  001 TM  YO2    59  001 TM
QSO: 3510 CW 2010-12-18 1401 YO1    59  002 TM  YO2    59  002 TM
QSO: 3500 CW 2010-12-18 1401 YO1    59  002 TM  YO2    59  002 TM
QSO: 3df500 CW 2010-12-18 1401 YO1    59  002 TM  YO2    59  002 TM
QSO: 3500 PH 2010-12-18 1401 YO1    59  002 TM  YO2    59  002 TM
END-OF-LOG:
";


            byte[] byteArray1 = Encoding.ASCII.GetBytes(test1);
            MemoryStream memStream1 = new MemoryStream(byteArray1);
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false, logger);
            cabrillo1.ParseQSO(false, Contest.Program.ScorCupaTimisului);


            List<Cabrillo> logList = new List<Cabrillo>();
            logList.Add(cabrillo1);

            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 14, 00, 00), new DateTime(2010, 12, 18, 15, 00, 00)));
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 15, 00, 00), new DateTime(2010, 12, 18, 16, 00, 00)));
            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, false, logger);
            Cabrillo.ParseFrequency(cabrillo1.QSOs[0], false, logger);
            Assert.That(cabrillo1.QSOs[0].InvalidResons.Count, Is.EqualTo(0));
            Cabrillo.ParseFrequency(cabrillo1.QSOs[1], false, logger);
            Assert.That(cabrillo1.QSOs[1].InvalidResons.Count, Is.EqualTo(1));
            Cabrillo.ParseFrequency(cabrillo1.QSOs[2], false, logger);
            Assert.That(cabrillo1.QSOs[2].InvalidResons.Count, Is.EqualTo(0));
            Cabrillo.ParseFrequency(cabrillo1.QSOs[3], false, logger);
            Assert.That(cabrillo1.QSOs[3].InvalidResons.Count, Is.EqualTo(1));
            Cabrillo.ParseFrequency(cabrillo1.QSOs[4], false, logger);
            Assert.That(cabrillo1.QSOs[4].InvalidResons.Count, Is.EqualTo(1));
            Cabrillo.ParseFrequency(cabrillo1.QSOs[5], false, logger);
            Assert.That(cabrillo1.QSOs[5].InvalidResons.Count, Is.EqualTo(1));

        }

        [Test]
        public void Cabrillo_ModeChangeCheck()
        {
            string test1 = @"START-OF-LOG: 3.0
CALLSIGN:YO1
QSO: 3500 PH 2010-12-18 1400 YO1    59  001 TM  YO2    59  001 TM
QSO: 3500 CW 2010-12-18 1405 YO1    59  002 TM  YO2    59  002 TM
QSO: 3500 PH 2010-12-18 1410 YO1    59  003 TM  YO3    59  003 TM
QSO: 3500 CW 2010-12-18 1415 YO1    59  004 TM  YO3    59  004 TM
QSO: 3500 PH 2010-12-18 1416 YO1    59  005 TM  YO2    59  005 TM
QSO: 3500 CW 2010-12-18 1417 YO1    59  006 TM  YO2    59  006 TM
END-OF-LOG:
";


            byte[] byteArray1 = Encoding.ASCII.GetBytes(test1);
            MemoryStream memStream1 = new MemoryStream(byteArray1);
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false, logger);
            cabrillo1.ParseQSO(false, Contest.Program.ScorCupaTimisului);


            List<Cabrillo> logList = new List<Cabrillo>();
            logList.Add(cabrillo1);

            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 14, 00, 00), new DateTime(2010, 12, 18, 15, 00, 00)));
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 15, 00, 00), new DateTime(2010, 12, 18, 16, 00, 00)));

            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, false, logger);
            Cabrillo.ModeChangeCheck(cabrillo1, logger);
            Assert.That(logList[0].QSOs[5].InvalidResons.Count, Is.EqualTo(1));


        }

        [Test]
        public void Cabrillo_CheckScore()
        {


            string test1 = @"START-OF-LOG: 3.0
CALLSIGN:YO1
QSO: 3550 CW 2010-12-18 1400 YO1    59  001 TM  YO2    59  001 TM
SOAPBOX: 1p
QSO: 3550 CW 2010-12-18 1401 YO1    59  002 TM  YO3    59  001 AR
SOAPBOX: 4p
QSO: 3550 CW 2010-12-18 1402 YO1    59  003 TM  YO2    59  002 TM
SOAPBOX: only once/etapa
QSO: 3675 PH 2010-12-18 1407 YO1    59  004 TM  YO2    59  003 TM
SOAPBOX: only once/etapa

QSO: 3675 PH 2010-12-18 1500 YO1    59  005 TM  YO2    59  004 TM
SOAPBOX: 1p
QSO: 2550 CW 2010-12-18 1501 YO1    59  006 TM  YO4    59  003 MM
SOAPBOX: wrong freq
QSO: 3675 PH 2010-12-18 1502 YO1    59  007 TM  YO5    59  003 MM
SOAPBOX: mode change
QSO: 3675 PH 2010-12-18 1503 YO1    59  008 TM  YO3    59  003 AR
SOAPBOX: mode change
QSO: 3675 PH 2010-12-18 1530 YO1    59  009 TM  YO6    59  001 SM
SOAPBOX: 4p
QSO: 3675 PH 2010-12-18 1540 YO1    59  010 TM  YO7    59  001 SM
SOAPBOX: 4p
QSO: 3675 PH 2010-12-18 1550 YO1    59  011 TM  YO8    59  001 SM
SOAPBOX: minim 5 QSO
QSO: 3675 PH 2010-12-18 1601 YO1    59  011 TM  YO2    59  005 TM
SOAPBOX: Invalid hour

SOAPBOX:Total1 1 5 5 
SOAPBOX:Total2 1 8 8
SOAPBOX:Total 13
END-OF-LOG:
";
            string test2 = @"START-OF-LOG: 3.0
CALLSIGN:YO2
QSO: 3550 CW 2010-12-18 1400 YO2    59  001 TM  YO1    59  001 TM
SOAPBOX: 4p
QSO: 3550 CW 2010-12-18 1402 YO2    59  002 TM  YO1    59  003 TM
SOAPBOX: only once/etapa
QSO: 3675 PH 2010-12-18 1402 YO2    59  003 TM  YO1    59  004 TM
SOAPBOX: only once/etapa

QSO: 3550 CW 2010-12-18 1500 YO2    59  004 TM  YO1    59  005 TM
SOAPBOX: 4p
QSO: 3675 PH 2010-12-18 1531 YO2    59  005 TM  YO6    59  002 SM
SOAPBOX: 2p
QSO: 3675 PH 2010-12-18 1541 YO2    59  006 TM  YO7    59  002 SM
SOAPBOX: 2p
QSO: 3675 PH 2010-12-18 1551 YO2    59  007 TM  YO8    59  002 SM
SOAPBOX: minim 5 QSO

SOAPBOX:Total1 0 1 0 
SOAPBOX:Total2 1 5 5
SOAPBOX:Total 5
END-OF-LOG:
";
            string test3 = @"START-OF-LOG: 3.0
CALLSIGN:YO3
QSO: 3550 CW 2010-12-18 1401 YO3    59  001 AR  YO1    59  002 TM
SOAPBOX: 4p
QSO: 3550 CW 2010-12-18 1401 YO3    59  002 AR  YO4    59  001 MM
SOAPBOX: 2p

QSO: 3675 PH 2010-12-18 1503 YO3    59  003 AR  YO1    59  008 TM
SOAPBOX: 4p
QSO: 3675 PH 2010-12-18 1532 YO3    59  004 AR  YO6    59  003 SM
SOAPBOX: 2p
QSO: 3675 PH 2010-12-18 1542 YO3    59  005 AR  YO7    59  003 SM
SOAPBOX: 2p
QSO: 3675 PH 2010-12-18 1552 YO3    59  006 AR  YO8    59  003 SM
SOAPBOX: minim 5 QSO

SOAPBOX:Total1 2 6 12 
SOAPBOX:Total2 2 8 16
SOAPBOX:Total 28
END-OF-LOG:
";
            string test4 = @"START-OF-LOG: 3.0
CALLSIGN:YO4
QSO: 3550 CW 2010-12-18 1401 YO4    59  001 MM  YO3    59  002 AR
SOAPBOX: 2p
QSO: 3550 CW 2010-12-18 1402 YO4    59  002 MM  YO5    59  001 MM
SOAPBOX: 1p

QSO: 2500 CW 2010-12-18 1501 YO4    59  003 MM  YO1    59  006 TM
SOAPBOX: wrong freq
QSO: 3675 PH 2010-12-18 1533 YO4    59  004 MM  YO6    59  004 SM
SOAPBOX: 2p
QSO: 3675 PH 2010-12-18 1543 YO4    59  005 MM  YO7    59  004 SM
SOAPBOX: wrong time
QSO: 3675 PH 2010-12-18 1553 YO4    59  006 MM  YO8    59  004 SM
SOAPBOX: minim 5 QSO

SOAPBOX:Total1 1 3 3 
SOAPBOX:Total2 1 2 2
SOAPBOX:Total 5
END-OF-LOG:
";
            string test5 = @"START-OF-LOG: 3.0
CALLSIGN:YO5
QSO: 3550 CW 2010-12-18 1402 YO5    59  001 MM  YO4    59  002 MM
SOAPBOX: 1p
QSO: 3675 PH 2010-12-18 1407 YO5    59  002 MM  YO6    59  001 SM
SOAPBOX: 2p

QSO: 3675 PH 2010-12-18 1502 YO5    59  003 MM  YO1    59  007 TM
SOAPBOX: 4p
QSO: 3675 PH 2010-12-18 1534 YO5    59  004 MM  YO6    59  005 SM
SOAPBOX: 2p
QSO: 3675 PH 2010-12-18 1545 YO5    59  005 MM  YO7    59  005 SM
SOAPBOX: 0p

SOAPBOX:Total1 1 3 3 
SOAPBOX:Total2 2 8 16
SOAPBOX:Total 19
END-OF-LOG:
";

            string test6 = @"START-OF-LOG: 3.0
CALLSIGN:YO6
QSO: 3675 PH 2010-12-18 1407 YO6    59  001 SM  YO5    59  002 MM
SOAPBOX: 2p

QSO: 3675 PH 2010-12-18 1530 YO6    59  001 SM  YO1    59  009 TM
SOAPBOX: 4p
QSO: 3675 PH 2010-12-18 1531 YO6    59  002 SM  YO2    59  005 TM
SOAPBOX: 4p
QSO: 3675 PH 2010-12-18 1532 YO6    59  003 SM  YO3    59  004 AR
SOAPBOX: 2p
QSO: 3675 PH 2010-12-18 1533 YO6    59  004 SM  YO4    59  004 MM
SOAPBOX: 2p
QSO: 3675 PH 2010-12-18 1534 YO6    59  005 SM  YO5    59  004 MM
SOAPBOX: 2p

SOAPBOX:Total1 1 2 2 
SOAPBOX:Total2 4 14 56
SOAPBOX:Total 58
END-OF-LOG:
";

            string test7 = @"START-OF-LOG: 3.0
CALLSIGN:YO7
QSO: 3675 PH 2010-12-18 1540 YO7    59  001 SM  YO1    59  010 TJ
SOAPBOX: wrong county
QSO: 3675 PH 2010-12-18 1541 YO7    59  002 SM  YO2    59  060 TM
SOAPBOX: wrong serial
QSO: 3675 PH 2010-12-18 1542 YO7    59  003 SM  YO3    58  005 AR
SOAPBOX: wrong RST
QSO: 3675 PH 2010-12-18 1549 YO7    59  004 SM  YO4    59  005 MM
SOAPBOX: wrong time
QSO: 3675 PH 2010-12-18 1549 YO7    59  005 SM  YO5    59  005 MM
SOAPBOX: 2p 

SOAPBOX:Total1 0 0 0 
SOAPBOX:Total2 1 2 2
SOAPBOX:Total 2
END-OF-LOG:
";
            string test8 = @"START-OF-LOG: 3.0
CALLSIGN:YO8
QSO: 3675 PH 2010-12-18 1550 YO8    59  001 SM  YO1    59  011 TJ
SOAPBOX: minim 5 QSO
QSO: 3675 PH 2010-12-18 1551 YO8    59  002 SM  YO2    59  007 TM
SOAPBOX: minim 5 QSO
QSO: 3675 PH 2010-12-18 1552 YO8    59  003 SM  YO3    59  008 AR
SOAPBOX: minim 5 QSO
QSO: 3675 PH 2010-12-18 1553 YO8    59  004 SM  YO4    59  009 MM
SOAPBOX: minim 5 QSO

SOAPBOX:Total1 0 0 0 
SOAPBOX:Total2 0 0 0
SOAPBOX:Total 0
END-OF-LOG:
";


            byte[] byteArray1 = Encoding.ASCII.GetBytes(test1);
            MemoryStream memStream1 = new MemoryStream(byteArray1);
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false, logger);
            cabrillo1.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            byte[] byteArray2 = Encoding.ASCII.GetBytes(test2);
            MemoryStream memStream2 = new MemoryStream(byteArray2);
            Cabrillo cabrillo2 = Cabrillo.Parse("File2.test", memStream2, false, logger);
            cabrillo2.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            byte[] byteArray3 = Encoding.ASCII.GetBytes(test3);
            MemoryStream memStream3 = new MemoryStream(byteArray3);
            Cabrillo cabrillo3 = Cabrillo.Parse("File3.test", memStream3, false, logger);
            cabrillo3.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            byte[] byteArray4 = Encoding.ASCII.GetBytes(test4);
            MemoryStream memStream4 = new MemoryStream(byteArray4);
            Cabrillo cabrillo4 = Cabrillo.Parse("File4.test", memStream4, false, logger);
            cabrillo4.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            byte[] byteArray5 = Encoding.ASCII.GetBytes(test5);
            MemoryStream memStream5 = new MemoryStream(byteArray5);
            Cabrillo cabrillo5 = Cabrillo.Parse("File5.test", memStream5, false, logger);
            cabrillo5.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            byte[] byteArray6 = Encoding.ASCII.GetBytes(test6);
            MemoryStream memStream6 = new MemoryStream(byteArray6);
            Cabrillo cabrillo6 = Cabrillo.Parse("File6.test", memStream6, false, logger);
            cabrillo6.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            byte[] byteArray7 = Encoding.ASCII.GetBytes(test7);
            MemoryStream memStream7 = new MemoryStream(byteArray7);
            Cabrillo cabrillo7 = Cabrillo.Parse("File7.test", memStream7, false, logger);
            cabrillo7.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            byte[] byteArray8 = Encoding.ASCII.GetBytes(test8);
            MemoryStream memStream8 = new MemoryStream(byteArray8);
            Cabrillo cabrillo8 = Cabrillo.Parse("File8.test", memStream8, false, logger);
            cabrillo8.ParseQSO(false, Contest.Program.ScorCupaTimisului);

            List<Cabrillo> logList = new List<Cabrillo>();
            logList.Add(cabrillo1);
            logList.Add(cabrillo2);
            logList.Add(cabrillo3);
            logList.Add(cabrillo4);
            logList.Add(cabrillo5);
            logList.Add(cabrillo6);
            logList.Add(cabrillo7);
            logList.Add(cabrillo8);

            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 14, 00, 00), new DateTime(2010, 12, 18, 15, 00, 00)));
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 15, 00, 00), new DateTime(2010, 12, 18, 16, 00, 00)));
            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, false, logger);
            List<QSO> etapa1 = etape[0];
            List<QSO> etapa2 = etape[1];
            Cabrillo.CheckOneQSO(etapa1, logger);
            Cabrillo.CheckOneQSO(etapa2, logger);
            foreach (var cabrilloLog in logList)
            {
                foreach (var qso in cabrilloLog.QSOs)
                {
                    Cabrillo.ParseFrequency(qso, false, logger);
                }
            }
            foreach (var cabrilloLog in logList)
            {
                Cabrillo.ModeChangeCheck(cabrilloLog, logger);
            }
            foreach (var cabrilloLog in logList)
            {
                if (cabrilloLog.QSOs.Count < 5)
                {
                    cabrilloLog.Valid = false;
                }
            }
            int total10 = (cabrillo1.Multiplicator[0] * cabrillo1.Etape[0].Sum(qso => qso.Score));
            int total11 = (cabrillo1.Multiplicator[1] * cabrillo1.Etape[1].Sum(qso => qso.Score));
            Assert.That(cabrillo1.Multiplicator[0], Is.EqualTo(1));
            Assert.That(cabrillo1.Multiplicator[1], Is.EqualTo(1));
            Assert.That(total10, Is.EqualTo(5));
            Assert.That(total11, Is.EqualTo(8));

            int total20 = (cabrillo2.Multiplicator[0] * cabrillo2.Etape[0].Sum(qso => qso.Score));
            int total21 = (cabrillo2.Multiplicator[1] * cabrillo2.Etape[1].Sum(qso => qso.Score));
            Assert.That(cabrillo2.Multiplicator[0], Is.EqualTo(0));
            Assert.That(cabrillo2.Multiplicator[1], Is.EqualTo(1));
            Assert.That(total20, Is.EqualTo(0));
            Assert.That(total21, Is.EqualTo(8));

            int total30 = (cabrillo3.Multiplicator[0] * cabrillo3.Etape[0].Sum(qso => qso.Score));
            int total31 = (cabrillo3.Multiplicator[1] * cabrillo3.Etape[1].Sum(qso => qso.Score));
            Assert.That(cabrillo3.Multiplicator[0], Is.EqualTo(2));
            Assert.That(cabrillo3.Multiplicator[1], Is.EqualTo(2));
            Assert.That(total30, Is.EqualTo(12));
            Assert.That(total31, Is.EqualTo(16));

            int total40 = (cabrillo4.Multiplicator[0] * cabrillo4.Etape[0].Sum(qso => qso.Score));
            int total41 = (cabrillo4.Multiplicator[1] * cabrillo4.Etape[1].Sum(qso => qso.Score));
            Assert.That(cabrillo4.Multiplicator[0], Is.EqualTo(1));
            Assert.That(cabrillo4.Multiplicator[1], Is.EqualTo(1));
            Assert.That(total40, Is.EqualTo(3));
            Assert.That(total41, Is.EqualTo(2));

            int total50 = (cabrillo5.Multiplicator[0] * cabrillo5.Etape[0].Sum(qso => qso.Score));
            int total51 = (cabrillo5.Multiplicator[1] * cabrillo5.Etape[1].Sum(qso => qso.Score));
            Assert.That(cabrillo5.Multiplicator[0], Is.EqualTo(1));
            Assert.That(cabrillo5.Multiplicator[1], Is.EqualTo(2));
            Assert.That(total50, Is.EqualTo(3));
            Assert.That(total51, Is.EqualTo(16));

            int total60 = (cabrillo6.Multiplicator[0] * cabrillo6.Etape[0].Sum(qso => qso.Score));
            int total61 = (cabrillo6.Multiplicator[1] * cabrillo6.Etape[1].Sum(qso => qso.Score));
            Assert.That(cabrillo6.Multiplicator[0], Is.EqualTo(1));
            Assert.That(cabrillo6.Multiplicator[1], Is.EqualTo(4));
            Assert.That(total60, Is.EqualTo(2));
            Assert.That(total61, Is.EqualTo(56));


            int total70 = (cabrillo7.Multiplicator[0] * cabrillo7.Etape[0].Sum(qso => qso.Score));
            int total71 = (cabrillo7.Multiplicator[1] * cabrillo7.Etape[1].Sum(qso => qso.Score));
            Assert.That(cabrillo7.Multiplicator[0], Is.EqualTo(0));
            Assert.That(cabrillo7.Multiplicator[1], Is.EqualTo(1));
            Assert.That(total70, Is.EqualTo(0));
            Assert.That(total71, Is.EqualTo(2));

            int total80 = (cabrillo8.Multiplicator[0] * cabrillo8.Etape[0].Sum(qso => qso.Score));
            int total81 = (cabrillo8.Multiplicator[1] * cabrillo8.Etape[1].Sum(qso => qso.Score));
            Assert.That(cabrillo8.Multiplicator[0], Is.EqualTo(0));
            Assert.That(cabrillo8.Multiplicator[1], Is.EqualTo(0));
            Assert.That(total80, Is.EqualTo(0));
            Assert.That(total81, Is.EqualTo(0));
        }


        [Test]
        public void Cabrillo_ProcessNonStandardQSOs()
        {
            string test1 = @"START-OF-LOG: 3.0
CALLSIGN:YO2KJJ
QSO:  3500 PH 2011-12-18 1410 YO2KJJ        59  018 TM YO5OBA        59  003 BH
QSO:  3500 PH 2011-12-18 1502 YO2KJJ        59  051 TM YO5OBA        59  048 BH

END-OF-LOG:
";
            string test2 = @"START-OF-LOG: 3.0
CALLSIGN:YO5OBA
QSO: 3500  PH 18.12.11   1502 YO5OBA        59 048 BH   YO2KJJ        59 051 TM   
END-OF-LOG:
";


            byte[] byteArray1 = Encoding.ASCII.GetBytes(test1);
            MemoryStream memStream1 = new MemoryStream(byteArray1);
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false, logger);
            cabrillo1.ParseQSO(true, Contest.Program.ScorCupaTimisului, new DateTime(2011, 12, 18, 00, 00, 00));

            byte[] byteArray2 = Encoding.ASCII.GetBytes(test2);
            MemoryStream memStream2 = new MemoryStream(byteArray2);
            Cabrillo cabrillo2 = Cabrillo.Parse("File2.test", memStream2, false, logger);
            cabrillo2.ParseQSO(true, Contest.Program.ScorCupaTimisului, new DateTime(2011, 12, 18, 00, 00, 00));


            List<Cabrillo> logList = new List<Cabrillo>();
            logList.Add(cabrillo1);
            logList.Add(cabrillo2);

            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            startendPairs.Add(Tuple.Create(new DateTime(2011, 12, 18, 14, 00, 00), new DateTime(2011, 12, 18, 15, 00, 00)));
            startendPairs.Add(Tuple.Create(new DateTime(2011, 12, 18, 15, 00, 00), new DateTime(2011, 12, 18, 16, 00, 00)));
            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, true, logger);
            Assert.That(etape.Count, Is.EqualTo(2));
            Assert.That(etape[0].Count, Is.EqualTo(1));
            Assert.That(etape[1].Count, Is.EqualTo(2));
            Assert.That(logList[0].QSOs[0].InvalidResons.Count, Is.EqualTo(0));
            Assert.That(logList[0].QSOs[1].InvalidResons.Count, Is.EqualTo(0));
            Assert.That(logList[0].QSOs[0].InvalidResons.Count, Is.EqualTo(0));



            Assert.That(cabrillo1.QSOs[0].PairQSO, Is.EqualTo(null));
            Assert.That(cabrillo2.QSOs[0], Is.EqualTo(cabrillo1.QSOs[1].PairQSO));
        }
    }
}
