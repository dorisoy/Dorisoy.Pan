namespace Dorisoy.Pan.Services
{
    public enum Platform : byte
    {
        Linux,
        MacOs,
        Windows,
        Unknown
    }

    public class PlatformService : IPlatformService
    {
        public Platform GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Platform.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Platform.MacOs;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Platform.Windows;
            }

            return Platform.Unknown;
        }
    }
}