using Microsoft.AspNetCore.Mvc;
using Necli.Entities; 
using Necli.WebApi.Services; 

namespace Necli.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{

    private readonly GestionarUsuarios _gestionarUsuarios = new();


    [HttpPost]
    public ActionResult<UsuariosYCuentaDTO> CrearUsuarioYCuenta([FromBody] UsuariosYCuentaDTO usuarioDTO)
    {
        try
        {
            bool creado = _gestionarUsuarios.CrearUsuarioYCuenta(usuarioDTO);
            if (creado)
            {
                return CreatedAtAction(nameof(CrearUsuarioYCuenta), usuarioDTO);
            }
            else
            {
                return BadRequest("No se pudo completar la solicitud.");
            }
        }
        catch (InvalidOperationException)
        {
            return BadRequest("No se pudo completar la solicitud.");
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno. Inténtelo más tarde.");
        }
    }


    [HttpGet("{id}")]
    public ActionResult<UsuariosYCuentaDTO> ConsultarUsuario(string id)
    {
        try
        {
            var usuario = _gestionarUsuarios.ConsultarUsuario(id);
            if (usuario == null)
            {
                return NotFound("El usuario no existe.");
            }
            return Ok(usuario);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno. Inténtelo más tarde.");
        }
    }

    [HttpPut("{id}")]
    public ActionResult ActualizarUsuario(string id, [FromBody] UsuariosYCuentaDTO usuario)
    {
        try
        {
            var usuarioExistente = _gestionarUsuarios.ConsultarUsuario(id);
            if (usuarioExistente == null)
            {
                return NotFound("El usuario no existe.");
            }

            bool actualizado = _gestionarUsuarios.ActualizarUsuario(id, usuario);
            if (!actualizado)
            {
                return BadRequest("No se pudo actualizar el usuario.");
            }

            return Ok("Usuario actualizado con éxito.");
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocurrió un error interno. Inténtelo más tarde.");
        }
    }
}

