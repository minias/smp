// Infrastructure/Update/VersionHelper.cs
using SMP.Infrastructure.Update;

namespace SMP.Infrastructure.Update;

public static class VersionHelper
{
    public static bool IsUpdateAvailable(string current, string latest)
    {
        var v1 = new Version(current);
        var v2 = new Version(latest);

        return v2 > v1;
    }
}