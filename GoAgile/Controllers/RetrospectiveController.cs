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
            var eventInfo = man.FindModel(id);

            if (eventInfo == null)
                return HttpNotFound();
            else
                return View(new RetrospectiveViewModel { GuidId = id, State = eventInfo.State});
        }

        //
        // GET Retrospecive/ManageRetrospective{Id}
        [Authorize]
        public ActionResult ManageRetrospective(string id)
        {
            // TODO:  RetrospectiveManager, interface, ...
            var man = new RetrospectiveManager();
            var eventInfo = man.FindModel(id);

            if (eventInfo == null)
                return HttpNotFound();
            
            if (User.Identity.Name == eventInfo.Owner)
            {
                string url = Url.Action("Retrospective/" + id, "Retrospective", null, Request.Url.Scheme);
                return View(new ManageRetrospectiveViewModel { Url = url, State = eventInfo.State, GuidId = id });
            }                
            else
                return RedirectToAction("ManageRetrospective/" + id, "Retrospective");
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
                return RedirectToAction("ManageRetrospective/" + guidId, "Retrospective");
            }
            return View(model);
        }

    }


}