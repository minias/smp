// Infrastructure/Youtube/YtDlpService.cs
using System.Diagnostics;

namespace SMP.Infrastructure.Youtube;

// 유튜브 URL → 실제 스트림 URL 추출
public class YtDlpService
{
    private readonly string _exePath;

    public YtDlpService(string exePath)
    {
        _exePath = exePath;
    }

    public async Task<string?> GetStreamUrlAsync(string youtubeUrl)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _exePath,
            Arguments = $"-f bestaudio --no-playlist -g --js-runtimes node {youtubeUrl}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null) return null;

        string output = await process.StandardOutput.ReadLineAsync() ?? "";
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        Console.WriteLine($"[yt-dlp] {output}");
        Console.WriteLine($"[yt-dlp-error] {error}");

        return string.IsNullOrWhiteSpace(output) ? null : output;
    }
    // 제목 가져오기
    public async Task<string?> GetTitleAsync(string youtubeUrl)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _exePath,
            Arguments = $"--get-title {youtubeUrl}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null) return null;

        string title = await process.StandardOutput.ReadLineAsync() ?? "";
        await process.WaitForExitAsync();

        return string.IsNullOrWhiteSpace(title) ? null : title.Trim();
    }
}