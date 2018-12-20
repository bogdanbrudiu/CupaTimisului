using Contest;
using CoreMailer.Interfaces;
using CoreMailer.Models;
using Submitter.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace LogSubmit.Controllers
{
    public class HomeController : Controller
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(HomeController));
        private Contest.Options options;
        private readonly ICoreMvcMailer _mailer;
        private readonly MailerSettings _mailerSettings;
        private IHostingEnvironment _env;
        public HomeController(ICoreMvcMailer mailer, IOptions<MailerSettings> mailerSettings, IHostingEnvironment env)
        {
            _mailer = mailer;
            _mailerSettings = mailerSettings.Value;
            _env = env;

            options = new Contest.Options();
            options.IgnoreDateTimeValidation = false;
            options.IgnoreModeFrequencyValidation = true;
            options.IgnoreStartOfLogTag = true;
            options.InputFolder = System.IO.Path.Combine(_env.WebRootPath, Contest.Program.UploadPath);
            options.CupaTimisului = true;
            options.IgnoreCallsigns = "";

            options.Etapa1Start = Program.ContestDate.AddHours(14);// new DateTime(2018, 12, 16, 14, 00, 00);
            options.Etapa1End = Program.ContestDate.AddHours(15); //new DateTime(2018, 12, 16, 15, 00, 00);

            options.Etapa2Start = Program.ContestDate.AddHours(15); //new DateTime(2018, 12, 16, 15, 00, 00);
            options.Etapa2End = Program.ContestDate.AddHours(16); //new DateTime(2018, 12, 16, 16, 00, 00);
        }
        public ActionResult Results()
        {
            if (options.CupaTimisului)
            {
                ViewBag.Title = "Cupa Timisului - "+Contest.Program.ContestDate.Year.ToString();
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                ViewBag.Title = "Ziua Telecomunicatiilor";
            }
            FileInfo result = new FileInfo(System.IO.Path.Combine(_env.WebRootPath,   Contest.Program.UploadPath + "/Result/total.cshtml"));
            if (!result.Exists || result.CreationTime.AddMinutes(5) < DateTime.Now)
            {
                //recreate result file

                Contest.Program.ProcessFolder(options);
            }
            return View();
        }

        public ActionResult Index()
        {
            if (!Directory.Exists(options.InputFolder)) {
                Directory.CreateDirectory(options.InputFolder);
            }
            if (options.CupaTimisului)
            {
                ViewBag.Title = "Cupa Timisului - " + Contest.Program.ContestDate.Year.ToString();
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                ViewBag.Title = "Ziua Telecomunicatiilor";
            }
            logger.Debug("Landing Page "+ViewBag.Title);
            return View(new Entry());
        }
        public ActionResult Retry(string calsign, string email, string phone)
        {
            if (options.CupaTimisului)
            {
                ViewBag.Title = "Cupa Timisului - "+Contest.Program.ContestDate.Year.ToString();
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                ViewBag.Title = "Ziua Telecomunicatiilor";
            }
            logger.Debug("Change file " + ViewBag.Title);
            return View("Index", new Entry() { Calsign = calsign, Email = email, Phone = phone });
        }
        [HttpPost]
        public  ActionResult Index(Entry model, IFormFile log, string Submit)
        {
            
            if (options.CupaTimisului)
            {
                ViewBag.Title = "Cupa Timisului - "+Contest.Program.ContestDate.Year.ToString();
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                ViewBag.Title = "Ziua Telecomunicatiilor";
            }
            logger.Debug("Submission "+ViewBag.Title);
            if (Submit == "Incarca alt log")
            {
                logger.Debug("Change file requested!");
                return RedirectToAction("Retry", new { calsign = model.Calsign, email = model.Email, phone = model.Phone });
            }

            string fileName;
            if ((string.IsNullOrEmpty(model.FileName) || string.IsNullOrEmpty(model.Log)) && log!=null)
            {
                model.Log = new StreamReader(log.OpenReadStream()).ReadToEnd();
                model.FileName = fileName = log.FileName;
                ModelState.Remove("Log");
            }
            if (ModelState.IsValid)
            {
                List<Cabrillo> logList = new List<Cabrillo>();

                Stream logStream;


                logStream = new MemoryStream(System.Text.Encoding.Default.GetBytes(model.Log));
                fileName = model.FileName;

                logList.Add(Contest.Program.Parse(logStream, fileName, options.IgnoreStartOfLogTag));
                if (options.CupaTimisului)
                {
                    Contest.Program.ParseQSO(logList, options.IgnoreDateTimeValidation, Contest.Program.ScorCupaTimisului);
                }
                if (options.ZiuaTelecomunicatiilor)
                {
                    Contest.Program.ParseQSO(logList, options.IgnoreDateTimeValidation, Contest.Program.ScorZT);
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
                    messages.Add("Linie invalida:" + item);
                }
                foreach (var q in logList[0].QSOs)
                {
                    foreach (var r in q.InvalidResons)
                    {
                        messages.Add("Linie invalida:" + q.ROWQSO);
                        messages.Add("Motiv:" + r);
                    }
                }
                ViewBag.Errors = messages.ToArray();
                ViewBag.QSOS = logList[0].QSOs;
                if (Submit == "Verifica")
                {
                    logger.Debug("Check file requested!");
                    ViewBag.Check = true;
                    return View(model);
                }
                else
                {
                    logger.Debug("Submit file requested!");
                    bool ok = false;
                    while (!ok)
                    {

                        string fn = model.Calsign + " " + model.Email + " " + model.Phone + " -" + System.IO.Path.GetRandomFileName() + ".cabrillo";
                        string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

                        foreach (char c in invalid)
                        {
                            fn = fn.Replace(c.ToString(), "_");
                        }
                        string pathString = System.IO.Path.Combine(System.IO.Path.Combine(_env.WebRootPath, Contest.Program.UploadPath), fn.Replace("/", "_").Replace("\\", "_"));
                        if (!System.IO.File.Exists(pathString))
                        {
                            using (System.IO.FileStream fs = System.IO.File.Create(pathString))
                            {
                                Byte[] b = System.Text.Encoding.Default.GetBytes("CALLSIGN:"+model.Calsign +System.Environment.NewLine+ model.Log);
                                fs.Write(b, 0, b.Length);
                            }
                            ok = true;
                        }
                    }

                    try
                    {
                        string subject = "";
                        if (options.CupaTimisului)
                        {
                            subject = "Log - Cupa Timisului - " + Contest.Program.ContestDate.Year.ToString();
                        }
                        if (options.ZiuaTelecomunicatiilor)
                        {
                            subject = "Log - Ziua Telecomunicatiilor";
                        }
                        MailerModel mdl = new MailerModel(_mailerSettings.Host, _mailerSettings.Port)
                        {
                            ToAddresses = new List<string>() { model.Email },
                            FromAddress = _mailerSettings.From,
                            IsHtml = true,
                            ViewFile = "Mailer/ThankYou",
                            Subject = subject,
                            User = _mailerSettings.Username,
                            Key = _mailerSettings.Password,
                            EnableSsl = _mailerSettings.EnableSSL,
                            Model = new // Your actual class model
                            {
                            }
                        };
                        logger.Debug("Sending Email");
                        _mailer.SendAsyn(mdl);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                    logger.Debug("Thankyou!");
                    return View("ThankYou");
                }
            }
            else {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                logger.Debug("Model not valid: "+ string.Join(Environment.NewLine, allErrors.Select(e => e.ErrorMessage)));
            }
            return View(model);
        }



    }
}
