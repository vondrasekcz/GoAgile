using System.Collections.Generic;

namespace GoAgile.Models.Retrospective
{
    public class LoginValidation
    {
        public bool Valid { get; set; }

        public List<string> Message { get; set; }
    }
}