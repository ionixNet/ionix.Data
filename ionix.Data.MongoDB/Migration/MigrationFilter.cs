namespace ionix.Data.MongoDB.Migration
{
	public abstract class MigrationFilter
	{
		public abstract bool Exclude(Migration migration);
	}
}