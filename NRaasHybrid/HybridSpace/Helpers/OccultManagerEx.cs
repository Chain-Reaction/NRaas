using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Helpers
{
    public class OccultManagerEx
    {
        public static SimInfo.SimOccultInfo GetSimOccultInfo()
        {
            Sim sim = Sim.ActiveActor;
            if (sim == null) return new SimInfo.SimOccultInfo(Sims3.SimIFace.CAS.CASAgeGenderFlags.Human);

            if (sim.OccultManager == null) return new SimInfo.SimOccultInfo(Sims3.SimIFace.CAS.CASAgeGenderFlags.Human);

            SimInfo.SimOccultInfo info = new SimInfo.SimOccultInfo(sim.SimDescription.Species);

            if (sim.SimDescription.IsAlienEvolved)
            {
                AlienUtils.GetAlienOccultInfo(ref info);
            }

            if (sim.OccultManager.mOccultList != null)
            {
                foreach (OccultBaseClass occult in sim.OccultManager.mOccultList)
                {
                    occult.UpdateSimOccultInfo(ref info);
                }

                info.OccultType = sim.OccultManager.CurrentOccultTypes;
            }

            if (sim.Motives != null)
            {
                if (sim.SimDescription.IsWitch)
                {
                    Motive motive = sim.Motives.GetMotive(CommodityKind.MagicFatigue);
                    if (motive != null)
                    {
                        info.MagicMotiveValue = -motive.UIValue;

                        //Common.DebugNotify(delegate { return "MagicFatigue2: " + motive.UIValue; });
                    }
                }
                else if (sim.SimDescription.IsFairy)
                {
                    Motive motive = sim.Motives.GetMotive(CommodityKind.AuraPower);
                    if (motive != null)
                    {
                        info.MagicMotiveValue = motive.UIValue;

                        //Common.DebugNotify(delegate { return "AuraPower2: " + motive.UIValue; });
                    }
                }
            }

            return info;
        }
    }
}
