namespace ionix.Data.MongoDB.Migration
{
    using global::MongoDB.Driver;

    public abstract class Migration
    {
        public MigrationVersion Version { get; protected set; }
        public string Description { get; set; }

        //public string Script { get; set; }

        protected Migration(MigrationVersion version)
        {
            Version = version;
        }

        public IMongoDatabase Database { get; set; }

        public virtual string Script { get; }

        public abstract void Update();
    }
}