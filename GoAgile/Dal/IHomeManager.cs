using System.Collections.Generic;
using GoAgile.Models.Retrospective;

namespace GoAgile.Dal
{
    public interface IHomeManager
    {
        /// <summary>
        /// Get all User's Events
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        IList<EventModel> GetUsersAllEvents(string userName);
    }
}
