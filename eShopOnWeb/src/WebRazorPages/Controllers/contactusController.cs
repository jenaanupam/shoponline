using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebRazorPages.Controllers
{
    [Route("[controller]/[action]")]
    public class contactusController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Pages/contactus/Index.cshtml");
        }
    }
}