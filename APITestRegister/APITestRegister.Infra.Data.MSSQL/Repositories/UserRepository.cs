using APITestRegister.Domain.Domain.Interfaces.Repositories;
using APITestRegister.Domain.Domain.Models;
using System.Linq;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace APITestRegister.Infra.Data.MYSQL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ConnectionStrings conStrings;
        public UserRepository(ConnectionStrings c)
        {
            conStrings = c;
        }

        public bool Exists(string email)
        {
            using (var connection = new SqlConnection(conStrings.DefaultConnection))
            {
                string _sql = "SELECT * FROM dbo.users WHERE UPPER(email) = UPPER('{0}')";
                string sql = string.Format(_sql, email);
                var exists = connection.Query<User>(sql).Any();
                return exists;
            }
        }

        public User Register(User vm)
        {
            using (var connection = new SqlConnection(conStrings.DefaultConnection))
            {
                string sql =    @"INSERT INTO dbo.users (name, email, password)
                                VALUES (@name, @email, @password);
                                SELECT CAST(SCOPE_IDENTITY() as int)";

                vm.idUser = connection.Query<int>(sql, vm).Single();
                return vm;
            }
        }

        public User Login(User vm)
        {
            using (var connection = new SqlConnection(conStrings.DefaultConnection))
            {
                string sql = "dbo.sp_Login";

                DynamicParameters dp = new DynamicParameters();
                dp.Add("@pEmail", vm.email, DbType.AnsiString);
                dp.Add("@pPassword", vm.password, DbType.AnsiString);

                var user = connection.Query<User>(sql, commandType: CommandType.StoredProcedure, param: dp).FirstOrDefault();
                return user;
            }
        }
    }
}
