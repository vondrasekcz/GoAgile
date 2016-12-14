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


        // TODO: lock it all
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
            List<string> connectionIds = new List<string>();
            GetAllConnectionIds(GuidId, connectionIds);

            foreach (var conId in connectionIds)
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

        public bool  GetUserName(string connectionId, string retrospectiveGuidId, RetrospectiveItemModel model)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return false;

            return retros.GetUserName(connectionId, model);
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

        public void UserIsReady(string connectionId, string retrospectiveGuidId, List<string> connectionIds)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return;

            retros.UserIsReady(connectionId: connectionId, connectionIds: connectionIds);
        }

        public void NoOneIsReady(string retrospectiveGuidId)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return;

            retros.NoOneIsReady();
        }

        public bool AmIReady(string connectionId, string retrospectiveGuidId)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return false;

            return retros.AmIReady(connectionId);
        }

        public void GetAllUsers(string retrospectiveGuidId, List<OnlineUser> list)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return;

            retros.GetAllUsers(list);
        }

        public void GetAllConnectionIds(string retrospectiveGuidId, List<string> recievers)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return;

            retros.GetAllConnectionsIds(recievers);
        }

        public bool Vote(string retrospectiveGuidId, string connectionId, string voteId, int maxVotes)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return false;

            return retros.Vote(connectionId: connectionId, voteId: voteId, maxVotes: maxVotes);
        }

        public void GetUsersAndVotes(string retrospectiveGuidId, string sharedItemGuid, List<UsersVotes> list)
        {
            EventRet retros;
            if (!_retrospectives.TryGetValue(retrospectiveGuidId, out retros))
                return;

            retros.GetUsersAndVotes(sharedItemGuid, list);
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
        private bool _pmReady;
        private int _pmColor;
        private int _lastColor;

        public EventRet()
        {
            _projectManager = new HashSet<string>();
            _votedPm = new HashSet<string>();
            _users = new Dictionary<string, User>();
            _pmName = "Project Manager";
            _lastColor = 0;
        }

        public void AddPm(string connectionId)
        {
            _projectManager.Add(connectionId);
            if (_projectManager.Count == 1)
                _pmColor = AddColor();
        }

        public void AddUser(string name, string connectionId)
        {
            var user = new User() { UserName = name, Voted = new HashSet<string>(), Color = AddColor() };
            _users.Add(key: connectionId, value: user);
        }

        public bool GetUserName(string connectionId, RetrospectiveItemModel model)
        {
            if (_projectManager.Contains(connectionId))
            {
                model.Autor = _pmName;
                model.Color = _pmColor;
                return true;
            }
               
            User user;
            if (_users.TryGetValue(connectionId, out user))
            {
                model.Autor = user.UserName;
                model.Color = user.Color;
                return true;
            }
            return false;
        }

        public void DeleteUser(string connectionId)
        {
            _projectManager.Remove(connectionId);
            _users.Remove(connectionId);
        }

        public void UserIsReady(string connectionId, List<string> connectionIds)
        {
            if (_projectManager.Contains(connectionId))
            {
                _pmReady = true;
                foreach (var conn in _projectManager)
                    connectionIds.Add(conn);
            }                
            else
            {
                User user;
                if (!_users.TryGetValue(connectionId, out user))
                    return;

                user.Ready = true;
                connectionIds.Add(connectionId);
            }
        }

        public void NoOneIsReady()
        {
            _pmReady = false;
            foreach (var user in _users)
                user.Value.Ready = false;
        }

        public bool AmIReady(string connectionId)
        {
            if (_projectManager.Contains(connectionId))
                return _pmReady;
            else
            {
                User user;
                if (!_users.TryGetValue(connectionId, out user))
                    return false;
                return user.Ready;
            }
        }

        public void GetAllUsers(List<OnlineUser> ret)
        {
            if ((_projectManager.Count) > 0)
                ret.Add(new OnlineUser() { Name = _pmName, Ready = _pmReady, Color = _pmColor });
                        
            foreach (var item in _users)
                ret.Add(new OnlineUser() { Name = item.Value.UserName, Ready = item.Value.Ready, Color = item.Value.Color });
        }

        public List<string> GetAllConnectionsIds(List<string> ret)
        {
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

        public void GetUsersAndVotes(string sharedItemGuid, List<UsersVotes> ret)
        {
            int pmVoted = _votedPm.Count;
            foreach (var pm in _projectManager)
                ret.Add(new UsersVotes { ConnectionId = pm, Voted = pmVoted, EnableVotingForItem = (_votedPm.Contains(sharedItemGuid) == true ) ? false : true });
            foreach (var user in _users)
                ret.Add(new UsersVotes { ConnectionId = user.Key, Voted = user.Value.Voted.Count, EnableVotingForItem = (user.Value.Voted.Contains(sharedItemGuid) == true) ? false : true  });
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

        private int AddColor()
        {
            if (_lastColor >= 19)
            {
                _lastColor = 0;
                return _lastColor;
            }
            else
                return _lastColor++;            
        }
    }







    public class User
    {
        public string UserName { get; set; }

        public bool Ready { get; set; }

        public int Color { get; set; }

        public HashSet<string> Voted = new HashSet<string>();
    }
}