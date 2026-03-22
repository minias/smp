// Applications/Interfaces/IUpdateService.cs
public interface IUpdateService
{
    Task<UpdateInfo?> CheckForUpdateAsync();
    Task DownloadAndInstallAsync(UpdateInfo info);
}