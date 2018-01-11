using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogSubmit.Models;
using Contest;
using System.IO;
using System.Collections;
using LogSubmit.Mailers;
using System.Web.Hosting;

namespace LogSubmit.Controllers
{
    public class HomeController : Controller
    {
        private Program.Options options;

        public HomeController(){
            options = new Contest.Program.Options();
            options.IgnoreDateTimeValidation = false;
            options.IgnoreModeFrequencyValidation = true;
            options.IgnoreStartOfLogTag = true;
            options.InputFolder = HostingEnvironment.MapPath("~/Uploads/");
            options.CupaTimisului = true;
            options.IgnoreCallsigns = "";

            options.Etapa1Start = new DateTime(2017, 12, 17, 14, 00, 00);
            options.Etapa1End = new DateTime(2017, 12, 17, 15, 00, 00);

            options.Etapa2Start = new DateTime(2017, 12, 17, 15, 00, 00);
            options.Etapa2End = new DateTime(2017, 12, 17, 16, 00, 00);
        }
        public ActionResult Results()
        {
            if (options.CupaTimisului)
            {
                ViewBag.Title = "Cupa Timisului";
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                ViewBag.Title = "Ziua Telecomunicatiilor";
            }
            FileInfo result = new FileInfo(Server.MapPath("~/Uploads/Result/total.cshtml"));
            if (!result.Exists || result.CreationTime.AddMinutes(5) < DateTime.Now) {
                //recreate result file
              
                Contest.Program.ProcessFolder(options);
            }
            return View();
        }

        public ActionResult Index()
        {
            if (options.CupaTimisului)
            {
                ViewBag.Title = "Cupa Timisului";
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                ViewBag.Title = "Ziua Telecomunicatiilor";
            }
            return View(new Entry());
        }
        public ActionResult Retry(string calsign, string email, string phone)
        {
            if (options.CupaTimisului)
            {
                ViewBag.Title = "Cupa Timisului";
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                ViewBag.Title = "Ziua Telecomunicatiilor";
            }
            return View("Index",new Entry() { Calsign = calsign, Email = email, Phone = phone });
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(Entry model, HttpPostedFileBase log, string Submit)
        {
            if (options.CupaTimisului) {
                ViewBag.Title = "Cupa Timisului";
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                ViewBag.Title = "Ziua Telecomunicatiilor";
            }
            if (Submit == "Incarca alt log")
            {
                return RedirectToAction("Retry", new { calsign = model.Calsign, email = model.Email, phone = model.Phone });
            }
            if (ModelState.IsValid)
            {



                List<Cabrillo> logList = new List<Cabrillo>();

                Stream logStream;
                string fileName;
                if (string.IsNullOrEmpty(model.FileName))
                {
                    logStream = log.InputStream;
                    fileName = log.FileName;

                } else
                {
                    logStream = new MemoryStream(System.Text.Encoding.Default.GetBytes(model.Log));
                    fileName = model.FileName;
                }
                logList.Add(Contest.Program.Parse(logStream, fileName, options.IgnoreStartOfLogTag));
                if (options.CupaTimisului) { 
                    Contest.Program.ParseQSO(logList, options.IgnoreDateTimeValidation,Program.ScorCupaTimisului);
                }
                if (options.ZiuaTelecomunicatiilor) { 
                    Contest.Program.ParseQSO(logList, options.IgnoreDateTimeValidation, Program.ScorZT);
                }
                List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
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
                Cabrillo.ModeChangeCheck(logList[0]);

                ArrayList messages = new ArrayList();
                foreach (string item in logList[0].InvalidLines)
                {
                    messages.Add("Linie invalida:"+item);
                }
                foreach (var q in logList[0].QSOs)
                {
                        foreach (var r in q.InvalidResons)
                        {
                            messages.Add("Linie invalida:"+q.ROWQSO);
                            messages.Add("Motiv:"+r);
                        }
                }
                ViewBag.Errors = messages.ToArray();
                ViewBag.QSOS = logList[0].QSOs;
                if(Submit=="Verifica")
                {
                    ViewBag.Check = true;
                    logStream.Seek(0, SeekOrigin.Begin);
                    StreamReader streamReader = new StreamReader(logStream);
                    model.Log = streamReader.ReadToEnd();
                    model.FileName = fileName;
                    streamReader.Close();
                    return View(model);
                }else{
                    bool ok = false;
                    while (!ok)
                    {

                        string fn = model.Calsign + " " + model.Email + " " + model.Phone + " -" + System.IO.Path.GetRandomFileName() + ".cabrillo";
                        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

                        foreach (char c in invalid)
                        {
                            fn = fn.Replace(c.ToString(), "_");
                        }
                        string pathString = System.IO.Path.Combine(Server.MapPath("~/Uploads/"), fn.Replace("/", "_").Replace("\\", "_"));
                        if (!System.IO.File.Exists(pathString))
                        {
                            using (System.IO.FileStream fs = System.IO.File.Create(pathString))
                            {
                                Byte[] b = System.Text.Encoding.Default.GetBytes(model.Log);
                                fs.Write(b, 0, b.Length);
                            }
                            ok = true;
                        }
                    }
                    IMailer mailer = new Mailer();
                    try
                    {
                        string subject = "";
                        if (options.CupaTimisului)
                        {
                            subject = "Log - Cupa Timisului";
                        }
                        if (options.ZiuaTelecomunicatiilor)
                        {
                            subject = "Log - Ziua Telecomunicatiilor";
                        }

                        mailer.ThankYou(model.Email, subject).SendAsync();
                    }
                    catch (Exception ex) { }
                    return View("ThankYou");
                }
            }
            return View(model);
        }
      


    }
}
