using Dapper;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zhengwei.Project.Infrastructure;

namespace Zhengwei.Project.Api.Applications.Queries
{
    public class ProjectQueries : IProjectQueries
    {
       
        public ProjectContext _context;
        public ProjectQueries(ProjectContext context)
        {
       
            _context = context;
        }
        public async Task<dynamic> GetProjectByUserId(int userId)
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                string sql = "";
                conn.Open();
                var result = await conn.QueryAsync<dynamic>(sql);
                return result;
            }

        }

        public Task<dynamic> GetProjectDetail(int projectId)
        {
            throw new NotImplementedException();
        }
    }
}
