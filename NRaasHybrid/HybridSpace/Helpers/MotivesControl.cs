using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
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
    public class MotivesControl : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kSimInstantiated, OnInstantiated);
            new Common.DelayedEventListener(EventTypeId.kBecameOccult, OnBecameOccult);

            foreach (Sim sim in LotManager.Actors)
            {
                UpdateMotives(sim);
            }
        }

        protected static void OnInstantiated(Event e)
        {
            UpdateMotives(e.TargetObject as Sim);
        }

        protected static void OnBecameOccult(Event e)
        {
            UpdateMotives(e.Actor as Sim);
        }

        protected static void UpdateMotives(Sim sim)
        {
            try
            {
                if (sim == null) return;

                if (sim.Motives == null) return;

                if (sim.OccultManager == null) return;

                if (sim.SimDescription.IsMummy)
                {
                    Motive motive = sim.Motives.GetMotive(CommodityKind.Bladder);
                    if (motive != null)
                    {
                        motive.DisableMotive();
                    }

                    motive = sim.Motives.GetMotive(CommodityKind.Energy);
                    if (motive != null)
                    {
                        motive.DisableMotive();
                    }
                }

                if (sim.SimDescription.IsFrankenstein)
                {
                    Motive motive = sim.Motives.GetMotive(CommodityKind.Hygiene);
                    if (motive != null)
                    {
                        motive.DisableMotive();
                    }
                }

                if (sim.SimDescription.IsVampire)
                {
                    Motive motive = sim.Motives.GetMotive(CommodityKind.Fatigue);
                    if (motive != null)
                    {
                        motive.DisableMotive();
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
            }
        }
    }
}
