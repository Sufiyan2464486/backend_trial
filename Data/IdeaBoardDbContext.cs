using Microsoft.EntityFrameworkCore;

namespace backend_trial.Data
{
    public class IdeaBoardDbContext : DbContext
    {
        public IdeaBoardDbContext(DbContextOptions<IdeaBoardDbContext> options) : base(options)
        {
        }

        
    }
}
