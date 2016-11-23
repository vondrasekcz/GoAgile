namespace GoAgile.Models.Retrospective
{
    public class RetrospectiveInitModel
    {
        public string GuidId { get; set; }

        public string Url { get; set; }

        public string State { get; set; }

        public string Owner { get; set; }

        public bool EnableVoting { get; set; }

        public int Votes { get; set; }
    }
}