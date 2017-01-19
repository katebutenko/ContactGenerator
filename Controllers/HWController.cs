using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Controllers
{
    public class HWController: Controller
    {
        public ActionResult Index()
        {
            return View(DateTime.Today);
        }

    public ActionResult RandomGenerateContacts()
    {
      return View(DateTime.Today);
    }
  }
}