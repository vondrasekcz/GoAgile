using System.Collections.Generic;
using System.Linq;
using GoAgile.Models.Retrospective;
using GoAgile.DataContexts;

namespace GoAgile.Dal
{
    public class HomeManager : IHomeManager
    {
        /// <inheritdoc />
        public IList<EventModel> GetUsersAllEvents(string userName)
        {
            // TODO: sort, add other dates
            using (var db = AgileDb.Create())
            {
                var dataRetrospective = db.Retrospectives
                    .Where(w => w.Owner == userName)
                    .OrderBy(o => o.DateStared)
                    .Select(s => new EventModel()
                    {
                        IdGuid = s.Id,
                        Name = s.RetrospectiveName,
                        Project = s.Project,
                        DatePlanned = s.DatePlanned == null ? "-"
                                      : s.DatePlanned.Value.Day.ToString() + "." + 
                                      s.DatePlanned.Value.Month.ToString() + "." + 
                                      s.DatePlanned.Value.Year.ToString(),
                        DateFinished = s.DateFinished == null ? "-" 
                                       : s.DateFinished.Value.Day.ToString() + "." +
                                       s.DateFinished.Value.Month.ToString() + "." +
                                       s.DateFinished.Value.Year.ToString() + " " +
                                       s.DateFinished.Value.Hour.ToString() + ":" +
                                       s.DateFinished.Value.Minute.ToString(),
                        DateStarted = s.DateStared == null ? "-"
                                      : s.DateStared.Value.Day.ToString() + "." +
                                      s.DateStared.Value.Month.ToString() + "." +
                                      s.DateStared.Value.Year.ToString() + " " +
                                      s.DateStared.Value.Hour.ToString() + ":" +
                                      s.DateStared.Value.Minute.ToString(),
                        Comment = s.Comment == null ? "-" : s.Comment,
                        State = s.State.ToString()
                    }).ToList();

                return dataRetrospective;
            }
        }
    }
}