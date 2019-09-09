using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TakeHome.Mediator.Requests;


namespace TakeHome.Web.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/TakeHome")]
    public class TakeHomeController : Controller
    {
        private readonly IMediator _mediator;

        public TakeHomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get(GetRequest request)
        {
            var response = await _mediator.Send(new GetShortestRouteRequest(request?.Origin, request?.Destination));

            if (!string.IsNullOrEmpty(response.ErrorMessage))
                return StatusCode(StatusCodes.Status500InternalServerError, response.ErrorMessage);

            if (!response.IsValid)
                return BadRequest(response.Content);

            if (response.HasNoRoute)
                return NotFound(response.Content);

            return Ok(response.Content);
        }
    }
}
