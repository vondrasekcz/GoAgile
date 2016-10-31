using System;
using System.Collections.Generic;
using GoAgile.Models.DB;

namespace GoAgile.Helpers.Objects
{
    public class Event
    {
        public string IdGuid { get; set; }

        public string State { get; set; }

        public List<User> Users { get; set; }
    }
}