using System.IO;
using System.Threading.Tasks;
using MM2Randomizer.Utilities;
using Xunit;

namespace MM2Randomizer.Tests
{
    public class RandomMM2Tests
    {
        [Theory]
        [InlineData("AAAAAAA")]
        [InlineData("SEEEEED")]
        [InlineData("ABCDEFG")]
        public void RandomizerCreate_GeneratesSameRomForSameSeed(string seedText)
        {
            Assert.True(File.Exists("mm2.nes"));
            lock (ParallelGuard.randomizerLock)
            {
                RandomMM2.Settings = new RandoSettings
                {
                    SourcePath = "mm2.nes"
                };
                RandomMM2.Seed = SeedConvert.ConvertBase26To10(seedText);

                string romPath = null;
                try
                {
                    romPath = RandomMM2.RandomizerCreate(fromClientApp: true);
                    byte[] rom1Bytes = File.ReadAllBytes(romPath);
                    File.Delete(romPath);

                    // delay just in case something is using clock ticks for seeds
                    Task.Delay(1_000).GetAwaiter().GetResult();

                    romPath = RandomMM2.RandomizerCreate(fromClientApp: true);
                    byte[] rom2Bytes = File.ReadAllBytes(romPath);

                    Assert.Equal(rom1Bytes, rom2Bytes);
                }
                finally
                {
                    if (File.Exists(romPath))
                    {
                        File.Delete(romPath);
                    }
                }
            }
        }
    }
}
