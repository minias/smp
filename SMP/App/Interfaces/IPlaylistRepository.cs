// /App/Interfaces/IPlaylistRepository.cs
using SMP.Domain;

namespace SMP.App.Interfaces;

/// <summary>
/// 플레이리스트 저장소 Port
/// </summary>
public interface IPlaylistRepository
{
    Task<List<PlaylistItem>> LoadAsync();

    /// <summary>
    /// IEnumerable로 변경 (유연성 확보)
    /// </summary>
    Task SaveAsync(IEnumerable<PlaylistItem> items);
}