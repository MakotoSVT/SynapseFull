namespace Synapse.Providers
{
    public interface IAlertProvider
    {
        bool SendAlertMessage(string message);
    }
}
