﻿using MM2Randomizer.Enums;
using MM2Randomizer.Patcher;

using System;
using System.Collections.Generic;

namespace MM2Randomizer.Randomizers.Stages
{
    public class RStages : IRandomizer
    {
        public static List<StageFromSelect> StageSelect;

        public RStages() { }

        public void FixPortraits(ref byte[] portraitBG_x, ref byte[] portraitBG_y)
        {
            // Get the new stage order
            int[] newOrder = new int[8];
            foreach (StageFromSelect stage in StageSelect)
                newOrder[stage.PortraitDestinationOriginal] = stage.PortraitDestinationNew;

            // Permute portrait x/y values via the shuffled stage-order array 
            byte[] cpy = new byte[8];
            for (int i = 0; i < 8; i++)
                cpy[newOrder[i]] = portraitBG_y[i];
            Array.Copy(cpy, portraitBG_y, 8);

            for (int i = 0; i < 8; i++)
                cpy[newOrder[i]] = portraitBG_x[i];
            Array.Copy(cpy, portraitBG_x, 8);

            //for (int i = 0; i < 8; i++)
            //    cpy[i] = portraitSprite_y[newOrder[i]];
            //Array.Copy(cpy, portraitSprite_y, 8);

            //for (int i = 0; i < 8; i++)
            //    cpy[i] = portraitSprite_x[newOrder[i]];
            //Array.Copy(cpy, portraitSprite_x, 8);
        }

        /// <summary>
        /// Shuffle the Robot Master stages.  This shuffling will not be indicated by the Robot Master portraits.
        /// </summary>
        public void Randomize(Patch Patch, Random r)
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

            StageSelect = new List<StageFromSelect>();

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
                PortraitDestinationNew = 0, // 4 = quick
                StageClearAddress = ERMStageClearAddress.HeatMan,
                StageClearDestinationOriginal = 1,
                StageClearDestinationNew = 1
            });


            List<byte> newStageOrder = new List<byte>();
            for (byte i = 0; i < 8; i++) newStageOrder.Add(i);

            newStageOrder.Shuffle(r);

            for (int i = 0; i < 8; i++)
            {
                string portrait = StageSelect[i].PortraitName;
                StageSelect[i].PortraitDestinationNew = StageSelect[newStageOrder[i]].PortraitDestinationOriginal;
            }

            foreach (StageFromSelect stage in StageSelect)
            {
                Patch.Add((int)stage.PortraitAddress, (byte)stage.PortraitDestinationNew, String.Format("Stage Select {0} Destination", stage.PortraitName));
            }
        }
    }

}