using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContxt : DbContext
    {
        public DataContxt(DbContextOptions<DataContxt> options) : base(options) 
        {
            
        }
        public DbSet<Value> Values {get; set;}
    }
}