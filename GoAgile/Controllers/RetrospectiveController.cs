using System.Web.Mvc;
using GoAgile.Models;
using GoAgile.Dal;
using GoAgile.Models.Retrospective;

namespace GoAgile.Controllers
{
    public class RetrospectiveController : Controller
    {
        /// <summary>
        /// Dal for Retrospective controller
        /// </summary>
        private IRetrospectiveManager _retrospectiveMan;

        /// <summary>
        /// Constructor
        /// </summary>
        public RetrospectiveController()
        {
            _retrospectiveMan = new RetrospectiveManager();
        }

        //
        // GET Retrospecive/CreateRetrospective
        [Authorize]
        public ActionResult CreateRetrospective()
        {
            ViewBag.Message = "Create Retrospecive.";

            return View();
        }

        //
        // GET Retrospecive/RetrospectiveDetail{Id}
        public ActionResult RetrospectiveDetail(string id)
        {
            var retrospectiveModel = _retrospectiveMan.GetModel(id);

            if (retrospectiveModel == null)
                return HttpNotFound();

            var retrospectiveItemsModel = _retrospectiveMan.GetAllSharedItems(id);
            var model = new FullRetrospectiveModel() { ModelItem = retrospectiveModel, Items = retrospectiveItemsModel };

            return View(model);
        }

        //
        // GET Retrospecive/Retrospective{Id}
        public ActionResult Retrospective(string id)
        {
            var initModel = _retrospectiveMan.GetInitModel(id);

            if (initModel == null)
                return HttpNotFound();
            else
                return View(initModel);
        }

        //
        // GET Retrospecive/ManageRetrospective{Id}
        [Authorize]
        public ActionResult ManageRetrospective(string id)
        {
            var initModel = _retrospectiveMan.GetInitModel(id);

            if (initModel == null)
                return HttpNotFound();
            
            if (User.Identity.Name == initModel.Owner)
            {
                initModel.Url = Url.Action("Retrospective/" + id, "Retrospective", null, Request.Url.Scheme);
                return View(initModel);
            }                
            else
                return RedirectToAction("ManageRetrospective/" + id, "Retrospective");
        }

        //
        // POST Retrospective/CreateRerospective
        [HttpPost]
        [Authorize]
        public ActionResult CreateRetrospective(CreateRetrospectiveViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var guidId = _retrospectiveMan.AddModel(model: viewModel, user: User.Identity.Name);
                
                return RedirectToAction("ManageRetrospective/" + guidId, "Retrospective");
            }
            return View(viewModel);
        }

        [Authorize]
        public ActionResult DeleteRetrospective(string id)
        {
            if (_retrospectiveMan.DeleteModel(id))
                return Json(new { success = true });
            else
                return Json(new { success = false });
        }


    }
}