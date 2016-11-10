using System;

namespace GoAgile.Models.Retrospective
{
    public class EventModel
    {
        public string IdGuid { get; set; }

        public string RetrospectiveName { get; set; }

        public string Project { get; set; }

        public string Comment { get; set; }

        public DateTime DateStart { get; set; }

        public string State { get; set; }
    }
}