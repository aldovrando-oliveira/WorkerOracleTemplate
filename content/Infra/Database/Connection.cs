using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace WorkerOracleTemplate.Infra.Database
{
    public sealed class Connection : IDisposable
    {
        private bool _disposed;
        private readonly string _connectionString;
        private OracleConnection _connection;
        private readonly ILogger<Connection> _logger;
        private readonly object _lockObject = new Object();
        private IDbTransaction _transaction = null;
        private string _callerMemberTransaction;

        public OracleConnection Database
        {
            get
            {
                lock (_lockObject)
                {
                    return _connection;
                }
            }
            private set => _connection = value;
        }

        /// <summary>
        /// Método construtor. Retorna uma nova instância da classe <see cref="Connection" />
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="options">Objeto com as parametrizações para o banco de dados</param>
        public Connection(ILogger<Connection> logger, IOptions<ConnectionOptions> options) : this(logger, options.Value)
        {
        }

        /// <summary>
        /// Método construtor. Retorna uma nova instância da classe <see cref="Connection" />
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="options">Objeto com as parametrizações para o banco de dados</param>
        public Connection (ILogger<Connection> logger, ConnectionOptions options)
        {
            var strConnectionBuilder = new OracleConnectionStringBuilder();
            strConnectionBuilder.DataSource = options.DataSource;
            strConnectionBuilder.UserID = options.Username;
            strConnectionBuilder.Password = options.Password;

            if (options.Pooling?.Pooling == true)
            {
                strConnectionBuilder.Pooling = true;
                strConnectionBuilder.MaxPoolSize = options.Pooling.MaxSize;
                strConnectionBuilder.MinPoolSize = options.Pooling.MinSize;
            }

            _connectionString = strConnectionBuilder.ToString();
            _logger = logger;
        }

        /// <summary>
        /// Executa consulta no banco de dados de forma assincrona
        /// </summary>
        /// <param name="query">Consulta SQL que será executada</param>
        /// <param name="parameters">Objeto contendo os parâmetros a serem utilizadas na consulta</param>
        /// <returns></returns>
        public async Task<ICollection<T>> ExecQueryAsync<T>(string query, object parameters)
        {
            IEnumerable<T> result;
            await OpenConnectionAsync();

            var watch = new Stopwatch();
            watch.Start();

            _logger.LogDebug("Oracle Connection - Executing query: {0} | Params: {1}", query, parameters);

            try
            {
                result = await _connection.QueryAsync<T>(query, parameters, _transaction, 30);
                watch.Stop();
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError(new EventId(), ex, "Oracle Connection - Unexpected error while trying execute query");
                throw;
            }

            _logger.LogDebug("Oracle Connection - Executed success - Time elapsed: {0}", watch.ElapsedMilliseconds);

            await CloseConnectionAsync();

            return result.ToList();
        }

        /// <summary>
        /// Executa consulta no banco de dados de forma assincrona
        /// </summary>
        /// <param name="query">Consulta SQL que será executada</param>
        public Task<ICollection<T>> ExecQueryAsync<T>(string query)
        {
            return this.ExecQueryAsync<T>(query, null);
        }

        /// <summary>
        /// Executa comando no banco de dados de forma assincrona
        /// </summary>
        /// <param name="query">Comando SQL que será executado</param>
        /// <param name="parameters">Objeto contendo os parâmetros para a execução do comando</param>
        /// <returns></returns>
        public async Task<int> ExecCommandAsync(string query, object parameters)
        {
            int result;
            await OpenConnectionAsync();

            var watch = new Stopwatch();
            watch.Start();

            _logger.LogDebug("Oracle Connection - Executing command: {0} | Params: {1}", query, parameters);
            try
            {
                result = await _connection.ExecuteAsync(query, parameters, _transaction, commandTimeout: 30);
                watch.Stop();
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError(new EventId(), ex, "Oracle Connection - Unexpected error while trying execute command");
                throw;
            }

            _logger.LogDebug("Oracle Connection - Command executed - Time elapsed: {0}", watch.ElapsedMilliseconds);
            await CloseConnectionAsync();

            return result;
        }

        /// <summary>
        /// Abre a conexão com o banco de dados
        /// </summary>
        /// <remarks>
        /// <para>
        /// Só é aberta uma nova conexão caso não haja conexão. 
        /// </para>
        /// <para>
        /// Caso a conexão co o banco esteja quebrada, a conexão é fechada e depois reaberta
        /// </para>
        /// </remarks>
        /// <returns></returns>
        public async Task OpenConnectionAsync()
        {
            if (Database == null)
            {
                Database = new OracleConnection { ConnectionString = _connectionString };
            }

            if (Database.State == ConnectionState.Broken)
            {
                _logger.LogDebug("Oracle Connection - Connection broken, closing and reoping");
                Database.Close();
                await Database.OpenAsync();
                _logger.LogDebug("Oracle Connection - Connetion open");
            }
            else if (Database.State == ConnectionState.Closed)
            {
                await Database.OpenAsync();
                _logger.LogDebug("Oracle Connection - Connetion open");
            }
        }

        /// <summary>
        /// Fecha a conexão com o banco de dados
        /// </summary>
        /// <remarks>
        /// <para>
        /// Fecha a conexão com o bancode dados.
        /// </para>
        /// <para>
        /// Caso haja transação aberta a conexão não é fechada até que seja feito rollback ou o commit
        /// </para>
        /// </remarks>
        /// <returns></returns>
        public async Task CloseConnectionAsync()
        {
            try
            {
                if (_transaction == null && _connection != null)
                {
                    try
                    {
                        _connection.Close();
                    }
                    catch
                    {
                        // ignored
                    }

                    try
                    {
                        _connection.Dispose();
                    }
                    catch
                    {
                        // ignored
                    }

                    _connection = null;
                    _logger.LogDebug("Oracle Connection - Connection closed");
                }
            }
            catch
            {
                await this.Rollback();
            }
        }

        /// <summary>
        /// Abre uma nova transação com o banco de dados
        /// </summary>
        /// <remarks>
        /// <para>
        /// Abre uma nova transação com o banco, Se necessário abre a conexão com o banco.
        /// </para>
        /// <para>
        /// Caso esse método seja chamado já existindo uma transação, é utilizada a transação atual, sem retornar uma exceção
        /// </para>
        /// <para>
        /// Para controle interno é necessário informar o método que está abrindo a transação. Será aceito o commit que for realiado pelo mesmo método
        /// </para>
        /// </remarks>
        /// <param name="callerMember">Método que está tentando abrir a transação</param>
        /// <returns></returns>
        public async Task BeginTransaction([CallerMemberName]string callerMember = "")
        {
            if (_transaction == null)
            {
                await OpenConnectionAsync();
                _transaction = _connection.BeginTransaction();
                _callerMemberTransaction = callerMember;
                _logger.LogDebug("Oracle Connection - Begin Transaction");
            }
        }

        /// <summary>
        /// Consolida as atualizações no banco de dados
        /// </summary>
        /// <param name="callerMember">Método que está tentando consolidar as informações</param>
        /// <returns></returns>
        public async Task Commit([CallerMemberName]string callerMember = "")
        {
            if (_transaction != null && _callerMemberTransaction == callerMember)
            {
                try
                {
                    _transaction.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(new EventId(), ex, "Oracle Connection - Error while trying commit ({0})", ex.Message);
                    throw;
                }
                finally
                {
                    try
                    {
                        _transaction.Dispose();
                    }
                    catch
                    {
                        // ignored
                    }

                    _transaction = null;
                    _callerMemberTransaction = string.Empty;
                    _logger.LogDebug("Oracle Connection - Commit transaction");
                    await CloseConnectionAsync();
                }

            }
        }

        /// <summary>
        /// Cancela as atualizações feitas no banco de dados
        /// </summary>
        /// <returns></returns>
        public async Task Rollback()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Rollback();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(new EventId(), ex, "Oracle Connection - Error while trying rollback ({0})", ex.Message);
                    throw;
                }
                finally
                {
                    try
                    {
                        _transaction.Dispose();
                    }
                    catch
                    {
                        // ignored
                    }

                    _transaction = null;
                    _callerMemberTransaction = string.Empty;
                    _logger.LogDebug("Oracle Connection - Rollback transaction");
                    await CloseConnectionAsync();
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            if (_transaction != null)
                Rollback().Wait();
            else
                CloseConnectionAsync().Wait();

            _disposed = true;
        }
    }
}