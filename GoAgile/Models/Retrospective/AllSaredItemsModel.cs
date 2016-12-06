using System.Collections.Generic;

namespace GoAgile.Models.Retrospective
{
    public class AllSaredItemsModel
    {
        public List<RetrospectiveItemModel> Items { get; set; }

        public int RemainingVotes { get; set; }

        public string Phase { get; set; }
    }
}