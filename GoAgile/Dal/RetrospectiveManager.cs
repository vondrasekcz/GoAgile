using GoAgile.DataContexts;
using GoAgile.Models;
using GoAgile.Models.DB;
using System;
using System.Linq;
using GoAgile.Models.Retrospective;
using System.Collections.Generic;

namespace GoAgile.Dal
{
    public class RetrospectiveManager : IRetrospectiveManager
    {
        /// <inheritdoc />
        string IRetrospectiveManager.AddModel(CreateRetrospectiveViewModel model, string user)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = new Retrospective();

                dbItem.Id = Guid.NewGuid().ToString();
                dbItem.RetrospectiveName = model.RetrospectiveName;
                dbItem.Project = model.Project;
                dbItem.Comment = model.Comment;
                dbItem.DatePlanned = model.DatePlanned;
                dbItem.State = EventState.waiting;
                dbItem.Owner = user;

                db.Retrospectives.Add(dbItem);
                db.SaveChanges();

                return dbItem.Id.ToString();
            }
        }

        /// <inheritdoc />
        bool IRetrospectiveManager.DeleteModel(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                try
                {
                    var model = db.Retrospectives
                        .Single(s => s.Id == guidId);

                    var items = db.RetrospectiveItems
                        .Where(w => w.Retrospective == guidId);

                    foreach (var item in items)
                        db.RetrospectiveItems.Remove(item);

                    db.Retrospectives.Remove(model);
                    db.SaveChanges();

                    return true;
                }
                catch(Exception)
                {
                    return false;
                }
            }
        }

        /// <inheritdoc />
        RetrospectiveModel IRetrospectiveManager.GetModel(string guidId)
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
                        Comment = model.Comment == null ? "-" : model.Comment,
                        DatePlanned = model.DatePlanned == null ? "-"
                                      : model.DatePlanned.Value.Day.ToString() + "." +
                                      model.DatePlanned.Value.Month.ToString() + "." +
                                      model.DatePlanned.Value.Year.ToString(),
                        DateFinished = model.DateFinished == null ? "-"
                                       : model.DateFinished.Value.Day.ToString() + "." +
                                       model.DateFinished.Value.Month.ToString() + "." +
                                       model.DateFinished.Value.Year.ToString() + " " +
                                       model.DateFinished.Value.Hour.ToString() + ":" +
                                       model.DateFinished.Value.Minute.ToString(),
                        DateStarted = model.DateStared == null ? "-"
                                      : model.DateStared.Value.Day.ToString() + "." +
                                      model.DateStared.Value.Month.ToString() + "." +
                                      model.DateStared.Value.Year.ToString() + " " +
                                      model.DateStared.Value.Hour.ToString() + ":" +
                                      model.DateStared.Value.Minute.ToString(),
                        State = Enum.GetName(typeof(EventState), model.State),
                    };

                return ret;
            }
        }

        /// <inheritdoc />
        RetrospectiveInitModel IRetrospectiveManager.GetInitModel(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var model = db
                    .Retrospectives.SingleOrDefault(s => s.Id == guidId);

                if (model == null)
                    return null;

                var ret = new RetrospectiveInitModel()
                {
                   GuidId = model.Id,
                   State = Enum.GetName(typeof(EventState), model.State),
                   Owner = model.Owner
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
                dbItem.UserName = modelItem.Autor;
                dbItem.Text = modelItem.Text;

                db.RetrospectiveItems.Add(dbItem);
                db.SaveChanges();

                return dbItem.Id;
            }
        }

        /// <inheritdoc />
        IList<RetrospectiveItemModel> IRetrospectiveManager.GetAllSharedItems(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var ret = db.RetrospectiveItems
                    .Where(w => w.Retrospective == guidId)
                    .Select(s => new RetrospectiveItemModel()
                    {
                        Autor = s.UserName,
                        Column = s.Section,
                        Text = s.Text,
                        ItemGuid = s.Id,
                        ListId = s.Section == "Start" ? "list_start" : (s.Section == "Stop" ? "list_stop" : "list_continue")
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
                dbItem.DateStared = DateTime.Now;

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
                dbItem.DateFinished = DateTime.Now;

                db.SaveChanges();
            }
        }       

    }
}