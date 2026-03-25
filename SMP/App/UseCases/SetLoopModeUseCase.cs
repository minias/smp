// /App/UseCases/SetLoopModeUseCase.cs

using SMP.App.Service;
using SMP.Domain;

namespace SMP.App.UseCases;

/// <summary>
/// 루프 모드 설정 UseCase
/// </summary>
public class SetLoopModeUseCase
{
    private readonly PlayerState _state;

    public SetLoopModeUseCase(PlayerState state)
    {
        _state = state;
    }

    /// <summary>
    /// 루프 모드 설정
    /// </summary>
    public void Execute(LoopMode mode)
    {
        _state.LoopMode = mode;
    }
}