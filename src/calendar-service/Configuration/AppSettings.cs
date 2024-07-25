using System;

namespace Calendar.Service.Configuration
{
    public static class AppSettings
    {
        private static Lazy<IAppConfiguration> config = new Lazy<IAppConfiguration>(LoadConfig);

        public static IAppConfiguration Config => config.Value;

        public static void InitConfig(IAppConfiguration configuration)
        {
            config = new Lazy<IAppConfiguration>(() => configuration);
        }

        private static IAppConfiguration LoadConfig()
        {
            return new AppConfiguration().Get();
        }
    }
}