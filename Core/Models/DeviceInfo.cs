using System.Net;

namespace Core.Models;

public record DeviceInfo(string? DeviceId, string? UserAgent, IPAddress? IpAddress);