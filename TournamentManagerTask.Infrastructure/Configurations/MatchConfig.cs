using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentManagerTask.Infrastructure.Entities;

namespace TournamentManagerTask.Infrastructure.Configurations;

public class MatchConfig : IEntityTypeConfiguration<MatchEntity>
{
    public void Configure(EntityTypeBuilder<MatchEntity> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.State).IsRequired();
        builder.HasOne(m => m.TeamA).WithMany()
               .HasForeignKey(m => m.TeamAId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.TeamB).WithMany()
               .HasForeignKey(m => m.TeamBId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.Winner).WithMany()
               .HasForeignKey(m => m.WinnerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
