using System;

namespace MM2Randomizer.Tests
{
    public class FakeRandom : Random
    {
        public override int Next() => 0;

        public override int Next(int maxValue) => 0;

        public override int Next(int minValue, int maxValue) => minValue;

        public override void NextBytes(byte[] buffer) => Array.Clear(buffer, 0, buffer.Length);

        public override double NextDouble() => 0.0;
    }
}
