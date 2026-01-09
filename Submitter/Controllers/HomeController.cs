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
using Microsoft.Extensions.Logging;
using Submitter;
using System.Reflection;

namespace LogSubmit.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Contest.Options options;
        private readonly ICoreMvcMailer _mailer;
        private readonly MailerSettings _mailerSettings;
        private readonly IWebHostEnvironment _env;
        public HomeController(ICoreMvcMailer mailer, IOptions<MailerSettings> mailerSettings, IWebHostEnvironment env,ILogger<HomeController> logger )
        {
            _logger = logger;
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

            options.Etapa1Start = Contest.Program.ContestDate.AddHours(14);
            options.Etapa1End = Contest.Program.ContestDate.AddHours(15);

            options.Etapa2Start = Contest.Program.ContestDate.AddHours(15);
            options.Etapa2End = Contest.Program.ContestDate.AddHours(16);
        }
        [HttpGet("/Results/{year?}")]
        public ActionResult Results(int? year)
        {
            // selected year defaults to contest year
            int selectedYear = year ?? Contest.Program.ContestDate.Year;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.ContestDate = Contest.Program.ContestDate;

            // Normalize UploadPath and remove trailing year if present
            var uploadPathRaw = (Contest.Program.UploadPath ?? "").Replace('\\', '/').Trim('/');
            string uploadRootRelative;
            var parts = uploadPathRaw.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1 && int.TryParse(parts.Last(), out _))
            {
                uploadRootRelative = string.Join('/', parts.Take(parts.Length - 1));
            }
            else
            {
                uploadRootRelative = uploadPathRaw;
            }

            ViewBag.UploadRootRelative = uploadRootRelative; // e.g. "Uploads"

            // physical uploads root
            var uploadsRoot = Path.Combine(_env.WebRootPath, uploadRootRelative ?? string.Empty);

            // enumerate available years (subfolders under uploadsRoot)
            List<int> years = new List<int>();
            if (Directory.Exists(uploadsRoot))
            {
                years = Directory.GetDirectories(uploadsRoot)
                    .Select(d => Path.GetFileName(d))
                    .Select(s => { int y; return int.TryParse(s, out y) ? (int?)y : null; })
                    .Where(n => n.HasValue)
                    .Select(n => n.Value)
                    .OrderByDescending(y => y)
                    .ToList();
            }
            ViewBag.AvailableYears = years;

            // path to result file for selected year: wwwroot/{uploadRootRelative}/{selectedYear}/Result/total.cshtml
            var resultFilePath = Path.Combine(uploadsRoot, selectedYear.ToString(), "Result", "total.cshtml");
            FileInfo result = new FileInfo(resultFilePath);
            if (!result.Exists || result.CreationTime.AddMinutes(5) < DateTime.Now)
            {
                // recreate result file for the selected year
                var originalInput = options.InputFolder;
                try
                {
                    options.InputFolder = Path.Combine(uploadsRoot, selectedYear.ToString());
                    Contest.Program.ProcessFolder(options, _logger);
                }
                finally
                {
                    options.InputFolder = originalInput;
                }
            }

            // Read result file content (moved from view)
            if (System.IO.File.Exists(resultFilePath))
            {
                try
                {
                    ViewBag.ResultHtml = System.IO.File.ReadAllText(resultFilePath);
                    ViewBag.ResultExists = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading result file for year {Year}", selectedYear);
                    ViewBag.ResultHtml = null;
                    ViewBag.ResultExists = false;
                }
            }
            else
            {
                ViewBag.ResultHtml = null;
                ViewBag.ResultExists = false;
            }

            // set view title (include year)
            if (options.CupaTimisului)
            {
                ViewBag.Title = "Cupa Timisului - " + selectedYear.ToString();
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                ViewBag.Title = "Ziua Telecomunicatiilor";
            }

            return View();
        }
        [HttpGet()]
        public ActionResult Index()
        {
            if (!Directory.Exists(options.InputFolder)) {
                Directory.CreateDirectory(options.InputFolder);
            }
            var title = "";
            if (options.CupaTimisului)
            {
                title = "Cupa Timisului - " + Contest.Program.ContestDate.Year.ToString();
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                title = "Ziua Telecomunicatiilor";
            }
            ViewBag.ContestDate = Contest.Program.ContestDate;
            ViewBag.Title = title;
            _logger.LogDebug(MyLogEvents.LandingPage, "Landing Page {Title}" , title);
            return View(new Entry());
        }
        public ActionResult Retry(string calsign, string email, string phone)
        {
            var title = "";
            if (options.CupaTimisului)
            {
                title = "Cupa Timisului - " + Contest.Program.ContestDate.Year.ToString();
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                title = "Ziua Telecomunicatiilor";
            }
            ViewBag.ContestDate = Contest.Program.ContestDate;
            ViewBag.Title = title;
            _logger.LogDebug(MyLogEvents.ChangeFile, "Change file {Title}", title);
            return View("Index", new Entry() { Calsign = calsign, Email = email, Phone = phone });
        }
        [HttpPost]
        public  ActionResult Index(Entry model, IFormFile log, string Submit)
        {

            var title = "";
            if (options.CupaTimisului)
            {
                title = "Cupa Timisului - " + Contest.Program.ContestDate.Year.ToString();
            }
            if (options.ZiuaTelecomunicatiilor)
            {
                title = "Ziua Telecomunicatiilor";
            }
            ViewBag.ContestDate = Contest.Program.ContestDate;
            ViewBag.Title = title;
            _logger.LogDebug(MyLogEvents.Submission, "Submission {Title}", title);
            if (Submit == "Incarca alt log")
            {
                _logger.LogDebug(MyLogEvents.ChangeFileRequest,"Change file requested!");
                return RedirectToAction("Retry", new { calsign = model.Calsign, email = model.Email, phone = model.Phone });
            }

            if ((string.IsNullOrEmpty(model.FileName) || string.IsNullOrEmpty(model.Log)) && log!=null)
            {
                model.Log = new StreamReader(log.OpenReadStream()).ReadToEnd();
                model.FileName = log.FileName;
                ModelState.Remove("Log");
            }
            if (ModelState.IsValid)
            {
                List<Cabrillo> logList = new List<Cabrillo>();

                Stream logStream;


                logStream = new MemoryStream(System.Text.Encoding.Default.GetBytes(model.Log));

                logList.Add(Contest.Program.Parse(logStream, model.FileName, options.IgnoreStartOfLogTag, _logger));
                if (options.CupaTimisului)
                {
                    Contest.Program.ParseQSO(logList, options.IgnoreDateTimeValidation, Contest.Program.ScorCupaTimisului, _logger);
                }
                if (options.ZiuaTelecomunicatiilor)
                {
                    Contest.Program.ParseQSO(logList, options.IgnoreDateTimeValidation, Contest.Program.ScorZT, _logger);
                }
                List<Tuple<DateTime, DateTime>> startendPairs = new List<Tuple<DateTime, DateTime>>();
                startendPairs.Add(Tuple.Create(options.Etapa1Start, options.Etapa1End));
                startendPairs.Add(Tuple.Create(options.Etapa2Start, options.Etapa2End));
                List<List<QSO>> etape = Cabrillo.ProcessQSOs(logList, startendPairs, options.IgnoreDateTimeValidation, _logger);
                foreach (List<QSO> etapa in etape)
                {
                    Cabrillo.CheckOneQSO(etapa, _logger);
                }

                foreach (var q in logList[0].QSOs)
                {
                    Cabrillo.ParseFrequency(q, options.IgnoreModeFrequencyValidation, _logger);
                }
                Cabrillo.ModeChangeCheck(logList[0], _logger);

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
                    _logger.LogDebug(MyLogEvents.CheckFileRequest, "Check file requested!");
                    ViewBag.Check = true;
                    return View(model);
                }
                else
                {
                    _logger.LogDebug(MyLogEvents.SubmitFileRequest, "Submit file requested!");
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
                        _logger.LogDebug(MyLogEvents.SendingEmail, "Sending Email");
                        _mailer.SendAsync(mdl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(MyLogEvents.Error, ex, "Send Email error");
                    }
                    _logger.LogDebug(MyLogEvents.Thankyou, "Thankyou!");
                    return View("ThankYou");
                }
            }
            else {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                _logger.LogDebug(
                    MyLogEvents.ModelNotValied,
                    "Model not valid: {ErrorMessages}",
                    string.Join(Environment.NewLine, allErrors.Select(e => e.ErrorMessage))
                );
            }
            return View(model);
        }



    }


}
