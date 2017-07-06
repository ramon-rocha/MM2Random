﻿using System;
using System.Collections.Generic;

using MM2Randomizer.Enums;
using MM2Randomizer.Patcher;
using System.Text;

namespace MM2Randomizer.Randomizers
{
    public class RText : IRandomizer
    {
        public static readonly int MAX_CHARS = 12;
        public static readonly int offsetLetters = 0x037e22;
        public static readonly int offsetAtomicFire = 0x037e2e;
        public static readonly int offsetCutscenePage1L1 = 0x036D56;

        public RText() { }

        public void Randomize(Patch p, Random r)
        {
            // Write in intro text
            int numIntros = IntroTexts.GetLength(0);
            int introIndex = r.Next(numIntros);
            char[] introText = IntroTexts[introIndex].ToCharArray();
            
            for (int i = 0; i < 270; i++) // 27 characters per line, 5 pages, 2 lines per page
            {
                try
                {
                    byte charByte = IntroCipher[introText[i]];
                    p.Add(
                        offsetCutscenePage1L1 + i,
                        charByte,
                        $"Intro Text: {introText[i]}");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Exception. Probably unsupported character in dictionary. Message:\n{ex.ToString()}");
                }
            }

            // Write in new weapon names
            for (int i = 0; i < 8; i++)
            {
                int offset = offsetAtomicFire + i * 0x10;

                string name = GetRandomName(r);
                char[] chars = name.ToCharArray();

                for (int j = 0; j < MAX_CHARS; j++)
                {
                    if (j < chars.Length)
                    {
                        byte b = Convert.ToByte(chars[j]);
                        p.Add(offset + j, b, String.Format("Weapon Name {0} Char #{1}: {2}", ((EDmgVsBoss.Offset)i).Name, j, chars[j].ToString()));

                    }
                    else
                    {
                        p.Add(offset + j, Convert.ToByte('@'), String.Format("Weapon Name {0} Char #{1}: @", ((EDmgVsBoss.Offset)i).Name, j));
                    }
                }
            }

            // Erase "Boomerang" for now
            for (int i = 0; i < 10; i++)
            {
                p.Add(0x037f5e + i, Convert.ToByte('@'), String.Format("Quick Boomerang Name Erase Char #{0}: @", i));
            }

            // Write in new weapon letters
            for (int i = 0; i < 8; i++)
            {
                int randLetter = 0x41 + r.Next(26);
                p.Add(offsetLetters + i, (byte)randLetter, String.Format("Weapon Get {0} Letter: {1}", ((EDmgVsBoss.Offset)i).Name, Convert.ToChar(randLetter).ToString()));
            }
        }

        static Dictionary<char, byte> IntroCipher = new Dictionary<char, byte>()
        {
            { ' ', 0x00 },
            { '0', 0xA0 },
            { '8', 0xA1 },
            { '2', 0xA2 },
            { '©', 0xA3 },
            { 't', 0xA4 }, // tm
            { '9', 0xA5 },
            { '7', 0xA6 },
            { '1', 0xA7 },
            { '3', 0xA8 }, // check
            { '4', 0xA9 }, // check
            { '5', 0xAA }, // check
            { '6', 0xAB }, // check
            { '|', 0xC0 }, // Blank space, change later? also, there may be a blank row (B) to use...
            { 'A', 0xC1 },
            { 'B', 0xC2 },
            { 'C', 0xC3 },
            { 'D', 0xC4 },
            { 'E', 0xC5 },
            { 'F', 0xC6 },
            { 'G', 0xC7 },
            { 'H', 0xC8 },
            { 'I', 0xC9 },
            { 'J', 0xCA },
            { 'K', 0xCB },
            { 'L', 0xCC },
            { 'M', 0xCD },
            { 'N', 0xCE },
            { 'O', 0xCF },
            { 'P', 0xD0 },
            { 'Q', 0xD1 },
            { 'R', 0xD2 },
            { 'S', 0xD3 },
            { 'T', 0xD4 },
            { 'U', 0xD5 },
            { 'V', 0xD6 },
            { 'W', 0xD7 },
            { 'X', 0xD8 },
            { 'Y', 0xD9 },
            { 'Z', 0xDA },
            { '?', 0xDB }, // "r." change to ?
            { '.', 0xDC },
            { ',', 0xDD },
            { '\'', 0xDE },
            { '!', 0xDF },
        };

        static string[] IntroTexts = new string[]
        {
            "DURING 15TH CENTURY EUROPE "+
            "  THERE LIVED A PERSON     "+
            "    NAMED DRACULA. HE      "+
            "PRACTICED SORCERY IN ORDER "+
            "   TO CREATE A BAD WORLD   "+
            "     FILLED WITH EVIL.     "+
            "   THE CURSE OF DRACULA    "+
            "        HAS BEGUN.         "+
            "    THE FATE OF EUROPE     "+
            " LIES WITH TREVOR BELMONT. ",

            " WITH WHOM DID FATHER HAVE "+
            "     A DUEL AND LOSE?      "+
            "  FOR WHAT REASON DID HE   "+
            "      FIGHT AND DIE?       "+
            "  AFTER HE DISAPPEARED, I  "+
            "FOUND A LETTER FROM FATHER."+
            " \'TAKE THE DRAGON SWORD OF "+
            "    THE HAYABUSA FAMILY\'   "+
            "   I WILL GET MY REVENGE!  "+
            "                           ",

            " MARIO OPENED A DOOR AFTER "+
            " CLIMBING A LONG STAIR IN  "+
            " HIS DREAM, ANOTHER WORLD  "+
            " SPREAD BEFORE HIM AND HE  "+
            "  HEARD A VOICE CALL FOR   "+
            "  HELP TO BE FREED FROM A  "+
            "SPELL. AFTER WAKING, MARIO "+
            " WENT TO A CAVE NEARBY AND "+
            "  TO HIS SURPRISE HE SAW   "+
            "  WHAT WAS IN HIS DREAM... ",

            "   MANY YEARS AGO PRINCE   "+
            "  DARKNESS \'GANNON\' STOLE  "+
            " ONE OF THE TRIFORCE WITH  "+
            " POWER. PRINCESS ZELDA HAD "+
            " ONE OF THE TRIFORCE WITH  "+
            "  WISDOM. SHE DIVIDED IT   "+
            "  INTO 8 UNITS TO HIDE IT  "+
            "FROM GANNON BEFORE SHE WAS "+
            "  CAPTURED. GO FIND THE 8  "+
            "  UNITS LINK TO SAVE HER.  ",

            "          METROID          "+
            "                           "+
            "         EMERGENCY         "+
            "           ORDER           "+
            "   DEFEAT THE METROID OF   "+
            "   THE PLANET ZEBETH AND   "+
            " DESTROY THE MOTHER BRAIN  "+
            " THE MECHANICAL LIFE VEIN  "+
            "  GALAXY FEDEREAL POLICE   "+
            "          M510             ",

            "ONE DAY, THE PEACEFUL LIFE "+
            "OF DREAMLAND WAS SHATTERED "+
            "  BY A MYSTERIOUS CRISIS!  "+
            " THE PEOPLE DIDN\'T DREAM!  "+
            "DEDEDE HAD BROKEN THE STAR "+
            " ROD AND GIVEN THE PIECES  "+
            "  TO HIS FRIENDS, WHO ARE  "+
            "    HIDING IN DREAMLAND!   "+
            " TO BRING BACK LOST DREAMS,"+
            " KIRBY SOUGHT THE STAR ROD!",

            "    I FIRST BATTLED THE    "+
            " METROIDS ON PLANET ZEBES. "+
            "IT WAS THERE THAT I FOILED "+
            "  THE PLANS OF THE SPACE   "+
            "PIRATE LEADER MOTHER BRAIN "+
            "  TO USE THE CREATURES TO  "+
            "      ATTACK GALACTIC      "+
            "       CIVILIZATION...     "+
            "     CERES STATION IS      "+
            "      UNDER ATTACK!!!      ",

            "  THE WORLD IS VEILED IN   "+
            " DARKNESS. THE WIND STOPS, "+
            " THE SEA IS WILD, AND THE  "+
            " EARTH BEGINS TO ROT. THE  "+
            "  PEOPLE WAIT, THEIR ONLY  "+
            "HOPE, A PROPHECY. WHEN THE "+
            "WORLD IS IN DARKNESS, FOUR "+
            "WARRIORS WILL COME. AFTER A"+
            "LONG JOURNEY, FOUR WARRIORS"+
            "ARRIVE, EACH HOLDING AN ORB",

            " LISTEN NOW TO MY WORDS.   "+
            " DESCENDANT OF ERDRICK, IT "+
            " IS TOLD THAT IN AGES PAST "+
            "ERDRICK FOUGHT DEMONS WITH "+
            "A BALL OF LIGHT. THEN CAME "+
            " THE DRAGONLORD WHO STOLE  "+
            "THE PRECIOUS GLOBE AND HID "+
            " IT IN THE DARKNESS. NOW,  "+
            "MEGAMAN, THOU MUST HELP US "+
            "RESTORE PEACE TO OUR LAND. ",

            "IT\'S A PERIOD OF CIVIL WAR."+
            "REBELS HAVE WON THEIR FIRST"+
            " VICTORY AGAINST THE EVIL  "+
            "  GALACTIC EMPIRE. REBEL   "+
            " SPIES STOLE PLANS TO THE  "+
            " EMPIRE\'S ULTIMATE WEAPON  "+
            "  THE DEATH STAR. MEGAMAN  "+
            " ESCAPES WITH THE PLANS TO "+
            " SAVE HIS PEOPLE AND BRING "+
            "   FREEDOM TO THE GALAXY.  ",
        };

        private string GetRandomName(Random r)
        {
            string finalName = "";

            // Start with random list
            int l = r.Next(1);
            string name0, name1;
            if (l == 0)
            {
                // Get random name from first list
                int random = r.Next(Names0.Length);
                name0 = Names0[random];

                // From second list, get subset of names with valid character count
                int charsLeft = MAX_CHARS - name0.Length - 1; // 1 space
                List<string> names1Left = new List<string>();
                for (int j = 0; j < Names1.Length; j++)
                {
                    if (Names1[j].Length <= charsLeft)
                    {
                        names1Left.Add(Names1[j]);
                    }
                }

                // Get random name from modified second list
                if (names1Left.Count > 0)
                {
                    random = r.Next(names1Left.Count);
                    name1 = names1Left[random];
                }
                else
                {
                    name1 = "";
                }
            }
            else
            {
                // Get random name from second list
                int random = r.Next(Names1.Length);
                name1 = Names1[random];

                // From first list, get subset of names with valid character count
                int charsLeft = MAX_CHARS - name1.Length - 1; // 1 space
                List<string> names0Left = new List<string>();
                for (int j = 0; j < Names0.Length; j++)
                {
                    if (Names0[j].Length <= charsLeft)
                    {
                        names0Left.Add(Names0[j]);
                    }
                }

                // Get random name from modified first list
                if (names0Left.Count > 0)
                {
                    random = r.Next(names0Left.Count);
                    name0 = names0Left[random];
                }
                else
                {
                    name0 = "";
                }
            }

            // Handle cases for only one name
            if (name0.Length == 0)
            {
                finalName = name1;
            }
            else if (name1.Length == 0)
            {
                finalName = name0;
            }
            // Concatenate final name
            else
            {
                finalName = name0 + "@" + name1;
            }

            return finalName;
        }

        static string[] Names0 = new string[]
            {
                "TIME",
                "MEGA",
                "SUPER",
                "METAL",
                "ATOMIC",
                "AIR",
                "WILY",
                "ROLL",
                "RUSH",
                "FRANKERZ",
                "WATER",
                "GUTS",
                "ELEC",
                "GEMINI",
                "COSSACK",
                "TOAD",
                "HYPER",
                "TIME",
                "CRASH",
                "LEAF",
                "QUICK",
                "DRILL",
                "FLAME",
                "PLANT",
                "KNIGHT",
                "SILVER",
                "JUNK",
                "THUNDER",
                "WILD",
                "NOISE",
                "SLASH",
                "TORNADO",
                "ASTRO",
                "CLOWN",
                "SOLAR",
                "CHILL",
                "TRIPLE",
                "REBOUND",
                "NUDUA",
                "JOKA",
                "ELLO",
                "COOLKID",
                "PROTO",
                "PLUG",
                "RTA",
                "MASH",
                "BIG",
                "CUT",
                "URN",
                "TWITCH",
                "PRO",
                "ION",
                "AUTO",
                "BEAT",
            };

        static string[] Names1 = new string[]
        {
            "BLAST",
            "BLASTER",
            "FIRE",
            "CUTTER",
            "BLADE",
            "STOPPER",
            "GUN",
            "CANNON",
            "HIT",
            "SHOT",
            "COIL",
            "SHOOTER",
            "BOMB",
            "BOMBER",
            "LASER",
            "FLUSH",
            "BEAM",
            "DASH",
            "MISSILE",
            "STORM",
            "CUTTER",
            "SHIELD",
            "KNUCKLE",
            "SNAKE",
            "SHOCK",
            "SPIN",
            "CRUSHER",
            "HOLD",
            "EYE",
            "ATTACK",
            "KICK",
            "STONE",
            "WAVE",
            "SPEAR",
            "CLAW",
            "BALL",
            "TRIDENT",
            "WOOL",
            "SPIKE",
            "BLAZE",
            "STRIKER",
            "WALL",
            "BALLOON",
            "MARINE",
            "WIRE",
            "BURNER",
            "BUSTER",
            "ZIP",
            "GLITCH",
            "ADAPTER",
            "RAID",
            "DEVICE",
            "BOX",
            "AXE",
            "ARC",
            "JAB",
            "RESET",
            "STRAT",
        };
    }
}