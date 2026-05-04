using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using THSocialMedia.Application.Models.Analysis;
using THSocialMedia.Application.Services;
using THSocialMedia.Domain.Abstractions.IWriteRepositories;
using THSocialMedia.Domain.Entities;
using THSocialMedia.Domain.Enums;

namespace THSocialMedia.Tests
{
    [TestClass]
    public class ProposalServiceRunAnalysisAsyncTests
    {
        private ProposalService _service = null!;
        private Mock<IProposalRepository> _proposalRepository = null!;
        private Mock<IProposalHistoryRepository> _proposalHistoryRepository = null!;
        private Mock<ILogger<ProposalService>> _logger = null!;

        [TestInitialize]
        public void Setup()
        {
            _proposalRepository = new Mock<IProposalRepository>();
            _proposalHistoryRepository = new Mock<IProposalHistoryRepository>();
            _logger = new Mock<ILogger<ProposalService>>();

            _service = new ProposalService(
                _proposalRepository.Object,
                _proposalHistoryRepository.Object,
                _logger.Object);
        }

        [TestMethod]
        public async Task RunAnalysisAsync_WithValidData_ReturnsProposalWithPendingReviewStatus()
        {
            var dto = CreateValidDataAnalysisDto_150Posts_120Days();

            _proposalRepository
                .Setup(x => x.AddAsync(It.IsAny<Proposal>()))
                .ReturnsAsync((Proposal p) => p);

            _proposalHistoryRepository
                .Setup(x => x.AddAsync(It.IsAny<ProposalHistory>()))
                .ReturnsAsync((ProposalHistory h) => h);

            var result = await _service.RunAnalysisAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(ProposalStatus.PendingReview, result.Status);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ProposalId));
            Assert.AreEqual(150, result.Summary.TotalPostsAnalyzed);
            Assert.IsTrue(result.Details.Count > 0);
            Assert.IsTrue(result.Recommendations.Count >= 0);
        }

        [TestMethod]
        public async Task RunAnalysisAsync_WithNullData_ThrowsArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _service.RunAnalysisAsync(null!));
        }

        [TestMethod]
        public async Task RunAnalysisAsync_WithEmptyPosts_ThrowsInvalidOperationException()
        {
            var dto = new DataAnalysisDto
            {
                Posts = new List<Application.Models.Analysis.Post>(),
                Reports = new List<Report>()
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.RunAnalysisAsync(dto));
        }

        [TestMethod]
        public async Task RunAnalysisAsync_WithOrphanedComment_ThrowsInvalidOperationException()
        {
            var dto = new DataAnalysisDto
            {
                Posts = new List<Application.Models.Analysis.Post>
                {
                    new()
                    {
                        PostId = "P001",
                        Content = "Test",
                        CreatedAt = DateTime.UtcNow.AddDays(-50),
                        AuthorId = "U001",
                        Comments = new List<Application.Models.Analysis.Comment>
                        {
                            new()
                            {
                                CommentId = "C001",
                                PostId = "P999",
                                Content = "Orphaned",
                                CreatedAt = DateTime.UtcNow.AddDays(-49),
                                AuthorId = "U002"
                            }
                        },
                        Reactions = new List<Application.Models.Analysis.Reaction>()
                    }
                },
                Reports = new List<Report>()
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.RunAnalysisAsync(dto));
        }

        [TestMethod]
        public async Task RunAnalysisAsync_WithInvalidReportTarget_ThrowsInvalidOperationException()
        {
            var dto = new DataAnalysisDto
            {
                Posts = new List<Application.Models.Analysis.Post>
                {
                    new()
                    {
                        PostId = "P001",
                        Content = "Test",
                        CreatedAt = DateTime.UtcNow.AddDays(-50),
                        AuthorId = "U001",
                        Comments = new List<Application.Models.Analysis.Comment>(),
                        Reactions = new List<Application.Models.Analysis.Reaction>()
                    }
                },
                Reports = new List<Report>
                {
                    new()
                    {
                        ReportId = "REP001",
                        TargetId = "INVALID_ID",
                        Type = ReportType.Spam,
                        Status = ReportStatus.Pending,
                        CreatedAt = DateTime.UtcNow.AddDays(-40),
                        Reason = "Spam"
                    }
                }
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.RunAnalysisAsync(dto));
        }

        [TestMethod]
        public async Task RunAnalysisAsync_WithPostsHavingNoEngagement_CalculatesCorrectly()
        {
            var dto = new DataAnalysisDto
            {
                Posts = new List<Application.Models.Analysis.Post>
                {
                    new()
                    {
                        PostId = "P001",
                        Content = "Silent post",
                        CreatedAt = DateTime.UtcNow.AddDays(-50),
                        AuthorId = "U001",
                        Comments = null,
                        Reactions = null
                    }
                },
                Reports = new List<Report>()
            };

            _proposalRepository
                .Setup(x => x.AddAsync(It.IsAny<Proposal>()))
                .ReturnsAsync((Proposal p) => p);

            _proposalHistoryRepository
                .Setup(x => x.AddAsync(It.IsAny<ProposalHistory>()))
                .ReturnsAsync((ProposalHistory h) => h);

            var result = await _service.RunAnalysisAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Summary.TotalCommentsAnalyzed);
            Assert.AreEqual(0, result.Summary.TotalReactionsAnalyzed);
        }

        [TestMethod]
        public async Task RunAnalysisAsync_With30DaysOfData_Succeeds()
        {
            var baseDate = DateTime.UtcNow.AddDays(-30);
            var dto = new DataAnalysisDto
            {
                Posts = new List<Application.Models.Analysis.Post>
                {
                    new() { PostId = "P001", Content = "Old", CreatedAt = baseDate, AuthorId = "U001" },
                    new() { PostId = "P002", Content = "New", CreatedAt = DateTime.UtcNow, AuthorId = "U002" }
                },
                Reports = new List<Report>()
            };

            _proposalRepository
                .Setup(x => x.AddAsync(It.IsAny<Proposal>()))
                .ReturnsAsync((Proposal p) => p);

            _proposalHistoryRepository
                .Setup(x => x.AddAsync(It.IsAny<ProposalHistory>()))
                .ReturnsAsync((ProposalHistory h) => h);

            var result = await _service.RunAnalysisAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(ProposalStatus.PendingReview, result.Status);
        }

        private DataAnalysisDto CreateValidDataAnalysisDto_150Posts_120Days()
        {
            var posts = new List<Application.Models.Analysis.Post>();
            for (int i = 0; i < 150; i++)
            {
                var post = new Application.Models.Analysis.Post
                {
                    PostId = $"P{i:D4}",
                    Content = $"Post {i}",
                    CreatedAt = DateTime.UtcNow.AddDays(-120 + (i % 120)),
                    AuthorId = $"U{i % 20}",
                    Comments = new List<Application.Models.Analysis.Comment>(),
                    Reactions = new List<Application.Models.Analysis.Reaction>()
                };

                for (int j = 0; j < i % 5; j++)
                {
                    post.Comments!.Add(new Application.Models.Analysis.Comment
                    {
                        CommentId = $"C{i:D4}_{j}",
                        PostId = post.PostId,
                        Content = $"Comment {j}",
                        CreatedAt = post.CreatedAt.AddHours(j + 1),
                        AuthorId = $"U{(i + j) % 20}"
                    });
                }

                for (int j = 0; j < (i % 8); j++)
                {
                    post.Reactions!.Add(new Application.Models.Analysis.Reaction
                    {
                        ReactionId = $"R{i:D4}_{j}",
                        PostId = post.PostId,
                        UserId = $"U{(i + j) % 20}",
                        Type = (ReactionType)(j % 6),
                        CreatedAt = post.CreatedAt.AddHours(j)
                    });
                }

                posts.Add(post);
            }

            return new DataAnalysisDto
            {
                Posts = posts,
                Reports = new List<Report>()
            };
        }
    }
}
