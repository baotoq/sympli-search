using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SearchService.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.IgnoreMember(nameof(StackTrace));
        VerifierSettings.ScrubInlineGuids();
        VerifierSettings.DisableDateCounting();
    }
}