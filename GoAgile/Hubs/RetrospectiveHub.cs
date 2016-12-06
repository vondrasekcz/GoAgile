using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using GoAgile.Helpers.Logic;
using GoAgile.Models.DB;
using GoAgile.Models.Retrospective;
using GoAgile.Helpers.StoreModels;
using GoAgile.Dal;
using Newtonsoft.Json;

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
        public void sendSharedItem(string column, string text, string user)
        {
            // TODO: error, send it to caller
            if (string.IsNullOrWhiteSpace(column)
                || string.IsNullOrWhiteSpace(text) 
                || string.IsNullOrWhiteSpace(user)
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

            var itemModel = new RetrospectiveItemModel { Column = column, Text = text, Autor = user };
             _retrospectiveMan.AddRetrospectiveItem(itemModel, retrospectiveGuidId: eventGuid);
            var item = JsonConvert.SerializeObject(itemModel);

            var recievers = _store.GetAllConnectionIds(eventGuid);
            Clients.Clients(recievers).recieveSharedItem(item);
        }

        /// <summary>
        /// Send all shared items
        /// </summary>
        /// <param name="eventGuid"></param>
        public void sendAllSharedItems()
        {
            var connectionId = GetClientId();
            var eventGuid = _store.GetUsersEventId(connectionId);

            // Retrospective doesn't exist or isn't in 'presenting', 'voting' or 'finished' phase
            var phase = _retrospectiveMan.GetRetrospectivePhase(eventGuid);
            if (phase == EventState.waiting.ToString() || phase == EventState.running.ToString())
                return;

            // pridat pro ktere uz nemuze hlasovat

            var list = _retrospectiveMan.GetAllSharedItems(eventGuid);
            string ret = JsonConvert.SerializeObject(list);

            Clients.Caller.recieveAllSharedItems(ret);
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
            List<UsersVotes> users;
            users = _store.GetUsersAndVotes(eventGuid, sharedItemGuid);

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

            var recievers = _store.GetAllConnectionIds(eventGuid);
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

            var recievers = _store.GetAllConnectionIds(eventGuid);
            Clients.Clients(recievers).startPresentingMode();
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

            var recievers = _store.GetAllConnectionIds(eventGuid);
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

            var recievers = _store.GetAllConnectionIds(eventGuid);
            Clients.Clients(recievers).retrospectiveFinished();

            // delete event in store
            _store.DeleteRet(eventGuid);
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
            var list = _store.GetAllUsers(guidId);
            string item = JsonConvert.SerializeObject(list);
            var recievers = _store.GetAllConnectionIds(guidId);

            Clients.Clients(recievers).recieveOnlineUsers(item);
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
            if (!_retrospectiveMan.ValidateOwner(eventGuid, name))
                // TODO: return error message
                return;
                       
            _store.AddPm(connectionId: connectionId, retrospectiveGuidId: eventGuid);
            UsersChanged(eventGuid);

            // Retrospective doesn't exist or isn't in 'presenting', 'voting' or 'finished' phase
            var phase = _retrospectiveMan.GetRetrospectivePhase(eventGuid);
            if (phase == EventState.waiting.ToString() || phase == EventState.running.ToString())
                return;        

            var list = _retrospectiveMan.GetAllSharedItems(eventGuid);
            var allItems = new AllSaredItemsModel() { items = list, remainingVotes = 0 };
            var maxVotes = _retrospectiveMan.GetMaxVotes(eventGuid);

            if (maxVotes > 0)
            {
                allItems.remainingVotes = maxVotes;
                if (phase == EventState.voting.ToString())
                    _store.AddUserVotes(eventGuid, connectionId, allItems);
            }

            string item = JsonConvert.SerializeObject(allItems);

            Clients.Caller.recieveAllSharedItems(item);
        }








        public void logUser(string name, string guidId)
        {
            if (!_retrospectiveMan.ExistRetrospective(guidId))
                // TODO: add error message
                return;

            _store.AddUser(name: name,connectionId: GetClientId(), retrospectiveGuidId: guidId);
            UsersChanged(guidId);
        }

        /// <summary>
        /// Recieve login user request from client
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eventGuid"></param>
        public void loginUser(string name, string eventGuid)
        {
            if (!IsUserLoginInputValid(name: name))
                // TODO: Return object with valdiation messages
                Clients.Caller.invalidLoginInput();
            else
                Clients.Caller.userLogged(name);
        }




        /// <summary>
        /// Valdiate User Login Input
        /// </summary>
        /// <param name="name"></param>
        /// <returns>InvalidLoginUserMessage with ivalid inputs</returns>
        private bool IsUserLoginInputValid(string name)
        {
            // TODO: return error and validation messages
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return true;
        }


    }
}