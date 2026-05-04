using Microsoft.AspNetCore.Mvc;
using THSocialMedia.Application.Models.Analysis;
using THSocialMedia.Application.Services;
using THSocialMedia.Domain.Entities;
using THSocialMedia.Presentation.Extensions.Models;

namespace THSocialMedia.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProposalController : ControllerBase
    {
        private readonly IProposalService _proposalService;

        public ProposalController(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze([FromBody] DataAnalysisDto dto)
        {
            try
            {
                var proposal = await _proposalService.RunAnalysisAsync(dto);
                return Ok(ApiResponse<Proposal>.Ok(proposal, "Analysis completed successfully"));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ApiResponse.BadRequest(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.InternalServerError("An error occurred during analysis"));
            }
        }
    }
}
