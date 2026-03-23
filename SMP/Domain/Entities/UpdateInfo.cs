// Domain/Entities/UpdateInfo.cs
namespace SMP.Domain.Entities;

public class UpdateInfo
{
    public string Version { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
}