namespace ionix.Data.MongoDB.Migration
{
	using System;
	using System.Linq;
	using global::MongoDB.Driver;

    public class DatabaseMigrationStatus
	{
		private readonly MigrationRunner _Runner;

		public string VersionCollectionName = "DatabaseVersion";

		public DatabaseMigrationStatus(MigrationRunner runner)
		{
			_Runner = runner;
		}

		public virtual IMongoCollection<AppliedMigration> GetMigrationsApplied()
		{
			return _Runner.Database.GetCollection<AppliedMigration>(VersionCollectionName);
		}

		public virtual bool IsNotLatestVersion()
		{
			return _Runner.MigrationLocator.LatestVersion()
			       != GetVersion();
		}

		public virtual void ThrowIfNotLatestVersion()
		{
			if (!IsNotLatestVersion())
			{
				return;
			}
			var databaseVersion = GetVersion();
			var migrationVersion = _Runner.MigrationLocator.LatestVersion();
			throw new Exception("Database is not the expected version, database is at version: " + databaseVersion + ", migrations are at version: " + migrationVersion);
		}

		public virtual MigrationVersion GetVersion()
		{
			var lastAppliedMigration = GetLastAppliedMigration();
			return lastAppliedMigration == null
			       	? MigrationVersion.Default()
			       	: lastAppliedMigration.Version;
		}

		public virtual AppliedMigration GetLastAppliedMigration()
		{
			return GetMigrationsApplied().AsQueryable() // in memory but this will never get big enough to matter
				.OrderByDescending(v => v.Version)
				.FirstOrDefault();
		}

		public virtual AppliedMigration StartMigration(Migration migration)
		{
			var appliedMigration = new AppliedMigration(migration);
			GetMigrationsApplied().InsertOne(appliedMigration);
			return appliedMigration;
		}

		public virtual void CompleteMigration(AppliedMigration appliedMigration)
		{
			appliedMigration.CompletedOn = DateTime.Now;
            GetMigrationsApplied().FindOneAndReplace
                (Builders<AppliedMigration>.Filter.Eq(p => p.Version, appliedMigration.Version), appliedMigration);
                // appliedMigration, appliedMigration);
		}

		public virtual void MarkUpToVersion(MigrationVersion version)
		{
			_Runner.MigrationLocator.GetAllMigrations()
				.Where(m => m.Version <= version)
				.ToList()
				.ForEach(m => MarkVersion(m.Version));
		}

		public virtual void MarkVersion(MigrationVersion version)
		{
			var appliedMigration = AppliedMigration.MarkerOnly(version);
			GetMigrationsApplied().InsertOne(appliedMigration);
		}
	}
}