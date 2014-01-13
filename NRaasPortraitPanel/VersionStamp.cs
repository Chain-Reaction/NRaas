using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class PortraitPanelPopupTuning
    {
        [Tunable, TunableComment("Whether to use a popup menu approach when displaying the interactions")]
        public static bool kPopupMenuStyle = false;
    }

    public class VersionStamp : Common.ProtoVersionStamp, Common.IPreLoad
    {
        public static readonly string sNamespace = "NRaas.PortraitPanel";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            PortraitPanel.ResetSettings();
        }

        public void OnPreLoad()
        {
            sPopupMenuStyle = PortraitPanelPopupTuning.kPopupMenuStyle;
        }

        /* TODO
         * 
         * Option to define what type of data to display in the portrait tooltip (Age, etc). 
         * 
         * PortraitPanel double click effect when switching between sims, caused by the clicked sims portrait moving due to a refresh ?
         * Issue with name sorting still not sorting one of the sims properly in PortraitPanel
         * 
         * Known Info tooltip gets cut off by the top of the screen when looking at the eight sim in a column
         * Alter the PortraitPanel drag/drop to not bypass Inventory Purge
         * 
         * Add null checking to IsSimListed
         * 
         * Issue where settings are not applied immediately on loadup ?
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 32;
    }
}
