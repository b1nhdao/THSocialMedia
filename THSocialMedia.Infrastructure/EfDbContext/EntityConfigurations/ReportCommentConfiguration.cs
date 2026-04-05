using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using THSocialMedia.Domain.Entities;

namespace THSocialMedia.Infrastructure.EfDbContext.EntityConfigurations
{
    public class ReportCommentConfiguration : IEntityTypeConfiguration<ReportComment>
    {
        public void Configure(EntityTypeBuilder<ReportComment> builder)
        {
            builder.HasKey(rc => new { rc.ReportsId, rc.CommentsId });
        }
    }
}
