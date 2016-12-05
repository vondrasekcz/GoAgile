using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Security.AccessControl;

namespace GoAgile.Helpers.Logic
{
    public class UserStorageVer2
    {
        private  Dictionary<string, EventStore> _retrospectives = new Dictionary<string, EventStore>();

        private  Dictionary<string, string> _connectionIds = new Dictionary<string, string>();

        public void AddUser(string name, string connectionId, string retrospectiveGuidId)
        {
            if (!_retrospectives.ContainsKey(retrospectiveGuidId))
                _retrospectives.Add(retrospectiveGuidId, new EventStore());

            EventStore retros;
            _retrospectives.TryGetValue(retrospectiveGuidId, out retros);

            if (retros != null)
                retros.AddUser(name: name, connectionId: connectionId);

            _connectionIds.Add(connectionId, retrospectiveGuidId);
        }

        // rewrite
        public void AddPm(string connectionId, string retrospectiveGuidId)
        {
            if (!_retrospectives.ContainsKey(retrospectiveGuidId))
                _retrospectives.Add(retrospectiveGuidId, new EventStore());

            EventStore retros;
            _retrospectives.TryGetValue(retrospectiveGuidId, out retros);

            if (retros != null)
                retros.AddPm(connectionId);

            _connectionIds.Add(connectionId, retrospectiveGuidId);
        }

        public string Delete(string connectionId)
        {
            if (!_connectionIds.ContainsKey(connectionId))
                return null;

            string retroId;
            _connectionIds.TryGetValue(connectionId, out retroId);
            if (retroId == null)
                return null;

            EventStore retros;
            _retrospectives.TryGetValue(retroId, out retros);
            if (retros == null)
                return null;

            retros.DeleteUser(connectionId);
            
            _connectionIds.Remove(connectionId);
            return retroId;
        }

        public List<string> GetAllUsers(string retrospectiveGuidId)
        {
            EventStore retros;
            _retrospectives.TryGetValue(retrospectiveGuidId, out retros);
            if (retros == null)
                return null;

            return retros.GetAllUsers();
        }

        public List<string> GetAllConnectionIds(string retrospectiveGuidId)
        {
            EventStore retros;
            _retrospectives.TryGetValue(retrospectiveGuidId, out retros);
            if (retros == null)
                return null;

            return retros.GetAllConnectionsIds();
        }

        public bool AddVote(string retrospectiveGuidId, string connectionId, string voteId)
        {
            EventStore retros;
            _retrospectives.TryGetValue(retrospectiveGuidId, out retros);
            if (retros == null)
                return false;

            return retros.AddVote(connectionId: connectionId, voteId: voteId);
        }
    }








    public class EventStore
    {
        private HashSet<string> _projectManager = new HashSet<string>();

        private HashSet<string> _voted = new HashSet<string>();

        private Dictionary<string, User> _users = new Dictionary<string, User>();

        private int _maxVotes;


        public void AddPm(string connectionId)
        {
            _projectManager.Add(connectionId);
        }

        public void AddUser(string name, string connectionId)
        {
            var user = new User() {UserName = name, Voted = new HashSet<string>()};
            _users.Add(key: connectionId, value: user);
        }

        // rewrite
        public void DeleteUser(string connectionId)
        {
            _projectManager.Remove(connectionId);
            _users.Remove(connectionId);
        }

        public List<string> GetAllUsers()
        {
            List<string> ret = new List<string>();

            int count;

            if ((count = _projectManager.Count) > 0)
            {

                ret.Add("PM " + count + "x");
            }
            
            foreach (var item in _users)
            {
                ret.Add(item.Value.UserName);
            }
            return ret;
        }

        public List<string> GetAllConnectionsIds()
        {
            List<string> ret = new List<string>();

            foreach (var item in _projectManager)
            {
                ret.Add(item);
            }

            foreach (var item in _users)
            {
                ret.Add(item.Key);
            }

            return ret;
        }

        public bool AddVote(string connectionId, string voteId)
        {
            if (_projectManager.Contains(connectionId))
            {
                if (_voted.Contains(voteId))
                    return false;
                else
                {
                    _voted.Add(voteId);
                    return true;
                }
            }
            else
            {
                User user;
                var ret = _users.TryGetValue(connectionId, out user);
                if (ret == false)
                    return false;
                if (user.Voted.Contains(voteId))
                    return false;
                else
                {
                    user.Voted.Add(voteId);
                    return true;
                }
            }

        }
    }








}