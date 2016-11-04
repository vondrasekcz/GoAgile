using GoAgile.DataContexts;
using GoAgile.Models.DB;
using System;
using System.Linq;
using GoAgile.Helpers.Objects;
using System.Collections.Generic;

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

        // TODO rewrite
        public void ChangeRetrospectiveToRunning(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .Single(s => s.Id == guidId);

                dbItem.State = EventState.running;

                db.SaveChanges();
            }
        }

        // TODO rewrite
        public void ChangeRetrospectiveToPresenting(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .Single(s => s.Id == guidId);

                dbItem.State = EventState.presenting;

                db.SaveChanges();
            }
        }

        // TODO rewrite
        public void ChangeToRetrospectiveToFinished(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .Single(s => s.Id == guidId);

                dbItem.State = EventState.finished;

                db.SaveChanges();
            }
        }

        // TODO rewrite
        public string AddRetrospectiveItem(string column, string text, string user, string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = new RetrospectiveItem();


                dbItem.Id = Guid.NewGuid().ToString();
                dbItem.Retrospective = guidId;
                dbItem.Section = column;
                dbItem.UserName = user;
                dbItem.Text = text;

                db.RetrospectiveItems.Add(dbItem);
                db.SaveChanges();

                return dbItem.Id;
            }
            
        }

        // TODO rewrite
        public List<ItemObject> GetAllSharedItems(string GuidId)
        {
            using (var db = AgileDb.Create())
            {
                var ret = db.RetrospectiveItems
                    .Where(w => w.Retrospective == GuidId)
                    .Select(s => new ItemObject()
                    {
                        autor = s.UserName,
                        column = s.Section,
                        text = s.Text,
                        itemGuid = s.Id,
                        listId = s.Section == "Start" ? "list_start" : (s.Section == "Stop" ? "list_stop" : "list_continue")
                    }).ToList();

                return ret;
            }

        }

    }
}