namespace GoAgile.Models.Retrospective
{
    public class VotingModel
    {
        public string Column { get; set; }

        public string SharedItemGuid { get; set; }

        public int VotesTotal { get; set; }

        public int RemainnigVotes { get; set; }

        public bool EnableThisItemVoting { get; set; }
    }
}