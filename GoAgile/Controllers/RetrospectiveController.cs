using GoAgile.Models;
using GoAgile.Models.EntityManager;
using System.Web.Mvc;

namespace GoAgile.Controllers
{
    // TODO Managers instancing
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
        // GET Retrospecive/Retrospective{Id}
        public ActionResult Retrospective(string id)
        {
            // TODO rewrite
            var man = new RetrospectiveManager();
            var ret = man.FindModel(id);

            if (ret == null)
            {
                return HttpNotFound();
            }
            // maybe only GuidId
            return View(new RetrospectiveViewModel { GuidId = id, State = 0});
        }

        //
        // POST Retrospective/CreateRerospective
        [HttpPost]
        [Authorize]
        public ActionResult CreateRetrospective(CreateRetrospectiveViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO rewrite
                var man = new RetrospectiveManager();
                var guidId = man.AddModel(model: model, user: User.Identity.Name);

                // return retrospective view
                return RedirectToAction("Retrospective/" + guidId, "Retrospective");
            }
            return View(model);
        }
    }


}