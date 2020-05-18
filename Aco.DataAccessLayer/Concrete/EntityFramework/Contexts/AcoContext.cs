using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aco.DataAccessLayer.Concrete.EntityFramework.Contexts
{
    public class AcoContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /* Local Db */
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=AcoDb;Trusted_Connection=true");
        }
    }
}
