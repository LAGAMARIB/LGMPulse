using Microsoft.Extensions.Configuration;

namespace LGMPulse.Connections;

public sealed class ConnectionSettings
{
    public static readonly ConnectionSettings Instance = new();

    public string ConnectionName { get; private set; }
    public string LGM_SESSION { get; private set; }
    public string LGM_REFRESH { get; private set; }

    private ConnectionSettings() { }

    public void Initialize(IConfiguration configuration)
    {
        ConnectionName = "LGMPULSE";
        LGM_SESSION = configuration.GetSection("Cookies")["LGM_SESSION"] ?? "Pulse_LGMSession";
        LGM_REFRESH = configuration.GetSection("Cookies")["LGM_REFRESH"] ?? "Pulse_LGMRefresh";
    }
}

