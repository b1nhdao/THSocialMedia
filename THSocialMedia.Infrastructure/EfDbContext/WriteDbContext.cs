using Microsoft.EntityFrameworkCore;
using THSocialMedia.Domain.Entities;
using THSocialMedia.Infrastructure.EfDbContext.EntityConfigurations;

namespace THSocialMedia.Infrastructure.EfDbContext
{
    public class WriteDbContext : DbContext
    {
        public WriteDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ChatMember> ChatMembers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostReport> PostReports { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<ReactionPost> ReactionPosts { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportComment> ReportComments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ReportUser> ReportUsers { get; set; }

        protected WriteDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ChatMembersConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new ConversationConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new PostReportConfiguration());
            modelBuilder.ApplyConfiguration(new ReactionConfiguration());
            modelBuilder.ApplyConfiguration(new RelationshipConfiguration());
            modelBuilder.ApplyConfiguration(new ReportConfiguration());
            modelBuilder.ApplyConfiguration(new ReportCommentConfiguration());
            modelBuilder.ApplyConfiguration(new ReportUserConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ReactionPostConfiguration());

        }
    }
}
