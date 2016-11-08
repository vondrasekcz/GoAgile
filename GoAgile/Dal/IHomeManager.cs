using System.Collections.Generic;
using GoAgile.Helpers.Objects;

namespace GoAgile.Dal
{
    public interface IHomeManager
    {
        /// <summary>
        /// Get all User's Events
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        IList<Event> GetUsersAllEvents(string userName);
    }
}
