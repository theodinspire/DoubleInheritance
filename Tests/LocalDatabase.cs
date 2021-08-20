using System;
using System.Data;
using System.Data.SqlClient;

namespace Tests
{
	public class LocalDatabase : IDisposable
	{
		private const string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;";

		private readonly string databaseName;

		private readonly Lazy<SqlConnection> lazyConnection;

		public LocalDatabase(string databaseName)
		{
			this.databaseName = databaseName;

			this.CreateDatabase();
			this.lazyConnection = new Lazy<SqlConnection>(this.GetConnection);
		}

		public SqlConnection Connection => this.lazyConnection.Value;

		public void Dispose()
		{
			if (this.lazyConnection.IsValueCreated)
			{
				this.Connection.Dispose();
			}

			DropDatabase(this.databaseName);
		}

		private SqlConnection GetConnection()
		{
			var sqlConnection = new SqlConnection(ConnectionString);
			try
			{
				sqlConnection.Open();
				sqlConnection.ChangeDatabase(this.databaseName);
				return sqlConnection;
			}
			catch (Exception)
			{
				sqlConnection.Dispose();
				throw;
			}
		}

		private void CreateDatabase()
		{
			using (var connection = new SqlConnection(ConnectionString))
			{
				connection.Open();

				using (var createDbCommand = connection.CreateCommand())
				{
					createDbCommand.CommandText = $"CREATE DATABASE {this.databaseName}";
					createDbCommand.CommandType = CommandType.Text;
					createDbCommand.ExecuteScalar();
				}

				connection.ChangeDatabase(this.databaseName);
			}
		}

		private static void DropDatabase(string databaseName)
		{
			using (var connection = new SqlConnection(ConnectionString))
			{
				connection.Open();

				using (var createDbCommand = connection.CreateCommand())
				{
					createDbCommand.CommandText = string.Format(
						@"IF (SELECT DB_ID('{0}')) IS NOT NULL
						BEGIN
							-- Kill all active connections to the DB
							DECLARE @Kill NVARCHAR(4000) = '';

							SELECT @Kill = @Kill + 'kill ' + CONVERT(VARCHAR(5), session_id) + ';'
							FROM sys.dm_exec_sessions
							WHERE database_id = DB_ID('{0}');

							EXEC sp_executesql @Kill;

							DROP DATABASE [{0}]
						end",
						databaseName);

					createDbCommand.CommandType = CommandType.Text;
					createDbCommand.ExecuteScalar();
				}
			}
		}
	}
}
