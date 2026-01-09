using Microsoft.EntityFrameworkCore;
using SurveyBasketAPI.Models;

namespace SurveyBasketAPI.Persistence;

public class SurveyBasketDbContext : DbContext
{
    public DbSet<Poll> Polls { get; set; }
    public SurveyBasketDbContext(DbContextOptions<SurveyBasketDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SurveyBasketDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
