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
            var model = new FullRetrospectiveModel() { Model = retrospectiveModel, Items = retrospectiveItemsModel };

            return View(model);
        }

        //
        // GET Retrospecive/Retrospective{Id}
        public ActionResult Retrospective(string id)
        {
            var eventInfo = _retrospectiveMan.GetModel(id);

            if (eventInfo == null)
                return HttpNotFound();
            else
                return View(new RetrospectiveInitModel { GuidId = id, State = eventInfo.State});
        }

        //
        // GET Retrospecive/ManageRetrospective{Id}
        [Authorize]
        public ActionResult ManageRetrospective(string id)
        {
            var eventInfo = _retrospectiveMan.GetModel(id);

            if (eventInfo == null)
                return HttpNotFound();
            
            if (User.Identity.Name == eventInfo.Owner)
            {
                string url = Url.Action("Retrospective/" + id, "Retrospective", null, Request.Url.Scheme);
                return View(new RetrospectiveInitModel { Url = url, State = eventInfo.State, GuidId = id });
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
                var retModel = new RetrospectiveModel();
                retModel.Comment = model.Comment;
                retModel.Owner = User.Identity.Name;
                retModel.Project = model.Project;
                retModel.RetrospectiveName = model.RetrospectiveName;
                retModel.DatePlanned = model.DatePlanned;

                var guidId = _retrospectiveMan.AddModel(retModel);
                
                return RedirectToAction("ManageRetrospective/" + guidId, "Retrospective");
            }
            return View(model);
        }




    }
}