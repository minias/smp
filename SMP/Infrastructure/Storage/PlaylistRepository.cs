// Infrastructure/Storage/PlaylistRepository.cs
using SMP.Domain;
using System.Text.Json;
using SMP.Infrastructure.Serialization;
using System.Diagnostics;

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
        // ✅ JsonOptionsProvider 사용 (CA1869 해결)
        var json = JsonSerializer.Serialize(items, JsonOptionsProvider.Default);

        // TODO: 안정성 향상 (temp 파일 → replace 방식 고려 가능)
        await File.WriteAllTextAsync(_filePath, json);
    }


    /// <summary>
    /// 플레이리스트 로드
    /// </summary>
    public async Task<List<PlaylistItem>> LoadAsync()
    {
        // 🔍 디버깅 로그 (디버그 모드에서만 실행됨)
        Debug.WriteLine($"[DEBUG] Load called: {_filePath}");

        if (!File.Exists(_filePath))
            return [];

        var json = await File.ReadAllTextAsync(_filePath);
        Debug.WriteLine($"[DEBUG] raw json: {json}");

        return JsonSerializer.Deserialize<List<PlaylistItem>>(json)
               ?? [];
    }
}