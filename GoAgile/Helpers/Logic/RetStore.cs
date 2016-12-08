using System.Collections.Generic;
using GoAgile.Helpers.StoreModels;
using GoAgile.Models.Retrospective;

namespace GoAgile.Helpers.Logic
{
    public sealed class StoreRet
    {
        private static readonly StoreRet _instance = new StoreRet();

        private Dictionary<string, EventRet> _retrospectives = new Dictionary<string, EventRet>();

        private Dictionary<string, string> _connectionIds = new Dictionary<string, string>();


        // TODO:
        private static readonly object Locker = new object();



        private StoreRet()
        {
        }

        public static StoreRet GetInstance
        {
            get
            {
                return _instance;
            }
        }

        // maybe delete this method
        public void AddRet(string GuidId)
        {
            if (!_retrospectives.ContainsKey(GuidId))
                _retrospectives.Add(GuidId, new EventRet());
        }

        public void DeleteRet(string GuidId)
        {
            var ConnectionIds = GetAllConnectionIds(GuidId);

            foreach (var conId in ConnectionIds)
                _connectionIds.Remove(conId);

            _retrospectives.Remove(GuidId);
        }

        public bool AddPm(string connectionId, string retrospectiveGuidId)
        {
            if (!_retrospectives.ContainsKey(retrospectiveGuidId))
                AddRet(retrospectiveGuidId);

            EventRet ret;
            if (_retrospectives.TryGetValue(retrospectiveGuidId, out ret))
                ret.AddPm(connectionId);
            else
                return false;                

            _connectionIds.Add(connectionId, retrospectiveGuidId);

            return true;
        }

        public bool AddUser(string name, string connectionId, string retrospectiveGuidId)
        {
            if (!_retrospectives.ContainsKey(retrospectiveGuidId))
                AddRet(retrospectiveGuidId);

            EventRet ret;
            if (_retrospectives.TryGetValue(retrospectiveGuidId, out ret))
                ret.AddUser(name: name, connectionId: connectionId);
            else
                return false;

            _connectionIds.Add(connectionId, retrospectiveGuidId);

            return true;
        }

        public string GetUserName(string connectionId, string retrospectiveGuidId)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return null;

            return retros.GetUserName(connectionId);
        }

        public string GetUsersEventId(string connectionId)
        {
            string eventGuid;
            if (_connectionIds.TryGetValue(connectionId, out eventGuid))
                return eventGuid;
            return null;
        }

        public string DeleteUser(string connectionId)
        {
            string retroId;
            if (_connectionIds.TryGetValue(connectionId, out retroId))
                _connectionIds.Remove(connectionId);
            else
                return null;

            EventRet retros;
            if (_retrospectives.TryGetValue(retroId, out retros))
                retros.DeleteUser(connectionId);

            return retroId;
        }

        public List<string> GetAllUsers(string retrospectiveGuidId)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return null;
        
            return retros.GetAllUsers();
        }

        public List<string> GetAllConnectionIds(string retrospectiveGuidId)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return null;

            return retros.GetAllConnectionsIds();
        }

        public bool Vote(string retrospectiveGuidId, string connectionId, string voteId, int maxVotes)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return false;

            return retros.Vote(connectionId: connectionId, voteId: voteId, maxVotes: maxVotes);
        }

        public List<UsersVotes> GetUsersAndVotes(string retrospectiveGuidId, string sharedItemGuid)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return null;

            return retros.GetUsersAndVotes(sharedItemGuid);
        }

        public AllSaredItemsModel AddUserVotes(string retrospectiveGuidId, string connectionId, AllSaredItemsModel allItems)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return allItems;

            return retros.AddUserVotes(allItems, connectionId);
        }
    }










    public class EventRet
    {
        private HashSet<string> _projectManager;
        private HashSet<string> _votedPm;
        private Dictionary<string, User> _users;
        private string _pmName;

        public EventRet()
        {
            _projectManager = new HashSet<string>();
            _votedPm = new HashSet<string>();
            _users = new Dictionary<string, User>();
            _pmName = "Project Manager";
        }

        public void AddPm(string connectionId)
        {
            _projectManager.Add(connectionId);
        }

        public void AddUser(string name, string connectionId)
        {
            var user = new User() { UserName = name, Voted = new HashSet<string>() };
            _users.Add(key: connectionId, value: user);
        }

        public string GetUserName(string connectionId)
        {

            if (_projectManager.Contains(connectionId))
                return "Project Manager";

            User user;
            if (!_users.TryGetValue(connectionId, out user))
                return null;
            return user.UserName;
        }

        public void DeleteUser(string connectionId)
        {
            _projectManager.Remove(connectionId);
            _users.Remove(connectionId);
        }

        public List<string> GetAllUsers()
        {
            List<string> ret = new List<string>();

            if ((_projectManager.Count) > 0)
                ret.Add(_pmName);
            
            foreach (var item in _users)
                ret.Add(item.Value.UserName);
            
            return ret;
        }

        public List<string> GetAllConnectionsIds()
        {
            List<string> ret = new List<string>();

            foreach (var item in _projectManager)
                ret.Add(item);

            foreach (var item in _users)
                ret.Add(item.Key);

            return ret;
        }

        public bool Vote(string connectionId, string voteId, int maxVotes)
        {
            if (_projectManager.Contains(connectionId))
            {
                if (_votedPm.Contains(voteId))
                    return false;
                else
                {
                    if (_votedPm.Count < maxVotes)
                        _votedPm.Add(voteId);
                    else
                        return false;
                }
            }
            else
            {
                User user;
                if (!_users.TryGetValue(connectionId, out user))
                    return false;
                if (user.Voted.Contains(voteId))
                    return false;
                else
                {
                    if (user.Voted.Count < maxVotes)
                        user.Voted.Add(voteId);
                    else
                        return false;
                }   
            }
            return true;               
        }

        public List<UsersVotes> GetUsersAndVotes(string sharedItemGuid)
        {
            var ret = new List<UsersVotes>();

            int pmVoted = _votedPm.Count;
            foreach (var pm in _projectManager)
                ret.Add(new UsersVotes { ConnectionId = pm, Voted = pmVoted, EnableVotingForItem = (_votedPm.Contains(sharedItemGuid) == true ) ? false : true });
            foreach (var user in _users)
                ret.Add(new UsersVotes { ConnectionId = user.Key, Voted = user.Value.Voted.Count, EnableVotingForItem = (user.Value.Voted.Contains(sharedItemGuid) == true) ? false : true  });

            return ret;
        }

        public AllSaredItemsModel AddUserVotes(AllSaredItemsModel allItems, string connectionId)
        {
            if (!_projectManager.Contains(connectionId))
                return allItems;

            foreach (var item in allItems.Items)
            {
                if (_votedPm.Contains(item.ItemGuid))
                    item.CanVote = false;
            }
            allItems.RemainingVotes -= _votedPm.Count;

            return allItems;
        }
    }







    public class User
    {
        public string UserName { get; set; }

        public HashSet<string> Voted = new HashSet<string>();
    }
}