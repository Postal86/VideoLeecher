using Ninject.Modules;
using VideoLeecher.services.Interfaces;
using VideoLeecher.services.Services;


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
            Bind<IProcessingService>().To<Proces>
        }




        #endregion


    }
}
