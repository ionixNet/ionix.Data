namespace ionix.Data.MongoDB.Migration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using global::MongoDB.Bson.Serialization;
	using global::MongoDB.Driver;

    public class MigrationRunner
    {
        static MigrationRunner()
        {
            Init();
        }

        public static void Init()
        {
            BsonSerializer.RegisterSerializer(typeof(MigrationVersion), new MigrationVersionSerializer());
        }

        private readonly string connectionString;
        public MigrationRunner(string connectionString, string databaseName)
            : this(new MongoClient(connectionString).GetDatabase(databaseName))
        {
            this.connectionString = connectionString;
        }
        public MigrationRunner()
        {

        }

        public MigrationRunner(IMongoDatabase database)
        {
            Database = database;
            DatabaseStatus = new DatabaseMigrationStatus(this);
            MigrationLocator = new MigrationLocator();
        }

        public IMongoDatabase Database { get; set; }
        public MigrationLocator MigrationLocator { get; set; }
        public DatabaseMigrationStatus DatabaseStatus { get; set; }

        public virtual void UpdateToLatest()
        {
            Console.WriteLine(WhatWeAreUpdating() + " to latest...");
            UpdateTo(MigrationLocator.LatestVersion());
        }

        private string WhatWeAreUpdating()
        {
            return string.Format("Updating server(s) \"{0}\" for database \"{1}\"", ServerAddresses(), Database.DatabaseNamespace.DatabaseName);
        }

        private string ServerAddresses()
        {
            MongoUrl u = new MongoUrl(this.connectionString);
            return u.Server.Host;
        }

        protected virtual void ApplyMigrations(IEnumerable<Migration> migrations)
        {
            migrations.ToList()
                .ForEach(ApplyMigration);
        }


        protected virtual void ApplyMigration(Migration migration)
        {
            Console.WriteLine(new { Message = "Applying migration", migration.Version, migration.Description, DatabaseName = Database.DatabaseNamespace.DatabaseName });

            var appliedMigration = DatabaseStatus.StartMigration(migration);
            migration.Database = Database;
            try
            {
                migration.Update();
            }
            catch (Exception exception)
            {
                appliedMigration.Exception = exception.ToString();

                try
                {
                    DatabaseStatus.FindOneAndReplace(appliedMigration);
                }
                catch (Exception exn)
                {
                    Console.WriteLine(exn);
                }

                OnMigrationException(migration, exception);

            }
            DatabaseStatus.CompleteMigration(appliedMigration);
        }

        protected virtual void OnMigrationException(Migration migration, Exception exception)
        {
            var message = new
            {
                Message = "Migration failed to be applied: " + exception.Message,
                migration.Version,
                Name = migration.GetType(),
                migration.Description,
                DatabaseName = Database.DatabaseNamespace.DatabaseName
            };
            Console.WriteLine(message);
            throw new MigrationException(message.ToString(), exception);
        }

        public virtual void UpdateTo(MigrationVersion updateToVersion)
        {
            var currentVersion = DatabaseStatus.GetLastAppliedMigration();
            Console.WriteLine(new
            {
                Message = WhatWeAreUpdating(),
                currentVersion,
                updateToVersion
                ,
                DatabaseName = Database.DatabaseNamespace.DatabaseName
            });

            var migrations = MigrationLocator.GetMigrationsAfter(currentVersion)
                .Where(m => m.Version <= updateToVersion);

            ApplyMigrations(migrations);
        }
    }
}