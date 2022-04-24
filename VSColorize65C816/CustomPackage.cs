using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;

namespace VSColorize65C816
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("Syntax highlighting for 64tass W65C816", "Applies syntax highlighting for 64tass W65C816 source code.", "Version 0.1")]

    [ProvideService(typeof(CustomLanguageInfo), ServiceName = nameof(CustomLanguageInfo))]
    [ProvideLanguageExtension(typeof(CustomLanguageInfo), ".s")]
    [ProvideLanguageExtension(typeof(CustomLanguageInfo), ".asm")]
    [ProvideLanguageService(typeof(CustomLanguageInfo), CustomLanguageInfo.LanguageName, 100)]

    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]

    [Guid("7AF1CD21-E0AB-445F-A458-871AC5AD3E7B")]
    class CustomPackage : AsyncPackage
    {
        // This type is needed for setting up the LanguageInfo.

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            IServiceContainer serviceContainer = this;
            serviceContainer.AddService(typeof(CustomLanguageInfo), new CustomLanguageInfo(), true);

        }
    }
}
