using Microsoft.Extensions.Logging;
using THSocialMedia.Application.Models.Analysis;
using THSocialMedia.Domain.Abstractions.IWriteRepositories;
using THSocialMedia.Domain.Entities;
using THSocialMedia.Domain.Enums;
using AnalysisComment = THSocialMedia.Application.Models.Analysis.Comment;
using AnalysisPost = THSocialMedia.Application.Models.Analysis.Post;
using AnalysisReaction = THSocialMedia.Application.Models.Analysis.Reaction;
using AnalysisReport = THSocialMedia.Application.Models.Analysis.Report;

namespace THSocialMedia.Application.Services
{
    public class ProposalService : IProposalService
    {
        private readonly IProposalRepository _proposalRepository;
        private readonly IProposalHistoryRepository _proposalHistoryRepository;
        private readonly ILogger<ProposalService> _logger;

        public ProposalService(
            IProposalRepository proposalRepository,
            IProposalHistoryRepository proposalHistoryRepository,
            ILogger<ProposalService> logger)
        {
            _proposalRepository = proposalRepository;
            _proposalHistoryRepository = proposalHistoryRepository;
            _logger = logger;
        }

        public async Task<Proposal> RunAnalysisAsync(DataAnalysisDto data)
        {
            try
            {
                _logger.LogInformation("Starting analysis with {PostCount} posts and {ReportCount} reports",
                    data?.Posts?.Count ?? 0, data?.Reports?.Count ?? 0);

                ValidateInputData(data);

                var preparedData = PrepareData(data);

                var contentQualityAnalysis = AnalyzeContentQuality(preparedData);
                var communityHealthAnalysis = AnalyzeCommunityHealth(preparedData);
                var reactionPatternAnalysis = AnalyzeReactionPatterns(preparedData);
                var moderationSafetyAnalysis = AnalyzeModerationSafety(preparedData);

                var insights = GenerateInsights(
                    contentQualityAnalysis,
                    communityHealthAnalysis,
                    reactionPatternAnalysis,
                    moderationSafetyAnalysis,
                    preparedData.Posts.Count);

                var proposal = BuildProposal(
                    preparedData,
                    contentQualityAnalysis,
                    communityHealthAnalysis,
                    reactionPatternAnalysis,
                    moderationSafetyAnalysis,
                    insights);

                var savedProposal = await PersistProposal(proposal);

                _logger.LogInformation("Analysis completed successfully. Proposal ID: {ProposalId}",
                    savedProposal.ProposalId);

                return savedProposal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during analysis execution");
                throw;
            }
        }

        private void ValidateInputData(DataAnalysisDto data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Posts == null || data.Posts.Count == 0)
            {
                throw new InvalidOperationException("Posts collection cannot be empty");
            }

            if (data.Reports == null)
            {
                throw new InvalidOperationException("Reports collection cannot be null");
            }

            var oldestPost = data.Posts.Min(p => p.CreatedAt);
            var newestPost = data.Posts.Max(p => p.CreatedAt);
            var dataAgeInDays = (newestPost - oldestPost).TotalDays;

            if (dataAgeInDays < 30)
            {
                throw new InvalidOperationException(
                    $"Insufficient data period. Requires minimum 30 days, got {dataAgeInDays:F0} days");
            }

            ValidateRelationshipIntegrity(data);

            _logger.LogInformation("Data validation passed. Data span: {Days} days", dataAgeInDays);
        }

        private void ValidateRelationshipIntegrity(DataAnalysisDto data)
        {
            var postIds = data.Posts.Select(p => p.PostId).ToHashSet();

            foreach (var post in data.Posts)
            {
                if (post.Comments != null)
                {
                    foreach (var comment in post.Comments)
                    {
                        if (!postIds.Contains(comment.PostId))
                        {
                            throw new InvalidOperationException(
                                $"Comment {comment.CommentId} references non-existent post {comment.PostId}");
                        }
                    }
                }

                if (post.Reactions != null)
                {
                    foreach (var reaction in post.Reactions)
                    {
                        if (!postIds.Contains(reaction.PostId))
                        {
                            throw new InvalidOperationException(
                                $"Reaction {reaction.ReactionId} references non-existent post {reaction.PostId}");
                        }
                    }
                }
            }

            var allCommentIds = data.Posts
                .Where(p => p.Comments != null)
                .SelectMany(p => p.Comments!.Select(c => c.CommentId))
                .ToHashSet();

            foreach (var report in data.Reports)
            {
                if (!postIds.Contains(report.TargetId) && !allCommentIds.Contains(report.TargetId))
                {
                    throw new InvalidOperationException(
                        $"Report {report.ReportId} references invalid target {report.TargetId}");
                }
            }
        }

        private PreparedAnalysisData PrepareData(DataAnalysisDto data)
        {
            _logger.LogInformation("Preparing data for analysis");

            return new PreparedAnalysisData
            {
                Posts = data.Posts,
                Reports = data.Reports,
                AnalysisStartDate = data.Posts.Min(p => p.CreatedAt),
                AnalysisEndDate = data.Posts.Max(p => p.CreatedAt),
                AllComments = data.Posts
                    .Where(p => p.Comments != null)
                    .SelectMany(p => p.Comments!)
                    .ToList(),
                AllReactions = data.Posts
                    .Where(p => p.Reactions != null)
                    .SelectMany(p => p.Reactions!)
                    .ToList()
            };
        }

        private ContentQualityAnalysis AnalyzeContentQuality(PreparedAnalysisData data)
        {
            _logger.LogInformation("Analyzing content quality");

            var analysis = new ContentQualityAnalysis();

            foreach (var post in data.Posts)
            {
                var commentCount = post.Comments?.Count ?? 0;
                var reactionCount = post.Reactions?.Count ?? 0;
                var totalEngagement = commentCount + reactionCount;

                var postMetrics = new PostMetrics
                {
                    PostId = post.PostId,
                    CommentCount = commentCount,
                    ReactionCount = reactionCount,
                    CreatedAt = post.CreatedAt,
                    EngagementRate = data.Posts.Count > 0
                        ? totalEngagement / (double)data.Posts.Count
                        : 0
                };

                analysis.PostMetrics.Add(postMetrics);
            }

            var averageEngagement = analysis.PostMetrics.Count > 0
                ? analysis.PostMetrics.Average(m => m.EngagementRate)
                : 0;

            analysis.AverageEngagementRate = averageEngagement;

            List<PostMetrics> highPerformingPosts;
            List<PostMetrics> lowPerformingPosts;

            if (averageEngagement <= 0)
            {
                highPerformingPosts = new List<PostMetrics>();
                lowPerformingPosts = analysis.PostMetrics.ToList();
            }
            else
            {
                highPerformingPosts = analysis.PostMetrics
                    .Where(m => m.EngagementRate >= averageEngagement * 1.5)
                    .ToList();

                lowPerformingPosts = analysis.PostMetrics
                    .Where(m => m.EngagementRate < averageEngagement * 0.5)
                    .ToList();
            }

            analysis.HighPerformingPostsCount = highPerformingPosts.Count;
            analysis.LowPerformingPostsCount = lowPerformingPosts.Count;

            analysis.LowQualityPosts = lowPerformingPosts
                .Where(p => string.IsNullOrWhiteSpace(
                    data.Posts.FirstOrDefault(post => post.PostId == p.PostId)?.Content))
                .Select(p => p.PostId)
                .ToList();

            _logger.LogDebug("Content Quality: {HighPerforming} high-performing, {LowPerforming} low-performing",
                analysis.HighPerformingPostsCount, analysis.LowPerformingPostsCount);

            return analysis;
        }

        private CommunityHealthAnalysis AnalyzeCommunityHealth(PreparedAnalysisData data)
        {
            _logger.LogInformation("Analyzing community health");

            var analysis = new CommunityHealthAnalysis
            {
                TotalComments = data.AllComments.Count,
                CommentsPerPost = data.Posts.Count > 0
                    ? data.AllComments.Count / (double)data.Posts.Count
                    : 0
            };

            var participantIds = new HashSet<string>();
            foreach (var post in data.Posts)
            {
                participantIds.Add(post.AuthorId);
                if (post.Comments != null)
                {
                    foreach (var comment in post.Comments)
                    {
                        participantIds.Add(comment.AuthorId);
                    }
                }
            }

            analysis.UniqueParticipants = participantIds.Count;
            analysis.ParticipationRate = data.Posts.Count > 0
                ? analysis.UniqueParticipants / (double)data.Posts.Count
                : 0;

            var postsWithDiscussion = data.Posts
                .Count(p => p.Comments != null && p.Comments.Count >= 3);

            analysis.DiscussionQualityScore = data.Posts.Count > 0
                ? (postsWithDiscussion / (double)data.Posts.Count) * 100
                : 0;

            analysis.CommunityHealthScore = Math.Min(100, analysis.DiscussionQualityScore);

            _logger.LogDebug("Community Health: {Participants} participants, {HealthScore:F2} health score",
                analysis.UniqueParticipants, analysis.CommunityHealthScore);

            return analysis;
        }

        private ReactionPatternAnalysis AnalyzeReactionPatterns(PreparedAnalysisData data)
        {
            _logger.LogInformation("Analyzing reaction patterns");

            var analysis = new ReactionPatternAnalysis
            {
                TotalReactions = data.AllReactions.Count,
                ReactionsPerPost = data.Posts.Count > 0
                    ? data.AllReactions.Count / (double)data.Posts.Count
                    : 0,
                ReactionDistribution = data.AllReactions
                    .GroupBy(r => r.Type)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count())
            };

            var negativeTypes = new[] { ReactionType.Sad, ReactionType.Angry };
            var volatilePostIds = data.Posts
                .Where(p => p.Reactions != null &&
                       p.Reactions.Count(r => negativeTypes.Contains(r.Type)) > p.Reactions.Count * 0.3)
                .Select(p => p.PostId)
                .ToList();

            analysis.VolatilePostsCount = volatilePostIds.Count;
            analysis.VolatilePostIds = volatilePostIds;

            var ratioSum = 0.0;
            var ratioCount = 0;
            foreach (var post in data.Posts)
            {
                var reactions = post.Reactions?.Count ?? 0;
                var comments = post.Comments?.Count ?? 0;
                if (comments > 0)
                {
                    ratioSum += reactions / (double)comments;
                    ratioCount++;
                }
            }

            analysis.ReactionToCommentRatio = ratioCount > 0 ? ratioSum / ratioCount : 0;

            _logger.LogDebug("Reaction Patterns: {Total} reactions, {Volatile} volatile posts",
                analysis.TotalReactions, analysis.VolatilePostsCount);

            return analysis;
        }

        private ModerationSafetyAnalysis AnalyzeModerationSafety(PreparedAnalysisData data)
        {
            _logger.LogInformation("Analyzing moderation and safety");

            var analysis = new ModerationSafetyAnalysis
            {
                TotalReports = data.Reports.Count,
                ReportRate = data.Posts.Count > 0
                    ? data.Reports.Count / (double)data.Posts.Count
                    : 0,
                ReportDistribution = data.Reports
                    .GroupBy(r => r.Type)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                ApprovedReports = data.Reports.Count(r => r.Status == ReportStatus.Approved),
                RejectedReports = data.Reports.Count(r => r.Status == ReportStatus.Rejected),
                PendingReports = data.Reports.Count(r => r.Status == ReportStatus.Pending)
            };

            analysis.FalsePositiveRate = analysis.TotalReports > 0
                ? analysis.RejectedReports / (double)analysis.TotalReports
                : 0;

            var reportedTargets = data.Reports
                .GroupBy(r => r.TargetId)
                .Where(g => g.Count() >= 2)
                .Select(g => new
                {
                    TargetId = g.Key,
                    ReportCount = g.Count(),
                    SeverityScore = g.Count(r => r.Type == ReportType.Hate || r.Type == ReportType.Violence) * 2
                                    + g.Count(r => r.Type == ReportType.Spam) * 1
                })
                .OrderByDescending(x => x.SeverityScore)
                .ToList();

            analysis.ProblematicTargetsCount = reportedTargets.Count;
            analysis.ProblematicTargetIds = reportedTargets.Select(t => t.TargetId).ToList();

            var safetyScore = 100 - (analysis.ReportRate * 50) - (analysis.FalsePositiveRate * 20);
            analysis.SafetyScore = Math.Max(0, Math.Min(100, safetyScore));

            _logger.LogDebug("Safety Analysis: {Reports} reports, {Safety:F2} safety score",
                analysis.TotalReports, analysis.SafetyScore);

            return analysis;
        }

        private List<Insight> GenerateInsights(
            ContentQualityAnalysis contentAnalysis,
            CommunityHealthAnalysis communityAnalysis,
            ReactionPatternAnalysis reactionAnalysis,
            ModerationSafetyAnalysis safetyAnalysis,
            int postCount)
        {
            _logger.LogInformation("Generating insights");

            var insights = new List<Insight>();

            if (contentAnalysis.PostMetrics.Count > 0 &&
                contentAnalysis.LowPerformingPostsCount > contentAnalysis.PostMetrics.Count * 0.3)
            {
                insights.Add(new Insight
                {
                    Category = "ContentQuality",
                    Title = "High Volume of Low-Engagement Content",
                    Description =
                        $"{contentAnalysis.LowPerformingPostsCount} posts ({(contentAnalysis.LowPerformingPostsCount * 100.0 / contentAnalysis.PostMetrics.Count):F1}%) show significantly lower engagement than average.",
                    Severity = "High",
                    ActionRequired = true
                });
            }

            if (communityAnalysis.CommunityHealthScore < 40)
            {
                insights.Add(new Insight
                {
                    Category = "CommunityHealth",
                    Title = "Community Engagement Declining",
                    Description =
                        $"Community health score is {communityAnalysis.CommunityHealthScore:F1}/100. Limited discussions and interactions detected.",
                    Severity = "High",
                    ActionRequired = true
                });
            }

            if (postCount > 0 && reactionAnalysis.VolatilePostsCount > postCount * 0.2)
            {
                insights.Add(new Insight
                {
                    Category = "ReactionPattern",
                    Title = "Emotionally Volatile Content Detected",
                    Description =
                        $"{reactionAnalysis.VolatilePostsCount} posts trigger predominantly negative reactions.",
                    Severity = "Medium",
                    ActionRequired = true
                });
            }

            if (safetyAnalysis.SafetyScore < 50)
            {
                insights.Add(new Insight
                {
                    Category = "ModerationSafety",
                    Title = "Safety Concerns Identified",
                    Description =
                        $"Safety score is {safetyAnalysis.SafetyScore:F1}/100 with {safetyAnalysis.ProblematicTargetsCount} problematic items.",
                    Severity = "Critical",
                    ActionRequired = true
                });
            }

            if (safetyAnalysis.FalsePositiveRate > 0.3)
            {
                insights.Add(new Insight
                {
                    Category = "ModerationSafety",
                    Title = "High False Positive Report Rate",
                    Description =
                        $"False positive rate is {safetyAnalysis.FalsePositiveRate * 100:F1}%. Consider improving report guidelines.",
                    Severity = "Medium",
                    ActionRequired = false
                });
            }

            _logger.LogDebug("Generated {InsightCount} insights", insights.Count);

            return insights;
        }

        private Proposal BuildProposal(
            PreparedAnalysisData data,
            ContentQualityAnalysis contentAnalysis,
            CommunityHealthAnalysis communityAnalysis,
            ReactionPatternAnalysis reactionAnalysis,
            ModerationSafetyAnalysis safetyAnalysis,
            List<Insight> insights)
        {
            _logger.LogInformation("Building proposal");

            var proposal = new Proposal
            {
                Id = Guid.NewGuid(),
                ProposalId = GenerateProposalId(),
                AnalysisVersion = $"{DateTime.UtcNow:yyyy-MM-dd}-{Guid.NewGuid().ToString().Substring(0, 8)}",
                Status = ProposalStatus.PendingReview,
                Summary = new AnalysisSummary
                {
                    TotalPostsAnalyzed = data.Posts.Count,
                    TotalCommentsAnalyzed = data.AllComments.Count,
                    TotalReactionsAnalyzed = data.AllReactions.Count,
                    TotalReportsAnalyzed = data.Reports.Count,
                    AverageEngagementRate = contentAnalysis.AverageEngagementRate,
                    AverageCommentPerPost = communityAnalysis.CommentsPerPost,
                    AverageReactionPerPost = reactionAnalysis.ReactionsPerPost,
                    ProblematicPostsCount = contentAnalysis.LowPerformingPostsCount,
                    ProblematicCommentsCount = safetyAnalysis.ProblematicTargetsCount,
                    CommunityHealthScore = communityAnalysis.CommunityHealthScore,
                    AnalysisStartDate = data.AnalysisStartDate,
                    AnalysisEndDate = data.AnalysisEndDate
                }
            };

            var details = new List<ProposalDetail>
            {
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.ContentQuality,
                    Key = "HighPerformingPosts",
                    Value = contentAnalysis.HighPerformingPostsCount,
                    Description = "Posts with engagement rate >= 1.5x average"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.ContentQuality,
                    Key = "LowPerformingPosts",
                    Value = contentAnalysis.LowPerformingPostsCount,
                    Description = "Posts with engagement rate <= 0.5x average"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.ContentQuality,
                    Key = "LowQualityPostIds",
                    Value = string.Join(",", contentAnalysis.LowQualityPosts),
                    Description = "Posts with quality issues"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.CommunityHealth,
                    Key = "UniqueParticipants",
                    Value = communityAnalysis.UniqueParticipants,
                    Description = "Total unique users participating in community"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.CommunityHealth,
                    Key = "DiscussionQualityScore",
                    Value = communityAnalysis.DiscussionQualityScore,
                    Description = "Percentage of posts with 3+ comments"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.CommunityHealth,
                    Key = "CommunityHealthScore",
                    Value = communityAnalysis.CommunityHealthScore,
                    Description = "Overall community health metric (0-100)"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.ReactionPattern,
                    Key = "ReactionDistribution",
                    Value = reactionAnalysis.ReactionDistribution,
                    Description = "Breakdown of reaction types"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.ReactionPattern,
                    Key = "VolatilePostsCount",
                    Value = reactionAnalysis.VolatilePostsCount,
                    Description = "Posts with high negative reaction rates"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.ReactionPattern,
                    Key = "ReactionToCommentRatio",
                    Value = reactionAnalysis.ReactionToCommentRatio,
                    Description = "Average ratio of reactions to comments"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.ModerationSafety,
                    Key = "ReportDistribution",
                    Value = safetyAnalysis.ReportDistribution,
                    Description = "Breakdown of report types"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.ModerationSafety,
                    Key = "ApprovedReports",
                    Value = safetyAnalysis.ApprovedReports,
                    Description = "Valid reports that were approved"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.ModerationSafety,
                    Key = "ProblematicTargets",
                    Value = string.Join(",", safetyAnalysis.ProblematicTargetIds),
                    Description = "Posts/comments reported 2+ times"
                },
                new()
                {
                    ProposalEntityId = proposal.Id,
                    Proposal = proposal,
                    DetailId = Guid.NewGuid().ToString(),
                    Category = AnalysisCategory.ModerationSafety,
                    Key = "SafetyScore",
                    Value = safetyAnalysis.SafetyScore,
                    Description = "Overall platform safety score (0-100)"
                }
            };

            proposal.Details = details;
            proposal.Recommendations = GenerateRecommendations(insights, contentAnalysis, safetyAnalysis, proposal);

            _logger.LogDebug("Proposal built with {DetailCount} details and {RecommendationCount} recommendations",
                proposal.Details.Count, proposal.Recommendations.Count);

            return proposal;
        }

        private List<Recommendation> GenerateRecommendations(
            List<Insight> insights,
            ContentQualityAnalysis contentAnalysis,
            ModerationSafetyAnalysis safetyAnalysis,
            Proposal proposal)
        {
            var recommendations = new List<Recommendation>();

            foreach (var insight in insights.Where(i => i.ActionRequired))
            {
                switch (insight.Category)
                {
                    case "ContentQuality":
                        recommendations.Add(new Recommendation
                        {
                            ProposalEntityId = proposal.Id,
                            Proposal = proposal,
                            RecommendationId = Guid.NewGuid().ToString(),
                            Type = RecommendationType.PerformanceOptimization,
                            Title = "Improve Content Quality",
                            Description =
                                $"Review and improve {contentAnalysis.LowQualityPosts.Count} low-quality posts",
                            Priority = 4,
                            ActionItems =
                                "1. Analyze content of low-performing posts\n2. Identify content patterns\n3. Provide guidelines for future content"
                        });
                        break;

                    case "CommunityHealth":
                        recommendations.Add(new Recommendation
                        {
                            ProposalEntityId = proposal.Id,
                            Proposal = proposal,
                            RecommendationId = Guid.NewGuid().ToString(),
                            Type = RecommendationType.CommunityEngagement,
                            Title = "Boost Community Engagement",
                            Description = "Implement strategies to increase community participation",
                            Priority = 4,
                            ActionItems =
                                "1. Create discussion-focused content\n2. Encourage comments and replies\n3. Highlight best discussions"
                        });
                        break;

                    case "ReactionPattern":
                        recommendations.Add(new Recommendation
                        {
                            ProposalEntityId = proposal.Id,
                            Proposal = proposal,
                            RecommendationId = Guid.NewGuid().ToString(),
                            Type = RecommendationType.ContentModeration,
                            Title = "Address Emotionally Volatile Content",
                            Description =
                                $"Review and moderate {contentAnalysis.LowPerformingPostsCount} volatile posts",
                            Priority = 3,
                            ActionItems =
                                "1. Review negative reaction triggers\n2. Improve content framing\n3. Add context or warnings where needed"
                        });
                        break;

                    case "ModerationSafety":
                        recommendations.Add(new Recommendation
                        {
                            ProposalEntityId = proposal.Id,
                            Proposal = proposal,
                            RecommendationId = Guid.NewGuid().ToString(),
                            Type = RecommendationType.SafetyImprovement,
                            Title = "Enhance Platform Safety",
                            Description =
                                $"Address {safetyAnalysis.ProblematicTargetsCount} problematic content items",
                            Priority = 5,
                            ActionItems =
                                "1. Immediately review flagged content\n2. Take moderation actions\n3. Update safety policies"
                        });
                        break;
                }
            }

            return recommendations;
        }

        private async Task<Proposal> PersistProposal(Proposal proposal)
        {
            _logger.LogInformation("Persisting proposal {ProposalId}", proposal.ProposalId);

            var savedProposal = await _proposalRepository.AddAsync(proposal);

            var historyEntry = new ProposalHistory
            {
                Id = Guid.NewGuid(),
                ProposalEntityId = savedProposal.Id,
                ProposalId = savedProposal.ProposalId,
                HistoryId = Guid.NewGuid().ToString(),
                Status = ProposalStatus.PendingReview,
                ChangeDescription = "Proposal created from analysis",
                ChangedAt = DateTime.UtcNow,
                ChangedBy = "System"
            };

            await _proposalHistoryRepository.AddAsync(historyEntry);

            _logger.LogInformation("Proposal {ProposalId} persisted successfully", savedProposal.ProposalId);

            return savedProposal;
        }

        private string GenerateProposalId()
        {
            return $"PROP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        private sealed class PreparedAnalysisData
        {
            public List<AnalysisPost> Posts { get; set; } = new();
            public List<AnalysisComment> AllComments { get; set; } = new();
            public List<AnalysisReaction> AllReactions { get; set; } = new();
            public List<AnalysisReport> Reports { get; set; } = new();
            public DateTime AnalysisStartDate { get; set; }
            public DateTime AnalysisEndDate { get; set; }
        }

        private sealed class PostMetrics
        {
            public string PostId { get; set; } = string.Empty;
            public int CommentCount { get; set; }
            public int ReactionCount { get; set; }
            public double EngagementRate { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        private sealed class ContentQualityAnalysis
        {
            public List<PostMetrics> PostMetrics { get; set; } = new();
            public int HighPerformingPostsCount { get; set; }
            public int LowPerformingPostsCount { get; set; }
            public double AverageEngagementRate { get; set; }
            public List<string> LowQualityPosts { get; set; } = new();
        }

        private sealed class CommunityHealthAnalysis
        {
            public int TotalComments { get; set; }
            public double CommentsPerPost { get; set; }
            public int UniqueParticipants { get; set; }
            public double ParticipationRate { get; set; }
            public double DiscussionQualityScore { get; set; }
            public double CommunityHealthScore { get; set; }
        }

        private sealed class ReactionPatternAnalysis
        {
            public int TotalReactions { get; set; }
            public double ReactionsPerPost { get; set; }
            public Dictionary<string, int> ReactionDistribution { get; set; } = new();
            public int VolatilePostsCount { get; set; }
            public List<string> VolatilePostIds { get; set; } = new();
            public double ReactionToCommentRatio { get; set; }
        }

        private sealed class ModerationSafetyAnalysis
        {
            public int TotalReports { get; set; }
            public double ReportRate { get; set; }
            public Dictionary<string, int> ReportDistribution { get; set; } = new();
            public int ApprovedReports { get; set; }
            public int RejectedReports { get; set; }
            public int PendingReports { get; set; }
            public double FalsePositiveRate { get; set; }
            public int ProblematicTargetsCount { get; set; }
            public List<string> ProblematicTargetIds { get; set; } = new();
            public double SafetyScore { get; set; }
        }

        private sealed class Insight
        {
            public string Category { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Severity { get; set; } = string.Empty;
            public bool ActionRequired { get; set; }
        }
    }
}
