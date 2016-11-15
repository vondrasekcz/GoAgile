using System.Collections.Generic;

namespace GoAgile.Models.Retrospective
{
    public class FullRetrospectiveModel
    {
        public RetrospectiveModel ModelItem { get; set; }

        public IList<RetrospectiveItemModel> Items { get; set; }
    }
}