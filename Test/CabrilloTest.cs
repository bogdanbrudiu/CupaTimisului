using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Contest;

namespace Test
{
    [TestClass]
    public class CabrilloTest
    {
        [TestMethod]
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
            byte[] byteArray = Encoding.ASCII.GetBytes( test );
            MemoryStream memStream = new MemoryStream(byteArray);
            Cabrillo cabrillo = Cabrillo.Parse("FileName.test", memStream, false);
            Assert.AreEqual("Cupa Timisului", cabrillo.Contest);
            Assert.AreEqual("YO2MKE", cabrillo.CallSign);
            Assert.AreEqual("NON-ASSISTED", cabrillo.CategoryAssisted);
            Assert.AreEqual("20M", cabrillo.CategoryBand);
            Assert.AreEqual("CW", cabrillo.CategoryMode);
            Assert.AreEqual("SINGLE-OP", cabrillo.CategoryOperator);
            Assert.AreEqual("QRP", cabrillo.CategoryPower);
            Assert.AreEqual("K3", cabrillo.CategoryStation);
            Assert.AreEqual("time", cabrillo.CategoryTime);
            Assert.AreEqual("ONE", cabrillo.CategoryTransmiter);
            Assert.AreEqual("overlay", cabrillo.CategoryOverlay);
            Assert.AreEqual("160", cabrillo.ClaimedScore);
            Assert.AreEqual("YO2KQT", cabrillo.Club);
            Assert.AreEqual("YO2MKE", cabrillo.CreatedBy);
            Assert.AreEqual("YO2MKE", cabrillo.Operators);
            Assert.AreEqual("bogdan@brudiu.ro", cabrillo.Email);
            Assert.AreEqual("KN", cabrillo.Location);
            Assert.AreEqual("Bogdan BRUDIU", cabrillo.Name);
            Assert.AreEqual(2, cabrillo.Address.Count);
            Assert.AreEqual("7 Torontalului", cabrillo.Address[0]);
            Assert.AreEqual("sc. A ap. 9", cabrillo.Address[1]);
            Assert.AreEqual("Timisoara", cabrillo.AddressCity);
            Assert.AreEqual("Timis", cabrillo.AddressStateProvince);
            Assert.AreEqual("300627", cabrillo.AddressPostalCode);
            Assert.AreEqual("ROMAINA", cabrillo.AddressCountry);
            Assert.AreEqual("test", cabrillo.SoapBox[0]);
            Assert.AreEqual("offtime", cabrillo.OffTime);
            Assert.AreEqual("debug", cabrillo.Debug);
            Assert.AreEqual(2, cabrillo.InvalidLines.Count);
            Assert.AreEqual("INVALIDTAG1:this is an invalid tag", cabrillo.InvalidLines[0]);
            Assert.AreEqual("INVALIDTAG2:this is an invalid tag", cabrillo.InvalidLines[1]);
            Assert.AreEqual(7, cabrillo.ROWQSOs.Count);
            Assert.AreEqual(" 3500 PH 2010-12-18 1400 YO2MKE    59  001 TM  YO5OED    59  001 BH", cabrillo.ROWQSOs[0]);
            Assert.AreEqual(" 3500 PH 2010-12-18 1409 YO2MKE    59  007 TM  YO9FL     59  015 CL", cabrillo.ROWQSOs[6]);
        }

        [TestMethod]
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
            Cabrillo cabrillo = Cabrillo.Parse("FileName.test", memStream, false);
            cabrillo.ParseQSO(false);
            Assert.AreEqual(7, cabrillo.QSOs.Count);
            QSO qso1 = cabrillo.QSOs[0];
            Assert.AreEqual("YO2MKE", qso1.CallSign1);
            Assert.AreEqual("YO5OED", qso1.CallSign2);
            Assert.AreEqual("TM", qso1.County1);
            Assert.AreEqual("BH", qso1.County2);
            Assert.AreEqual("2010-12-18", qso1.DateTime.ToString("yyyy-MM-dd"));
            Assert.AreEqual("001", qso1.Exchange1);
            Assert.AreEqual("001", qso1.Exchange2);
            Assert.AreEqual("3500", qso1.Frequency);
            Assert.AreEqual("PH", qso1.Mode);
            Assert.AreEqual("59", qso1.RST1);
            Assert.AreEqual("59", qso1.RST2);
            Assert.AreEqual("FileName.test", qso1.Log.FileName);
        }


        [TestMethod]
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
            Cabrillo cabrillo = Cabrillo.Parse("FileName.test", memStream, false);
            cabrillo.ParseQSO(false);
            Assert.AreEqual(2, cabrillo.QSOs.Count);
            QSO qso0 = cabrillo.QSOs[0];
            Assert.AreEqual("YO2MKE", qso0.CallSign1);
            Assert.AreEqual("YO5OED", qso0.CallSign2);
            Assert.AreEqual("TM", qso0.County1);
            Assert.AreEqual("BH", qso0.County2);
            Assert.AreEqual("2010-12-18", qso0.DateTime.ToString("yyyy-MM-dd"));
            Assert.AreEqual("001", qso0.Exchange1);
            Assert.AreEqual("001", qso0.Exchange2);
            Assert.AreEqual("3500", qso0.Frequency);
            Assert.AreEqual("PH", qso0.Mode);
            Assert.AreEqual("59", qso0.RST1);
            Assert.AreEqual("59", qso0.RST2);
            QSO qso1 = cabrillo.QSOs[1];
            Assert.AreEqual("YO2MKE", qso1.CallSign1);
            Assert.AreEqual("YO2KAR", qso1.CallSign2);
            Assert.AreEqual("TM", qso1.County1);
            Assert.AreEqual("HD", qso1.County2);
            Assert.AreEqual("2010-12-18", qso1.DateTime.ToString("yyyy-MM-dd"));
            Assert.AreEqual("002", qso1.Exchange1);
            Assert.AreEqual("003", qso1.Exchange2);
            Assert.AreEqual("3500", qso1.Frequency);
            Assert.AreEqual("PH", qso1.Mode);
            Assert.AreEqual("59", qso1.RST1);
            Assert.AreEqual("59", qso1.RST2);
            Assert.AreEqual("FileName.test", qso1.Log.FileName);
        }
        [TestMethod]
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
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false);
            cabrillo1.ParseQSO(false);

            byte[] byteArray2 = Encoding.ASCII.GetBytes(test2);
            MemoryStream memStream2 = new MemoryStream(byteArray2);
            Cabrillo cabrillo2 = Cabrillo.Parse("File2.test", memStream2, false);
            cabrillo2.ParseQSO(false);

            byte[] byteArray3 = Encoding.ASCII.GetBytes(test3);
            MemoryStream memStream3 = new MemoryStream(byteArray3);
            Cabrillo cabrillo3 = Cabrillo.Parse("File3.test", memStream3, false);
            cabrillo3.ParseQSO(false);

            List<Cabrillo> logList = new List<Cabrillo>();
            logList.Add(cabrillo1);
            logList.Add(cabrillo2);
            logList.Add(cabrillo3);

            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 14, 00, 00), new DateTime(2010, 12, 18, 15, 00, 00)));
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 15, 00, 00), new DateTime(2010, 12, 18, 16, 00, 00)));
            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, false);
            Assert.AreEqual(2,etape.Count);
            Assert.AreEqual(4, etape[0].Count);
            Assert.AreEqual(4, etape[1].Count);
            Assert.AreEqual(1, logList[0].QSOs[4].InvalidResons.Count);

            Assert.AreEqual(cabrillo2.QSOs[0], cabrillo1.QSOs[0].PairQSO);
            Assert.AreEqual(cabrillo2.QSOs[1], cabrillo1.QSOs[1].PairQSO);
            Assert.AreEqual(cabrillo3.QSOs[0], cabrillo1.QSOs[2].PairQSO);
            Assert.AreEqual(cabrillo3.QSOs[1], cabrillo1.QSOs[3].PairQSO);
        }

        [TestMethod]
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
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false);
            cabrillo1.ParseQSO(false);

            byte[] byteArray2 = Encoding.ASCII.GetBytes(test2);
            MemoryStream memStream2 = new MemoryStream(byteArray2);
            Cabrillo cabrillo2 = Cabrillo.Parse("File2.test", memStream2, false);
            cabrillo2.ParseQSO(false);

            byte[] byteArray3 = Encoding.ASCII.GetBytes(test3);
            MemoryStream memStream3 = new MemoryStream(byteArray3);
            Cabrillo cabrillo3 = Cabrillo.Parse("File3.test", memStream3, false);
            cabrillo3.ParseQSO(false);

            List<Cabrillo> logList = new List<Cabrillo>();
            logList.Add(cabrillo1);
            logList.Add(cabrillo2);
            logList.Add(cabrillo3);

            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 14, 00, 00), new DateTime(2010, 12, 18, 15, 00, 00)));
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 15, 00, 00), new DateTime(2010, 12, 18, 16, 00, 00)));
            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, false);
            Cabrillo.CheckOneQSO(etape[0]);
            Assert.AreEqual(1, logList[0].QSOs[5].InvalidResons.Count);

       
        }

        [TestMethod]
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
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false);
            cabrillo1.ParseQSO(false);


            List<Cabrillo> logList = new List<Cabrillo>();
            logList.Add(cabrillo1);

            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 14, 00, 00), new DateTime(2010, 12, 18, 15, 00, 00)));
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 15, 00, 00), new DateTime(2010, 12, 18, 16, 00, 00)));
            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, false);
            Cabrillo.ParseFrequency(cabrillo1.QSOs[0], false);
            Assert.AreEqual(0, cabrillo1.QSOs[0].InvalidResons.Count);
            Cabrillo.ParseFrequency(cabrillo1.QSOs[1], false);
            Assert.AreEqual(1, cabrillo1.QSOs[1].InvalidResons.Count);
            Cabrillo.ParseFrequency(cabrillo1.QSOs[2], false);
            Assert.AreEqual(0, cabrillo1.QSOs[2].InvalidResons.Count);
            Cabrillo.ParseFrequency(cabrillo1.QSOs[3], false);
            Assert.AreEqual(1, cabrillo1.QSOs[3].InvalidResons.Count);
            Cabrillo.ParseFrequency(cabrillo1.QSOs[4], false);
            Assert.AreEqual(1, cabrillo1.QSOs[4].InvalidResons.Count);
            Cabrillo.ParseFrequency(cabrillo1.QSOs[5], false);
            Assert.AreEqual(1, cabrillo1.QSOs[5].InvalidResons.Count);
            
        }

        [TestMethod]
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
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false);
            cabrillo1.ParseQSO(false);


            List<Cabrillo> logList = new List<Cabrillo>();
            logList.Add(cabrillo1);

            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 14, 00, 00), new DateTime(2010, 12, 18, 15, 00, 00)));
            startendPairs.Add(Tuple.Create(new DateTime(2010, 12, 18, 15, 00, 00), new DateTime(2010, 12, 18, 16, 00, 00)));

            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, false);
            Cabrillo.ModeChangeCheck(cabrillo1);
            Assert.AreEqual(1, logList[0].QSOs[5].InvalidResons.Count);
         

        }

        [TestMethod]
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
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false);
            cabrillo1.ParseQSO(false);

            byte[] byteArray2 = Encoding.ASCII.GetBytes(test2);
            MemoryStream memStream2 = new MemoryStream(byteArray2);
            Cabrillo cabrillo2 = Cabrillo.Parse("File2.test", memStream2, false);
            cabrillo2.ParseQSO(false);

            byte[] byteArray3 = Encoding.ASCII.GetBytes(test3);
            MemoryStream memStream3 = new MemoryStream(byteArray3);
            Cabrillo cabrillo3 = Cabrillo.Parse("File3.test", memStream3, false);
            cabrillo3.ParseQSO(false);

            byte[] byteArray4 = Encoding.ASCII.GetBytes(test4);
            MemoryStream memStream4 = new MemoryStream(byteArray4);
            Cabrillo cabrillo4 = Cabrillo.Parse("File4.test", memStream4, false);
            cabrillo4.ParseQSO(false);

            byte[] byteArray5 = Encoding.ASCII.GetBytes(test5);
            MemoryStream memStream5 = new MemoryStream(byteArray5);
            Cabrillo cabrillo5 = Cabrillo.Parse("File5.test", memStream5, false);
            cabrillo5.ParseQSO(false);

            byte[] byteArray6 = Encoding.ASCII.GetBytes(test6);
            MemoryStream memStream6 = new MemoryStream(byteArray6);
            Cabrillo cabrillo6 = Cabrillo.Parse("File6.test", memStream6, false);
            cabrillo6.ParseQSO(false);

            byte[] byteArray7 = Encoding.ASCII.GetBytes(test7);
            MemoryStream memStream7 = new MemoryStream(byteArray7);
            Cabrillo cabrillo7 = Cabrillo.Parse("File7.test", memStream7, false);
            cabrillo7.ParseQSO(false);

            byte[] byteArray8 = Encoding.ASCII.GetBytes(test8);
            MemoryStream memStream8 = new MemoryStream(byteArray8);
            Cabrillo cabrillo8 = Cabrillo.Parse("File8.test", memStream8, false);
            cabrillo8.ParseQSO(false);

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
            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, false);
            List<QSO> etapa1 = etape[0];
            List<QSO> etapa2 = etape[1];
            Cabrillo.CheckOneQSO(etapa1);
            Cabrillo.CheckOneQSO(etapa2);
            foreach (var cabrilloLog in logList)
            {
                foreach (var qso in cabrilloLog.QSOs)
                {
                    Cabrillo.ParseFrequency(qso, false);
                }
            }
            foreach (var cabrilloLog in logList)
            {
                Cabrillo.ModeChangeCheck(cabrilloLog);
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
            Assert.AreEqual(1, cabrillo1.Multiplicator[0]);
            Assert.AreEqual(1, cabrillo1.Multiplicator[1]);
            Assert.AreEqual(5, total10);
            Assert.AreEqual(8, total11);

            int total20 = (cabrillo2.Multiplicator[0] * cabrillo2.Etape[0].Sum(qso => qso.Score));
            int total21 = (cabrillo2.Multiplicator[1] * cabrillo2.Etape[1].Sum(qso => qso.Score));
            Assert.AreEqual(0, cabrillo2.Multiplicator[0]);
            Assert.AreEqual(1, cabrillo2.Multiplicator[1]);
            Assert.AreEqual(0, total20);
            Assert.AreEqual(8, total21);

            int total30 = (cabrillo3.Multiplicator[0] * cabrillo3.Etape[0].Sum(qso => qso.Score));
            int total31 = (cabrillo3.Multiplicator[1] * cabrillo3.Etape[1].Sum(qso => qso.Score));
            Assert.AreEqual(2, cabrillo3.Multiplicator[0]);
            Assert.AreEqual(2, cabrillo3.Multiplicator[1]);
            Assert.AreEqual(12, total30);
            Assert.AreEqual(16, total31);

            int total40 = (cabrillo4.Multiplicator[0] * cabrillo4.Etape[0].Sum(qso => qso.Score));
            int total41 = (cabrillo4.Multiplicator[1] * cabrillo4.Etape[1].Sum(qso => qso.Score));
            Assert.AreEqual(1, cabrillo4.Multiplicator[0]);
            Assert.AreEqual(1, cabrillo4.Multiplicator[1]);
            Assert.AreEqual(3, total40);
            Assert.AreEqual(2, total41);

            int total50 = (cabrillo5.Multiplicator[0] * cabrillo5.Etape[0].Sum(qso => qso.Score));
            int total51 = (cabrillo5.Multiplicator[1] * cabrillo5.Etape[1].Sum(qso => qso.Score));
            Assert.AreEqual(1, cabrillo5.Multiplicator[0]);
            Assert.AreEqual(2, cabrillo5.Multiplicator[1]);
            Assert.AreEqual(3, total50);
            Assert.AreEqual(16, total51);

            int total60 = (cabrillo6.Multiplicator[0] * cabrillo6.Etape[0].Sum(qso => qso.Score));
            int total61 = (cabrillo6.Multiplicator[1] * cabrillo6.Etape[1].Sum(qso => qso.Score));
            Assert.AreEqual(1, cabrillo6.Multiplicator[0]);
            Assert.AreEqual(4, cabrillo6.Multiplicator[1]);
            Assert.AreEqual(2, total60);
            Assert.AreEqual(56, total61);


            int total70 = (cabrillo7.Multiplicator[0] * cabrillo7.Etape[0].Sum(qso => qso.Score));
            int total71 = (cabrillo7.Multiplicator[1] * cabrillo7.Etape[1].Sum(qso => qso.Score));
            Assert.AreEqual(0, cabrillo7.Multiplicator[0]);
            Assert.AreEqual(1, cabrillo7.Multiplicator[1]);
            Assert.AreEqual(0, total70);
            Assert.AreEqual(2, total71);

            int total80 = (cabrillo8.Multiplicator[0] * cabrillo8.Etape[0].Sum(qso => qso.Score));
            int total81 = (cabrillo8.Multiplicator[1] * cabrillo8.Etape[1].Sum(qso => qso.Score));
            Assert.AreEqual(0, cabrillo8.Multiplicator[0]);
            Assert.AreEqual(0, cabrillo8.Multiplicator[1]);
            Assert.AreEqual(0, total80);
            Assert.AreEqual(0, total81);
        }


        [TestMethod]
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
            Cabrillo cabrillo1 = Cabrillo.Parse("File1.test", memStream1, false);
            cabrillo1.ParseQSO(true, new DateTime(2011, 12, 18, 00, 00, 00));

            byte[] byteArray2 = Encoding.ASCII.GetBytes(test2);
            MemoryStream memStream2 = new MemoryStream(byteArray2);
            Cabrillo cabrillo2 = Cabrillo.Parse("File2.test", memStream2, false);
            cabrillo2.ParseQSO(true, new DateTime(2011, 12, 18, 00, 00, 00));


            List<Cabrillo> logList = new List<Cabrillo>();
            logList.Add(cabrillo1);
            logList.Add(cabrillo2);

            List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
            startendPairs.Add(Tuple.Create(new DateTime(2011, 12, 18, 14, 00, 00), new DateTime(2011, 12, 18, 15, 00, 00)));
            startendPairs.Add(Tuple.Create(new DateTime(2011, 12, 18, 15, 00, 00), new DateTime(2011, 12, 18, 16, 00, 00)));
            List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, true);
            Assert.AreEqual(2, etape.Count);
            Assert.AreEqual(1, etape[0].Count);
            Assert.AreEqual(2, etape[1].Count);
            Assert.AreEqual(0, logList[0].QSOs[0].InvalidResons.Count);
            Assert.AreEqual(0, logList[0].QSOs[1].InvalidResons.Count);
            Assert.AreEqual(0, logList[0].QSOs[0].InvalidResons.Count);



            Assert.AreEqual(null, cabrillo1.QSOs[0].PairQSO);
            Assert.AreEqual(cabrillo2.QSOs[0], cabrillo1.QSOs[1].PairQSO);
        }
    }
}
