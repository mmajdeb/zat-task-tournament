using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TournamentManagerTask.Infrastructure.Entities;

namespace TournamentManagerTask.Infrastructure.Configurations;

public class TournamentConfig : IEntityTypeConfiguration<TournamentEntity>
{
    public void Configure(EntityTypeBuilder<TournamentEntity> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Teams)
               .HasConversion(
                   v => string.Join(',', v),
                   v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        builder.HasMany(t => t.Matches)
               .WithOne(m => m.Tournament)
               .HasForeignKey(m => m.TournamentId);
    }
}
