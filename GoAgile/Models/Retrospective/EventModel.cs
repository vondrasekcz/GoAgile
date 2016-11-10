using System;

namespace GoAgile.Models.Retrospective
{
    public class EventModel
    {
        public string IdGuid { get; set; }

        public string Name { get; set; }

        public string Project { get; set; }

        public string Comment { get; set; }

        public string DatePlanned { get; set; }

        public string DateStarted { get; set; }
            
        public string DateFinished { get; set; }

        public string State { get; set; }
    }
}