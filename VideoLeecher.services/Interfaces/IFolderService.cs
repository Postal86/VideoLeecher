namespace VideoLeecher.services.Interfaces
{
   public  interface  IFolderService
   {
        string GetAppDataFolder();

        string GetTempFolder();

        string GetDownloadFolder();

   }
}
