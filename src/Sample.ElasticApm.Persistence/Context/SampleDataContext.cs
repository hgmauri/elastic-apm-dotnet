using Microsoft.EntityFrameworkCore;
using Sample.ElasticApm.Persistence.Entity;

namespace Sample.ElasticApm.Persistence.Context
{
    public class SampleDataContext : DbContext
    {
        public SampleDataContext(DbContextOptions<SampleDataContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Pessoa> Pessoas { get; set; }
    }
}
