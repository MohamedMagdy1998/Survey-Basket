using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasketAPI.Entities;

namespace SurveyBasketAPI.Persistence.EntitiesConfigurations;

public class QuestionConfigurations : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.Property(q => q.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasIndex(q => new { q.Content, q.PollId }).IsUnique();

        builder.HasOne(q => q.Poll)
            .WithMany(p => p.Questions)
            .HasForeignKey(q => q.PollId);
    }
}
