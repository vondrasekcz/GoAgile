using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using GoAgile.Helpers.Logic;
using GoAgile.Models.DB;
using GoAgile.Models.Retrospective;
using GoAgile.Helpers.StoreModels;
using GoAgile.Dal;

namespace GoAgile.Hubs
{
    public class RetrospectiveHub : Hub
    {
        /// <summary>
        /// User store and logic for users
        /// </summary>
        private static StoreRet _store;

        /// <summary>
        /// DAL for Retrospectives
        /// </summary>
        private IRetrospectiveManager _retrospectiveMan;

        /// <summary>
        /// Constructor
        /// </summary>
        public RetrospectiveHub()
        {
            _retrospectiveMan = new RetrospectiveManager();
            _store = StoreRet.GetInstance;
        }     

        /// <summary>
        /// Save and send shared Retrospective item
        /// </summary>
        /// <param name="column"></param>
        /// <param name="text"></param>
        /// <param name="user"></param>
        public void sendSharedItem(string column, string text)
        {
            // TODO: error, send it to caller
            if (string.IsNullOrWhiteSpace(column)
                || string.IsNullOrWhiteSpace(text) 
                || ( column != "Start" && column != "Stop" && column != "Continue") )
                return;

            var connectionId = GetClientId();
            var eventGuid = _store.GetUsersEventId(connectionId);

            // Cant find user connectionId
            if (eventGuid == null)
                return;

            // Retrospective doesn't exist or isn't in 'presenting' phase
            var phase = _retrospectiveMan.GetRetrospectivePhase(eventGuid);
            if (phase == null || phase != "presenting")
                return;

            var userName = _store.GetUserName(connectionId: connectionId, retrospectiveGuidId: eventGuid);
            if (userName == null)
                return;

            var itemModel = new RetrospectiveItemModel { Column = column, Text = text, Autor = userName };
             _retrospectiveMan.AddRetrospectiveItem(itemModel, retrospectiveGuidId: eventGuid);
            var item = JsonConvert.SerializeObject(itemModel);

            List<string> recievers = new List<string>();
            _store.GetAllConnectionIds(eventGuid, recievers);
            Clients.Clients(recievers).recieveSharedItem(item);
        }

        /// <summary>
        /// Add vote to Retrospective Item
        /// </summary>
        /// <param name="column"></param>
        /// <param name="sharedItemGuid"></param>
        public void sharedItemVoted(string column, string sharedItemGuid)
        {
            var connectionId = GetClientId();
            var eventGuid = _store.GetUsersEventId(connectionId);

            // Cant find user connectionId
            if (eventGuid == null)
                return;

            // Retrospective isn't in 'voting' phase or voding is disabled       
            var maxVotes = _retrospectiveMan.GetMaxVotesAndValidataVoting(eventGuid);
            if (maxVotes < 1)
                return;

            // retrospective item doens't exist
            if (!_retrospectiveMan.ExistRetrospectiveItem(sharedItemGuid, eventGuid))
                return;

            // Can't find user
            if (!_store.Vote(eventGuid, connectionId, sharedItemGuid, maxVotes))
                return;

            var totalVotes = _retrospectiveMan.AddVotesToItem(sharedItemGuid);
            List<UsersVotes> users = new List<UsersVotes>();
            _store.GetUsersAndVotes(eventGuid, sharedItemGuid, users);

            foreach (var user in users)
            {
                var model = new VotingModel() { Column = column, SharedItemGuid = sharedItemGuid, VotesTotal = totalVotes, RemainnigVotes = maxVotes - user.Voted, EnableThisItemVoting = user.EnableVotingForItem };
                var item = JsonConvert.SerializeObject(model);

                Clients.Client(user.ConnectionId).addVotesToSharedItem(item);
            }
        }

        /// <summary>
        /// Retrospective starts, change state to 'running'
        /// </summary>
        /// <param name="eventGuid"></param>
        [Authorize]
        public void startRetrospectiveRunning()
        {
            var name = Context.User.Identity.Name;
            var connectionId = GetClientId();
            var eventGuid = _store.GetUsersEventId(connectionId);

            // Cant find user connectionId
            if (eventGuid == null)
                return;

            // Check moderator authority 
            if (!_retrospectiveMan.ChangeRetrospectiveToRunning(eventGuid, name))
                return;

            List<string> recievers = new List<string>();
            _store.GetAllConnectionIds(eventGuid, recievers);
            Clients.Clients(recievers).startRunningMode();            
        }

        /// <summary>
        /// Retrospective presenting starts, change state to 'presenting'
        /// </summary>
        /// <param name="eventGuid"></param>
        [Authorize]
        public void startRetrospectivePresenting()
        {
            var name = Context.User.Identity.Name;
            var connectionId = GetClientId();
            var eventGuid = _store.GetUsersEventId(connectionId);

            // Cant find user connectionId
            if (eventGuid == null)
                return;

            // Check moderator authority 
            if (!_retrospectiveMan.ChangeRetrospectiveToPresenting(eventGuid, name))
                return;

            List<string> recievers = new List<string>();
            _store.GetAllConnectionIds(eventGuid, recievers);
            Clients.Clients(recievers).startPresentingMode();

            _store.NoOneIsReady(eventGuid);
            UsersChanged(eventGuid);
        }

        /// <summary>
        /// Retrospective voting starts, change state to 'voting'
        /// </summary>
        /// <param name="eventGuid"></param>
        [Authorize]
        public void startRetrospectiveVoting()
        {
            var name = Context.User.Identity.Name;
            var connectionId = GetClientId();
            var eventGuid = _store.GetUsersEventId(connectionId);

            // Cant find user connectionId
            if (eventGuid == null)
                return;

            // Check moderator authority 
            if (!_retrospectiveMan.ChangeToRetrospectiveToVoting(eventGuid, name))
                return;

            List<string> recievers = new List<string>();
            _store.GetAllConnectionIds(eventGuid, recievers);
            Clients.Clients(recievers).startVotingMode();
        }

        /// <summary>
        /// Retrospective is finished change state to 'finished'
        /// </summary>
        /// <param name="eventGuid"></param>
        [Authorize]
        public void retrospectiveComplete()
        {
            var name = Context.User.Identity.Name;
            var connectionId = GetClientId();
            var eventGuid = _store.GetUsersEventId(connectionId);

            // Cant find user connectionId
            if (eventGuid == null)
                return;

            // Check moderator authority 
            if (!_retrospectiveMan.ChangeToRetrospectiveToFinished(eventGuid, name))
                return;

            List<string> recievers = new List<string>();
            _store.GetAllConnectionIds(eventGuid, recievers);
            Clients.Clients(recievers).retrospectiveFinished();

            // delete event in store
            _store.DeleteRet(eventGuid);

            string item = JsonConvert.SerializeObject(new List<string>());
            Clients.Clients(recievers).recieveOnlineUsers(item);
        }

        /// <summary>
        /// user is Ready for next phase
        /// </summary>
        public void iamready()
        {
            var connectionId = GetClientId();
            var eventGuid = _store.GetUsersEventId(connectionId);

            // Cant find user connectionId
            if (eventGuid == null)
                return;

            if (_retrospectiveMan.GetRetrospectivePhase(eventGuid) == "running")
            {
                List<string> connectionIds = new List<string>();

                _store.UserIsReady(connectionId, eventGuid, connectionIds);

                if (connectionIds.Count > 0)
                {
                    foreach(var user in connectionIds)
                        Clients.Client(user).readyMode();

                    UsersChanged(eventGuid);
                }               
            }
            
        }

        /// <summary>
        /// On disconnect event
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            string eventGuid;
            if ((eventGuid = _store.DeleteUser(GetClientId())) != null)
                UsersChanged(eventGuid);

            return base.OnDisconnected(stopCalled);
        }
                        
        /// <summary>
        /// Add moderator to user store
        /// </summary>
        /// <param name="guidId"></param>
        [Authorize]
        public void logPm(string eventGuid)
        {
            var connectionId = GetClientId();
            var name = Context.User.Identity.Name;
            var loginValidation = _retrospectiveMan.ValidateOwner(eventGuid, name);

            if (loginValidation < 0)
                // TODO: return error message
                return;
            
            if (loginValidation > 0)
            {
                _store.AddPm(connectionId: connectionId, retrospectiveGuidId: eventGuid);
                UsersChanged(eventGuid);
            }                      

            // Retrospective doesn't exist or isn't in 'presenting', 'voting' or 'finished' phase
            var phase = _retrospectiveMan.GetRetrospectivePhase(eventGuid);
            switch (phase)
            {
                case "waiting":
                    return;
                case "running":
                    Clients.Caller.startRunningMode();
                    if (_store.AmIReady(connectionId, eventGuid))
                        Clients.Caller.readyMode();
                    return;
                case "presenting":
                    Clients.Caller.startPresentingMode();
                    break;
                case "voting":
                    Clients.Caller.startVotingMode();
                    break;
                case "finished":
                    Clients.Caller.retrospectiveFinished();
                    break;
                default:
                    return;
            }

            SendAllSharedItems(eventGuid, connectionId, phase);
        }  

        /// <summary>
        /// Recieve login user request from client
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eventGuid"></param>
        public void loginUser(string name, string eventGuid)
        {
            var connectionId = GetClientId();
            var loginValidation = _retrospectiveMan.ExistRetrospective(eventGuid);
            if (loginValidation < 0)
                // TODO: return error message
                return;

            var message = IsUserLoginInputValid(name);
            if (message != null)
            {
                Clients.Caller.invalidLoginInput(message);
                return;
            }

            if (loginValidation > 0)
            {
                _store.AddUser(name: name, connectionId: GetClientId(), retrospectiveGuidId: eventGuid);
                UsersChanged(eventGuid);
            }                
            Clients.Caller.userLogged(name);

            // Retrospective doesn't exist or isn't in 'presenting', 'voting' or 'finished' phase
            var phase = _retrospectiveMan.GetRetrospectivePhase(eventGuid);
            switch (phase)
            {
                case "waiting":
                    return;
                case "running":
                    Clients.Caller.startRunningMode();
                    return;
                case "presenting":
                    Clients.Caller.startPresentingMode();
                    break;
                case "voting":
                    Clients.Caller.startVotingMode();
                    break;
                case "finished":
                    Clients.Caller.retrospectiveFinished();
                    break;
                default:
                    return;
            }

            SendAllSharedItems(eventGuid, connectionId, phase);
        }

        /// <summary>
        /// Send All shared Items to caller
        /// </summary>
        /// <param name="eventGuid"></param>
        /// <param name="connectionId"></param>
        /// <param name="phase"></param>
        private void SendAllSharedItems(string eventGuid, string connectionId, string phase)
        {
            var list = _retrospectiveMan.GetAllSharedItems(eventGuid);
            var allItems = new AllSaredItemsModel() { Items = list, RemainingVotes = 0, Phase = phase };
            var maxVotes = _retrospectiveMan.GetMaxVotes(eventGuid);

            if (maxVotes > 0)
            {
                allItems.RemainingVotes = maxVotes;
                if (phase == EventState.voting.ToString())
                    _store.AddUserVotes(eventGuid, connectionId, allItems);
            }

            string item = JsonConvert.SerializeObject(allItems);

            Clients.Caller.recieveAllSharedItemsMng(item);
        }

        /// <summary>
        /// Get user connectID
        /// </summary>
        /// <returns></returns>
        private string GetClientId()
        {
            string clientId = "";
            if (Context.QueryString["clientId"] != null)
                clientId = this.Context.QueryString["clientId"];

            if (string.IsNullOrEmpty(clientId.Trim()))
                clientId = Context.ConnectionId;

            return clientId;
        }

        /// <summary>
        /// Connected users are changed, send it to all remaining users
        /// </summary>
        /// <param name="guidId"></param>
        private void UsersChanged(string guidId)
        {
            List<OnlineUser> list = new List<OnlineUser>();
            _store.GetAllUsers(guidId, list);
            string item = JsonConvert.SerializeObject(list);

            List<string> recievers = new List<string>();
            _store.GetAllConnectionIds(guidId, recievers);

            Clients.Clients(recievers).recieveOnlineUsers(item);
        }

        /// <summary>
        /// Validate User login input
        /// </summary>
        /// <param name="name">User name</param>
        /// <param name="validation">Validation object</param>
        /// <returns></returns>
        private string IsUserLoginInputValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Your name cannot be empty.";

            if (!name.All(char.IsLetterOrDigit))
                return "Only alphanumeric characters may be used";

            return null;
        }


    }
}