using System.ComponentModel.Design;

namespace SimpleDosboxFrontend
{
    static class Ioc
    {
        private static readonly ServiceContainer _services;

        static Ioc()
        {
            _services = new ServiceContainer();
        }

        internal static void Add<T>(T service)
        {
            _services.AddService(typeof(T), service);
        }

        internal static T Get<T>()
        {
            return (T)_services.GetService(typeof(T));
        }
    }
}