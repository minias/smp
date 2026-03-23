// File: SMP.Domain/LoopMode.cs
using System;

namespace SMP.Domain;

/// <summary>
/// 재생 루프 모드
/// </summary>
public enum LoopMode
{
    /// <summary>
    /// 반복 없음 (1회 재생)
    /// </summary>
    None,

    /// <summary>
    /// 전체 반복
    /// </summary>
    All,

    /// <summary>
    /// 한 곡 반복
    /// </summary>
    Single
}