using System;

namespace GoAgile.Models.Retrospective
{
    public class RetrospectiveModel
    {
        public string RetrospectiveName { get; set; }

        public string Project { get; set; }

        public string Owner { get; set; }

        public DateTime? DatePlanned { get; set; }

        public DateTime? DateStarted { get; set; }

        public DateTime? DateFinished { get; set; }

        public string  State { get; set; }

        public string Comment { get; set; }
    }
}