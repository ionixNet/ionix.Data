namespace ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;

    public partial class DbAccess : IDbAccess
    {
        private DbConnection connection;
        private readonly EventHandlerList events;
        private bool enableTransaction;

        public DbAccess(DbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (connection.State != ConnectionState.Open)
                throw new ArgumentException("'connection' is not open.");

            this.connection = connection;
            this.events = new EventHandlerList();
        }
        public DbConnection Connection
        {
            get { return this.connection; }
        }

        public bool EnableTransaction
        {
            get { return this.enableTransaction; }
            set { this.enableTransaction = value; }
        }

        public virtual void Dispose()
        {
            if (null != this.connection)
            {
                this.connection.Dispose();
                this.connection = null;
            }
            if (null != this.events)
                this.events.Dispose();
        }

        private sealed class OutParameters : Dictionary<SqlQueryParameter, DbParameter>
        {
            internal void SetOutParametersValues()
            {
                foreach (KeyValuePair<SqlQueryParameter, DbParameter> kvp in this)
                    kvp.Key.Value = kvp.Value.Value;
            }
        }
    }
}
