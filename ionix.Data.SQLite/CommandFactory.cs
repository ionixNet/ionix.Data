namespace ionix.Data.SQLite
{
    using System;

    public class CommandFactory : CommandFactoryBase
    {
        public CommandFactory(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public override char ParameterPrefix => GlobalInternal.Prefix;

        public override IEntityCommandExecute CreateEntityCommand(EntityCommandType commandType)
        {
            switch (commandType)
            {
                case EntityCommandType.Update:
                    return new EntityCommandUpdate(base.DataAccess);
                case EntityCommandType.Insert:
                    return new EntityCommandInsert(base.DataAccess);
                case EntityCommandType.Upsert:
                    return new EntityCommandUpsert(base.DataAccess);
                case EntityCommandType.Delete:
                    return new EntityCommandDelete(base.DataAccess);
                default:
                    throw new NotSupportedException(commandType.ToString());
            }
        }
        public override IBatchCommandExecute CreateBatchCommand(BatchCommandType commandType)
        {
            switch (commandType)
            {
                case BatchCommandType.Update:
                    return new BatchCommandUpdate(base.DataAccess);
                case BatchCommandType.Insert:
                    return new BatchCommandInsert(base.DataAccess);
                case BatchCommandType.Upsert:
                    return new BatchCommandUpsert(base.DataAccess);
                case BatchCommandType.Delsert:
                    return new BatchCommandDelsert(base.DataAccess);
                default:
                    throw new NotSupportedException(commandType.ToString());
            }
        }
    }
}
