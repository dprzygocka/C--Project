﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Albertslund.Controllers
{
    public class ContactAdvisorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
