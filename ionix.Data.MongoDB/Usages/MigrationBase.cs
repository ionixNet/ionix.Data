namespace ionix.Data.MongoDB
{
    using System;
    using System.Reflection;
    using Migration;

    public abstract class MigrationBase : Migration.Migration
    {
        protected MigrationBase(MigrationVersion version)
            : base(version)
        {
        }


        public Assembly GetMigrationsAssembly()
        {
            //var name = new AssemblyName("Lib");
            //return Assembly.Load(name);

            return this.GetType().Assembly;
        }

        public sealed override void Update()
        {
            if (String.IsNullOrEmpty(this.Script))
            {
                throw new InvalidOperationException("MigrationBase.GenerateMigrationScript() method should not returns null or empty script");
            }

            MongoAdmin.ExecuteScript(this.Database, this.Script);
        }

        private string _script;
        public sealed override string Script => _script ?? (_script = this.GenerateMigrationScript());


        //Template Method PAttern
        public abstract string GenerateMigrationScript();
    }
}
