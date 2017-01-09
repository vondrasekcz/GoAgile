namespace GoAgile.Services.StoreModels
{
    public class UsersVotes
    {
        public string ConnectionId { get; set; }

        public int Voted { get; set; }

        public bool EnableVotingForItem { get; set; }
    }
}