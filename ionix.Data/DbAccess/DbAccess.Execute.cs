namespace ionix.Data
{
    using System;
    using System.Data;
    using System.Data.Common;

    partial class DbAccess
    {
        private DbCommand CreateCommand(SqlQuery query, out OutParameters outParameters)
        {
            if (null == query)
                throw new ArgumentNullException(nameof(query));

            string commandText = query.Text.ToString();
            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentException("query, SqlQuery.Text is null");

            outParameters = null;

            this.OnPreExecuteSql(query);
            DbCommand cmd = null;
            try
            {
                cmd = this.connection.CreateCommand();

                foreach (SqlQueryParameter parameter in query.Parameters)
                {
                    DbParameter dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = parameter.ParameterName;

                    dbParameter.IsNullable = parameter.IsNullable;

                    if (parameter.dbType.HasValue)
                        dbParameter.DbType = parameter.dbType.Value;

                    object parameterValue = parameter.Value;
                    dbParameter.Value = (null == parameterValue) ? DBNull.Value : parameterValue;

                    ParameterDirection direction = parameter.Direction;
                    dbParameter.Direction = direction;

                    cmd.Parameters.Add(dbParameter);

                    if (direction == ParameterDirection.InputOutput || direction == ParameterDirection.Output)
                    {
                        if (null == outParameters)
                            outParameters = new OutParameters();
                        outParameters.Add(parameter, dbParameter);
                    }
                }

                cmd.CommandText = commandText;
                cmd.CommandType = query.CmdType;
            }
            catch (Exception)
            {
                if (null != cmd) cmd.Dispose();
                throw;
            }

            this.OnCommandCreated(cmd);

            return cmd;
        }

        protected virtual void OnCommandCreated(DbCommand cmd)
        {

        }

        public int ExecuteNonQuery(SqlQuery query)
        {
            DbCommand cmd = null;
            DbTransaction tran = null;
            OutParameters outParameters;
            int ret = 0;
            DateTime executionStart = DateTime.Now;
            try
            {
                cmd = this.CreateCommand(query, out outParameters);

                if (this.enableTransaction)
                {
                    tran = this.connection.BeginTransaction();
                    cmd.Transaction = tran;
                }

                ret = cmd.ExecuteNonQuery();

                if (this.enableTransaction)
                    tran.Commit();
            }
            catch (Exception ex)
            {
                if (this.enableTransaction && tran != null)
                    tran.Rollback();

                this.OnExecuteSqlComplete(query, executionStart, ex);

                throw;
            }
            finally
            {
                if (this.enableTransaction) tran.Dispose();
                if (null != cmd) cmd.Dispose();
            }

            if (null != outParameters)
            {
                outParameters.SetOutParametersValues();
            }

            this.OnExecuteSqlComplete(query, executionStart, null);

            return ret;
        }

        public object ExecuteScalar(SqlQuery query)
        {
            DbCommand cmd = null;
            DbTransaction tran = null;
            OutParameters outParameters;
            object ret = null;
            DateTime executionStart = DateTime.Now;
            try
            {
                cmd = this.CreateCommand(query, out outParameters);

                if (this.enableTransaction)
                {
                    tran = this.connection.BeginTransaction();
                    cmd.Transaction = tran;
                }

                ret = cmd.ExecuteScalar();

                if (this.enableTransaction)
                    tran.Commit();
            }
            catch (Exception ex)
            {
                if (this.enableTransaction && tran != null)
                    tran.Rollback();

                this.OnExecuteSqlComplete(query, executionStart, ex);

                throw;
            }
            finally
            {
                if (this.enableTransaction) tran.Dispose();
                if (cmd != null) cmd.Dispose();
            }

            if (null != outParameters)
            {
                outParameters.SetOutParametersValues();
            }

            this.OnExecuteSqlComplete(query, executionStart, null);

            return ret;
        }

        public DbDataReader CreateDataReader(SqlQuery query, CommandBehavior behavior)
        {
            DbCommand cmd = null;
            DbDataReader dr = null;
            OutParameters outParameters;
            DateTime executionStart = DateTime.Now;
            try
            {
                cmd = this.CreateCommand(query, out outParameters);
                dr = cmd.ExecuteReader(behavior);
            }
            catch (Exception ex)
            {
                if (dr != null) dr.Dispose();

                this.OnExecuteSqlComplete(query, executionStart, ex);

                throw;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
            }

            if (null != outParameters)
            {
                outParameters.SetOutParametersValues();
            }

            this.OnExecuteSqlComplete(query, executionStart, null);

            return dr;
        }

        IDataReader IDbAccess.CreateDataReader(SqlQuery query, CommandBehavior behavior)
        {
            return this.CreateDataReader(query, behavior);
        }
    }
}
