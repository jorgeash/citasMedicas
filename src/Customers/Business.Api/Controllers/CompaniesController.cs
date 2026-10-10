using Business.Core.DTOs;
using Business.Core.DTOs.Customers;
using Business.Core.Features.Companies;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Business.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<HttpResponse<PagedDto<List<CompanyDto>>>>> Get([FromQuery] GetCompaniesQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<HttpResponse<CompanyDto>>> Post([FromBody] CreateCompanyCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}