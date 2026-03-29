namespace NRaas.OverwatchSpace.Startup
{
    public abstract class StartupOption : Common.IStartupApp
    {
        public StartupOption()
        { }

        public abstract void OnStartupApp();
    }
}
