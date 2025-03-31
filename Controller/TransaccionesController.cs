using Microsoft.AspNetCore.Mvc;
using Necli.Entities;
using Necli.WebApi.Services;
using Microsoft.Data.SqlClient;

namespace Necli.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionesController : ControllerBase
    {
        private readonly GestionarTransacciones _gestionarTransacciones;

        public TransaccionesController(GestionarTransacciones gestionarTransacciones)
        {
            _gestionarTransacciones = gestionarTransacciones;
        }

        [HttpPost]
        public ActionResult<TransaccionesDTO> RealizarTransaccion([FromBody] Transacciones transaccion)
        {
            try
            {
                if (transaccion == null)
                {
                    return BadRequest("Los datos de la transacción no pueden ser nulos.");
                }

                if (transaccion.Monto < 1000 || transaccion.Monto > 5000000)
                {
                    return BadRequest("El monto de la transacción debe estar entre 1.000 y 5.000.000 COP.");
                }

                transaccion.FechaTransaccion = DateTime.Now;

                bool resultado = _gestionarTransacciones.RegistrarTransaccion(transaccion);
                if (!resultado)
                {
                    return BadRequest("Error al registrar la transacción. Verifique que las cuentas sean válidas y tenga saldo suficiente.");
                }

                var transaccionDTO = new TransaccionesDTO
                {
                    NumeroTransaccion = transaccion.NumeroTransaccion,
                    FechaTransaccion = transaccion.FechaTransaccion,
                    NumeroCuentaOrigen = transaccion.NumeroCuentaOrigen,
                    NumeroCuentaDestino = transaccion.NumeroCuentaDestino,
                    Monto = transaccion.Monto,
                    Tipo = transaccion.Tipo
                };

                return CreatedAtAction(nameof(ConsultarTransaccion), new { numeroTransaccion = transaccionDTO.NumeroTransaccion }, transaccionDTO);
            }
            catch (SqlException)
            {
                return StatusCode(500, "Error al acceder a la base de datos.");
            }
            catch (InvalidOperationException)
            {
                return StatusCode(500, "Error de operación no válida.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error inesperado al procesar la transacción.");
            }
        }

        [HttpGet("{numeroTransaccion}")]
        public IActionResult ConsultarTransaccion(int numeroTransaccion)
        {
            try
            {
                var transaccion = _gestionarTransacciones.ConsultarTransaccion(numeroTransaccion);

                if (transaccion == null)
                {
                    return NotFound("Transacción no encontrada.");
                }

                return Ok(transaccion);
            }
            catch (SqlException)
            {
                return StatusCode(500, "Error al acceder a la base de datos.");
            }
            catch (InvalidOperationException)
            {
                return StatusCode(500, "Error de operación no válida.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error inesperado al consultar la transacción.");
            }
        }
    }
}
