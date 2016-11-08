using System.Collections.Generic;

namespace GoAgile.Helpers.Objects
{
    public class FullRetrospectiveModel
    {
        public RetrospectiveModel Model { get; set; }

        public IList<ItemObject> Items { get; set; }
    }
}