using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Oracle.ManagedDataAccess.Client;
using SIG_VETERINARIA.Abstractions.IRepository;
using SIG_VETERINARIA.DTOs.Auth;
using SIG_VETERINARIA.DTOs.Common;
using SIG_VETERINARIA.DTOs.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SIG_VETERINARIA.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration configuration;
        private string _connectionSting = "";
        public UserRepository(IConfiguration configuration) {
            this.configuration = configuration;
#pragma warning disable CS8601 // Possible null reference assignment.
            _connectionSting = configuration.GetConnectionString("Connection");
#pragma warning restore CS8601 // Possible null reference assignment.
        }
        public async Task<ResultDto<int>> Create(UserCreateRequestDto newUser)
        {
            ResultDto<int> res = new ResultDto<int>
            {
                Data = new List<int>()
            };

            try
            {
                using (var cn = new OracleConnection(_connectionSting))
                {
                    await cn.OpenAsync();

                    using (var transaction = cn.BeginTransaction())
                    {
                        try
                        {
                            // Verificar si el usuario ya existe
                            string checkUserQuery = "SELECT COUNT(*) FROM tbl_users WHERE username = :username";
                            using (var checkCmd = new OracleCommand(checkUserQuery, cn))
                            {
                                checkCmd.Parameters.Add(new OracleParameter("username", newUser.username));
                                int existingUserCount = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

                                if (existingUserCount > 0)
                                {
                                    res.IsSuccess = false;
                                    res.Message = "El usuario ya existe en nuestros registros";
                                    return res;
                                }
                            }

                            int userId;

                            if (newUser.id == 0)
                            {
                                // Generar un nuevo ID
                                string generateIdQuery = "SELECT NVL(MAX(id), 0) + 1 FROM tbl_users";
                                using (var generateIdCmd = new OracleCommand(generateIdQuery, cn))
                                {
                                    userId = Convert.ToInt32(await generateIdCmd.ExecuteScalarAsync());
                                }

                                // Insertar el nuevo usuario
                                string insertQuery = @"
                            INSERT INTO tbl_users (id, username, password, role_id, state) 
                            VALUES (:id, :username, STANDARD_HASH(NVL(:password, '12345678'), 'SHA512'), :role_id, 1)";
                                using (var insertCmd = new OracleCommand(insertQuery, cn))
                                {
                                    insertCmd.Parameters.Add(new OracleParameter("id", userId));
                                    insertCmd.Parameters.Add(new OracleParameter("username", newUser.username));
                                    insertCmd.Parameters.Add(new OracleParameter("password", newUser.password ?? "12345678"));
                                    insertCmd.Parameters.Add(new OracleParameter("role_id", newUser.role_id));

                                    await insertCmd.ExecuteNonQueryAsync();
                                }
                            }
                            else
                            {
                                // Usar el ID proporcionado y actualizar el usuario existente
                                userId = newUser.id;

                                string updateQuery = @"
                            UPDATE tbl_users
                            SET username = :username,
                                password = STANDARD_HASH(NVL(:password, '12345678'), 'SHA512'),
                                role_id = :role_id
                            WHERE id = :id";
                                using (var updateCmd = new OracleCommand(updateQuery, cn))
                                {
                                    updateCmd.Parameters.Add(new OracleParameter("id", userId));
                                    updateCmd.Parameters.Add(new OracleParameter("username", newUser.username));
                                    updateCmd.Parameters.Add(new OracleParameter("password", newUser.password ?? "12345678"));
                                    updateCmd.Parameters.Add(new OracleParameter("role_id", newUser.role_id));

                                    await updateCmd.ExecuteNonQueryAsync();
                                }
                            }

                            // Commit de la transacción
                            transaction.Commit();

                            res.Data.Add(userId);
                            res.IsSuccess = true;
                            res.Message = "Usuario creado o actualizado exitosamente";
                        }
                        catch (Exception ex)
                        {
                            // Rollback de la transacción en caso de error
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.MessageException = ex.Message;
            }

            return res;
        }

        //public async Task<ResultDto<int>> Create(UserCreateRequestDto request)
        //{
        //    ResultDto<int> res = new ResultDto<int>();

        //    try
        //    {
        //        using(var cn = new OracleConnection(_connectionSting))
        //        {
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@p_id", request.id);
        //            parameters.Add("@p_username", request.username);
        //            parameters.Add("@p_password", request.password);
        //            parameters.Add("@p_rol_id", request.role_id);

        //            using (var lector = await cn.ExecuteReaderAsync("SP_CREATE_USER", parameters, commandType: System.Data.CommandType.StoredProcedure))
        //            {
        //                while (lector.Read())
        //                {
        //                    res.Item = Convert.ToInt32(lector["id"].ToString());
        //                    res.IsSuccess = Convert.ToInt32(lector["id"].ToString()) > 0 ? true : false;
        //                    res.Message = Convert.ToInt32(lector["id"].ToString()) > 0 ? "Informacion guardada correctamente" : "Informacion no se pudo guardar";
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex) 
        //    {
        //        res.IsSuccess=false;
        //        res.MessageException = ex.Message;
        //    }
        //    return res;
        //}
        public async Task<ResultDto<UserListResponseDTO>> GetAll()
        {
            ResultDto<UserListResponseDTO> res = new ResultDto<UserListResponseDTO>();
            List<UserListResponseDTO> list = new List<UserListResponseDTO>();

            try
            {
                using (var cn = new OracleConnection(_connectionSting))
                {
                    cn.Open();
                    OracleCommand command = new OracleCommand("SELECT*FROM tbl_users", cn);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        UserListResponseDTO response = new UserListResponseDTO();
                        response.id = Convert.ToInt32(reader["id"].ToString());
#pragma warning disable CS8601 // Possible null reference assignment.
                        response.username = reader["username"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
                        response.role_id = Convert.ToInt32(reader.GetString(3));
                        list.Add(response);
                    }
                }
                res.IsSuccess = list.Count > 0 ? true : false;
                res.Message = list.Count > 0 ? "Informacion encontrada" : "No se encontro informacion";
                res.Data = list.ToList();
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.MessageException = ex.Message;
            }
            return res;
        }

        //public async Task<ResultDto<UserListResponseDTO>> GetAll()
        //{
        //    ResultDto<UserListResponseDTO> res = new ResultDto<UserListResponseDTO>();
        //    List<UserListResponseDTO> list = new List<UserListResponseDTO>();

        //    try
        //    {
        //        using (var cn = new OracleConnection(_connectionSting))
        //        {
        //            await cn.OpenAsync();

        //            using (var command = new OracleCommand("SP_LIST_USERS", cn))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;

        //                // Crear el parámetro de salida para el cursor
        //                var outCursor = new OracleParameter
        //                {
        //                    OracleDbType = OracleDbType.RefCursor,
        //                    Direction = ParameterDirection.Output
        //                };
        //                command.Parameters.Add(outCursor);

        //                using (var reader = await command.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        var response = new UserListResponseDTO
        //                        {
        //                            id = Convert.ToInt32(reader["id"].ToString()),
        //                            username = reader["username"].ToString(),
        //                            role_id = Convert.ToInt32(reader["role_id"].ToString())
        //                        };
        //                        list.Add(response);
        //                    }
        //                }
        //            }
        //        }
        //        res.IsSuccess = list.Count > 0;
        //        res.Message = list.Count > 0 ? "Información encontrada" : "No se encontró información";
        //        res.Data = list;
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.MessageException = ex.Message;
        //    }
        //    return res;
        //}

        public async Task<ResultDto<int>> Delete(DeleteDto userId)
        {
            ResultDto<int> res = new ResultDto<int>
            {
                Data = new List<int>()
            };

            try
            {
                using (var cn = new OracleConnection(_connectionSting))
                {
                    await cn.OpenAsync();

                    using (var transaction = cn.BeginTransaction())
                    {
                        try
                        {
                            // Verificar si el usuario existe
                            string checkUserQuery = "SELECT COUNT(*) FROM tbl_users WHERE id = :id";
                            using (var checkCmd = new OracleCommand(checkUserQuery, cn))
                            {
                                checkCmd.Parameters.Add(new OracleParameter("id", userId.id));
                                int existingUserCount = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

                                if (existingUserCount == 0)
                                {
                                    res.IsSuccess = false;
                                    res.Message = "El usuario no existe";
                                    return res;
                                }
                            }

                            // Eliminar el usuario (físicamente)
                            string deleteQuery = "DELETE FROM tbl_users WHERE id = :id";
                            using (var deleteCmd = new OracleCommand(deleteQuery, cn))
                            {
                                deleteCmd.Parameters.Add(new OracleParameter("id", userId.id));
                                int rowsAffected = await deleteCmd.ExecuteNonQueryAsync();

                                if (rowsAffected == 0)
                                {
                                    res.IsSuccess = false;
                                    res.Message = "No se pudo actualizar el estado del usuario";
                                    return res;
                                }
                            }

                            // Commit de la transacción
                            transaction.Commit();

                            res.Data.Add(userId.id);
                            res.IsSuccess = true;
                            res.Message = "Usuario eliminado exitosamente";
                        }
                        catch (Exception ex)
                        {
                            // Rollback de la transacción en caso de error
                            transaction.Rollback();
                            res.IsSuccess = false;
                            res.MessageException = ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.MessageException = ex.Message;
            }

            return res;
        }

        //public async Task<ResultDto<int>> Delete(DeleteDto request)
        //{
        //    ResultDto<int> res = new ResultDto<int>
        //    {
        //        Data = new List<int>()
        //    };

        //    try
        //    {
        //        using (var cn = new OracleConnection(_connectionSting))
        //        {
        //            await cn.OpenAsync();

        //            using (var cmd = new OracleCommand("SP_DELETE_USER", cn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                // Parámetro de entrada para el ID del usuario
        //                cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = request.id;

        //                // Parámetro de salida para devolver el ID del usuario
        //                var outputParam = new OracleParameter("o_id", OracleDbType.Int32)
        //                {
        //                    Direction = ParameterDirection.Output
        //                };
        //                cmd.Parameters.Add(outputParam);

        //                // Ejecutar el procedimiento almacenado
        //                await cmd.ExecuteNonQueryAsync();

        //                // Leer el resultado del parámetro de salida
        //                int userId = Convert.ToInt32(((OracleDecimal)outputParam.Value).ToInt32());

        //                res.Data.Add(userId);
        //                res.IsSuccess = true;
        //                res.Message = "Usuario eliminado exitosamente";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.MessageException = ex.Message;
        //    }

        //    return res;
        //}
        public async Task<UserDetailResponsiveDto> GetUserByUsername(string username)
        {
            try
            {
                using (var cn = new OracleConnection(_connectionSting))
                {
                    await cn.OpenAsync();

                    string query = "SELECT id, username, password, role_id FROM tbl_users WHERE username = :username";

                    using (var cmd = new OracleCommand(query, cn))
                    {
                        cmd.Parameters.Add(new OracleParameter("username", username));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var user = new UserDetailResponsiveDto
                                {
                                    id = reader.GetInt32(0),
                                    username = reader.GetString(1),
                                    password = reader.GetString(2),
                                    role_id = reader.GetInt32(3)
                                };

                                return user;
                            }
                            else
                            {
                                throw new Exception("Usuario no encontrado.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el usuario: " + ex.Message);
            }
        }


        //public async Task<UserDetailResponsiveDto>GetUserByUsername(string username)
        //{
        //    DynamicParameters parameters = new DynamicParameters();
        //    parameters.Add("@p_username",username);
        //    using (var cn = new OracleConnection(_connectionSting))
        //    {
        //        var query = await cn.QueryAsync<UserDetailResponsiveDto>("SP_GET_USER_BY_USERNAME", commandType: System.Data.CommandType.StoredProcedure);
        //        if (query.Any())
        //        {
        //            return query.First();
        //        }
        //        throw new Exception("Usuario o contrseña incorrectos");
        //    }
        //}

        public async Task<UserDetailResponsiveDto> ValidateUser(LoginRequestDto request)
        {
            UserDetailResponsiveDto user = await GetUserByUsername(request.username);
            if (user.password == request.password)
            {
                return user;
            }
            throw new Exception("Usuario o contraseña incorrectos");
        }
        public async Task<TokenResponsiveDto> GenerateToken(UserDetailResponsiveDto request)
        {
            var key = configuration.GetSection("JWTSettings:Key").Value;
            var keyByte = Encoding.ASCII.GetBytes(key);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.id.ToString()));
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.username));
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.role_id.ToString()));

            var credentials = new SigningCredentials(new SymmetricSecurityKey(keyByte),SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = credentials,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(tokenConfig);
            return new TokenResponsiveDto { Token = token };
        }
        public async Task<AuthResponsiveDto> Login(LoginRequestDto request)
        {
            UserDetailResponsiveDto user = await ValidateUser(request);
            var token = await GenerateToken(user);
            return new AuthResponsiveDto { IsSuccess = true,User = user,Token=token.Token };
        }
    }
}
