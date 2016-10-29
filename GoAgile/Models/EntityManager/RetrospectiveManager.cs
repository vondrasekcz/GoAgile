using GoAgile.DataContexts;
using GoAgile.Models.DB;
using System;
using System.Linq;
using GoAgile.Helpers;

namespace GoAgile.Models.EntityManager
{
    // TODO: ineterface
    public class RetrospectiveManager
    {
        // TODO rewrite
        public ReturnObjRetrospective FindModel(string id)
        {
            using (var db = AgileDb.Create())
            {
                var ret = new ReturnObjRetrospective();

                var dbItem = db.Retrospectives.SingleOrDefault(s => s.Id.ToString() == id);

                if (dbItem == null)
                    return null;
                else
                {
                    ret.State = Enum.GetName(typeof(EventState), dbItem.State);
                    ret.Owner = dbItem.Owner;
                }
                return ret;
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
                dbItem.State = EventState.waiting;
                dbItem.Owner = user;

                db.Retrospectives.Add(dbItem);
                db.SaveChanges();

                return dbItem.Id.ToString();
            }
        }         
    }
}