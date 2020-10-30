using System.Data;

namespace Coded.Core.Data
{
    /// <summary>
    ///     Creates connections
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        ///     Create a new connection
        /// </summary>
        /// <returns>The newly created connection</returns>
        IDbConnection CreateConnection();
    }
}