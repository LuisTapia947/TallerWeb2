using Necli.Entities;
using Necli.Persistencia;
using Microsoft.Data.SqlClient;
using Necli.WebApi.Exceptions;
namespace Necli.WebApi.Services;

public class GestionarCuentas
{
    private readonly CuentaRepository _cuentaRepository;

    public GestionarCuentas(CuentaRepository cuentaRepository)
    {
        _cuentaRepository = cuentaRepository;
    }

    public UsuariosYCuentaDTO? ConsultarCuenta(string numero)
    {
        try
        {
            return _cuentaRepository.ConsultarCuenta(numero);
        }
        catch (SqlException)
        {
            throw new NegocioException("Error de base de datos al consultar la cuenta.");
        }
        catch (Exception)
        {
            throw new NegocioException("Ocurrió un error inesperado al consultar la cuenta.");
        }
    }



public bool EliminarCuenta(string numero)
{
    try
    {
        var cuenta = _cuentaRepository.ConsultarCuenta(numero);

        if (cuenta == null)
        {
            throw new NegocioException("La cuenta especificada no existe.");
        }

        if (cuenta.Saldo > 50000)
        {
            throw new NegocioException("No se puede eliminar una cuenta con saldo superior a $50,000.");
        }

        bool eliminada = _cuentaRepository.EliminarCuenta(numero);

        if (!eliminada)
        {
            throw new NegocioException("No se pudo eliminar la cuenta.");
        }

        return true;
    }
    catch (SqlException)
    {
        throw new NegocioException("Error de base de datos al intentar eliminar la cuenta.");
    }
    catch (Exception)
    {
        throw new NegocioException("Ocurrió un error inesperado al intentar eliminar la cuenta.");
    }
}

}
