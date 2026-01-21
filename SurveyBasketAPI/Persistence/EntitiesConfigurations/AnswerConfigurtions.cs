using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasketAPI.Entities;

namespace SurveyBasketAPI.Persistence.EntitiesConfigurations;

public class AnswerConfigurtions: IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.Property(a => a.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasIndex(x => new { x.Content, x.QuestionId }).IsUnique();
            

        builder.HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId);
    }
}
