using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasketAPI.Models;
using System.Reflection.Emit;

namespace SurveyBasketAPI.Persistence.EntitiesConfigurations;

public class PollsConfigurations : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.ToTable("Polls");
        builder.HasKey(p => p.Id);

        builder.HasIndex(x=>x.Title).IsUnique();
        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(500);


        builder.Property(p => p.Summary)
            .IsRequired()
            .HasMaxLength(2000);

     builder.Property(x=>x.CreatedOn).HasDefaultValueSql("GETDATE()");


    }
}
