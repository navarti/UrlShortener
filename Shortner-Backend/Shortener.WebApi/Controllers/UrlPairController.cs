using Microsoft.AspNetCore.Mvc;
using Shortener.WebApi.Dtos;
using Shortener.WebApi.Services.Interfaces;
using Shortener.WebApi.Util.Filters;

namespace Shortener.WebApi.Controllers;

[Route("[controller]/[action]")]
public class UrlPairController : ControllerBase
{
    private readonly IUrlPairService serviceUrl;

    public UrlPairController(IUrlPairService serviceUrl)
    {
        this.serviceUrl = serviceUrl;
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UrlPairDto))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var pairDTO = await serviceUrl.GetById(id).ConfigureAwait(false);
            return Ok(pairDTO);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UrlPairDto>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<IActionResult> GetAll(UrlPairFilter filter)
    {
        try
        {
            var pairDTOs = await serviceUrl.GetAll(filter).ConfigureAwait(false);
            return Ok(pairDTOs);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateUrlPairDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var creationResult = await serviceUrl.Create(dto).ConfigureAwait(false);

            return CreatedAtAction(
                nameof(GetById),
                new { id = creationResult.Id },
                creationResult);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UrlPairDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut]
    public async Task<IActionResult> Update(UrlPairDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var uldatedDTO = await serviceUrl.Update(dto).ConfigureAwait(false);

            return Ok(uldatedDTO);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await serviceUrl.SoftDelete(id).ConfigureAwait(false);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        return NoContent();
    }

}
