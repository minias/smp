// Infrastructure/Update/GithubReleaseClient.cs
using SMP.Domain.Entities;
using System.Net.Http;
using System.Text.Json;

namespace SMP.Infrastructure.Update;

public class GithubReleaseClient
{
    private readonly HttpClient _http = new();

    private const string API_URL =
        "https://api.github.com/repos/minias/smp/releases/latest";

    public async Task<UpdateInfo?> GetLatestAsync()
    {
        // GitHub API는 User-Agent 필수
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("SMP-App");

        var json = await _http.GetStringAsync(API_URL);

        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        var version = root.GetProperty("tag_name").GetString(); // v1.0.0

        var asset = root.GetProperty("assets")[0];
        var url = asset.GetProperty("browser_download_url").GetString();

        return new UpdateInfo
        {
            Version = version!.Replace("v", ""),
            DownloadUrl = url!
        };
    }
}