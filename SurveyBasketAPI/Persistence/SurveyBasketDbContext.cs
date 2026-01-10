using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveyBasketAPI.Entities;
using SurveyBasketAPI.Models;

namespace SurveyBasketAPI.Persistence;

public class SurveyBasketDbContext(DbContextOptions<SurveyBasketDbContext> options) :
    IdentityDbContext<ApplicationUser>(options)
{

    public DbSet<Poll> Polls { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SurveyBasketDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
