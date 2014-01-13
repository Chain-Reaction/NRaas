using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class AnimatorPopupTuning
    {
        [Tunable, TunableComment("Whether to use a popup menu approach when displaying the interactions")]
        public static bool kPopupMenuStyle = false;
    }

    public class VersionStamp : Common.ProtoVersionStamp, Common.IPreLoad
    {
        public static readonly string sNamespace = "NRaas.Animator";

        public class Version : ProtoVersion<GameObject>
        { }

        public void OnPreLoad()
        {
            sPopupMenuStyle = AnimatorPopupTuning.kPopupMenuStyle;
        }

        /* TODO
         * 
         * 
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 11;
    }
}
