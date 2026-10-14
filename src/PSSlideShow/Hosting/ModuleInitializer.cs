using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;
#if NET
using System.Runtime.InteropServices;
using System.Runtime.Loader;
#endif

namespace PSSlideShow.Hosting {
    /// <summary>
    /// Loads the module's private dependencies (Spectre.Console, TextMateSharp, Onigwrap, …) in
    /// isolation so their versions cannot clash with the host or other modules.
    /// </summary>
    /// <remarks>
    /// On .NET (PowerShell 7) the dependencies are loaded into a dedicated
    /// <c>AssemblyLoadContext</c>; a handler on the default context's <c>Resolving</c> event
    /// redirects any assembly we ship privately into that context, while framework assemblies fall
    /// through to the shared runtime. On .NET Framework (Windows PowerShell 5.1) there are no load
    /// contexts, so dependencies are resolved from the private folder via an <c>AssemblyResolve</c>
    /// handler (best-effort — the runtime offers no true isolation there).
    /// PowerShell instantiates this class and calls <see cref="OnImport"/> when the binary module is
    /// imported, and <see cref="OnRemove"/> when it is removed. The module's loader script also calls
    /// <see cref="Initialize"/> directly <em>before</em> the cmdlets are enumerated, because some
    /// cmdlets expose Spectre.Console types (e.g. <c>Justify</c>) as parameters — reflecting over those
    /// parameters forces the dependency to load during import, which is too late for
    /// <see cref="OnImport"/> alone. <see cref="Initialize"/> only touches framework types, so it can
    /// run safely before any private dependency is resolved.
    /// </remarks>
    public sealed class ModuleInitializer : IModuleAssemblyInitializer, IModuleAssemblyCleanup {
        private static readonly object Sync = new object();
        private static readonly string DependencyDir = ResolveDependencyDir();
        private static bool _registered;

        /// <summary>Registers the dependency resolver. Safe to call more than once.</summary>
        public static void Initialize() {
            lock (Sync) {
                if (_registered) {
                    return;
                }

                Register();
                _registered = true;
            }
        }

        /// <summary>Removes the dependency resolver. Safe to call more than once.</summary>
        public static void Teardown() {
            lock (Sync) {
                if (!_registered) {
                    return;
                }

                Unregister();
                _registered = false;
            }
        }

        /// <summary>Registers the dependency resolver when the module is imported.</summary>
        public void OnImport() => Initialize();

        /// <summary>Removes the dependency resolver when the module is unloaded.</summary>
        public void OnRemove(PSModuleInfo module) => Teardown();

        private static string ResolveDependencyDir() {
            var dir = Path.GetDirectoryName(typeof(ModuleInitializer).Assembly.Location);
            if (string.IsNullOrEmpty(dir)) {
                dir = AppContext.BaseDirectory;
            }

            return Path.Combine(dir, "dependencies");
        }

#if NET
        private static PSSlideShowLoadContext? _loadContext;

        private static void Register() {
            _loadContext ??= new PSSlideShowLoadContext(DependencyDir);
            AssemblyLoadContext.Default.Resolving += OnDefaultResolving;
        }

        private static void Unregister() {
            AssemblyLoadContext.Default.Resolving -= OnDefaultResolving;
        }

        private static Assembly? OnDefaultResolving(AssemblyLoadContext context, AssemblyName name) {
            // Only satisfy assemblies we actually ship privately; everything else falls through to the
            // default context and the shared framework, so we never shadow runtime assemblies.
            var candidate = Path.Combine(DependencyDir, name.Name + ".dll");
            return File.Exists(candidate) ? _loadContext!.LoadFromAssemblyName(name) : null;
        }
#else
        private static void Register()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            PreloadNativeLibrary();
        }

        private static void Unregister()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
        }

        private static Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name).Name;
            var candidate = Path.Combine(DependencyDir, name + ".dll");
            return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
        }

        private static void PreloadNativeLibrary()
        {
            // .NET Framework has no per-context native resolution; eagerly load Onigwrap's native
            // library so TextMateSharp's later P/Invoke resolves it by name.
            var arch = Environment.Is64BitProcess ? "x64" : "x86";
            var nativePath = Path.Combine(DependencyDir, "runtimes", "win-" + arch, "native", "libonigwrap.dll");
            if (File.Exists(nativePath))
            {
                NativeMethods.LoadLibrary(nativePath);
            }
        }

        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            public static extern IntPtr LoadLibrary(string lpFileName);
        }
#endif
    }

#if NET
    /// <summary>
    /// An isolated load context for the module's private dependencies. It only loads assemblies that
    /// physically ship in the dependency folder; framework assemblies return <c>null</c> so they fall
    /// through to the default context. It also resolves TextMateSharp's native Onigwrap library from
    /// the private <c>runtimes</c> folder, which lets syntax highlighting work even when the module is
    /// imported dynamically rather than launched as an application.
    /// </summary>
    internal sealed class PSSlideShowLoadContext : AssemblyLoadContext {
        private readonly string _dependencyDir;

        public PSSlideShowLoadContext(string dependencyDir)
            : base("PSSlideShow", isCollectible: false) {
            _dependencyDir = dependencyDir;
        }

        protected override Assembly? Load(AssemblyName assemblyName) {
            var candidate = Path.Combine(_dependencyDir, assemblyName.Name + ".dll");
            return File.Exists(candidate) ? LoadFromAssemblyPath(candidate) : null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) {
            var candidate = Path.Combine(_dependencyDir, "runtimes", NativeRid(), "native", NativeFileName(unmanagedDllName));
            return File.Exists(candidate) ? LoadUnmanagedDllFromPath(candidate) : IntPtr.Zero;
        }

        private static string NativeRid() {
            var arch = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return "win-" + arch;
            }

            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" : "linux-" + arch;
        }

        private static string NativeFileName(string name) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return name + ".dll";
            }

            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? name + ".dylib" : name + ".so";
        }
    }
#endif
}
