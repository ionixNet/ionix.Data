namespace ionix.Data.MongoDB
{
    using System;
    using System.Reflection;
    using Migration;

    public static class MongoHelper
    {
        public static bool InitializeMongo(Assembly asm, string connectionString, string databaseName, bool throwIfNotLatestVersion)
        {
            if (null != asm && !String.IsNullOrEmpty(connectionString) && !String.IsNullOrEmpty(databaseName))
            {
                var runner = new MigrationRunner(connectionString, databaseName);

                runner.MigrationLocator.LookForMigrationsInAssembly(asm);
                // runner.MigrationLocator.LookForMigrationsInAssemblyOfType<Migration1>();

                if (throwIfNotLatestVersion)
                    runner.DatabaseStatus.ThrowIfNotLatestVersion();
                runner.DatabaseStatus.ValidateMigrationsVersions();

                runner.UpdateToLatest();
                return true;
            }

            return false;
        }
    }
}
