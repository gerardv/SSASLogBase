using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSASLogBase.Models;

namespace SSASLogBase.Controllers
{
    public class RefreshesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
