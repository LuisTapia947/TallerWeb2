using Necli.Entities;
using Necli.Persistencia;
using Microsoft.Data.SqlClient;
using Necli.WebApi.Exceptions;

namespace Necli.WebApi.Services
{
    public class GestionarTransacciones
    {
        private readonly TransaccionesRepository _transaccionesRepository;

        public GestionarTransacciones(TransaccionesRepository transaccionesRepository)
        {
            _transaccionesRepository = transaccionesRepository;
        }

        public bool RegistrarTransaccion(Transacciones transaccion)
        {
            try
            {
                if (transaccion.Monto < 1000 || transaccion.Monto > 5000000)
                {
                    throw new NegocioException("El monto debe estar entre $1,000 y $5,000,000.");
                }

                return _transaccionesRepository.RegistrarTransaccion(transaccion);
            }
            catch (SqlException)
            {
                throw new NegocioException("Error de base de datos al registrar la transacción.");
            }
            catch (InvalidOperationException)
            {
                throw new NegocioException("Operación no válida al registrar la transacción.");
            }
            catch (Exception)
            {
                throw new NegocioException("Ocurrió un error inesperado al registrar la transacción.");
            }
        }


        public Transacciones? ConsultarTransaccion(int numeroTransaccion)
        {
            try
            {
                var transaccionDTO = _transaccionesRepository.ConsultarTransaccion(numeroTransaccion);

                if (transaccionDTO == null)
                {
                    return null;
                }

                return new Transacciones
                {
                    NumeroTransaccion = transaccionDTO.NumeroTransaccion,
                    FechaTransaccion = transaccionDTO.FechaTransaccion,
                    NumeroCuentaOrigen = transaccionDTO.NumeroCuentaOrigen,
                    NumeroCuentaDestino = transaccionDTO.NumeroCuentaDestino,
                    Monto = transaccionDTO.Monto,
                    Tipo = transaccionDTO.Tipo
                };
            }
            catch (SqlException)
            {
                throw new NegocioException("Error de base de datos al consultar la transacción.");
            }
            catch (InvalidOperationException)
            {
                throw new NegocioException("Operación no válida al consultar la transacción.");
            }
            catch (Exception)
            {
                throw new NegocioException("Ocurrió un error inesperado al consultar la transacción.");
            }
        }
    }

}
