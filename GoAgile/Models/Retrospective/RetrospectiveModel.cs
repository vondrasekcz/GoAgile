using System;

namespace GoAgile.Models.Retrospective
{
    public class RetrospectiveModel
    {
        public string RetrospectiveName { get; set; }

        public string Project { get; set; }

        public string Owner { get; set; }

        public string DatePlanned { get; set; }

        public string DateStarted { get; set; }

        public string DateFinished { get; set; }

        public string  State { get; set; }

        public string Comment { get; set; }
    }
}