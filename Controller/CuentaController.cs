using Microsoft.AspNetCore.Mvc;
using Necli.Entities;
using Necli.WebApi.Services;
using Microsoft.Data.SqlClient;

namespace Necli.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CuentasController : ControllerBase
{
    private readonly GestionarCuentas _gestionarCuentas;

    public CuentasController(GestionarCuentas gestionarCuentas)
    {
        _gestionarCuentas = gestionarCuentas;
    }

    [HttpGet("{numero}")]
    public ActionResult<UsuariosYCuentaDTO> ConsultarCuenta(string numero)
    {
        try
        {
            var cuenta = _gestionarCuentas.ConsultarCuenta(numero);
            if (cuenta == null)
            {
                return NotFound(new { mensaje = "No se encontró la cuenta." });
            }
            return Ok(cuenta);
        }
        catch (SqlException)
        {
            return StatusCode(500, new { mensaje = "Error al acceder a la base de datos." });
        }
        catch (InvalidOperationException)
        {
            return StatusCode(500, new { mensaje = "Error de operación no válida." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
        }
    }

    [HttpDelete("{numero}")]
    public ActionResult EliminarCuenta(string numero)
    {
        try
        {
            var cuenta = _gestionarCuentas.ConsultarCuenta(numero);

            if (cuenta == null)
            {
                return NotFound(new { mensaje = "No se encontró la cuenta." });
            }

            if (cuenta.Saldo > 50000)
            {
                return BadRequest(new { mensaje = "No se puede eliminar la cuenta porque tiene más de $50,000 COP." });
            }

            bool eliminada = _gestionarCuentas.EliminarCuenta(numero);
            if (!eliminada)
            {
                return BadRequest(new { mensaje = "No se pudo eliminar la cuenta." });
            }

            return Ok(new { mensaje = "Cuenta eliminada con éxito." });
        }
        catch (SqlException)
        {
            return StatusCode(500, new { mensaje = "Error al acceder a la base de datos." });
        }
        catch (InvalidOperationException)
        {
            return StatusCode(500, new { mensaje = "Error de operación no válida." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
        }
    }
}
