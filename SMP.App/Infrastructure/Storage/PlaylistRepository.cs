// Infrastructure/Storage/PlaylistRepository.cs
using SMP.Domain;
using System.Text.Json;

namespace SMP.Infrastructure.Storage;

/// <summary>
/// 플레이리스트 저장소
/// - 사용자 로컬 AppData에 JSON 저장
/// - 설치 폴더와 분리하여 쓰기 권한 문제 방지
/// </summary>
public class PlaylistRepository
{
    private readonly string _filePath;

    public PlaylistRepository()
    {
        // 사용자 AppData 경로 생성
        var appDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SMP"
        );

        // 디렉토리 없으면 생성
        Directory.CreateDirectory(appDataDir);

        // 실제 저장 파일 경로
        _filePath = Path.Combine(appDataDir, "playlist.json");
    }

    /// <summary>
    /// 플레이리스트 저장
    /// </summary>
    public async Task SaveAsync(IEnumerable<PlaylistItem> items)
    {
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(_filePath, json);
    }

    /// <summary>
    /// 플레이리스트 로드
    /// </summary>
    public async Task<List<PlaylistItem>> LoadAsync()
    {
        if (!File.Exists(_filePath))
            return new List<PlaylistItem>();

        var json = await File.ReadAllTextAsync(_filePath);

        return JsonSerializer.Deserialize<List<PlaylistItem>>(json)
               ?? new List<PlaylistItem>();
    }
}