using System.Collections.Generic;

namespace GoAgile.Models.Retrospective
{
    public class AllSaredItemsModel
    {
        public List<RetrospectiveItemModel> items { get; set; }

        public int remainingVotes;
    }
}