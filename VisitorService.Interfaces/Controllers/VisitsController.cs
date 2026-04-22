using isitorService.Application.UseCases.Visits.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisitorService.Application.DTOS;
using VisitorService.Application.UseCases.Visits.Commands;
using VisitorService.Application.UseCases.Visits.Queries;
using VisitorService.Interfaces.Extensions;

namespace VisitorService.Interfaces.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisitorController : ControllerBase
    {
        private readonly IUpdateVisitStatusHandler _updateVisitStatusHandler;
        private readonly IGetTodayApprovedVisitsHandler _getTodayApprovedVisitsHandler;
        private readonly IVisitCheckInHandler _visitCheckInHandler;
        private readonly IVisitCheckOutHandler _visitCheckOutHandler;
        private readonly IGetAllVisitsHandler _getAllVisitsHandler;
        private readonly IcreateVisitHandler _createVisitHandler;

        public VisitorController(
            IUpdateVisitStatusHandler updateVisitStatusHandler,
            IGetTodayApprovedVisitsHandler getTodayApprovedVisitsHandler,
            IVisitCheckInHandler visitCheckInHandler,
            IVisitCheckOutHandler visitCheckOutHandler,
            IGetAllVisitsHandler getAllVisitsHandler,
            IcreateVisitHandler createVisitHandler)
        {
            _updateVisitStatusHandler = updateVisitStatusHandler;
            _getTodayApprovedVisitsHandler = getTodayApprovedVisitsHandler;
            _visitCheckInHandler = visitCheckInHandler;
            _visitCheckOutHandler = visitCheckOutHandler;
            _getAllVisitsHandler = getAllVisitsHandler;
            _createVisitHandler = createVisitHandler;
        }

        [Authorize]
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateVisitStatusCommand dto)
        {
            var ManagerId = User.GetUserId();
            var result = await _updateVisitStatusHandler.Handle(dto, ManagerId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Security")]
        [HttpGet("today/approved")]
        public async Task<IActionResult> GetTodayApproved()
        {
            var result = await _getTodayApprovedVisitsHandler.Handle();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Security")]
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] VisitCheckCommand dto)
        {
            var SecurityId = User.GetUserId();

            var result = await _visitCheckInHandler.Handle(dto, SecurityId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Security")]
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] VisitCheckCommand dto)
        {
            var securityId = User.GetUserId();

            var result = await _visitCheckOutHandler.Handle(dto, securityId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpGet("gestor")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _getAllVisitsHandler.Handle();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateVisit([FromBody] CreateVisitCommand dto)
        {
            Guid user = User.GetUserId();
            var result = await _createVisitHandler.Handler(dto, user);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }
    }
}
