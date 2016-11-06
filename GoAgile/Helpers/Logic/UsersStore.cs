using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoAgile.Helpers.Objects;

namespace GoAgile.Helpers.Logic
{
    // TODO: this is only test version
    public class UsersStore
    {
        public static List<User> Users = new List<User>();

        public void AddUser(string name, string connection)
        {
            Users.Add(new User() { Name = name, ConnectionId = connection });
        }

        public bool Delete(string connection)
        {
            User item;

            if ((item = Users.SingleOrDefault(a => a.ConnectionId == connection)) != null)
            {
                Users.Remove(item);
                return true;
            }

            return false;
        }

        public List<string> GetAllUsers()
        {
            return Users.Select(s => s.Name).ToList();
        }
    }
}