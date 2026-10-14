using System;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace PSSlideShow.Tests {
    public class EditorConfigFormattingTests {
        [Fact]
        public void CSharp_sources_match_the_workspace_editorconfig() {
            var repositoryRoot = FindRepositoryRoot();
            var solutionPath = Path.Combine(repositoryRoot, "PSSlideShow.slnx");

            Assert.True(File.Exists(solutionPath), $"Expected solution file at {solutionPath}");

            var startInfo = new ProcessStartInfo("dotnet") {
                WorkingDirectory = repositoryRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            startInfo.ArgumentList.Add("format");
            startInfo.ArgumentList.Add(solutionPath);
            startInfo.ArgumentList.Add("--verify-no-changes");
            startInfo.ArgumentList.Add("--no-restore");
            startInfo.ArgumentList.Add("--verbosity");
            startInfo.ArgumentList.Add("minimal");

            using var process = Process.Start(startInfo);
            Assert.NotNull(process);

            var standardOutput = process.StandardOutput.ReadToEnd();
            var standardError = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Assert.True(process.ExitCode == 0,
                $"dotnet format reported formatting drift against the workspace EditorConfig. Exit code: {process.ExitCode}{Environment.NewLine}{standardOutput}{Environment.NewLine}{standardError}");
        }

        private static string FindRepositoryRoot() {
            var current = new DirectoryInfo(AppContext.BaseDirectory);

            while (current is not null) {
                var editorConfigPath = Path.Combine(current.FullName, ".editorconfig");
                var solutionPath = Path.Combine(current.FullName, "PSSlideShow.slnx");

                if (File.Exists(editorConfigPath) && File.Exists(solutionPath)) {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new DirectoryNotFoundException("Could not find the repository root containing .editorconfig and PSSlideShow.slnx.");
        }
    }
}
