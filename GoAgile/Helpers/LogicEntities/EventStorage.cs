using System.Collections.Generic;
using GoAgile.Helpers.Objects;
using System.Linq;
using GoAgile.Models.EntityManager;

namespace GoAgile.Helpers
{
    // TODO: 
    // - some locks
    // - Comments
    // - write own finding methods and structures?
    // - list of clientId -> need fast removeUser
    // TODO: rewrite this sh.t
    public class EventStorage
    {
        private Dictionary<string, Event> _eventStore;

        public EventStorage()
        {
            _eventStore = new Dictionary<string, Event>();
        }

        public bool ExistUser(string eventGuid, string userName)
        {
            Event eventItem;

            if (_eventStore.TryGetValue(eventGuid, out eventItem))
            {
                var ret = eventItem.Users
                    .Any(a => a.Name == userName);

                return ret;
            }
            else
                throw new System.InvalidOperationException("eventGuid: " + eventGuid + " must exist");
        }

        public void AddUser(string eventGuid, string userName, string email, string clientId)
        {
            Event eventItem;

            if (_eventStore.TryGetValue(eventGuid, out eventItem))
            {
                eventItem.Users.Add(new User { Name = userName, ConnectionId = clientId, Email = email });
            }
            else
            {
                var newUser = new User { Name = userName, ConnectionId = clientId, Email = email };
                var listUsers = new List<User>();
                listUsers.Add(newUser);

                var man = new RetrospectiveManager();
                var eventInfo = man.FindModel(eventGuid);
                var state = eventInfo.State;
                _eventStore.Add(eventGuid, new Event { IdGuid = eventGuid, State = state, Users = listUsers });
            }
        }

        public string RemoveUser(string clientId)
        {
            string ret = null;



            foreach (var item in _eventStore.Values)
            {
                foreach (var user in item.Users)
                {
                    if (user.ConnectionId == clientId)
                    {
                        item.Users.Remove(user);

                        ret = item.IdGuid;

                        if (item.Users.Count < 1)
                            _eventStore.Remove(ret);
                        return ret;
                    }                        
                }
            }
            return ret;
        }

        public List<User> EventUsers(string eventGuid)
        {
            Event eventItem;
            var ret = new List<string>();

            if (_eventStore.TryGetValue(eventGuid, out eventItem))
            {
                return eventItem.Users;
            }
            else
                throw new System.InvalidOperationException("eventGuid: " + eventGuid + " must exist");
        }

        public List<string> GetOnlineUsersExceptMe(string clientId, string eventGuid)
        {
            Event eventItem;
            var ret = new List<string>();

            if (_eventStore.TryGetValue(eventGuid, out eventItem))
            {
                ret = eventItem.Users
                    .Where(w => w.ConnectionId != clientId)
                    .Select(s => s.Name)
                    .ToList();

                return ret;
            }
            else
                throw new System.InvalidOperationException("eventGuid: " + eventGuid + " must exist");
        }
    }
}