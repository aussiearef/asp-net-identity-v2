using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityNetCore.Data;

public class ApplicationDbContext: IdentityDbContext
{

    public ApplicationDbContext()
    {
        
    }
    
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
}