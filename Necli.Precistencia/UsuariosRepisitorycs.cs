using Microsoft.Data.SqlClient;
using Necli.Entities;

namespace Necli.Persistencia;

public class UsuarioRepository
{
    private readonly string _cadenaConexion = "Server=DESKTOP-G6J98FN\\SQLEXPRESS; Database=necli;User ID=sa;Password=12345; TrustServerCertificate=True;";
    public bool CrearUsuarioYCuenta(UsuariosYCuentaDTO usuariosYCuentaDTO)
    {
        using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
        {
            conexion.Open();
            using (SqlTransaction transaccion = conexion.BeginTransaction())
            {
                var usuarioExistente = ConsultarUsuario(usuariosYCuentaDTO.Id);
                if (usuarioExistente != null)
                {
                    return false;
                }

                string insertarUsuario = "INSERT INTO Usuarios (Id, NombreUsuario, ApellidoUsuario, Correo) VALUES (@Id, @NombreUsuario, @ApellidoUsuario, @Correo)";

                using (SqlCommand cmd = new SqlCommand(insertarUsuario, conexion, transaccion))
                {
                    cmd.Parameters.AddWithValue("@Id", usuariosYCuentaDTO.Id);
                    cmd.Parameters.AddWithValue("@NombreUsuario", usuariosYCuentaDTO.NombreUsuario);
                    cmd.Parameters.AddWithValue("@ApellidoUsuario", usuariosYCuentaDTO.ApellidoUsuario);
                    cmd.Parameters.AddWithValue("@Correo", usuariosYCuentaDTO.Correo);
                    cmd.ExecuteNonQuery();
                }

                string insertarCuenta = "INSERT INTO Cuentas (Numero, UsuarioId, Saldo) VALUES (@Numero, @UsuarioId, @Saldo)";
                using (SqlCommand cmd = new SqlCommand(insertarCuenta, conexion, transaccion))
                {
                    cmd.Parameters.AddWithValue("@Numero", usuariosYCuentaDTO.Numero);
                    cmd.Parameters.AddWithValue("@UsuarioId", usuariosYCuentaDTO.Id);
                    cmd.Parameters.AddWithValue("@Saldo", usuariosYCuentaDTO.Saldo);
                    cmd.ExecuteNonQuery();
                }

                transaccion.Commit();
                return true;
            }
        }
    }


    public UsuariosYCuentaDTO? ConsultarUsuario(string id)
    {
        using (var conexion = new SqlConnection(_cadenaConexion))
        {
            string sql = @"SELECT U.Id, C.Numero, U.NombreUsuario, U.ApellidoUsuario, U.Correo, C.Saldo
                       FROM Usuarios U
                       LEFT JOIN Cuentas C ON U.Id = C.UsuarioId
                       WHERE U.Id = @Id";

            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Id", id);
                conexion.Open();

                using (var lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        return new UsuariosYCuentaDTO
                        {
                            Id = lector["Id"].ToString(),
                            Numero = lector["Numero"] != DBNull.Value ? lector["Numero"].ToString() : null,
                            NombreUsuario = lector["NombreUsuario"].ToString(),
                            ApellidoUsuario = lector["ApellidoUsuario"].ToString(),
                            Correo = lector["Correo"].ToString(),
                            Saldo = lector["Saldo"] != DBNull.Value ? Convert.ToDecimal(lector["Saldo"]) : 0
                        };
                    }
                }
            }
        }

        return null;
    }


    public bool ActualizarUsuario(string id, UsuariosYCuentaDTO usuario)
    {
        
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                string sql = @"UPDATE Usuarios 
                               SET NombreUsuario = @Nombre, 
                                   ApellidoUsuario = @Apellido, 
                                   Correo = @Correo 
                               WHERE Id = @Id";

                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", usuario.NombreUsuario);
                    comando.Parameters.AddWithValue("@Apellido", usuario.ApellidoUsuario);
                    comando.Parameters.AddWithValue("@Correo", usuario.Correo);
                    comando.Parameters.AddWithValue("@Id", id);

                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
       
    }
}
