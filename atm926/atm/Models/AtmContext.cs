using System;
using Microsoft.EntityFrameworkCore;

//Server's memory unused

namespace atm.Models
{
    public class AtmContext : DbContext
    {
        public AtmContext(DbContextOptions<AtmContext> options)
            : base(options)
        {
        }

        public DbSet<AtmItem> AtmItems { get; set; }

    }
}
