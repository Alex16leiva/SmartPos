
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Infraestructura.Core.Logging
{
    public class LogService : ILogService
    {
        private readonly string _connectionString;
        public LogService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("conectionDataBase");
        }

        public async Task LogErrorAsync(string modulo, string metodo, System.Exception ex, string usuario)
        {
            string sql = @"INSERT INTO LogsErrores (Usuario, Modulo, Metodo, Mensaje, StackTrace, InnerException, Maquina) 
                       VALUES (@Usuario, @Modulo, @Metodo, @Mensaje, @StackTrace, @Inner, @Maquina)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@Usuario", SqlDbType.VarChar) { Value = (object)usuario ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Modulo", SqlDbType.VarChar) { Value = modulo });
                    cmd.Parameters.Add(new SqlParameter("@Metodo", SqlDbType.VarChar) { Value = metodo });
                    cmd.Parameters.Add(new SqlParameter("@Mensaje", SqlDbType.VarChar) { Value = ex.Message });
                    cmd.Parameters.Add(new SqlParameter("@StackTrace", SqlDbType.VarChar) { Value = ex.StackTrace });
                    cmd.Parameters.Add(new SqlParameter("@Inner", SqlDbType.VarChar) { Value = (object)(ex.InnerException?.Message) ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@Maquina", SqlDbType.VarChar) { Value = Environment.MachineName });

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
