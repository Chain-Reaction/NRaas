using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class ConsignerPopupTuning
    {
        [Tunable, TunableComment("Whether to use a popup menu approach when displaying the interactions")]
        public static bool kPopupMenuStyle = false;
    }

    public class VersionStamp : Common.ProtoVersionStamp, Common.IPreLoad
    {
        public static readonly string sNamespace = "NRaas.Consigner";

        public class Version : ProtoVersion<GameObject>
        { }

        public class Reapply : ProtoReapply<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            Consigner.ResetSettings();
        }

        public void OnPreLoad()
        {
            sPopupMenuStyle = ConsignerPopupTuning.kPopupMenuStyle;
        }

        /* TODO
         * 
         * Remove the friend links between parents and child when selling animals via consignment (to stop the parents from mourning the loss)
         * 
         * FruitVeggieStand object has its own consignment system
         * 
         */

        /* Changes
         * 
         * (JunJayMdM) Added BotShopRegister to ConsignmentHelper
         * 
         */
        public static readonly int sVersion = 21;
    }
}
