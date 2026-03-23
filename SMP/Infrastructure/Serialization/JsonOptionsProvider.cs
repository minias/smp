// Infrastructure/Serialization/JsonOptionsProvider.cs

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SMP.Infrastructure.Serialization;

/// <summary>
/// JsonSerializerOptions 중앙 관리 클래스
/// - 모든 JSON 직렬화/역직렬화 옵션을 단일 위치에서 관리
/// - CA1869 대응: JsonSerializerOptions 재사용 (성능 최적화)
/// - 전역 정책 일관성 유지
/// </summary>
public static class JsonOptionsProvider
{
    /// <summary>
    /// 기본 JSON 옵션 (전역 공유)
    /// 
    /// 주의:
    /// - 절대 런타임 중 변경 금지 (Thread-safe 보장 위해)
    /// - 모든 Repository에서 동일하게 사용
    /// </summary>
    public static readonly JsonSerializerOptions Default = new()
    {
        // 사람이 읽기 좋은 JSON 포맷 (디버깅/로그용)
        WriteIndented = true,

        // 프로퍼티 camelCase 적용 (일관성 유지)
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

        // null 값은 JSON에 포함하지 않음
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}