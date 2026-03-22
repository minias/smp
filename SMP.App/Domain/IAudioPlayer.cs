// Domain/IAudioPlayer.cs
namespace SMP.Domain;

// 재생 인터페이스 (확장 가능)
public interface IAudioPlayer
{
    void Play(string source);
    void Stop();
}