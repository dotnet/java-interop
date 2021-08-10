using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany ("Microsoft Corporation")]
[assembly: AssemblyCopyright ("Microsoft Corporation")]
[assembly: AssemblyTrademark ("Microsoft Corporation")]
[assembly: AssemblyVersion (ThisAssembly.Git.BaseVersion.Major + "." + ThisAssembly.Git.BaseVersion.Minor + "." + ThisAssembly.Git.BaseVersion.Patch)]
[assembly: AssemblyFileVersion (ThisAssembly.Git.SemVer.Major + "." + ThisAssembly.Git.SemVer.Minor + "." + ThisAssembly.Git.SemVer.Patch)]
[assembly: AssemblyInformationalVersion (ThisAssembly.Git.BaseVersion.Major + "." + ThisAssembly.Git.BaseVersion.Minor + "; git-rev-head:" + ThisAssembly.Git.Commit + " git-branch:" + ThisAssembly.Git.Branch)]
