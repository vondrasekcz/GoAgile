using System.Data.Entity;
using GoAgile.Models.DB;

namespace GoAgile.DataContexts
{
    public class AgileDb : DbContext
    {        
        public AgileDb()
            : base("DefaultConnection")
        {
        }

        public static AgileDb Create()
        {
            return new AgileDb();
        }

        public DbSet<Retrospective> Retrospectives { get; set; }
    }
}