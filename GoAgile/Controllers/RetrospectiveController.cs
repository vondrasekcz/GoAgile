using GoAgile.Models;
using System.Web.Mvc;

namespace GoAgile.Controllers
{
    public class RetrospectiveController : Controller
    {
        //
        // GET Retrospecive/CreateRetrospective
        [Authorize]
        public ActionResult CreateRetrospective()
        {
            ViewBag.Message = "Create Retrospecive.";

            return View();
        }

        //
        // POST Retrospective/CreateRerospective
        [HttpPost]
        [Authorize]
        public ActionResult CreateRetrospective(CreateRetrospectiveViewModel model)
        {
            if (ModelState.IsValid)
            {

            }
            return View(model);
        }
    }


}