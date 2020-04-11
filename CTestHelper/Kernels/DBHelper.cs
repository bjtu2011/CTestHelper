/************************************************************************
* Copyright (c) 2020 All Rights Reserved.
*命名空间：CTestHelper.Kernel
*文件名： DBHelper
*创建人： XXX
*创建时间：2020/4/10 19:56:57
*描述
*=======================================================================
*修改标记
*修改时间：2020/4/10 19:56:57
*修改人：XXX
*描述：
************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTestHelper.Kernels
{
    public class DBHelper
    {
        private DbProviderFactory _provider;

        private DbConnection _connection;

        private string _server;

        public DbConnection Connection
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        public string Server
        {
            get
            {
                return _server;
            }
            set
            {
                _server = value;
            }
        }

        private string getDBProviderName(string dbType)
        {
            string result = "";
            switch (dbType.ToLower())
            {
                case "sqlserver":
                    result = "System.Data.SqlClient";
                    break;
                case "mysql":
                    result = "MySql.Data.MySqlClient";
                    break;
                case "oracle":
                    result = "Oracle.DataAccess.Client";
                    break;
                case "*.mdb":
                    result = "System.Data.OleDb";
                    break;
            }
            return result;
        }

        private string getConnectionString(string dbType, string server, string user, string pwd)
        {
            string[] array = server.Split(new string[2]
            {
                ":",
                "/"
            }, StringSplitOptions.RemoveEmptyEntries);
            string text = "Data Source={0};Database={1};User ID={2};Password={3}";
            switch (dbType.ToLower())
            {
                case "sqlserver":
                    text = $"server={array[0]};port={array[1]};database={array[2]};user id={user};password={pwd};";
                    break;
                case "mysql":
                    text = $"server={array[0]};port={array[1]};database={array[2]};user id={user};password={pwd};";
                    break;
                case "oracle":
                    text = "";
                    break;
                case "*.mdb":
                    text = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={server}; Jet OLEDB:Database Password={pwd};";
                    break;
            }
            string format = text;
            object[] args = array;
            return string.Format(format, args);
        }

        internal void close()
        {
            _connection.Close();
        }

        public DbConnection CreateConnection(string dbType, string server, string user, string pwd)
        {
            try { 
            _server = server;
            _provider = DbProviderFactories.GetFactory(getDBProviderName(dbType));
            _connection = _provider.CreateConnection();
            _connection.ConnectionString = getConnectionString(dbType, server, user, pwd);
            _connection.Open();
            }
            catch(Exception e)
            {
                Console.Write(e.ToString());
            }
            return _connection;

        }

        public DbCommand GetStoredProcCommond(string storedProcedure)
        {
            DbCommand dbCommand = _connection.CreateCommand();
            dbCommand.CommandText = storedProcedure;
            dbCommand.CommandType = CommandType.StoredProcedure;
            return dbCommand;
        }

        public DbCommand GetSqlStringCommond(string sqlQuery)
        {
            DbCommand dbCommand = _connection.CreateCommand();
            dbCommand.CommandText = sqlQuery;
            dbCommand.CommandType = CommandType.Text;
            return dbCommand;
        }

        public void AddParameterCollection(DbCommand cmd, DbParameterCollection dbParameterCollection)
        {
            foreach (DbParameter item in dbParameterCollection)
            {
                cmd.Parameters.Add(item);
            }
        }

        public void AddOutParameter(DbCommand cmd, string parameterName, DbType dbType, int size)
        {
            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Size = size;
            dbParameter.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(dbParameter);
        }

        public void AddInParameter(DbCommand cmd, string parameterName, DbType dbType, object value)
        {
            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(dbParameter);
        }

        public void AddReturnParameter(DbCommand cmd, string parameterName, DbType dbType)
        {
            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(dbParameter);
        }

        public DbParameter GetParameter(DbCommand cmd, string parameterName)
        {
            return cmd.Parameters[parameterName];
        }

        public DataSet ExecuteDataSet(DbCommand cmd)
        {
            DbDataAdapter dbDataAdapter = _provider.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            DataSet dataSet = new DataSet();
            dbDataAdapter.Fill(dataSet);
            return dataSet;
        }

        public DataTable ExecuteDataTable(DbCommand cmd)
        {
            DbDataAdapter dbDataAdapter = _provider.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            DataTable dataTable = new DataTable();
            dbDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public DbDataReader ExecuteReader(DbCommand cmd)
        {
            cmd.Connection.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public int ExecuteNonQuery(DbCommand cmd)
        {
            cmd.Connection.Open();
            int result = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return result;
        }

        public int ExecuteNonQuery(string sql)
        {
            DbCommand sqlStringCommond = GetSqlStringCommond(sql);
            sqlStringCommond.Connection.Open();
            int result = sqlStringCommond.ExecuteNonQuery();
            sqlStringCommond.Connection.Close();
            return result;
        }

        public object ExecuteScalar(DbCommand cmd)
        {
            cmd.Connection.Open();
            object result = cmd.ExecuteScalar();
            cmd.Connection.Close();
            return result;
        }
        
        public DataSet ExecuteDataSet(string sql)
        {
            DbCommand sqlStringCommond = GetSqlStringCommond(sql);
            return ExecuteDataSet(sqlStringCommond);
        }

        public DataTable ExecuteDataTable(string sql)
        {
            DbCommand sqlStringCommond = GetSqlStringCommond(sql);
            return ExecuteDataTable(sqlStringCommond);
        }
    }
}
