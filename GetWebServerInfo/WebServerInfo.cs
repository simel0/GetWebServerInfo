using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace GetWebServerInfo;

public class WebServerInfo
{
    private readonly IServiceProvider? _service;
    public WebServerInfo()
    {

    }

    public WebServerInfo(IServiceProvider service)
    {
        _service = service;
    }

    public RunTimeInfo Runtime => new();
    public EnvironmentInfo Environment => new();

    [JsonExtensionData]
    public Dictionary<string, object> AdditionalData
    {
        get
        {
            var dict = new Dictionary<string, object>();
            if (_service != null)
            {
                //https://stackoverflow.com/questions/71473870/how-can-i-know-if-my-app-is-running-under-kestrel-or-http-sys
                var context = _service.GetService<IHttpContextAccessor>()?.HttpContext; //TODO: Must use Services.AddHttpContextAccessor();
                if (context != null)
                {
                    dict.Add("isKestrel", context.Request.Headers.GetType().ToString().Contains(".Kestrel."));
                    dict.Add("isHTTPsys", context.Request.Headers.GetType().ToString().Contains(".HttpSys."));
                }
                //https://stackoverflow.com/questions/48546843/iserveraddressesfeature-addresses-empty-when-running-under-dotnet-exe
                var server = _service.GetService<IServer>();
                var addressFeature = server.Features.Get<IServerAddressesFeature>();
                if (addressFeature != null)
                {
                    dict.Add("addresses", addressFeature.Addresses.ToList());
                }
            }

            return dict;
        }
    }
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
                                 bool.Parse(Variables["DOTNET_RUNNING_IN_CONTAINER"]?.ToString() ?? "false");

    //https://stackoverflow.com/questions/61415922/how-do-i-determine-webserver-type-at-runtime-in-asp-net-core
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