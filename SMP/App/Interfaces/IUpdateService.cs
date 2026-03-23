// Apps/Interfaces/IUpdateService.cs
namespace SMP.App.Interfaces;

using SMP.Domain.Entities;

public interface IUpdateService
{
    Task<UpdateInfo?> CheckForUpdateAsync();
    Task DownloadAndInstallAsync(UpdateInfo info);
}