// Infrastructure/Update/UpdateService.cs
using System.Diagnostics;

public class UpdateService : IUpdateService
{
    private readonly GithubReleaseClient _client = new();

    public async Task<UpdateInfo?> CheckForUpdateAsync()
    {
        var latest = await _client.GetLatestAsync();

        var current = GetCurrentVersion();

        if (latest == null)
            return null;

        return VersionHelper.IsUpdateAvailable(current, latest.Version)
            ? latest
            : null;
    }

    public async Task DownloadAndInstallAsync(UpdateInfo info)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), "SMP_Update.exe");

        using var http = new HttpClient();

        // 다운로드
        var data = await http.GetByteArrayAsync(info.DownloadUrl);
        await File.WriteAllBytesAsync(tempFile, data);

        // 업데이트 실행
        RunUpdater(tempFile);
    }

    private void RunUpdater(string newExe)
    {
        var currentExe = Environment.ProcessPath!;

        var updaterPath = Path.Combine(
            Path.GetTempPath(),
            "smp_updater.bat"
        );

        File.WriteAllText(updaterPath, $@"
@echo off
timeout /t 2
taskkill /im {Path.GetFileName(currentExe)} /f
copy /y ""{newExe}"" ""{currentExe}""
start """" ""{currentExe}""
");

        Process.Start(new ProcessStartInfo
        {
            FileName = updaterPath,
            CreateNoWindow = true,
            UseShellExecute = false
        });

        Environment.Exit(0);
    }

    private string GetCurrentVersion()
    {
        return System.Reflection.Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version!
            .ToString(3);
    }
}