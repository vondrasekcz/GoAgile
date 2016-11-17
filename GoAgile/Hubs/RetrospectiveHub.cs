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
        private static UserStorageVer2 _store = new UserStorageVer2();

        private static readonly object Locker = new object();

        private IRetrospectiveManager _retrospectiveMan;

        /// <summary>
        /// Constructor
        /// </summary>
        public RetrospectiveHub()
        {
            _retrospectiveMan = new RetrospectiveManager();
        }


        /// <summary>
        /// Recieve login user request from client
        /// </summary>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="eventGuid"></param>
        public void loginUser(string name, string email, string eventGuid)
        {
            if (!IsUserLoginInputValid(email: email, name: name, eventGuid: eventGuid))
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

            var itemModel = new RetrospectiveItemModel { Column = column, Text = text, Autor = user, ListId = listId };
            itemModel.ItemGuid = _retrospectiveMan.AddRetrospectiveItem(itemModel, retrospectiveGuidId: eventGuid);

            var recievers = _store.GetAllConnectionIds(eventGuid);

            // TODO: only to specific group by eventGuid
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
        /// Retrospective is finished change state to 'finished'
        /// </summary>
        /// <param name="eventGuid"></param>
        [Authorize]
        public void retrospectiveComplete(string eventGuid)
        {
            _retrospectiveMan.ChangeToRetrospectiveToFinished(eventGuid);
            var recievers = _store.GetAllConnectionIds(eventGuid);

            Clients.Clients(recievers).retrospectiveFinished();
        }











       



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
            lock (_store) { ret = _store.Delete(GetClientId()); }
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
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="eventGuid"></param>
        /// <returns>InvalidLoginUserMessage with ivalid inputs</returns>
        private bool IsUserLoginInputValid(string email, string name, string eventGuid)
        {
            // TODO: return error and validation messages
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
                return false;

            return true;
        }


    }
}