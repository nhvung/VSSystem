using VSSystem.Logger;

namespace VSSystem.ServiceProcess.Hosting
{
    public interface IHost : Microsoft.Extensions.Hosting.IHost
    {
        public string Name { get; }
        public ALogger Logger { get; }
    }
}