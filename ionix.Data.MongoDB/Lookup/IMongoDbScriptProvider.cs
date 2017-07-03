namespace ionix.Data.MongoDB
{
    using System.Text;

    public interface IMongoDbScriptProvider
    {
        StringBuilder ToScript();
    }
}
