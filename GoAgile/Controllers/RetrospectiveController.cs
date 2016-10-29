using GoAgile.Models;
using GoAgile.Models.EntityManager;
using System.Web.Mvc;
using GoAgile.Helpers;

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

            if (User.Identity.IsAuthenticated)
            {

            }

            var man = new RetrospectiveManager();                         
            var ret = man.FindModel(id);  

            if (ret == null)
                return HttpNotFound();
            // Rewrite this owner system meybe own page for owner
            else
            {
                if (User.Identity.IsAuthenticated && User.Identity.Name == ret.Owner)
                    return View(new RetrospectiveViewModel { GuidId = id, State = ret.State, Owner = "owner" });
                else
                    return View(new RetrospectiveViewModel { GuidId = id, State = ret.State, Owner = "sorryBro" });
            }                   
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