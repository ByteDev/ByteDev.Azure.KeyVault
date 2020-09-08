using System.IO;
using System.Text.Json;

namespace ByteDev.Azure.KeyVault.IntTests
{
    public static class TestSettingsSerializer
    {
        public static TestSettings Deserialize()
        {
            var json = File.ReadAllText(@"Z:\Dev\ByteDev.Azure.KeyVault.IntTests.settings.json");

            return JsonSerializer.Deserialize<TestSettings>(json);
        }
    }
}