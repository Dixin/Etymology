namespace Etymology.Data.Models
{
    using Microsoft.EntityFrameworkCore;

    public partial class EtymologyContext
    {
        public EtymologyContext(DbContextOptions<EtymologyContext> options)
            : base(options)
        {
        }
    }
}
