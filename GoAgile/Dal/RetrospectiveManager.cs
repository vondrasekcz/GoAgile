using GoAgile.DataContexts;
using GoAgile.Models.DB;
using System;
using System.Linq;
using GoAgile.Helpers.Objects;
using System.Collections.Generic;

namespace GoAgile.Dal
{
    // TODO: ineterface
    public class RetrospectiveManager : IRetrospectiveManager
    {
        /// <inheritdoc />
        string IRetrospectiveManager.AddModel(RetrospectiveModel model)
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
                dbItem.Owner = model.Owner;

                db.Retrospectives.Add(dbItem);
                db.SaveChanges();

                return dbItem.Id.ToString();
            }
        }

        /// <inheritdoc />
        RetrospectiveModel IRetrospectiveManager.FindModel(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var model = db
                    .Retrospectives.SingleOrDefault(s => s.Id == guidId);

                    if (model == null)
                        return null;

                    var ret = new RetrospectiveModel() {
                        RetrospectiveName = model.RetrospectiveName,
                        Project = model.Project,
                        Owner = model.Owner,
                        Comment = model.Comment,
                        State = Enum.GetName(typeof(EventState), model.State),
                        StartDate = model.StartDate
                    };

                return ret;
            }
        }

        /// <inheritdoc />
        string IRetrospectiveManager.AddRetrospectiveItem(RetrospectiveItemModel modelItem, string retrospectiveGuidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = new RetrospectiveItem();

                dbItem.Id = Guid.NewGuid().ToString();
                dbItem.Retrospective = retrospectiveGuidId;
                dbItem.Section = modelItem.Column;
                dbItem.UserName = modelItem.User;
                dbItem.Text = modelItem.Text;

                db.RetrospectiveItems.Add(dbItem);
                db.SaveChanges();

                return dbItem.Id;
            }
        }

        /// <inheritdoc />
        IList<ItemObject> IRetrospectiveManager.GetAllSharedItems(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var ret = db.RetrospectiveItems
                    .Where(w => w.Retrospective == guidId)
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

        /// <inheritdoc />
        void IRetrospectiveManager.ChangeRetrospectiveToRunning(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .Single(s => s.Id == guidId);

                dbItem.State = EventState.running;

                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        void IRetrospectiveManager.ChangeRetrospectiveToPresenting(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .Single(s => s.Id == guidId);

                dbItem.State = EventState.presenting;

                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        void IRetrospectiveManager.ChangeToRetrospectiveToFinished(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .Single(s => s.Id == guidId);

                dbItem.State = EventState.finished;

                db.SaveChanges();
            }
        }       

    }
}