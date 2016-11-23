namespace GoAgile.Models.Retrospective
{
    public class RetrospectiveItemModel
    {
        public string ItemGuid { get; set; }

        public string Column { get; set; }

        public string ListId { get; set; }

        public string Text { get; set; }

        public string Autor { get; set; }   
        
        public int Votes { get; set; }     
    }
}