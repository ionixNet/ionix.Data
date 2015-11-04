namespace ionix.Data
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public enum ExecuteCommandType : int
    {
        Update = 0,
        Insert,
        Upsert,
        Delete,
        Delsert
    }

    public class PreExecuteCommandEventArgs<TEntity> : CancelEventArgs
    {
        internal PreExecuteCommandEventArgs(IEnumerable<TEntity> entityList, ExecuteCommandType commandType)
        {
            this.EntityList = entityList;
            this.CommandType = commandType;
        }

        public IEnumerable<TEntity> EntityList { get; }

        public ExecuteCommandType CommandType { get; }
    }

    public sealed class ExecuteCommandCompleteEventArgs<TEntity> : EventArgs
    {
        internal ExecuteCommandCompleteEventArgs(IEnumerable<TEntity> entityList, ExecuteCommandType commandType, Exception commandException)
        {
            this.EntityList = entityList;
            this.CommandType = commandType;
            this.CommandException = commandException;
        }


        public IEnumerable<TEntity> EntityList { get; }

        public ExecuteCommandType CommandType { get; }

        public Exception CommandException { get; }

        public bool Succeeded => this.CommandException == null;
    }


    internal static class RespositoryEventsKeys
    {
        internal static readonly object PreExecuteCommandEvent = new object();
        internal static readonly object ExecuteCommandCompletedEvent = new object();
    }

    partial class Repository<TEntity>
    {
        public event EventHandler<PreExecuteCommandEventArgs<TEntity>> PreExecuteCommand
        {
            add
            {
                lock (this)
                {
                    this.events.AddHandler(RespositoryEventsKeys.PreExecuteCommandEvent, value);
                }
            }
            remove
            {
                lock (this)
                {
                    this.events.RemoveHandler(RespositoryEventsKeys.PreExecuteCommandEvent, value);
                }
            }
        }
        private bool OnPreExecuteCommand(IEnumerable<TEntity> entityList, ExecuteCommandType commandType)
        {
            bool cancel = false;
            EventHandler<PreExecuteCommandEventArgs<TEntity>> fPtr = (EventHandler<PreExecuteCommandEventArgs<TEntity>>)this.events[RespositoryEventsKeys.PreExecuteCommandEvent];
            if (fPtr != null)
            {
                PreExecuteCommandEventArgs<TEntity> e = new PreExecuteCommandEventArgs<TEntity>(entityList, commandType);
                fPtr(this, e);
                cancel = e.Cancel;
            }
            return cancel;
        }

        public event EventHandler<ExecuteCommandCompleteEventArgs<TEntity>> ExecuteCommandCompleted
        {
            add
            {
                lock (this)
                {
                    this.events.AddHandler(RespositoryEventsKeys.ExecuteCommandCompletedEvent, value);
                }
            }
            remove
            {
                lock (this)
                {
                    this.events.RemoveHandler(RespositoryEventsKeys.ExecuteCommandCompletedEvent, value);
                }
            }
        }
        private void OnExecuteCommandComplete(IEnumerable<TEntity> entityList, ExecuteCommandType commandType, Exception commandException)
        {
            EventHandler<ExecuteCommandCompleteEventArgs<TEntity>> fPtr = (EventHandler<ExecuteCommandCompleteEventArgs<TEntity>>)this.events[RespositoryEventsKeys.ExecuteCommandCompletedEvent];
            if (fPtr != null)
            {
                ExecuteCommandCompleteEventArgs<TEntity> e = new ExecuteCommandCompleteEventArgs<TEntity>(entityList, commandType, commandException);
                fPtr(this, e);
            }
        }

        private sealed class CommandScope : IDisposable
        {
            private readonly Repository<TEntity> parent;
            private readonly IEnumerable<TEntity> entityList;
            private readonly ExecuteCommandType commandType;
            private Exception commandException;
            private readonly bool isEmptyList;

            internal CommandScope(Repository<TEntity> parent, IEnumerable<TEntity> entityList, ExecuteCommandType commandType)
            {
                this.parent = parent;
                this.entityList = entityList;
                this.commandType = commandType;
                this.isEmptyList = entityList.IsEmptyList();
            }

            internal CommandScope(Repository<TEntity> parent, TEntity entity, ExecuteCommandType commandType)
                : this(parent, entity.ToSingleItemList(), commandType)
            {

            }

            internal int Execute(Func<int> func)
            {
                int returnValue = 0;
                if (!this.isEmptyList && !this.parent.OnPreExecuteCommand(this.entityList, this.commandType))
                {
                    try
                    {
                        returnValue = func();
                    }
                    catch (Exception ex)
                    {
                        this.commandException = ex;
                        throw;
                    }
                }
                return returnValue;
            }

            public void Dispose()
            {
                if (!this.isEmptyList)
                    this.parent.OnExecuteCommandComplete(this.entityList, this.commandType, this.commandException);
            }
        }
    }
}
