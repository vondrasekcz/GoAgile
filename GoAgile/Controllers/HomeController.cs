using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using GoAgile.Dal;
using GoAgile.Helpers.Objects;

namespace GoAgile.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Dal for Home controller
        /// </summary>
        private IHomeManager _homeMan;

        /// <summary>
        /// Constructor
        /// </summary>
        public HomeController()
        {
            _homeMan = new HomeManager();
        }

        //
        // GET Home/GoAgile
        public ActionResult GoAgile()
        {
            ViewBag.Message = "Go Agile.";

            return View();
        }

        //
        // GET Home/Events
        [Authorize]
        public ActionResult Events()
        {
            var list = _homeMan.GetUsersAllEvents(User.Identity.Name);

            //string ret = JsonConvert.SerializeObject(list);
            return View(list);
        }

        //
        // GET Home/Contact
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}