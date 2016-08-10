﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MM2Randomizer.Enums;
using MM2Randomizer.Enemies;

namespace MM2Randomizer
{
    public static class RandomMM2
    {
        public static int Seed = -1;
        public static Random Random;
        public static MainWindowViewModel Settings;
        public static string DestinationFileName = "";

        /// <summary>
        /// Perform the randomization based on the seed and user-provided settings, and then
        /// generate the new ROM.
        /// </summary>
        public static void Randomize()
        {
            try
            {
                CopyRom();
                InitializeSeed();

                if (Settings.Is8StagesRandom)
                {
                    RandomStagePtrs();
                }
                if (Settings.IsWeaponsRandom)
                {
                    RandomWeapons();
                }
                if (Settings.IsItemsRandom)
                {
                    RandomItemNums();
                }
                if (Settings.IsTeleportersRandom)
                {
                    RandomTeleporters();
                }
                if (Settings.IsColorsRandom)
                {
                    RandomColors();
                }
                if (Settings.IsWeaknessRandom)
                {
                    // Offsets are different in Rockman 2 and Mega Man 2
                    if (Settings.IsJapanese)
                        RandomWeaknesses();
                    else
                        RandomWeaknessesMM2();
                }
                if (Settings.IsEnemiesRandom)
                {
                    RandomEnemies();
                }

                string newfilename = (Settings.IsJapanese) ? "RM2" : "MM2";
                newfilename = String.Format("{0}-RNG-{1}.nes", newfilename, Seed);

                if (File.Exists(newfilename))
                {
                    File.Delete(newfilename);
                }

                File.Move(DestinationFileName, newfilename);

                string str = string.Format("/select,\"{0}\\{1}\"",
                    Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
                newfilename);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private static void RandomEnemies()
        {
            EnemyRandomizer er = new EnemyRandomizer();

        }

        private static void RandomTeleporters()
        {
            // Create list of default teleporter position values
            List<byte[]> coords = new List<byte[]>
            {
                new byte[]{ 0x20, 0x3B }, // Teleporter X, Y (top-left)
                new byte[]{ 0x20, 0x7B },
                new byte[]{ 0x20, 0xBB },
                new byte[]{ 0x70, 0xBB },
                new byte[]{ 0x90, 0xBB },
                new byte[]{ 0xE0, 0x3B },
                new byte[]{ 0xE0, 0x7B },
                new byte[]{ 0xE0, 0xBB }
            };

            // Randomize them
            coords.Shuffle(Random);

            using (var stream = new FileStream(DestinationFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                // Write the new x-coordinates
                stream.Position = (long) EMiscAddresses.WarpXCoordinateStartAddress;
                foreach (byte[] location in coords)
                {
                    stream.WriteByte(location[0]);
                }

                // Write the new y-coordinates
                stream.Position = (long) EMiscAddresses.WarpYCoordinateStartAddress;
                foreach (byte[] location in coords)
                {
                    stream.WriteByte(location[1]);
                }

                // These values will be copied over to $04b0 (y) and $0470 (x), which will be checked
                // for in real time to determine where Mega will teleport to
            }
        }

        /// <summary>
        /// Create the modified ROM.
        /// </summary>
        private static void CopyRom()
        {
            string srcFile = "";
            if (Settings.IsJapanese)
            {
                srcFile = "j.dat";
            }
            else
            {
                srcFile = "u.dat";
            }
            DestinationFileName = "temp.nes";

            File.Copy(srcFile, DestinationFileName, true);
        }

        /// <summary>
        /// Create a random seed or use the user-provided seed.
        /// </summary>
        private static void InitializeSeed()
        {
            if (Seed < 0)
            {
                Random rndSeed = new Random();
                Seed = rndSeed.Next(int.MaxValue);
            }
            Random = new Random(Seed);
        }

        /// <summary>
        /// TODO
        /// </summary>
        private static void RandomColors()
        {
            List<ColorSet> colorSets = new List<ColorSet>()
            {
                new ColorSet() { // Heat | River 
                    addresses = new int[] { 0x3e3f, 0x3e4f, 0x3e5f,
                                            0x3e40, 0x3e50, 0x3e60,
                                            0x3e41, 0x3e51, 0x3e61 },
                    ColorBytes = new List<byte[]>() {
                        new byte[] {
                            (byte) EColorsHex.Taupe, (byte) EColorsHex.LightOrange, (byte) EColorsHex.Orange,
                            (byte) EColorsHex.LightOrange, (byte) EColorsHex.Orange, (byte) EColorsHex.Taupe,
                            (byte) EColorsHex.Orange, (byte) EColorsHex.Taupe, (byte) EColorsHex.LightOrange,
                        },
                        new byte[] {
                            (byte) EColorsHex.LightGreen, (byte) EColorsHex.Green, (byte) EColorsHex.ForestGreen,
                            (byte) EColorsHex.Green, (byte) EColorsHex.ForestGreen, (byte) EColorsHex.LightGreen,
                            (byte) EColorsHex.ForestGreen, (byte) EColorsHex.LightGreen, (byte) EColorsHex.Green,
                        },
                        new byte[] {
                            (byte) EColorsHex.Yellow, (byte) EColorsHex.GoldenRod, (byte) EColorsHex.Brown,
                            (byte) EColorsHex.GoldenRod, (byte) EColorsHex.Brown, (byte) EColorsHex.Yellow,
                            (byte) EColorsHex.Brown, (byte) EColorsHex.Yellow, (byte) EColorsHex.GoldenRod,
                        },
                        new byte[] {
                            (byte) EColorsHex.LightPink, (byte) EColorsHex.Magenta, (byte) EColorsHex.DarkMagenta,
                            (byte) EColorsHex.Magenta, (byte) EColorsHex.DarkMagenta, (byte) EColorsHex.LightPink,
                            (byte) EColorsHex.DarkMagenta, (byte) EColorsHex.LightPink, (byte) EColorsHex.Magenta,
                        }
                    }
                },
                new ColorSet() { // Heat | Background
                    addresses = new int[] { 0x3e3b, 0x3e4b, 0x3e5b,
                                            0x3e3c, 0x3e4c, 0x3e5c,
                                            0x3e3d, 0x3e4d, 0x3e5d },
                    ColorBytes = new List<byte[]>() {
                        new byte[] { 
                            (byte) EColorsHex.Orange, (byte) EColorsHex.Orange, (byte) EColorsHex.Orange,
                            (byte) EColorsHex.Crimson, (byte) EColorsHex.Crimson, (byte) EColorsHex.Crimson,
                            (byte) EColorsHex.DarkRed, (byte) EColorsHex.DarkRed, (byte) EColorsHex.DarkRed,
                        },
                        new byte[] { 
                            (byte) EColorsHex.Magenta, (byte) EColorsHex.Magenta, (byte) EColorsHex.Magenta,
                            (byte) EColorsHex.DarkMagenta, (byte) EColorsHex.DarkMagenta, (byte) EColorsHex.DarkMagenta,
                            (byte) EColorsHex.RoyalPurple, (byte) EColorsHex.RoyalPurple, (byte) EColorsHex.RoyalPurple,
                        },
                        new byte[] { 
                            (byte) EColorsHex.MediumGray, (byte) EColorsHex.MediumGray, (byte) EColorsHex.MediumGray,
                            (byte) EColorsHex.GoldenRod, (byte) EColorsHex.GoldenRod, (byte) EColorsHex.GoldenRod,
                            (byte) EColorsHex.Brown, (byte) EColorsHex.Brown, (byte) EColorsHex.Brown,
                        },
                        new byte[] { 
                            (byte) EColorsHex.DarkTeal, (byte) EColorsHex.DarkTeal, (byte) EColorsHex.DarkTeal,
                            (byte) EColorsHex.RoyalBlue, (byte) EColorsHex.RoyalBlue, (byte) EColorsHex.RoyalBlue,
                            (byte) EColorsHex.Blue, (byte) EColorsHex.Blue, (byte) EColorsHex.Blue,
                        },
                        new byte[] { 
                            (byte) EColorsHex.DarkGreen, (byte) EColorsHex.DarkGreen, (byte) EColorsHex.DarkGreen,
                            (byte) EColorsHex.Black3, (byte) EColorsHex.Black3, (byte) EColorsHex.Black3,
                            (byte) EColorsHex.Kelp, (byte) EColorsHex.Kelp, (byte) EColorsHex.Kelp,
                        },
                    }
                },
                new ColorSet() { // Heat | Foreground
                    addresses = new int[] { 0x3e33, 0x3e43, 0x3e53,
                                            0x3e34, 0x3e44, 0x3e54,
                                            0x3e35, 0x3e45, 0x3e55 },
                    ColorBytes = new List<byte[]>() {
                        new byte[] { 
                            (byte) EColorsHex.Taupe, (byte) EColorsHex.Taupe, (byte) EColorsHex.Taupe,
                            (byte) EColorsHex.LightOrange, (byte) EColorsHex.LightOrange, (byte) EColorsHex.LightOrange,
                            (byte) EColorsHex.VioletRed, (byte) EColorsHex.VioletRed, (byte) EColorsHex.VioletRed,
                        },
                        new byte[] { 
                            (byte) EColorsHex.PastelPink, (byte) EColorsHex.PastelPink, (byte) EColorsHex.PastelPink,
                            (byte) EColorsHex.LightPink, (byte) EColorsHex.LightPink, (byte) EColorsHex.LightPink,
                            (byte) EColorsHex.Purple, (byte) EColorsHex.Purple, (byte) EColorsHex.Purple,
                        },
                        new byte[] { 
                            (byte) EColorsHex.PastelGreen, (byte) EColorsHex.PastelGreen, (byte) EColorsHex.PastelGreen,
                            (byte) EColorsHex.LightGreen, (byte) EColorsHex.LightGreen, (byte) EColorsHex.LightGreen,
                            (byte) EColorsHex.Grass, (byte) EColorsHex.Grass, (byte) EColorsHex.Grass,
                        },
                        new byte[] { 
                            (byte) EColorsHex.PaleBlue, (byte) EColorsHex.PaleBlue, (byte) EColorsHex.PaleBlue,
                            (byte) EColorsHex.SoftBlue, (byte) EColorsHex.SoftBlue, (byte) EColorsHex.SoftBlue,
                            (byte) EColorsHex.MediumBlue, (byte) EColorsHex.MediumBlue, (byte) EColorsHex.MediumBlue,
                        },
                    }
                },
                new ColorSet() { // Heat | Foreground2
                    addresses = new int[] { 0x3e37, 0x3e47, 0x3e57,
                                            0x3e38, 0x3e48, 0x3e58,
                                            0x3e39, 0x3e49, 0x3e59 },
                    ColorBytes = new List<byte[]>() {
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White,
                            (byte) EColorsHex.LightGray, (byte) EColorsHex.LightGray, (byte) EColorsHex.LightGray,
                            (byte) EColorsHex.Gray, (byte) EColorsHex.Gray, (byte) EColorsHex.Gray,
                        },
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White,
                            (byte) EColorsHex.Beige, (byte) EColorsHex.Beige, (byte) EColorsHex.Beige,
                            (byte) EColorsHex.YellowOrange, (byte) EColorsHex.YellowOrange, (byte) EColorsHex.YellowOrange,
                        },
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White,
                            (byte) EColorsHex.PastelBlue, (byte) EColorsHex.PastelBlue, (byte) EColorsHex.PastelBlue,
                            (byte) EColorsHex.LightBlue, (byte) EColorsHex.LightBlue, (byte) EColorsHex.LightBlue,
                        },
                        new byte[] { 
                            (byte) EColorsHex.LightGray, (byte) EColorsHex.LightGray, (byte) EColorsHex.LightGray,
                            (byte) EColorsHex.Gray, (byte) EColorsHex.Gray, (byte) EColorsHex.Gray,
                            (byte) EColorsHex.Brown, (byte) EColorsHex.Brown, (byte) EColorsHex.Brown,
                        },
                    }
                },
                new ColorSet() { // Air | Platforms
                    addresses = new int[] { 0x7e37, 0x7e47, 0x7e57, 0x7e67,
                                            0x7e38, 0x7e48, 0x7e58, 0x7e68,
                                            0x7e39, 0x7e49, 0x7e59, 0x7e69 },
                    ColorBytes = new List<byte[]>() {
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White,
                            (byte) EColorsHex.VioletRed, (byte) EColorsHex.VioletRed, (byte) EColorsHex.VioletRed, (byte) EColorsHex.VioletRed,
                            (byte) EColorsHex.Black2, (byte) EColorsHex.Black2, (byte) EColorsHex.Black2, (byte) EColorsHex.Black2,
                        },
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White,
                            (byte) EColorsHex.Orange, (byte) EColorsHex.Orange, (byte) EColorsHex.Orange, (byte) EColorsHex.Orange,
                            (byte) EColorsHex.Black2, (byte) EColorsHex.Black2, (byte) EColorsHex.Black2, (byte) EColorsHex.Black2,
                        },
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White,
                            (byte) EColorsHex.Yellow, (byte) EColorsHex.Yellow, (byte) EColorsHex.Yellow, (byte) EColorsHex.Yellow,
                            (byte) EColorsHex.Black2, (byte) EColorsHex.Black2, (byte) EColorsHex.Black2, (byte) EColorsHex.Black2,
                        },
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White,
                            (byte) EColorsHex.Grass, (byte) EColorsHex.Grass, (byte) EColorsHex.Grass, (byte) EColorsHex.Grass,
                            (byte) EColorsHex.Black2, (byte) EColorsHex.Black2, (byte) EColorsHex.Black2, (byte) EColorsHex.Black2,
                        },
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White,
                            (byte) EColorsHex.SoftBlue, (byte) EColorsHex.SoftBlue, (byte) EColorsHex.SoftBlue, (byte) EColorsHex.SoftBlue,
                            (byte) EColorsHex.Black2, (byte) EColorsHex.Black2, (byte) EColorsHex.Black2, (byte) EColorsHex.Black2,
                        },
                    }
                },
                new ColorSet() { // Air | Clouds 
                    addresses = new int[] { 0x7e33, 0x7e43, 0x7e53, 0x7e63,
                                            0x7e34, 0x7e44, 0x7e54, 0x7e64,
                                            0x7e35, 0x7e45, 0x7e55, 0x7e65 },
                    ColorBytes = new List<byte[]>() {
                        new byte[] {
                            (byte) EColorsHex.LightBlue, (byte) EColorsHex.PastelBlue, (byte) EColorsHex.White, (byte) EColorsHex.PastelBlue,
                            (byte) EColorsHex.PastelBlue, (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White,
                            (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White, (byte) EColorsHex.White,
                        },
                        new byte[] { 
                            (byte) EColorsHex.LightGray, (byte) EColorsHex.Gray, (byte) EColorsHex.DarkRed, (byte) EColorsHex.Gray,
                            (byte) EColorsHex.LightGray, (byte) EColorsHex.LightGray, (byte) EColorsHex.Gray, (byte) EColorsHex.LightGray,
                            (byte) EColorsHex.LightGray, (byte) EColorsHex.LightGray, (byte) EColorsHex.LightGray, (byte) EColorsHex.LightGray,
                        },
                    }
                },
                new ColorSet() { // Air | Sky 
                    addresses = new int[] { 0x7e22 },
                    ColorBytes = new List<byte[]>() {
                        new byte[] { (byte) EColorsHex.LightBlue },
                        new byte[] { (byte) EColorsHex.LightPurple },
                        new byte[] { (byte) EColorsHex.LightOrange },
                        new byte[] { (byte) EColorsHex.YellowOrange },
                        new byte[] { (byte) EColorsHex.Yellow },
                        new byte[] { (byte) EColorsHex.Lime },
                        new byte[] { (byte) EColorsHex.RoyalBlue },
                        new byte[] { (byte) EColorsHex.RoyalBlue },
                        new byte[] { (byte) EColorsHex.DarkGreen },
                        new byte[] { (byte) EColorsHex.Black3 },
                    }
                },
                new ColorSet() { // Flash | Background
                    addresses = new int[] { 0x017e14, 0x017e15,
                                            0x017e34, 0x017e44, 0x017e54,
                                            0x017e35, 0x017e45, 0x017e55 },
                    ColorBytes = new List<byte[]>() {
                        new byte[] {
                            (byte) EColorsHex.Blue, (byte) EColorsHex.DarkBlue,
                            (byte) EColorsHex.Blue, (byte) EColorsHex.Blue, (byte) EColorsHex.Blue,
                            (byte) EColorsHex.DarkBlue, (byte) EColorsHex.DarkBlue, (byte) EColorsHex.DarkBlue,
                        },
                        new byte[] {
                            (byte) EColorsHex.Magenta, (byte) EColorsHex.DarkMagenta,
                            (byte) EColorsHex.Magenta, (byte) EColorsHex.Magenta, (byte) EColorsHex.Magenta,
                            (byte) EColorsHex.DarkMagenta, (byte) EColorsHex.DarkMagenta, (byte) EColorsHex.DarkMagenta,
                        },
                        new byte[] {
                            (byte) EColorsHex.Orange, (byte) EColorsHex.Red,
                            (byte) EColorsHex.Orange, (byte) EColorsHex.Orange, (byte) EColorsHex.Orange,
                            (byte) EColorsHex.Red, (byte) EColorsHex.Red, (byte) EColorsHex.Red,
                        },
                        new byte[] {
                            (byte) EColorsHex.GoldenRod, (byte) EColorsHex.Brown,
                            (byte) EColorsHex.GoldenRod, (byte) EColorsHex.GoldenRod, (byte) EColorsHex.GoldenRod,
                            (byte) EColorsHex.Brown, (byte) EColorsHex.Brown, (byte) EColorsHex.Brown,
                        },
                        new byte[] {
                            (byte) EColorsHex.Green, (byte) EColorsHex.ForestGreen,
                            (byte) EColorsHex.Green, (byte) EColorsHex.Green, (byte) EColorsHex.Green,
                            (byte) EColorsHex.ForestGreen, (byte) EColorsHex.ForestGreen, (byte) EColorsHex.ForestGreen,
                        },
                        new byte[] {
                            (byte) EColorsHex.Gray, (byte) EColorsHex.Black3,
                            (byte) EColorsHex.Gray, (byte) EColorsHex.Gray, (byte) EColorsHex.Gray,
                            (byte) EColorsHex.Black3, (byte) EColorsHex.Black3, (byte) EColorsHex.Black3,
                        },
                    }
                },
                new ColorSet() { // Flash | Foreground
                    // Note: 3 color sets are used for the flashing blocks.
                    // I've kept them grouped here for common color themes.
                    addresses = new int[] { 0x017e37, 0x017e47, 0x017e57,
                                            0x017e38, 0x017e48, 0x017e58,
                                            0x017e39, 0x017e49, 0x017e59,
                                            0x017e3b, 0x017e4b, 0x017e5b,
                                            0x017e3c, 0x017e4c, 0x017e5c,
                                            0x017e3d, 0x017e4d, 0x017e5d,
                                            0x017e3f, 0x017e4f, 0x017e5f,
                                            0x017e40, 0x017e50, 0x017e60,
                                            0x017e41, 0x017e51, 0x017e61},
                    ColorBytes = new List<byte[]>() {
                         new byte[] {
                            (byte) EColorsHex.White, (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.PastelBlue, (byte) EColorsHex.LightBlue, (byte) EColorsHex.LightBlue,
                            (byte) EColorsHex.LightCyan, (byte) EColorsHex.MediumBlue, (byte) EColorsHex.MediumBlue,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.White, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.LightBlue, (byte) EColorsHex.PastelBlue, (byte) EColorsHex.LightBlue,
                            (byte) EColorsHex.MediumBlue, (byte) EColorsHex.LightCyan, (byte) EColorsHex.MediumBlue,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite, (byte) EColorsHex.White,
                            (byte) EColorsHex.LightBlue, (byte) EColorsHex.LightBlue, (byte) EColorsHex.PastelBlue,
                            (byte) EColorsHex.MediumBlue, (byte) EColorsHex.MediumBlue, (byte) EColorsHex.LightCyan,
                        },
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.PastelPurple, (byte) EColorsHex.LightPurple, (byte) EColorsHex.LightPurple,
                            (byte) EColorsHex.LightPink, (byte) EColorsHex.Purple, (byte) EColorsHex.Purple,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.White, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.LightPurple, (byte) EColorsHex.PastelPurple, (byte) EColorsHex.LightPurple,
                            (byte) EColorsHex.Purple, (byte) EColorsHex.LightPink, (byte) EColorsHex.Purple,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite, (byte) EColorsHex.White,
                            (byte) EColorsHex.LightPurple, (byte) EColorsHex.LightPurple, (byte) EColorsHex.PastelPurple,
                            (byte) EColorsHex.Purple, (byte) EColorsHex.Purple, (byte) EColorsHex.LightPink,
                        },
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.Taupe, (byte) EColorsHex.LightOrange, (byte) EColorsHex.Orange,
                            (byte) EColorsHex.LightOrange, (byte) EColorsHex.Orange, (byte) EColorsHex.Red,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.White, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.Orange, (byte) EColorsHex.Taupe, (byte) EColorsHex.LightOrange,
                            (byte) EColorsHex.Red, (byte) EColorsHex.LightOrange, (byte) EColorsHex.Orange,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite, (byte) EColorsHex.White,
                            (byte) EColorsHex.LightOrange, (byte) EColorsHex.Orange, (byte) EColorsHex.Taupe,
                            (byte) EColorsHex.Orange, (byte) EColorsHex.Red, (byte) EColorsHex.LightOrange,
                        },
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.PastelYellow, (byte) EColorsHex.Yellow, (byte) EColorsHex.GoldenRod,
                            (byte) EColorsHex.Yellow, (byte) EColorsHex.GoldenRod, (byte) EColorsHex.Brown,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.White, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.GoldenRod, (byte) EColorsHex.PastelYellow, (byte) EColorsHex.Yellow,
                            (byte) EColorsHex.Brown, (byte) EColorsHex.Yellow, (byte) EColorsHex.GoldenRod,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite, (byte) EColorsHex.White,
                            (byte) EColorsHex.Yellow, (byte) EColorsHex.GoldenRod, (byte) EColorsHex.PastelYellow,
                            (byte) EColorsHex.GoldenRod, (byte) EColorsHex.Brown, (byte) EColorsHex.Yellow,
                        },
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.PastelGreen, (byte) EColorsHex.LightGreen, (byte) EColorsHex.Green,
                            (byte) EColorsHex.LightGreen, (byte) EColorsHex.Green, (byte) EColorsHex.ForestGreen,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.White, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.Green, (byte) EColorsHex.PastelGreen, (byte) EColorsHex.LightGreen,
                            (byte) EColorsHex.ForestGreen, (byte) EColorsHex.LightGreen, (byte) EColorsHex.Green,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite, (byte) EColorsHex.White,
                            (byte) EColorsHex.LightGreen, (byte) EColorsHex.Green, (byte) EColorsHex.PastelGreen,
                            (byte) EColorsHex.Green, (byte) EColorsHex.ForestGreen, (byte) EColorsHex.LightGreen,
                        },
                        new byte[] { 
                            (byte) EColorsHex.White, (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.PastelCyan, (byte) EColorsHex.Lime, (byte) EColorsHex.Moss,
                            (byte) EColorsHex.Lime, (byte) EColorsHex.Moss, (byte) EColorsHex.DarkGreen,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.White, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.Moss, (byte) EColorsHex.PastelCyan, (byte) EColorsHex.Lime,
                            (byte) EColorsHex.DarkGreen, (byte) EColorsHex.Lime, (byte) EColorsHex.Moss,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite, (byte) EColorsHex.White,
                            (byte) EColorsHex.Lime, (byte) EColorsHex.Moss, (byte) EColorsHex.PastelCyan,
                            (byte) EColorsHex.Moss, (byte) EColorsHex.DarkGreen, (byte) EColorsHex.Lime,
                        },
                        new byte[] {
                            (byte) EColorsHex.White, (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.Black3, (byte) EColorsHex.Black3, (byte) EColorsHex.Black3,
                            (byte) EColorsHex.LightGray, (byte) EColorsHex.Gray, (byte) EColorsHex.DarkRed,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.White, (byte) EColorsHex.NearWhite,
                            (byte) EColorsHex.Black3, (byte) EColorsHex.Black3, (byte) EColorsHex.Black3,
                            (byte) EColorsHex.DarkRed, (byte) EColorsHex.LightGray, (byte) EColorsHex.Gray,
                            (byte) EColorsHex.NearWhite, (byte) EColorsHex.NearWhite, (byte) EColorsHex.White,
                            (byte) EColorsHex.Black3, (byte) EColorsHex.Black3, (byte) EColorsHex.Black3,
                            (byte) EColorsHex.Gray, (byte) EColorsHex.DarkRed, (byte) EColorsHex.LightGray,
                        },
                    }
                },
                // TODO: Comment later, missing a color in boss corridor
                new ColorSet() {
                    addresses = new int[] { 0x01fe13, 0x01fe14, 0x03b63a, 0x03b63b, 0x03b642, 0x03b643, 0x03b646, 0x03b647, 0x03b64a, 0x03b64b, 0x03b64e, 0x03b64f, 0x039188, 0x039189, 0x03918c, 0x03918d, },
                    ColorBytes = new List<byte[]>() {
                        // Clash | Border1 | Default
                        new byte[] { (byte) EColorsHex.PastelLemon,(byte) EColorsHex.GoldenRod,(byte) EColorsHex.PastelLemon,(byte) EColorsHex.GoldenRod,(byte) EColorsHex.PastelLemon,(byte) EColorsHex.GoldenRod,(byte) EColorsHex.PastelLemon,(byte) EColorsHex.GoldenRod,(byte) EColorsHex.PastelLemon,(byte) EColorsHex.GoldenRod,(byte) EColorsHex.PastelLemon,(byte) EColorsHex.GoldenRod,(byte) EColorsHex.PastelLemon,(byte) EColorsHex.GoldenRod,(byte) EColorsHex.PastelLemon,(byte) EColorsHex.GoldenRod,},
                        // Clash | Border1 | Blue
                        new byte[] { (byte) EColorsHex.MediumBlue,(byte) EColorsHex.RoyalBlue,(byte) EColorsHex.MediumBlue,(byte) EColorsHex.RoyalBlue,(byte) EColorsHex.MediumBlue,(byte) EColorsHex.RoyalBlue,(byte) EColorsHex.MediumBlue,(byte) EColorsHex.RoyalBlue,(byte) EColorsHex.MediumBlue,(byte) EColorsHex.RoyalBlue,(byte) EColorsHex.MediumBlue,(byte) EColorsHex.RoyalBlue,(byte) EColorsHex.MediumBlue,(byte) EColorsHex.RoyalBlue,(byte) EColorsHex.MediumBlue,(byte) EColorsHex.RoyalBlue,},
                        // Clash | Border1 | Orange
                        new byte[] { (byte) EColorsHex.YellowOrange,(byte) EColorsHex.Orange,(byte) EColorsHex.YellowOrange,(byte) EColorsHex.Orange,(byte) EColorsHex.YellowOrange,(byte) EColorsHex.Orange,(byte) EColorsHex.YellowOrange,(byte) EColorsHex.Orange,(byte) EColorsHex.YellowOrange,(byte) EColorsHex.Orange,(byte) EColorsHex.YellowOrange,(byte) EColorsHex.Orange,(byte) EColorsHex.YellowOrange,(byte) EColorsHex.Orange,(byte) EColorsHex.YellowOrange,(byte) EColorsHex.Orange,},
                        // Clash | Border1 | Green
                        new byte[] { (byte) EColorsHex.Lime,(byte) EColorsHex.ForestGreen,(byte) EColorsHex.Lime,(byte) EColorsHex.ForestGreen,(byte) EColorsHex.Lime,(byte) EColorsHex.ForestGreen,(byte) EColorsHex.Lime,(byte) EColorsHex.ForestGreen,(byte) EColorsHex.Lime,(byte) EColorsHex.ForestGreen,(byte) EColorsHex.Lime,(byte) EColorsHex.ForestGreen,(byte) EColorsHex.Lime,(byte) EColorsHex.ForestGreen,(byte) EColorsHex.Lime,(byte) EColorsHex.ForestGreen,},
                        // Clash | Border1 | Red Black
                        new byte[] { (byte) EColorsHex.Black2,(byte) EColorsHex.Red,(byte) EColorsHex.Black2,(byte) EColorsHex.Red,(byte) EColorsHex.Black2,(byte) EColorsHex.Red,(byte) EColorsHex.Black2,(byte) EColorsHex.Red,(byte) EColorsHex.Black2,(byte) EColorsHex.Red,(byte) EColorsHex.Black2,(byte) EColorsHex.Red,(byte) EColorsHex.Black2,(byte) EColorsHex.Red,(byte) EColorsHex.Black2,(byte) EColorsHex.Red,},
                    }
                },
                new ColorSet() {
                    addresses = new int[] { 0x01fe15,0x01fe17,0x03b63c,0x03b644,0x03b648,0x03b64c,0x03b650,},
                    ColorBytes = new List<byte[]>() {
                        // Clash | Background | Default
                        new byte[] { (byte) EColorsHex.Blue,(byte) EColorsHex.Blue,(byte) EColorsHex.RoyalBlue,(byte) EColorsHex.RoyalBlue,(byte) EColorsHex.Black2,(byte) EColorsHex.Black2,(byte) EColorsHex.Black2,},
                        // Clash | Background | Yellow
                        new byte[] { (byte) EColorsHex.Yellow,(byte) EColorsHex.Yellow,(byte) EColorsHex.Brown,(byte) EColorsHex.Brown,(byte) EColorsHex.Black2,(byte) EColorsHex.Black2,(byte) EColorsHex.Black2,},
                        // Clash | Background | Orange
                        new byte[] { (byte) EColorsHex.Orange,(byte) EColorsHex.Orange,(byte) EColorsHex.Red,(byte) EColorsHex.Red,(byte) EColorsHex.Black2,(byte) EColorsHex.Black2,(byte) EColorsHex.Black2,},
                        // Clash | Background | Green
                        new byte[] { (byte) EColorsHex.Lime,(byte) EColorsHex.Lime,(byte) EColorsHex.Moss,(byte) EColorsHex.Moss,(byte) EColorsHex.Black2,(byte) EColorsHex.Black2,(byte) EColorsHex.Black2,},
                        // Clash | Background | Purple
                        new byte[] { (byte) EColorsHex.LightPink,(byte) EColorsHex.LightPink,(byte) EColorsHex.DarkMagenta,(byte) EColorsHex.DarkMagenta,(byte) EColorsHex.Black2,(byte) EColorsHex.Black2,(byte) EColorsHex.Black2,}
                    }
                },
                new ColorSet() {
                    addresses = new int[] { 0x01fe18,0x01fe19, },
                    ColorBytes = new List<byte[]>() {
                        // Clash | Doodads | Default
                        new byte[] { (byte) EColorsHex.YellowOrange,(byte) EColorsHex.NearWhite,},
                        // Clash | Doodads | Green
                        new byte[] { (byte) EColorsHex.Green,(byte) EColorsHex.NearWhite,},
                        // Clash | Doodads | Teal
                        new byte[] { (byte) EColorsHex.Teal,(byte) EColorsHex.NearWhite,},
                        // Clash | Doodads | Purple
                        new byte[] { (byte) EColorsHex.Purple,(byte) EColorsHex.NearWhite,},
                        // Clash | Doodads | Red
                        new byte[] { (byte) EColorsHex.Crimson,(byte) EColorsHex.NearWhite,},
                        // Clash | Doodads | Gray
                        new byte[] { (byte) EColorsHex.Gray,(byte) EColorsHex.NearWhite,},
                    }
                },
                new ColorSet() {
                    addresses = new int[] { 0xbe13, 0xbe14, },
                    ColorBytes = new List<byte[]>() {
                        // Wood | Leaves | Default
                        new byte[] { (byte) EColorsHex.Lemon,(byte) EColorsHex.Grass,},
                        // Wood | Leaves | Blue
                        new byte[] { (byte) EColorsHex.MediumBlue,(byte) EColorsHex.RoyalBlue,},
                        // Wood | Leaves | Red
                        new byte[] { (byte) EColorsHex.Orange,(byte) EColorsHex.Red,},
                    }
                },

                new ColorSet() {
                    addresses = new int[] { 0xbe17, 0xbe18, },
                    ColorBytes = new List<byte[]>() {
                        // Wood | Trunk | Default
                        new byte[] { (byte) EColorsHex.Yellow, (byte) EColorsHex.GoldenRod },
                        // Wood | Trunk | Purple
                        new byte[] { (byte) EColorsHex.LightPurple, (byte) EColorsHex.Purple },
                        // Wood | Trunk | Pink
                        new byte[] { (byte) EColorsHex.LightVioletRed, (byte) EColorsHex.VioletRed },
                        // Wood | Trunk | Orange
                        new byte[] { (byte) EColorsHex.YellowOrange, (byte) EColorsHex.Tangerine },
                        // Wood | Trunk | Green
                        new byte[] { (byte) EColorsHex.LightGreen, (byte) EColorsHex.Green },
                        // Wood | Trunk | Teal
                        new byte[] { (byte) EColorsHex.LightCyan, (byte) EColorsHex.Teal },
                    }
                },

                new ColorSet() {
                    addresses = new int[] { 0xbe1b,0xbe1c,0xbe1d,},
                    ColorBytes = new List<byte[]>() {
                        // Wood | Floor | Default
                        new byte[] { (byte) EColorsHex.YellowOrange,(byte) EColorsHex.Tangerine,(byte) EColorsHex.DarkRed,},
                        // Wood | Floor | Yellow
                        new byte[] { (byte) EColorsHex.Yellow,(byte) EColorsHex.GoldenRod,(byte) EColorsHex.Brown,},
                        // Wood | Floor | Green
                        new byte[] { (byte) EColorsHex.LightGreen,(byte) EColorsHex.Green,(byte) EColorsHex.ForestGreen,},
                        // Wood | Floor | Teal
                        new byte[] { (byte) EColorsHex.LightCyan,(byte) EColorsHex.Teal,(byte) EColorsHex.DarkTeal,},
                        // Wood | Floor | Purple
                        new byte[] { (byte) EColorsHex.LightPurple,(byte) EColorsHex.Purple,(byte) EColorsHex.RoyalPurple,},
                        // Wood | Floor | Gray
                        new byte[] { (byte) EColorsHex.NearWhite,(byte) EColorsHex.LightGray,(byte) EColorsHex.Black2,},
                    }
                },

                new ColorSet() {
                    addresses = new int[] { 0xbe1f, 0x03a118, },
                    ColorBytes = new List<byte[]>() {
                        // Wood | UndergroundBG | Default
                        new byte[] { (byte) EColorsHex.Brown, (byte) EColorsHex.Brown },
                        // Wood | UndergroundBG | Dark Purple
                        new byte[] { (byte) EColorsHex.DarkMagenta, (byte) EColorsHex.DarkMagenta },
                        // Wood | UndergroundBG | Dark Red
                        new byte[] { (byte) EColorsHex.Crimson, (byte) EColorsHex.Crimson },
                        // Wood | UndergroundBG | Dark Green
                        new byte[] { (byte) EColorsHex.Kelp, (byte) EColorsHex.Kelp },
                        // Wood | UndergroundBG | Dark Teal
                        new byte[] { (byte) EColorsHex.DarkGreen, (byte) EColorsHex.DarkGreen },
                        // Wood | UndergroundBG | Dark Blue1
                        new byte[] { (byte) EColorsHex.DarkTeal, (byte) EColorsHex.DarkTeal },
                        // Wood | UndergroundBG | Dark Blue2
                        new byte[] { (byte) EColorsHex.RoyalBlue, (byte) EColorsHex.RoyalBlue },
                    }
                },

                new ColorSet() {
                    addresses = new int[] { 0xbe15,0xbe19,},
                    ColorBytes = new List<byte[]>() {
                        // Wood | SkyBG | Default
                        new byte[] { (byte) EColorsHex.LightCyan, (byte) EColorsHex.LightCyan },
                        // Wood | SkyBG | Light Green
                        new byte[] { (byte) EColorsHex.LightGreen ,(byte) EColorsHex.LightGreen },
                        // Wood | SkyBG | Blue
                        new byte[] { (byte) EColorsHex.Blue, (byte) EColorsHex.Blue },
                        // Wood | SkyBG | Dark Purple
                        new byte[] { (byte) EColorsHex.RoyalPurple, (byte) EColorsHex.RoyalPurple },
                        // Wood | SkyBG | Dark Red
                        new byte[] { (byte) EColorsHex.Crimson, (byte) EColorsHex.Crimson },
                        // Wood | SkyBG | Light Yellow
                        new byte[] { (byte) EColorsHex.PastelYellow, (byte) EColorsHex.PastelYellow },
                        // Wood | SkyBG | Black
                        new byte[] { (byte) EColorsHex.Black2, (byte) EColorsHex.Black2 },
                    }
                },
            };

            using (var stream = new FileStream(DestinationFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                foreach (ColorSet set in colorSets)
                {
                    set.RandomizeAndWrite(stream, Random);
                }
            }
        }

        /// <summary>
        /// Shuffle which Robot Master awards which weapon.
        /// </summary>
        private static void RandomWeapons()
        {
            // StageBeat    Address    Value
            // -----------------------------
            // Heat Man     0x03C289   1
            // Air Man      0x03C28A   2
            // Wood Man     0x03C28B   4
            // Bubble Man   0x03C28C   8
            // Quick Man    0x03C28D   16
            // Flash Man    0x03C28E   32
            // Metal Man    0x03C28F   64
            // Crash Man    0x03C290   128

            var newWeaponOrder = new List<ERMWeaponValue>()
            {
                ERMWeaponValue.HeatMan,
                ERMWeaponValue.AirMan,
                ERMWeaponValue.WoodMan,
                ERMWeaponValue.BubbleMan,
                ERMWeaponValue.QuickMan,
                ERMWeaponValue.FlashMan,
                ERMWeaponValue.MetalMan,
                ERMWeaponValue.CrashMan
            }.Select(s => (byte)s).ToList();
            
            newWeaponOrder.Shuffle(Random);

            using (var stream = new FileStream(DestinationFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                // Create table for which weapon is awarded by which robot master
                // This also affects which portrait is blacked out on the stage select
                // This also affects which teleporter deactivates after defeating a Wily 5 refight boss
                stream.Position = (long) ERMStageWeaponAddress.HeatMan;// 0x03c289;
                for (int i = 0; i < 8; i++)
                {
                    stream.WriteByte((byte)newWeaponOrder[i]);
                }

                // Create a copy of the default weapon order table to be used by teleporter function
                // This is needed to fix teleporters breaking from the new weapon order.
                stream.Position = 0x03f2D0; // Unused space at end of bank
                stream.WriteByte((byte) ERMWeaponValue.HeatMan);
                stream.WriteByte((byte) ERMWeaponValue.AirMan);
                stream.WriteByte((byte) ERMWeaponValue.WoodMan);
                stream.WriteByte((byte) ERMWeaponValue.BubbleMan);
                stream.WriteByte((byte) ERMWeaponValue.QuickMan);
                stream.WriteByte((byte) ERMWeaponValue.FlashMan);
                stream.WriteByte((byte) ERMWeaponValue.MetalMan);
                stream.WriteByte((byte) ERMWeaponValue.CrashMan);

                // Change function to call $f2c0 instead of $c279 when looking up defeated refight boss to
                // get our default weapon table, fixing the teleporter softlock
                stream.Position = 0x03843b;
                stream.WriteByte(0xc0);
                stream.WriteByte(0xf2);

                // Create table for which stage is selectable on the stage select screen (independent of it being blacked out)
                stream.Position = (long) ERMStageSelect.FirstStageInMemory; // 0x0346E1;
                for (int i = 0; i < 8; i++)
                {
                    stream.WriteByte((byte)newWeaponOrder[i]);
                }
            }
        }

        /// <summary>
        /// Shuffle which Robot Master awards Items 1, 2, and 3.
        /// </summary>
        private static void RandomItemNums()
        {
            // 0x03C291 - Item # from Heat Man
            // 0x03C292 - Item # from Air Man
            // 0x03C293 - Item # from Wood Man
            // 0x03C294 - Item # from Bubble Man
            // 0x03C295 - Item # from Quick Man
            // 0x03C296 - Item # from Flash Man
            // 0x03C297 - Item # from Metal Man
            // 0x03C298 - Item # from Crash Man

            List<byte> newItemOrder = new List<byte>();
            for (byte i = 0; i < 5; i++) newItemOrder.Add((byte) EItemNumber.None);
            newItemOrder.Add((byte) EItemNumber.One);
            newItemOrder.Add((byte) EItemNumber.Two);
            newItemOrder.Add((byte) EItemNumber.Three);

            newItemOrder.Shuffle(Random);

            using (var stream = new FileStream(DestinationFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                stream.Position = (long) EItemStageAddress.HeatMan; //0x03C291;
                for (int i = 0; i < 8; i++)
                { 
                    stream.WriteByte((byte)newItemOrder[i]);
                }


            }
        }

        /// <summary>
        /// Modify the damage values of each weapon against each Robot Master for Rockman 2 (J).
        /// </summary>
        private static void RandomWeaknesses()
        {
            List<WeaponTable> Weapons = new List<WeaponTable>();

            Weapons.Add(new WeaponTable()
            {
                Name = "Buster",
                ID = 0,
                Address = ERMWeaponAddress.Buster,
                RobotMasters = new int[8] { 2,2,1,1,2,2,1,1 }
                // Heat = 2,
                // Air = 2,
                // Wood = 1,
                // Bubble = 1,
                // Quick = 2,
                // Flash = 2,
                // Metal = 1,
                // Clash = 1,
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Atomic Fire",
                ID = 1,
                Address = ERMWeaponAddress.AtomicFire,
                // Note: These values only affect a fully charged shot.  Partially charged shots use the Buster table.
                RobotMasters = new int[8] { 0xFF, 6, 0x0E, 0, 0x0A, 6, 4, 6 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Air Shooter",
                ID = 2,
                Address = ERMWeaponAddress.AirShooter,
                RobotMasters = new int[8] {2, 0, 4, 0, 2, 0, 0, 0x0A }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Leaf Shield",
                ID = 3,
                Address = ERMWeaponAddress.LeafShield,
                RobotMasters = new int[8] { 0, 8, 0xFF, 0, 0, 0, 0, 0 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Bubble Lead",
                ID = 4,
                Address = ERMWeaponAddress.BubbleLead,
                RobotMasters = new int[8] { 6, 0, 0, 0xFF, 0, 2, 0, 1 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Quick Boomerang",
                ID = 5,
                Address = ERMWeaponAddress.QuickBoomerang,
                RobotMasters = new int[8] { 2, 2, 0, 2, 0, 0, 4, 1 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Time Stopper",
                ID = 6,
                Address = ERMWeaponAddress.TimeStopper,
                // NOTE: These values affect damage per tick
                RobotMasters = new int[8] { 0, 0, 0, 0, 1, 0, 0, 0 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Metal Blade",
                ID = 7,
                Address = ERMWeaponAddress.MetalBlade,
                RobotMasters = new int[8] { 1, 0, 2, 4, 0, 4, 0x0E, 0 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Clash Bomber",
                ID = 8,
                Address = ERMWeaponAddress.ClashBomber,
                RobotMasters = new int[8] { 0xFF, 0, 2, 2, 4, 3, 0, 0 }
            });

            foreach (WeaponTable weapon in Weapons)
            {
                weapon.RobotMasters.Shuffle(Random);
            }

            using (var stream = new FileStream(DestinationFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                foreach (WeaponTable weapon in Weapons)
                {
                    stream.Position = (long)weapon.Address;
                    for (int i = 0; i < 8; i++)
                    { 
                        stream.WriteByte((byte)weapon.RobotMasters[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Identical to RandomWeaknesses() but using Mega Man 2 (U).nes offsets
        /// </summary>
        private static void RandomWeaknessesMM2()
        {
            List<WeaponTable> Weapons = new List<WeaponTable>();

            Weapons.Add(new WeaponTable()
            {
                Name = "Buster",
                ID = 0,
                Address = ERMWeaponAddress.Eng_Buster,
                RobotMasters = new int[8] { 2, 2, 1, 1, 2, 2, 1, 1 }
                // Heat = 2,
                // Air = 2,
                // Wood = 1,
                // Bubble = 1,
                // Quick = 2,
                // Flash = 2,
                // Metal = 1,
                // Clash = 1,
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Atomic Fire",
                ID = 1,
                Address = ERMWeaponAddress.Eng_AtomicFire,
                // Note: These values only affect a fully charged shot.  Partially charged shots use the Buster table.
                RobotMasters = new int[8] { 0xFF, 6, 0x0E, 0, 0x0A, 6, 4, 6 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Air Shooter",
                ID = 2,
                Address = ERMWeaponAddress.Eng_AirShooter,
                RobotMasters = new int[8] { 2, 0, 4, 0, 2, 0, 0, 0x0A }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Leaf Shield",
                ID = 3,
                Address = ERMWeaponAddress.Eng_LeafShield,
                RobotMasters = new int[8] { 0, 8, 0xFF, 0, 0, 0, 0, 0 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Bubble Lead",
                ID = 4,
                Address = ERMWeaponAddress.Eng_BubbleLead,
                RobotMasters = new int[8] { 6, 0, 0, 0xFF, 0, 2, 0, 1 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Quick Boomerang",
                ID = 5,
                Address = ERMWeaponAddress.Eng_QuickBoomerang,
                RobotMasters = new int[8] { 2, 2, 0, 2, 0, 0, 4, 1 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Time Stopper",
                ID = 6,
                Address = ERMWeaponAddress.Eng_TimeStopper,
                // NOTE: These values affect damage per tick
                RobotMasters = new int[8] { 0, 0, 0, 0, 1, 0, 0, 0 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Metal Blade",
                ID = 7,
                Address = ERMWeaponAddress.Eng_MetalBlade,
                RobotMasters = new int[8] { 1, 0, 2, 4, 0, 4, 0x0E, 0 }
            });

            Weapons.Add(new WeaponTable()
            {
                Name = "Clash Bomber",
                ID = 8,
                Address = ERMWeaponAddress.Eng_ClashBomber,
                RobotMasters = new int[8] { 0xFF, 0, 2, 2, 4, 3, 0, 0 }
            });

            foreach (WeaponTable weapon in Weapons)
            {
                weapon.RobotMasters.Shuffle(Random);
            }

            using (var stream = new FileStream(DestinationFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                foreach (WeaponTable weapon in Weapons)
                {
                    stream.Position = (long) weapon.Address;
                    for (int i = 0; i < 8; i++)
                    {
                        stream.WriteByte((byte)weapon.RobotMasters[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Shuffle the Robot Master stages.  This shuffling will not be indicated by the Robot Master portraits.
        /// </summary>
        private static void RandomStagePtrs()
        {
            // StageSelect  Address    Value
            // -----------------------------
            // Bubble Man   0x034670   3
            // Air Man      0x034671   1
            // Quick Man    0x034672   4
            // Wood Man     0x034673   2
            // Crash Man    0x034674   7
            // Flash Man    0x034675   5
            // Metal Man    0x034676   6
            // Heat Man     0x034677   0
            
            List<StageFromSelect> StageSelect = new List<StageFromSelect>();

            StageSelect.Add(new StageFromSelect()
            {
                PortraitName = "Bubble Man",
                PortraitAddress = ERMPortraitAddress.BubbleMan,
                PortraitDestinationOriginal = 3,
                PortraitDestinationNew = 3,
                StageClearAddress = ERMStageClearAddress.BubbleMan,
                StageClearDestinationOriginal = 8,
                StageClearDestinationNew = 8
            });
            StageSelect.Add(new StageFromSelect()
            {
                PortraitName = "Air Man",
                PortraitAddress = ERMPortraitAddress.AirMan,
                PortraitDestinationOriginal = 1,
                PortraitDestinationNew = 1,
                StageClearAddress = ERMStageClearAddress.AirMan,
                StageClearDestinationOriginal = 2,
                StageClearDestinationNew = 2
            });
            StageSelect.Add(new StageFromSelect()
            {
                PortraitName = "Quick Man",
                PortraitAddress = ERMPortraitAddress.QuickMan,
                PortraitDestinationOriginal = 4,
                PortraitDestinationNew = 4,
                StageClearAddress = ERMStageClearAddress.QuickMan,
                StageClearDestinationOriginal = 16,
                StageClearDestinationNew = 16
            });
            StageSelect.Add(new StageFromSelect()
            {
                PortraitName = "Wood Man",
                PortraitAddress = ERMPortraitAddress.WoodMan,
                PortraitDestinationOriginal = 2,
                PortraitDestinationNew = 2,
                StageClearAddress = ERMStageClearAddress.WoodMan,
                StageClearDestinationOriginal = 4,
                StageClearDestinationNew = 4
            });
            StageSelect.Add(new StageFromSelect()
            {
                PortraitName = "Clash Man",
                PortraitAddress = ERMPortraitAddress.CrashMan,
                PortraitDestinationOriginal = 7,
                PortraitDestinationNew = 7,
                StageClearAddress = ERMStageClearAddress.CrashMan,
                StageClearDestinationOriginal = 128,
                StageClearDestinationNew = 128
            });
            StageSelect.Add(new StageFromSelect()
            {
                PortraitName = "Flash Man",
                PortraitAddress = ERMPortraitAddress.FlashMan,
                PortraitDestinationOriginal = 5,
                PortraitDestinationNew = 5,
                StageClearAddress = ERMStageClearAddress.FlashMan,
                StageClearDestinationOriginal = 32,
                StageClearDestinationNew = 32
            });
            StageSelect.Add(new StageFromSelect()
            {
                PortraitName = "Metal Man",
                PortraitAddress = ERMPortraitAddress.MetalMan,
                PortraitDestinationOriginal = 6,
                PortraitDestinationNew = 6,
                StageClearAddress = ERMStageClearAddress.MetalMan,
                StageClearDestinationOriginal = 64,
                StageClearDestinationNew = 64
            });
            StageSelect.Add(new StageFromSelect()
            {
                PortraitName = "Heat Man",
                PortraitAddress = ERMPortraitAddress.HeatMan,
                PortraitDestinationOriginal = 0,
                PortraitDestinationNew = 0,
                StageClearAddress = ERMStageClearAddress.HeatMan,
                StageClearDestinationOriginal = 1,
                StageClearDestinationNew = 1
            });


            List<byte> newStageOrder = new List<byte>();
            for (byte i = 0; i < 8; i++) newStageOrder.Add(i);

            newStageOrder.Shuffle(Random);

            for (int i = 0; i < 8; i++)
            {
                string portrait = StageSelect[i].PortraitName;
                StageSelect[i].PortraitDestinationNew = StageSelect[newStageOrder[i]].PortraitDestinationOriginal;
                //StageSelect[i].StageClearDestinationNew = StageSelect[newStageOrder[i]].StageClearDestinationOriginal;
            }
            
            using (var stream = new FileStream(DestinationFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                foreach (StageFromSelect stage in StageSelect)
                {
                    stream.Position = (long)stage.PortraitAddress;
                    stream.WriteByte((byte)stage.PortraitDestinationNew);
                    //stream.Position = stage.StageClearAddress;
                    //stream.WriteByte((byte)stage.StageClearDestinationNew);
                }
            }
        }

        /// <summary>
        /// Shuffle the elements of the provided list.
        /// </summary>
        /// <typeparam name="T">The Type of the elements in the list.</typeparam>
        /// <param name="list">The object to be shuffled.</param>
        /// <param name="rng">The seed used to perform the shuffling.</param>
        public static void Shuffle<T>(this IList<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
