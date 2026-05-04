using Microsoft.EntityFrameworkCore;

namespace THSocialMedia.Domain.Entities
{
    [Owned]
    public class AnalysisSummary
    {
        public int TotalPostsAnalyzed { get; set; }
        public int TotalCommentsAnalyzed { get; set; }
        public int TotalReactionsAnalyzed { get; set; }
        public int TotalReportsAnalyzed { get; set; }
        public double AverageEngagementRate { get; set; }
        public double AverageCommentPerPost { get; set; }
        public double AverageReactionPerPost { get; set; }
        public int ProblematicPostsCount { get; set; }
        public int ProblematicCommentsCount { get; set; }
        public double CommunityHealthScore { get; set; }
        public DateTime AnalysisStartDate { get; set; }
        public DateTime AnalysisEndDate { get; set; }
    }
}
