using Microsoft.AspNetCore.Mvc;
using VibeLoopBE.Models.Requests;
using VibeLoopBE.Models.Responses;
using VibeLoopBE.Services;

namespace VibeLoopBE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMembers()
    {
        var members = await _memberService.GetAllMembersAsync();
        return Ok(members);
    }

    [HttpPost("mood")]
    public async Task<IActionResult> UpdateMood([FromBody] UpdateMoodRequest request)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.MemberId))
        {
            return BadRequest(new ErrorResponse 
            { 
                Error = "INVALID_REQUEST", 
                Message = "MemberId is required" 
            });
        }

        if (string.IsNullOrWhiteSpace(request.Mood))
        {
            return BadRequest(new ErrorResponse 
            { 
                Error = "INVALID_REQUEST", 
                Message = "Mood is required" 
            });
        }

        if (request.Timestamp == default)
        {
            return BadRequest(new ErrorResponse 
            { 
                Error = "INVALID_REQUEST", 
                Message = "Timestamp is required" 
            });
        }

        var (success, errorCode, errorMessage) = await _memberService.UpdateMoodAsync(request);

        if (!success)
        {
            if (errorCode == "NOT_FOUND")
            {
                return NotFound(new ErrorResponse { Error = errorCode, Message = errorMessage! });
            }
            return BadRequest(new ErrorResponse { Error = errorCode!, Message = errorMessage! });
        }

        return Ok(new { success = true });
    }
}
