using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class GoalContributionConfiguration : IEntityTypeConfiguration<GoalContribution>
    {
        public void Configure(EntityTypeBuilder<GoalContribution> builder)
        {
            builder.Property(g => g.Amount)
                .HasPrecision(18, 2);
        }
    }
}
