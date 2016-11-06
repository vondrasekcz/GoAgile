using System;

namespace GoAgile.Helpers.Objects
{
    public class RetrospectiveModel
    {
        public string RetrospectiveName { get; set; }

        public string Project { get; set; }

        public string Owner { get; set; }

        public DateTime StartDate { get; set; }

        public string  State { get; set; }

        public string Comment { get; set; }
    }
}