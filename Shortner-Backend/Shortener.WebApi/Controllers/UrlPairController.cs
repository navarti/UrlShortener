using Microsoft.AspNetCore.Mvc;
using Shortener.WebApi.DTOs;
using Shortener.WebApi.Services.Interfaces;

namespace Shortener.WebApi.Controllers;

[Route("[controller]/[action]")]
public class UrlPairController : ControllerBase
{
    private readonly IUrlPairService serviceUrl;

    public UrlPairController(IUrlPairService serviceUrl)
    {
        this.serviceUrl = serviceUrl;
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UrlPairDTO))]
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

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> Create(UrlPairDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var creationResult = await serviceUrl.Create(dto).ConfigureAwait(false);

        return CreatedAtAction(
            nameof(GetById),
            new { id = creationResult.Id },
            creationResult);
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UrlPairDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut]
    public async Task<IActionResult> Update(UrlPairDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(await serviceUrl.Update(dto).ConfigureAwait(false));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await serviceUrl.Delete(id).ConfigureAwait(false);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        return NoContent();
    }

}
