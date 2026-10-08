using System.Reflection;

namespace VCuttingRelease;

internal static class VersionInfo
{
    static Assembly AppAssembly => Assembly.GetEntryAssembly() ?? typeof(VersionInfo).Assembly;
    static string Read(string key) => AppAssembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key == key)?.Value ?? "unknown";
    public static string Display => Read("CustomerVersion");
    public static string Development => Read("DevelopmentVersion");
    public static string Build => Read("BuildIdentity");
    public static string About(string product) => $"{product} {Display}\nDevelopment: {Development}\nBuild: {Build}";
}
