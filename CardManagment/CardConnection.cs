using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace CardManagment
{
    public class CardConnection : ICardConnection
    {
        private string? _serverName;
        private string? _databaseName;
        private string? _masterConnection;
        private string? _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardConnection"/> class with the specified server name and database name.
        /// </summary>
        /// <param name="serverName">The name of the server to connect to. Defaults to "localhost" if not provided.</param>
        /// <param name="databaseName">The name of the database to connect to. Defaults to "CardsManagement" if not provided.</param>
        /// <remarks>
        /// This constructor initializes the connection to a database using the provided server name and database name.
        /// If no database name is provided, it defaults to "CardsManagement".
        /// If no server name is provided, it defaults to "localhost".
        /// After setting the server and database names, the constructor calls the <see cref="InitDbConnection"/> method to initialize the database connection.
        /// </remarks>
        public CardConnection(string serverName = "localhost", string? databaseName = null)
        {
            this._databaseName = databaseName ?? "CardsManagement";
            this._serverName = serverName ?? "localhost";
            InitDbConnection();
        }

        private void InitDbConnection()
        {
            this._masterConnection = $"Server={_serverName};Integrated Security=True;TrustServerCertificate=True";
            this._connection = $"Server={_serverName};Database={_databaseName};Integrated Security=True;TrustServerCertificate=True";
        }

        /// <summary>
        /// Executes a SQL query on the database and returns the number of affected rows.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="connectionString">The connection string to use for the database connection. If not provided, the default connection string is used.</param>
        /// <returns>
        /// The number of rows affected by the query. If a unique key violation occurs (SQL error code 2627), the method returns 0.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if an error occurs during query execution, excluding unique key violations.
        /// </exception>
        /// <remarks>
        /// This method uses the provided connection string or the default connection string to establish a database connection.
        /// If a unique key violation occurs (SQL error code 2627), the method catches the exception and returns 0.
        /// For all other SQL exceptions, the method throws a new <see cref="Exception"/> with the error message.
        /// </remarks>
        public int ExecuteQuery(string query, string? connectionString = null)
        {
            string? finalConnectionString = connectionString ?? _connection;

            using (var connection = new SqlConnection(finalConnectionString))
            {
                try
                {
                    return connection.Execute(query);
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) return 0; // 2627 = unique key violation code
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// Executes a SQL query on the database and returns a single <see cref="Card"/> object as the result.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="connectionString">The connection string to use for the database connection. If not provided, the default connection string is used.</param>
        /// <returns>
        /// A <see cref="Card"/> object representing the first result of the query. If no results are found, an empty <see cref="Card"/> object is returned.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if an error occurs during query execution.
        /// </exception>
        /// <remarks>
        /// This method uses the provided connection string or the default connection string to establish a database connection.
        /// It executes the query and maps the first result to a <see cref="Card"/> object.
        /// If the query returns no results, an empty <see cref="Card"/> object is returned.
        /// If an exception occurs during execution, it is caught and rethrown as a new <see cref="Exception"/> with the error message.
        /// </remarks>
        public Card SelectQuery(string query, string? connectionString = null)
        {
            string? finalConnectionString = connectionString ?? _connection;
            using (var connection = new SqlConnection(finalConnectionString))
            {
                try
                {
                    var queryResult = connection.Query<Card>(query);
                    Card result = new Card();
                    //for getting multiple results if expecting more than one
                    //foreach(var r in queryResult)
                    //{
                    //    result.OwnerId = r.OwnerId;
                    //    result.Pin = r.Pin;
                    //    result.CardSerialNumber = r.CardSerialNumber;
                    //    result.CardId = r.CardId;
                    //}
                    if (queryResult.IsNullOrEmpty()) return new Card();
                    var singleResult = queryResult.First();
                    result.OwnerId = singleResult.OwnerId;
                    result.Pin = singleResult.Pin;
                    result.CardSerialNumber = singleResult.CardSerialNumber;
                    result.CardId = singleResult.CardId;
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public void DevInitDatabase()
        {
            string createDatabaseQuery = $@"
            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{_databaseName}')
            BEGIN
                CREATE DATABASE {_databaseName};
            END";
            ExecuteQuery(createDatabaseQuery, _masterConnection);

            string createTableQuery = $@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{_databaseName}' AND xtype='U')
            BEGIN
                CREATE TABLE Cards (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    OwnerId VARCHAR(34) NOT NULL UNIQUE,
                    Pin CHAR(4) NOT NULL DEFAULT '0000',
                    CardSerialNumber VARCHAR(19) NOT NULL UNIQUE,
                    CardId VARCHAR(32) NOT NULL ,
                );
            END";
            ExecuteQuery(createTableQuery);
        }
    }
}