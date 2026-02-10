using Npgsql;
using System.Collections.Generic;
using System.Xml.Linq;
namespace GymBot.Data
{
    public static class DbConfig
    {
        public static string ConnectionString =
       "Host=localhost;Port=5432;Username=gymbotuser;Password=gymbotpass;Database=gymbot_db";
    }
    public class UserRepository
    {
        public async Task<bool> AddUserIfNotExist(long tgId, string username)
        {
            await using var conn=new NpgsqlConnection(DbConfig.ConnectionString);
            await conn.OpenAsync();
            var sql= """
                  INSERT INTO users (tgid, username)
                  VALUES (@id, @name)
                  ON CONFLICT (tgid) DO NOTHING
                  """;
            await using var cmd= new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", tgId);
            cmd.Parameters.AddWithValue("name", username ?? "");
            int changed=await cmd.ExecuteNonQueryAsync();
            return changed > 0;
        }
    }
}
