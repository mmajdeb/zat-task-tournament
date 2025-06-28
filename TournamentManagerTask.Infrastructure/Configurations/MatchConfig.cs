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
              builder.Property(m => m.TeamA);
              builder.Property(m => m.TeamB);
              builder.Property(m => m.Winner);
       }
}
