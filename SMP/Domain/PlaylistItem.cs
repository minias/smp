// Domain/PlaylistItem.cs
namespace SMP.Domain;

// 플레이리스트 항목 (UI 표시 + 실제 재생 데이터 분리)
public class PlaylistItem
{
    // 사용자에게 보여줄 제목
    public string Title { get; set; } = string.Empty;

    // 실제 재생용 URL
    public string Url { get; set; } = string.Empty;

    // ListBox 표시용
    public override string ToString()
    {
        return Title;
    }
}