using GoAgile.DataContexts;
using GoAgile.Models.DB;
using System;
using System.Linq;

namespace GoAgile.Models.EntityManager
{
    public class RetrospectiveManager
    {
        // just for test
        public string FindModel(string id)
        {
            using (var db = AgileDb.Create())
            {
                var ret = db.Retrospectives.SingleOrDefault(s => s.Id.ToString() == id);

                return ret?.Id.ToString();
            }
        }

        // TODO rewrite
        public string AddModel(CreateRetrospectiveViewModel model, string user)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = new Retrospective();
                
              
                dbItem.Id = Guid.NewGuid().ToString();
                dbItem.RetrospectiveName = model.RetrospectiveName;
                dbItem.Project = model.Project;
                dbItem.Comment = model.Comment;
                dbItem.StartDate = model.StartDate;
                dbItem.State = EventState.Created;
                dbItem.Owner = user;

                db.Retrospectives.Add(dbItem);
                db.SaveChanges();

                return dbItem.Id.ToString();
            }
        }         
    }
}