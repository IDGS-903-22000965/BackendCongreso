using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CongresoTIC.API.Data;
using CongresoTIC.API.Models;

namespace CongresoTIC.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class ParticipantesController : ControllerBase
    {
        private readonly CongresoDbContext _context;
        private readonly ILogger<ParticipantesController> _logger;

        public ParticipantesController(CongresoDbContext context, ILogger<ParticipantesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("listado")]
        public async Task<ActionResult<IEnumerable<Participante>>> ObtenerListadoCompleto()
        {
            try
            {
                var participantes = await _context.Participantes
                    .OrderBy(p => p.Nombre)
                    .ToListAsync();

                return Ok(participantes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de participantes");
                return StatusCode(500, new { mensaje = "Error al obtener el listado de participantes" });
            }
        }

        [HttpGet("listado/{q}")]
        public async Task<ActionResult<IEnumerable<Participante>>> BuscarParticipantes(string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return BadRequest(new { mensaje = "El parámetro de búsqueda es requerido" });
                }

                string busqueda = q.ToLower();
                var participantes = await _context.Participantes
                    .Where(p =>
                        p.Nombre.ToLower().Contains(busqueda) ||
                        p.Apellidos.ToLower().Contains(busqueda) ||
                        p.Email.ToLower().Contains(busqueda) ||
                        p.UsuarioTwitter.ToLower().Contains(busqueda) ||
                        p.Ocupacion.ToLower().Contains(busqueda)
                    )
                    .OrderBy(p => p.Nombre)
                    .ToListAsync();

                return Ok(participantes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar participantes");
                return StatusCode(500, new { mensaje = "Error al buscar participantes" });
            }
        }

        [HttpGet("participante/{id}")]
        public async Task<ActionResult<Participante>> ObtenerParticipante(int id)
        {
            try
            {
                var participante = await _context.Participantes.FindAsync(id);

                if (participante == null)
                {
                    return NotFound(new { mensaje = $"No se encontró el participante con ID {id}" });
                }

                return Ok(participante);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el participante con ID {Id}", id);
                return StatusCode(500, new { mensaje = "Error al obtener el participante" });
            }
        }

        [HttpPost("registro")]
        public async Task<ActionResult<Participante>> RegistrarParticipante([FromBody] Participante participante)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _context.Participantes.AnyAsync(p => p.Email == participante.Email))
                {
                    return BadRequest(new { mensaje = "El email ya está registrado" });
                }

                if (await _context.Participantes.AnyAsync(p => p.UsuarioTwitter == participante.UsuarioTwitter))
                {
                    return BadRequest(new { mensaje = "El usuario de Twitter ya está registrado" });
                }

                participante.FechaRegistro = DateTime.Now;

                _context.Participantes.Add(participante);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(ObtenerParticipante),
                    new { id = participante.Id },
                    participante
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al registrar participante");
                return StatusCode(500, new { mensaje = "Error al guardar el participante en la base de datos" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar participante");
                return StatusCode(500, new { mensaje = "Error al registrar el participante" });
            }
        }
    }
}