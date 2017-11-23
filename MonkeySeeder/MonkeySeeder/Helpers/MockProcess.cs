using System.Diagnostics;

namespace MonkeySeeder.Helpers
{
    public class MockProcess : Process
    {
        public new string ProcessName { get; set; }
    }
}