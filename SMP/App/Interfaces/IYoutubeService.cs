// /App/Ports/IYoutubeService.cs
namespace SMP.App.Interfaces;

/// <summary>
/// Youtube 메타 정보 조회 Port
/// </summary>
public interface IYoutubeService
{
    /// <summary>
    /// 영상 제목 조회
    /// </summary>
    Task<string?> GetTitleAsync(string url);

    /// <summary>
    /// 스트리밍 URL 추출 (yt-dlp)
    /// </summary>
    Task<string?> GetStreamUrlAsync(string url); // ✅ nullable로 맞춤
}