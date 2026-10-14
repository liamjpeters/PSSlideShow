using System;
using System.Threading;
using PSSlideShow.Execution;
using PSSlideShow.Model;
using System.Management.Automation.Runspaces;
using Xunit;

namespace PSSlideShow.Tests.Runtime {
    public class DynamicUpdateTests {
        [Fact]
        public void Initial_update_is_applied_before_activation_returns() {
            using var runspaces = new RunspaceManager();
            using var updates = new DynamicUpdateManager(runspaces);
            var widget = Dynamic(
                "'initial'",
                "param($snapshot); [PSSlideShow.Model.TextWidget]::new($snapshot)");

            InHostRunspace(() => {
                updates.Activate(new Slide("Dynamic", new ISlideWidget[] { widget }));

                Assert.True(widget.HasSnapshot);
                Assert.Equal("initial", widget.Snapshot);
                Assert.Contains("initial", TestSupport.Render(widget));
            });
        }

        [Fact]
        public void Background_results_are_applied_when_the_host_processes_them() {
            using var runspaces = new RunspaceManager();
            using var updates = new DynamicUpdateManager(runspaces);
            var widget = Dynamic(
                "[DateTime]::UtcNow.Ticks",
                "param($snapshot); [PSSlideShow.Model.TextWidget]::new($snapshot.ToString())",
                TimeSpan.FromMilliseconds(20));
            var slide = new Slide("Dynamic", new ISlideWidget[] { widget });

            InHostRunspace(() => {
                updates.Activate(slide);
                var first = widget.LastUpdated;
                var changed = false;
                var deadline = DateTime.UtcNow.AddSeconds(2);
                while (!changed && DateTime.UtcNow < deadline) {
                    Thread.Sleep(20);
                    changed = updates.ProcessCompleted();
                }

                Assert.True(changed);
                Assert.NotEqual(first, widget.LastUpdated);
            });
        }

        [Fact]
        public void View_failure_keeps_the_last_successful_view() {
            using var runspaces = new RunspaceManager();
            using var updates = new DynamicUpdateManager(runspaces);
            var widget = Dynamic(
                "'value'",
                "param($snapshot); throw 'view failed'");

            InHostRunspace(() => {
                updates.Activate(new Slide("Dynamic", new ISlideWidget[] { widget }));

                Assert.False(widget.HasSnapshot);
                Assert.Contains("view failed", TestSupport.Render(widget));
            });
        }

        [Fact]
        public void Custom_object_snapshot_properties_are_available_to_the_view() {
            using var runspaces = new RunspaceManager();
            using var updates = new DynamicUpdateManager(runspaces);
            var widget = Dynamic(
                "[pscustomobject]@{ Updated = 'timestamp'; Value = 42 }",
                "param($snapshot); [PSSlideShow.Model.TextWidget]::new(\"$($snapshot.Updated): $($snapshot.Value)\")");

            InHostRunspace(() => {
                updates.Activate(new Slide("Dynamic", new ISlideWidget[] { widget }));

                Assert.True(widget.HasSnapshot);
                Assert.Contains("timestamp: 42", TestSupport.Render(widget));
            });
        }

        private static DynamicWidget Dynamic(
            string update,
            string view,
            TimeSpan? interval = null) =>
            new DynamicWidget(
                interval ?? TimeSpan.FromMinutes(1),
                System.Management.Automation.ScriptBlock.Create(update),
                System.Management.Automation.ScriptBlock.Create(view));

        private static void InHostRunspace(Action action) {
            using var hostRunspace = RunspaceFactory.CreateRunspace();
            hostRunspace.Open();
            var previous = Runspace.DefaultRunspace;
            Runspace.DefaultRunspace = hostRunspace;
            try {
                action();
            }
            finally {
                Runspace.DefaultRunspace = previous;
            }
        }
    }
}