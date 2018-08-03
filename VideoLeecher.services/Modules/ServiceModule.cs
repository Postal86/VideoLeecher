using Ninject.Modules;
using VideoLeecher.services.Interfaces;
using VideoLeecher.services.Services;
using VideoLeecher.services.Services.Processing;

namespace VideoLeecher.services.Modules
{
    public class ServiceModule :  NinjectModule
    {
        #region მეთოდები

        public override void Load()
        {
            Bind<IFilenameService>().To<FilenameService>().InSingletonScope();
            Bind<IFolderService>().To<FolderService>().InSingletonScope();
            Bind<ILogService>().To<LogService>().InSingletonScope();
            Bind<IPreferenceService>().To<PreferencesService>().InSingletonScope();
            Bind<IProcessingService>().To<ProcessingService>().InSingletonScope();
            Bind<IRuntimeDataService>().To<RuntimeDataService>().InSingletonScope();
            Bind<ITwitchService>().To<TwitchService>().InSingletonScope();
            Bind<IUpdateService>().To<IUpdateService>().InSingletonScope();
        }
 
        #endregion მეთოდები
    }
}
