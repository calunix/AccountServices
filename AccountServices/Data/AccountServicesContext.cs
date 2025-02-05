using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AccountServices.Models;

namespace AccountServices.Data
{
    public class AccountServicesContext : DbContext
    {
        public AccountServicesContext (DbContextOptions<AccountServicesContext> options)
            : base(options)
        {
        }

        public DbSet<AccountServices.Models.PasswordReset> PasswordReset { get; set; } = default!;
    }
}
