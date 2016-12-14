using System;
using System.Linq;
using System.Collections.Generic;
using GoAgile.DataContexts;
using GoAgile.Models;
using GoAgile.Models.DB;
using GoAgile.Models.Retrospective;

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

                var maxVotes = model.MaxVotes.GetValueOrDefault();

                if (model.EnableVotes == false || maxVotes <= 0)
                {
                    dbItem.Votes = 0;
                    dbItem.EnableVoting = false;
                }                    
                else
                {
                    dbItem.Votes = maxVotes;
                    dbItem.EnableVoting = true;
                }                    

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
                var dbItem = db
                    .Retrospectives.SingleOrDefault(s => s.Id == guidId);

                if (dbItem == null)
                    return null;

                var ret = new RetrospectiveInitModel()
                {
                   GuidId = dbItem.Id,
                   State = Enum.GetName(typeof(EventState), dbItem.State),
                   Owner = dbItem.Owner,
                   Votes = dbItem.Votes,
                   EnableVoting = dbItem.EnableVoting
                };

                return ret;
            }
        }

        /// <inheritdoc />
        void IRetrospectiveManager.AddRetrospectiveItem(RetrospectiveItemModel modelItem, string retrospectiveGuidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = new RetrospectiveItem();

                dbItem.Id = Guid.NewGuid().ToString();
                dbItem.Retrospective = retrospectiveGuidId;
                dbItem.Section = modelItem.Column;
                dbItem.UserName = modelItem.Autor;
                dbItem.Text = modelItem.Text;
                dbItem.Votes = 0;
                dbItem.Color = modelItem.Color;

                db.RetrospectiveItems.Add(dbItem);
                db.SaveChanges();

                modelItem.ItemGuid = dbItem.Id;
                modelItem.Votes = 0;
                modelItem.CanVote = true;
            }
        }

        /// <inheritdoc />
        List<RetrospectiveItemModel> IRetrospectiveManager.GetAllSharedItems(string guidId)
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
                        Votes = s.Votes,
                        CanVote = true,
                        Color = s.Color
                    }).ToList();

                return ret;
            }
        }

        /// <inheritdoc />
        bool IRetrospectiveManager.ChangeRetrospectiveToRunning(string guidId, string moderator)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .Single(s => s.Id == guidId);

                if (moderator == dbItem.Owner
                    && dbItem.State == EventState.waiting)
                {
                    dbItem.State = EventState.running;
                    dbItem.DateStared = DateTime.Now;

                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <inheritdoc />
        bool IRetrospectiveManager.ChangeRetrospectiveToPresenting(string guidId, string moderator)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .Single(s => s.Id == guidId);

                if (moderator == dbItem.Owner
                    && dbItem.State == EventState.running)
                {
                    dbItem.State = EventState.presenting;

                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <inheritdoc />
        bool IRetrospectiveManager.ChangeToRetrospectiveToVoting(string guidId, string moderator)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .Single(s => s.Id == guidId);

                if (moderator == dbItem.Owner
                    && dbItem.State == EventState.presenting)
                {
                    dbItem.State = EventState.voting;

                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <inheritdoc />
        bool IRetrospectiveManager.ChangeToRetrospectiveToFinished(string guidId, string moderator)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .Single(s => s.Id == guidId);

                if (moderator == dbItem.Owner
                    && (dbItem.State == EventState.presenting
                     || dbItem.State == EventState.voting) )
                {
                    dbItem.State = EventState.finished;
                    dbItem.DateFinished = DateTime.Now;

                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <inheritdoc />
        string IRetrospectiveManager.GetRetrospectivePhase(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .SingleOrDefault(s => s.Id == guidId);

                return (dbItem == null) ? null : Enum.GetName(typeof(EventState), dbItem.State);
            }
        }

        /// <inheritdoc />
        bool IRetrospectiveManager.ExistRetrospectiveItem(string itemGuidId, string eventGuidId)
        {
            using (var db = AgileDb.Create())
            {
                var ret = db.RetrospectiveItems
                    .Any(s => s.Id == itemGuidId
                              && s.Retrospective == eventGuidId);

                return ret;
            }
        }

        /// <inheritdoc />
        int IRetrospectiveManager.AddVotesToItem(string itemGuid)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.RetrospectiveItems
                    .Single(s => s.Id == itemGuid);

                dbItem.Votes++;
                db.SaveChanges();

                return dbItem.Votes;
            }
        }

        /// <inheritdoc />
        int IRetrospectiveManager.GetMaxVotesAndValidataVoting(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .Single(s => s.Id == guidId);

                if (dbItem.State != EventState.voting)
                    return -1;
                else if(!dbItem.EnableVoting)
                    return 0;
                else
                    return dbItem.Votes;
            }
        }

        /// <inheritdoc />
        int IRetrospectiveManager.ValidateOwner(string guidId, string name)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .SingleOrDefault(s => s.Id == guidId);

                if (dbItem == null
                    || dbItem.Owner != name)
                    return -1;
                else if (dbItem.State == EventState.finished)
                    return 0;
                else
                    return 1;
            }
        }

        /// <inheritdoc />
        int IRetrospectiveManager.ExistRetrospective(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .SingleOrDefault(s => s.Id == guidId);

                if (dbItem == null)
                    return -1;
                else if (dbItem.State == EventState.finished)
                    return 0;
                else
                    return 1;
            }
        }

        /// <inheritdoc />
        int IRetrospectiveManager.GetMaxVotes(string guidId)
        {
            using (var db = AgileDb.Create())
            {
                var dbItem = db.Retrospectives
                    .SingleOrDefault(s => s.Id == guidId);

                if (dbItem == null)
                    return -1;
                else if (dbItem.EnableVoting == false)
                    return 0;
                else
                    return dbItem.Votes;                
            }
        }
    }
}