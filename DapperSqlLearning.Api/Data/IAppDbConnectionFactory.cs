using System.Data.Common;

namespace DapperSqlLearning.Api.Data;

public interface IAppDbConnectionFactory
{
    string Provider { get; }
    DbConnection CreateConnection();
}
