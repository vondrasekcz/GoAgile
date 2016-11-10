using System;
using System.Collections.Generic;
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
            var itemModel = new RetrospectiveItemModel { Column = column, Text = text, Autor = user, ListId = listId };
            itemModel.ItemGuid = _retrospectiveMan.AddRetrospectiveItem(itemModel, retrospectiveGuidId: eventGuid);
            
            // TODO: only to specific group by eventGuid
            Clients.All.recieveSharedItem(itemModel);
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

            // TODO: only to specific group by eventGuid
            Clients.All.startRunningMode();
        }

        /// <summary>
        /// Retrospective presenting starts, change state to 'presenting'
        /// </summary>
        /// <param name="eventGuid"></param>
        [Authorize]
        public void startRetrospectivePresenting(string eventGuid)
        {
            _retrospectiveMan.ChangeRetrospectiveToPresenting(eventGuid);

            // TODO: only to specific group by eventGuid
            Clients.All.startPresentingMode();
        }

        /// <summary>
        /// Retrospective is finished change state to 'finished'
        /// </summary>
        /// <param name="eventGuid"></param>
        [Authorize]
        public void retrospectiveComplete(string eventGuid)
        {
            _retrospectiveMan.ChangeToRetrospectiveToFinished(eventGuid);

            // TODO: only to specific group by eventGuid
            Clients.All.retrospectiveFinished();
        }











        //*****************Counter Things***************************************************************************


        public static List<string> Users = new List<string>();


        public void Send(int count)
        {
            // Call the addNewMessageToPage method to update clients.
            var context = GlobalHost.ConnectionManager.GetHubContext<RetrospectiveHub>();
            context.Clients.All.updateUsersOnlineCount(count);
        }


        public override System.Threading.Tasks.Task OnConnected()
        {
            string clientId = GetClientId();

            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);
            }

            // Send the current count of users
            Send(Users.Count);

            return base.OnConnected();
        }


        public override System.Threading.Tasks.Task OnReconnected()
        {
            string clientId = GetClientId();
            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);
            }

            // Send the current count of users
            Send(Users.Count);

            return base.OnReconnected();
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

            string clientId = GetClientId();

            if (Users.IndexOf(clientId) > -1)
            {
                Users.Remove(clientId);
            }

            // Send the current count of users
            Send(Users.Count);

            return base.OnDisconnected(aaa);
        }


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














        //********************************************************************

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

            //var validationObj = new InvalidLoginUserMessage();

            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
                return false;

            /*if (_eventStorage.ExistUser(eventGuid: eventGuid, userName: name))
                return false;*/

            return true;
        }


    }
}