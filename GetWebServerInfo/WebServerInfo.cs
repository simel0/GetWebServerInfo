using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace GetWebServerInfo;

public class WebServerInfo
{
    public RunTimeInfo Runtime => new();
    public EnvironmentInfo Environment => new();
}

public class RunTimeInfo
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Architecture OSArchitecture => RuntimeInformation.OSArchitecture;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;

    public string FrameworkDescription => RuntimeInformation.FrameworkDescription;
    public string RuntimeIdentifier => RuntimeInformation.RuntimeIdentifier;
    public string OSDescription => RuntimeInformation.OSDescription;
}

public class EnvironmentInfo
{
    //public string[] LogicalDrives => Environment.GetLogicalDrives();
    public string MachineName => Environment.MachineName;
    public string CurrentDirectory => Environment.CurrentDirectory;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PlatformID PlatformID => Environment.OSVersion.Platform;

    public string ServicePack => Environment.OSVersion.ServicePack;
    public string Version => Environment.OSVersion.VersionString;
    public int ProcessId => Environment.ProcessId;
    public int ProcessorCount => Environment.ProcessorCount;

    public Hashtable Variables => new(Environment.GetEnvironmentVariables());

    public bool IsInContainer => Variables.ContainsKey("DOTNET_RUNNING_IN_CONTAINER") &&
                                 bool.Parse(Variables["DOTNET_RUNNING_IN_CONTAINER"].ToString());

    public string WebServer
    {
        get
        {
            var procName = Process.GetCurrentProcess().ProcessName;
            switch (procName)
            {
                case string IIS6Later when IIS6Later.Contains("w3wp"):
                    return "IIS6Later";
                case string IIS6Earlier when IIS6Earlier.Contains("aspnet_wp"):
                    return "IIS6Earlier ";
                case string IISExpress when IISExpress.Contains("iisexpress"):
                    return "IISExpress";
                case string OlderASPApp when OlderASPApp.Contains("inetinfo"):
                    return "OlderASPApp";
                default:
                    return "Unknown";
            }
        }
    }
}