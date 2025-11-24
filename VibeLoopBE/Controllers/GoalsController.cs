using Microsoft.AspNetCore.Mvc;
using VibeLoopBE.Models.Requests;
using VibeLoopBE.Models.Responses;
using VibeLoopBE.Services;

namespace VibeLoopBE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoalsController : ControllerBase
{
    private readonly IGoalService _goalService;

    public GoalsController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGoal([FromBody] CreateGoalRequest request)
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

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest(new ErrorResponse
            {
                Error = "INVALID_REQUEST",
                Message = "Description is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Date))
        {
            return BadRequest(new ErrorResponse
            {
                Error = "INVALID_REQUEST",
                Message = "Date is required"
            });
        }

        var (success, errorCode, errorMessage, goalId) = await _goalService.CreateGoalAsync(request);

        if (!success)
        {
            if (errorCode == "NOT_FOUND")
            {
                return NotFound(new ErrorResponse { Error = errorCode, Message = errorMessage! });
            }
            return BadRequest(new ErrorResponse { Error = errorCode!, Message = errorMessage! });
        }

        return Ok(new { success = true, goalId });
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateGoal([FromBody] UpdateGoalRequest request)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.GoalId))
        {
            return BadRequest(new ErrorResponse
            {
                Error = "INVALID_REQUEST",
                Message = "GoalId is required"
            });
        }

        var (success, errorCode, errorMessage) = await _goalService.UpdateGoalAsync(request);

        if (!success)
        {
            return NotFound(new ErrorResponse { Error = errorCode!, Message = errorMessage! });
        }

        return Ok(new { success = true });
    }

    [HttpPost("delete")]
    public async Task<IActionResult> DeleteGoal([FromBody] DeleteGoalRequest request)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.GoalId))
        {
            return BadRequest(new ErrorResponse
            {
                Error = "INVALID_REQUEST",
                Message = "GoalId is required"
            });
        }

        var (success, errorCode, errorMessage) = await _goalService.DeleteGoalAsync(request);

        if (!success)
        {
            return NotFound(new ErrorResponse { Error = errorCode!, Message = errorMessage! });
        }

        return Ok(new { success = true });
    }
}
