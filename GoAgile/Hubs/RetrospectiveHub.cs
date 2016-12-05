using System;
using System.Net.Mail;
using Microsoft.AspNet.SignalR;
using GoAgile.Helpers.Logic;
using GoAgile.Models.Retrospective;
using GoAgile.Dal;
using Newtonsoft.Json;

namespace GoAgile.Hubs
{
    public class RetrospectiveHub : Hub
    {
        private static StoreRet _store;

        private static readonly object Locker = new object();

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
        /// Recieve login user request from client
        /// </summary>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="eventGuid"></param>
        public void loginUser(string name, string email, string eventGuid)
        {
            if (!IsUserLoginInputValid(name: name))
                // TODO: Return object with valdiation messages
                Clients.Caller.invalidLoginInput();
            else
            {                
                Clients.Caller.userLogged(name);
            }
        }

        /// <summary>
        /// Save and send shared Retrospective item
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="column"></param>
        /// <param name="text"></param>
        /// <param name="user"></param>
        public void sendSharedItem(string listId, string column, string text, string user, string eventGuid)
        {
            // TODO: throw exception, handle it at client
            if (string.IsNullOrWhiteSpace(listId) || string.IsNullOrWhiteSpace(column) || string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(eventGuid))
                return;

            var itemModel = new RetrospectiveItemModel { Column = column, Text = text, Autor = user, ListId = listId, Votes = 0 };
            itemModel.ItemGuid = _retrospectiveMan.AddRetrospectiveItem(itemModel, retrospectiveGuidId: eventGuid);

            var recievers = _store.GetAllConnectionIds(eventGuid);

            Clients.Clients(recievers).recieveSharedItem(itemModel);
        }

        /// <summary>
        /// Send all shared items
        /// </summary>
        /// <param name="eventGuid"></param>
        public void sendAllSharedItems(string eventGuid)
        {
            var list = _retrospectiveMan.GetAllSharedItems(eventGuid);
            string ret = JsonConvert.SerializeObject(list);

            Clients.Caller.recieveAllSharedItems(ret);
        }




        public void sharedItemVoted(string column, string sharedItemGuid, string eventGuid)
        {
            var maxVotes = _retrospectiveMan.GetMaxVotes(eventGuid);
            if (maxVotes < 1)
                return;

            if (!_store.AddVote(eventGuid, GetClientId(), sharedItemGuid))
                return;

            int totalVotes;
            if ((totalVotes = _retrospectiveMan.AddVotesToItem(sharedItemGuid)) < 0)
                return;
                
            // musim si nechat vratit uyivatelovo conID a zbivajici pocet hlasu
            // potom vratim ten objekt jak mam a k tomu pridam kolik hlasu uzivatel jeste ma

            var model = new VotingModel() { Column = column, SharedItemGuid = sharedItemGuid, VotesTotal = totalVotes };
            var ret = JsonConvert.SerializeObject(model);
            var recievers = _store.GetAllConnectionIds(eventGuid);

            Clients.Clients(recievers).addVotesToSharedItem(ret);
        }

        /// <summary>
        /// Retrospective starts, change state to 'running'
        /// </summary>
        /// <param name="eventGuid"></param>
        [Authorize]
        public void startRetrospectiveRunning(string eventGuid)
        {
            _retrospectiveMan.ChangeRetrospectiveToRunning(eventGuid);
            var recievers = _store.GetAllConnectionIds(eventGuid);

            Clients.Clients(recievers).startRunningMode();
        }

        /// <summary>
        /// Retrospective presenting starts, change state to 'presenting'
        /// </summary>
        /// <param name="eventGuid"></param>
        [Authorize]
        public void startRetrospectivePresenting(string eventGuid)
        {
            _retrospectiveMan.ChangeRetrospectiveToPresenting(eventGuid);
            var recievers = _store.GetAllConnectionIds(eventGuid);

            Clients.Clients(recievers).startPresentingMode();
        }

        /// <summary>
        /// Retrospective voting starts, change state to 'voting'
        /// </summary>
        /// <param name="eventGuid"></param>
        [Authorize]
        public void startRetrospectiveVoting(string eventGuid)
        {
            _retrospectiveMan.ChangeToRetrospectiveToVoting(eventGuid);
            var recievers = _store.GetAllConnectionIds(eventGuid);

            Clients.Clients(recievers).startVotingMode();
        }

        /// <summary>
        /// Retrospective is finished change state to 'finished'
        /// </summary>
        /// <param name="eventGuid"></param>
        [Authorize]
        public void retrospectiveComplete(string eventGuid)
        {
            // TODO delete retrospective in _store

            _retrospectiveMan.ChangeToRetrospectiveToFinished(eventGuid);
            var recievers = _store.GetAllConnectionIds(eventGuid);

            Clients.Clients(recievers).retrospectiveFinished();

            _store.DeleteRet(eventGuid);
        }














        [Authorize]
        public void logPm(string guidId)
        {
            

            lock (_store) { _store.AddPm(connectionId: GetClientId(), retrospectiveGuidId: guidId); }
            UsersChanged(guidId);
        }

        public void logUser(string name, string guidId)
        {
            lock (_store) { _store.AddUser(name: name,connectionId: GetClientId(), retrospectiveGuidId: guidId); }
            UsersChanged(guidId);
        }

        public void UsersChanged(string guidId)
        {
            var list = _store.GetAllUsers(guidId);
            string ret = JsonConvert.SerializeObject(list);
            var recievers = _store.GetAllConnectionIds(guidId);

            Clients.Clients(recievers).recieveOnlineUsers(ret);
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool aaa)
        {
            string ret;
            lock (_store) { ret = _store.DeleteUser(GetClientId()); }
            if (ret != null)
                UsersChanged(ret);

            return base.OnDisconnected(aaa);
        }

        // Get user connectID
        private string GetClientId()
        {
            string clientId = "";
            if (Context.QueryString["clientId"] != null)
            {
                // clientId passed from application 
                clientId = this.Context.QueryString["clientId"];
            }

            if (string.IsNullOrEmpty(clientId.Trim()))
            {
                clientId = Context.ConnectionId;
            }

            return clientId;
        }













        /// <summary>
        /// Validate email
        /// </summary>
        /// <param name="emailaddress">Email</param>
        /// <returns>True is valid, otherwise false</returns>
        private bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
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