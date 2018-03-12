using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebRazorPages.Controllers
{
    public class weeklydealController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}