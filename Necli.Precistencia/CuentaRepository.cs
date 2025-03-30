using Microsoft.Data.SqlClient;
using Necli.Entities;

namespace Necli.Persistencia;

public class CuentaRepository
{
    private readonly string _cadenaConexion = "Server=DESKTOP-G6J98FN\\SQLEXPRESS; Database=necli;User ID=sa;Password=12345; TrustServerCertificate=True;";

    public UsuariosYCuentaDTO? ConsultarCuenta(string numero)
    {
        
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                string sql = @"SELECT C.Numero, C.Saldo, U.Id, U.NombreUsuario, 
                                      U.ApellidoUsuario, U.Correo
                               FROM Cuentas C
                               INNER JOIN Usuarios U ON C.UsuarioId = U.Id
                               WHERE C.Numero = @Numero";

                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Numero", numero);
                    conexion.Open();

                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new UsuariosYCuentaDTO
                            {
                                Numero = lector["Numero"]?.ToString() ?? string.Empty,
                                Saldo = lector["Saldo"] != DBNull.Value ? Convert.ToDecimal(lector["Saldo"]) : 0,
                                Id = lector["Id"]?.ToString() ?? string.Empty,
                                NombreUsuario = lector["NombreUsuario"]?.ToString() ?? string.Empty,
                                ApellidoUsuario = lector["ApellidoUsuario"]?.ToString() ?? string.Empty,
                                Correo = lector["Correo"]?.ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }
        
        return null;
    }

    public UsuariosYCuentaDTO? ConsultarUsuario(string id)
    {
       
        
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                string sql = "SELECT Id, NombreUsuario, ApellidoUsuario, Correo FROM Usuarios WHERE Id = @Id";

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
                                NombreUsuario = lector["NombreUsuario"].ToString(),
                                ApellidoUsuario = lector["ApellidoUsuario"].ToString(),
                                Correo = lector["Correo"].ToString()
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
        
    

    public bool EliminarCuenta(string numero)
    {
        
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                string sql = "DELETE FROM Cuentas WHERE Numero = @Numero";

                using (var comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@Numero", numero);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }
       
    }

