using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using GoAgile.Models;
using GoAgile.Helpers;
using GoAgile.Models.EntityManager;

namespace GoAgile.Hubs
{
    public class RetrospectiveHub : Hub
    {


        //------------------------------------------------------------------------------
        // TODO: some locks, own file
        public class EventSpectiveStorage
        {
            private Dictionary<string, Event> _eventStore;

            public bool ExistUser(string eventGuid, string userName)
            {
                // TODO:
                return false;
            }

            public void AddUser(string eventGuid, string userName, string email, string clientId)
            {
                // TODO:
            }

            public void RemoveUser(string clientId)
            {
                // TODO:
            }

            public List<User> EventUsers(string eventGuid)
            {
                // TODO:
                return null;
            }

            public List<User> GetOnlineUsersExceptMe(string clientId)
            {
                // TODO:
                return null;
            }
        }
        //------------------------------------------------------------------------------

        // TODO: own file
        private EventSpectiveStorage _eventStorage;

        /// <summary>
        /// Constructor
        /// </summary>
        public RetrospectiveHub()
        {
            // TODO: inicialize at better place
            var _eventStorage = new EventSpectiveStorage();
        }


        /// <summary>
        /// Recieve login user request from client
        /// </summary>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <param name="eventGuid"></param>
        public void loginUser(string name, string email, string eventGuid)
        {
            if (!IsUserLoginInputValid(email: "aaa@bbb.cz", name: name, eventGuid: eventGuid))
                // TODO: Return object with valdiation messages
                Clients.Caller.invalidLoginInput();
            else
            {
                //_eventStorage.AddUser(eventGuid: eventGuid, userName: name, email: email, clientId: GetClientId());
                Clients.Caller.userLogged();
            }
        }

        [Authorize]
        public void startRetrospective(string eventGuid)
        {
            //var users = _eventStorage.EventUsers(eventGuid);
            // TODO: only event users
            Clients.All.startRequest();
        }





















        //********************************************************************************************

        /// <summary>
        /// The count of users connected.
        /// </summary>
        public static List<string> Users = new List<string>();

        /// <summary>
        /// Sends the update user count to the listening view.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        public void Send(int count)
        {
            // Call the addNewMessageToPage method to update clients.
            var context = GlobalHost.ConnectionManager.GetHubContext<RetrospectiveHub>();
            context.Clients.All.updateUsersOnlineCount(count);
        }

        /// <summary>
        /// The OnConnected event.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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

        /// <summary>
        /// The OnReconnected event.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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

        /// <summary>
        /// The OnDisconnected event.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override System.Threading.Tasks.Task OnDisconnected(bool aaa)
        {
            string clientId = GetClientId();

            if (Users.IndexOf(clientId) > -1)
            {
                Users.Remove(clientId);
            }

            // Send the current count of users
            Send(Users.Count);

            return base.OnDisconnected(aaa);
        }

        /// <summary>
        /// Get's the currently connected Id of the client.
        /// This is unique for each client and is used to identify
        /// a connection.
        /// </summary>
        /// <returns>The client Id.</returns>
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