using System.Collections.Generic;
using System.IO;
using MM2Randomizer.Utilities;
using Xunit;

namespace MM2Randomizer.Tests
{
    public class FakeRandomTests
    {
        [Fact]
        public void FakeRandom_AlwaysGeneratesSameRom()
        {
            Assert.True(File.Exists("mm2.nes"));
            var seeds = new[] { "AAAAAAA", "SEEEEED", "ABCDEFG" };
            var romBytes = new List<byte[]>();
            lock (ParallelGuard.randomizerLock)
            {
                foreach (string seedText in seeds)
                {
                    Assert.Equal(7, seedText.Length);
                    RandomMM2.Settings = new RandoSettings
                    {
                        SourcePath = "mm2.nes"
                    };
                    RandomMM2.Seed = SeedConvert.ConvertBase26To10(seedText);

                    string romPath = null;
                    try
                    {
                        var fakeRandom = new FakeRandom();
                        romPath = RandomMM2.RandomizerCreate(fromClientApp: true, gameplayRng: fakeRandom, cosmeticRng: fakeRandom);
                        romBytes.Add(File.ReadAllBytes(romPath));
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
            // the ROMs are different at first since they include the seed text
            byte[] first = romBytes[0];
            for (int i = 1; i < romBytes.Count; i++)
            {
                Assert.NotEqual(first, romBytes[i]);
            }

            // remove seed text from ROMs
            for (int i = 0; i < romBytes.Count; i++)
            {
                for (int j = 0x037387; j < 0x037387 + 7; j++)
                {
                    romBytes[i][j] = 0;
                }
            }

            // the ROMs should be identical now
            for (int i = 1; i < romBytes.Count; i++)
            {
                for (int j = 0; j < romBytes[i].Length; j++)
                {
                    Assert.Equal(first[j], romBytes[i][j]);
                }
            }
        }
    }
}
