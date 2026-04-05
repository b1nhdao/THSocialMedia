using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using THSocialMedia.Domain.Abstractions;
using THSocialMedia.Domain.Entities;
using THSocialMedia.Infrastructure.EfDbContext.EntityConfigurations;

namespace THSocialMedia.Infrastructure.EfDbContext
{
    public class WriteDbContext : DbContext, IUnitOfWork
    {

        public WriteDbContext(DbContextOptions<WriteDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        private readonly IMediator _mediator;

        public DbSet<ChatMember> ChatMembers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<ReactionPost> ReactionPosts { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<User> Users { get; set; }

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
            modelBuilder.ApplyConfiguration(new ReactionConfiguration());
            modelBuilder.ApplyConfiguration(new RelationshipConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ReactionPostConfiguration());

        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            _ = await base.SaveChangesAsync(cancellationToken);

            return true;

        }
    }
}
