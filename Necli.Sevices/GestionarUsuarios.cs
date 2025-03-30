using Necli.Entities;
using Necli.Persistencia;
using Microsoft.Data.SqlClient;
using Necli.WebApi.Exceptions;

namespace Necli.WebApi.Services;

public class GestionarUsuarios
{
    private readonly UsuarioRepository _usuarioRepository = new();

    public bool CrearUsuarioYCuenta(UsuariosYCuentaDTO usuario)
    {
        try
        {
            return _usuarioRepository.CrearUsuarioYCuenta(usuario);
        }
        catch (SqlException)
        {
            throw new NegocioException("Error de base de datos al crear el usuario y la cuenta.");
        }
        catch (InvalidOperationException)
        {
            throw new NegocioException("Operación no válida al crear el usuario y la cuenta.");
        }
        catch (Exception)
        {
            throw new NegocioException("Ocurrió un error inesperado al crear el usuario y la cuenta.");
        }
    }



    public UsuariosYCuentaDTO? ConsultarUsuario(string id)
    {
        try
        {
            return _usuarioRepository.ConsultarUsuario(id);
        }
        catch (SqlException)
        {
            throw new NegocioException("Error de base de datos al consultar el usuario.");
        }
        catch (InvalidOperationException)
        {
            throw new NegocioException("Operación no válida al consultar el usuario.");
        }
        catch (Exception)
        {
            throw new NegocioException("Ocurrió un error inesperado al consultar el usuario.");
        }
    }


    public bool ActualizarUsuario(string id, UsuariosYCuentaDTO usuario)
    {
        try
        {
            return _usuarioRepository.ActualizarUsuario(id, usuario);
        }
        catch (SqlException)
        {
            throw new NegocioException("Error de base de datos al actualizar el usuario.");
        }
        catch (InvalidOperationException)
        {
            throw new NegocioException("Operación no válida al actualizar el usuario.");
        }
        catch (Exception)
        {
            throw new NegocioException("Ocurrió un error inesperado al actualizar el usuario.");
        }
    }
}
