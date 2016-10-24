using GoAgile.DataContexts;
using GoAgile.Models.DB;

namespace GoAgile.Models.EntityManager
{
    public class RetrospectiveManager
    {
        // TODO rewrite
        public void AddModel(CreateRetrospectiveViewModel model, string user)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = new Retrospective();

                dbItem.Id = "fasfs";
                dbItem.Project = model.Project;
                dbItem.RetrospectiveName = model.RetrospectiveName;
                dbItem.StartDate = model.StartDate;
                dbItem.State = EventState.Created;
                dbItem.Owner = user;

                db.Retrospectives.Add(dbItem);
                db.SaveChanges();
            }
        }         
    }
}