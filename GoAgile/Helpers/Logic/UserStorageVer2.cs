using System.Collections.Generic;

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
    }

    public class EventStore
    {
        private  HashSet<string> _projectManager = new HashSet<string>();

        private  Dictionary<string, string> _users = new Dictionary<string, string>();

        public void AddPm(string connectionId)
        {
            _projectManager.Add(connectionId);
        }

        public void AddUser(string name, string connectionId)
        {
            _users.Add(key: connectionId, value: name);
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
                ret.Add(item.Value);
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
    }
}