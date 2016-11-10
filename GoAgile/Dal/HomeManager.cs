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
            // TODO: sort, add other dates, rework data and table
            using (var db = AgileDb.Create())
            {
                var dataRetrospective = db.Retrospectives
                    .Where(w => w.Owner == userName)
                    .Select(s => new EventModel()
                    {
                        IdGuid = s.Id,
                        RetrospectiveName = s.RetrospectiveName,
                        Project = s.Project,
                        DateStart = s.StartDate,
                        Comment = s.Comment,
                        State = s.State.ToString()
                    }).ToList();

                return dataRetrospective;
            }
        }
    }
}