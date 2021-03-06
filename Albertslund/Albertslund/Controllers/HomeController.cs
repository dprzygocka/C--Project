using Albertslund.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Albertslund.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.UserLogged = HttpContext.Session.GetInt32("SessionUserId");
            ViewBag.SessionSuccess = HttpContext.Session.GetInt32("SessionSuccess");
            DbContext context = HttpContext.RequestServices.GetService(typeof(Albertslund.Models.DbContext)) as DbContext;
            if (HttpContext.Session.GetInt32("SessionUserId") == null) {
                Reader(context);
            }
            


            return View();
       
        }


        public void Reader(DbContext context)
        {
            String WatchingHere = @"New CSV";
            var fileSystemWatcher = new FileSystemWatcher(WatchingHere)
            {
                Filter = "*.csv",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.Attributes,
                EnableRaisingEvents = true
            };
    
            fileSystemWatcher.Created += (object sender, FileSystemEventArgs e) => {
                string TheNewFile = e.Name;
                string TheNewFilePath = Path.GetFullPath(TheNewFile);
                Debug.WriteLine("*** Hey! A new file was added.");
                Debug.WriteLine(e.ChangeType + ".");
                Debug.WriteLine(TheNewFile);
                Debug.WriteLine("The full filepath of the newly added .csv is:");
                Debug.WriteLine(TheNewFilePath);
                Debug.WriteLine("The file" + TheNewFile + " will now be read and saved in the database.");

                if (context.createDBEntries(TheNewFilePath))
                {
                    Debug.WriteLine("Wrote to DB");
                }
            }; 
            fileSystemWatcher.Deleted += ActionOccurOnFileDeled;
            fileSystemWatcher.Renamed += ActionOccurOnFileRenamed;
            Debug.WriteLine("File System Watcher is now running." +
                "\nThe following path is being monitored: '" + WatchingHere + "'." +
                "\nAny changes made will appear below.");
        }

     




        public void ActionOccurOnFileDeled(object sender, FileSystemEventArgs e)
        {
            string TheNewFile = e.Name;
            string TheNewFilePath = Path.GetFullPath(TheNewFile);
            Debug.WriteLine("*** Hey! A new file was added.");
            Debug.WriteLine(e.ChangeType + ".");
            Debug.WriteLine(TheNewFile);
            Debug.WriteLine(TheNewFilePath);
            Debug.WriteLine("*** The following file has been deleted:");
            Debug.WriteLine(TheNewFile);
        }

        public void ActionOccurOnFileRenamed(object sender, RenamedEventArgs e)
        {
            Debug.WriteLine("*** The following file has been renamed:");
            Debug.WriteLine($"The old file name was: '{e.OldName}'.");
            Debug.WriteLine($"The new file name is: '{e.Name}'.");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
