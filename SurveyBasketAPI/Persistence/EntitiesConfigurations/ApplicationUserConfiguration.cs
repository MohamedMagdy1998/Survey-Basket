using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasketAPI.Entities;

namespace SurveyBasketAPI.Persistence.EntitiesConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsMany(x => x.RefreshTokens, rt =>
        {
            rt.ToTable("RefreshTokens");
            rt.WithOwner().HasForeignKey("UserId");
            rt.Property(x => x.Token).IsRequired();
            rt.Property(x => x.ExpiresOn).IsRequired();
            rt.Property(x => x.CreatedOn).IsRequired();
            rt.Property(x => x.RevokedOn);
        });
    }

   
}
