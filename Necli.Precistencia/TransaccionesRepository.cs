using Microsoft.Data.SqlClient;
using Necli.Entities;
using System.Data;

namespace Necli.Persistencia;

public class TransaccionesRepository
{
    private readonly string _cadenaConexion = "Server=DESKTOP-G6J98FN\\SQLEXPRESS; Database=necli; User ID=sa; Password=12345; TrustServerCertificate=True;";

    public bool RegistrarTransaccion(Transacciones transaccion)
    {
        using (var conexion = new SqlConnection(_cadenaConexion))
        {
            conexion.Open();
            using (var transaccionSQL = conexion.BeginTransaction())
            {
                string validarCuentasYSaldo = @"
                SELECT (SELECT COUNT(*) FROM Cuentas WHERE Numero = @NumeroCuentaOrigen) AS CuentaOrigen,
                       (SELECT Saldo FROM Cuentas WHERE Numero = @NumeroCuentaOrigen) AS SaldoOrigen,
                       (SELECT COUNT(*) FROM Cuentas WHERE Numero = @NumeroCuentaDestino) AS CuentaDestino";

                int cuentaOrigenExiste, cuentaDestinoExiste;
                decimal saldoOrigen;

                using (var comando = new SqlCommand(validarCuentasYSaldo, conexion, transaccionSQL))
                {
                    comando.Parameters.AddWithValue("@NumeroCuentaOrigen", transaccion.NumeroCuentaOrigen);
                    comando.Parameters.AddWithValue("@NumeroCuentaDestino", transaccion.NumeroCuentaDestino);

                    using (var reader = comando.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            transaccionSQL.Rollback();
                            return false;
                        }

                        cuentaOrigenExiste = reader.GetInt32(0);
                        saldoOrigen = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                        cuentaDestinoExiste = reader.GetInt32(2);
                    }
                }

                if (cuentaOrigenExiste == 0 || cuentaDestinoExiste == 0 || saldoOrigen < transaccion.Monto)
                {
                    transaccionSQL.Rollback();
                    return false;
                }

                string actualizarSaldos = @"
                UPDATE Cuentas SET Saldo = Saldo - @Monto WHERE Numero = @NumeroCuentaOrigen;
                UPDATE Cuentas SET Saldo = Saldo + @Monto WHERE Numero = @NumeroCuentaDestino;";

                using (var comandoActualizar = new SqlCommand(actualizarSaldos, conexion, transaccionSQL))
                {
                    comandoActualizar.Parameters.AddWithValue("@Monto", transaccion.Monto);
                    comandoActualizar.Parameters.AddWithValue("@NumeroCuentaOrigen", transaccion.NumeroCuentaOrigen);
                    comandoActualizar.Parameters.AddWithValue("@NumeroCuentaDestino", transaccion.NumeroCuentaDestino);
                    comandoActualizar.ExecuteNonQuery();
                }

                string insertarTransaccion = @"
                INSERT INTO Transacciones (NumeroCuentaOrigen, NumeroCuentaDestino, Monto, Tipo)
                OUTPUT INSERTED.NumeroTransaccion, INSERTED.FechaTransaccion
                VALUES (@NumeroCuentaOrigen, @NumeroCuentaDestino, @Monto, @Tipo)";

                using (var comandoInsertar = new SqlCommand(insertarTransaccion, conexion, transaccionSQL))
                {
                    comandoInsertar.Parameters.AddWithValue("@NumeroCuentaOrigen", transaccion.NumeroCuentaOrigen);
                    comandoInsertar.Parameters.AddWithValue("@NumeroCuentaDestino", transaccion.NumeroCuentaDestino);
                    comandoInsertar.Parameters.AddWithValue("@Monto", transaccion.Monto);
                    comandoInsertar.Parameters.AddWithValue("@Tipo", transaccion.Tipo);

                    using (var reader = comandoInsertar.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            transaccion.NumeroTransaccion = reader.GetInt32(0);
                            transaccion.FechaTransaccion = reader.GetDateTime(1);
                        }
                    }
                }

                transaccionSQL.Commit();
                return true;
            }
        }
    }

    public TransaccionesDTO? ConsultarTransaccion(int numeroTransaccion)
    {
        using (var conexion = new SqlConnection(_cadenaConexion))
        {
            string sql = "SELECT * FROM Transacciones WHERE NumeroTransaccion = @NumeroTransaccion";

            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@NumeroTransaccion", numeroTransaccion);
                conexion.Open();

                using (var lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        return new TransaccionesDTO
                        {
                            NumeroTransaccion = Convert.ToInt32(lector["NumeroTransaccion"]),
                            FechaTransaccion = Convert.ToDateTime(lector["FechaTransaccion"]),
                            NumeroCuentaOrigen = Convert.ToInt32(lector["NumeroCuentaOrigen"]),
                            NumeroCuentaDestino = Convert.ToInt32(lector["NumeroCuentaDestino"]),
                            Monto = Convert.ToDecimal(lector["Monto"]),
                            Tipo = (Tipo)Convert.ToInt32(lector["Tipo"])
                        };
                    }
                }
            }
        }

        return null;
    }
}
