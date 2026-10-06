using Microsoft.AspNetCore.Mvc;
using Parcial1_P4_Yerferson.Models;
using Parcial1_P4_Yerferson.Services;

namespace Parcial1_P4_Yerferson.Controllers;

[ApiController]
[Route("api/autores")]
public class AutoresController(AutoresService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return Ok(await service.GetListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var autor = await service.GetByIdAsync(id);

        if (autor is null) return NotFound();

        return Ok(autor);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] Autor autor)
    {
        var error = Validate(autor);

        if (error is not null)
            return BadRequest(new { mensaje = error });

        var nuevo = autor with
        {
            IdAutor = 0,
            Nombres = autor.Nombres.Trim(),
            Nacionalidad = autor.Nacionalidad.Trim(),
            FechaNacimiento = autor.FechaNacimiento.Date
        };

        int id = await service.SaveAsync(nuevo);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            nuevo with { IdAutor = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] Autor autor)
    {
        var error = Validate(autor);

        if (error is not null)
            return BadRequest(new { mensaje = error });

        var actualizado = autor with
        {
            IdAutor = id,
            Nombres = autor.Nombres.Trim(),
            Nacionalidad = autor.Nacionalidad.Trim(),
            FechaNacimiento = autor.FechaNacimiento.Date
        };

        bool updated = await service.UpdateAsync(actualizado);

        if (!updated) return NotFound();

        return Ok(actualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await service.DeleteAsync(id);

        if (!deleted) return NotFound();

        return NoContent();
    }

    private static string? Validate(Autor autor)
    {
        if (string.IsNullOrWhiteSpace(autor.Nombres)
            || autor.Nombres.Length > 150)
            return "Nombres es obligatorio y permite hasta 150 caracteres.";

        if (string.IsNullOrWhiteSpace(autor.Nacionalidad)
            || autor.Nacionalidad.Length > 80)
            return "Nacionalidad es obligatoria y permite hasta 80 caracteres.";

        if (autor.FechaNacimiento == default
            || autor.FechaNacimiento.Date > DateTime.UtcNow.Date)
            return "La fecha de nacimiento debe existir y no ser futura.";

        if (autor.Sueldo < 0
            || autor.Sueldo > 1000000000000m
            || decimal.Round(autor.Sueldo, 2) != autor.Sueldo)
            return "El sueldo debe ser valido, no negativo y con hasta 2 decimales.";

        return null;
    }
}