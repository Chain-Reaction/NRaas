using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class MaxMotives : SimFromList, IIntermediateOption
    {
        static CommodityKind[] sCommodities = new CommodityKind[] { CommodityKind.VampireThirst, CommodityKind.Hygiene, CommodityKind.Bladder, CommodityKind.Energy, CommodityKind.Fun, CommodityKind.Hunger, CommodityKind.Social, CommodityKind.HorseThirst, CommodityKind.HorseExercise, CommodityKind.CatScratch, CommodityKind.DogDestruction, CommodityKind.AlienBrainPower, CommodityKind.Maintenence, CommodityKind.BatteryPower };

        public override string GetTitlePrefix()
        {
            return "MaxMotives";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return (me.CreatedSim != null);
        }

        protected void ForceSetMax (Motives ths, CommodityKind commodity)
        {
            Motive motive = ths.GetMotive(commodity);
            if ((motive != null) && (motive.Value != motive.Tuning.Max))
            {
                motive.UpdateMotiveBuffs(ths.mSim, commodity, (int) motive.Tuning.Max);
                motive.mValue = motive.Tuning.Max;
            }
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (me.CreatedSim != null)
            {
                bool reset = false;
                if (me.CreatedSim.Motives == null)
                {
                    reset = true;
                }
                else
                {
                    foreach (KeyValuePair<int, Motive> motive in me.CreatedSim.Motives.mMotives)
                    {
                        if ((motive.Value == null) || (motive.Value.Tuning == null))
                        {
                            reset = true;
                            break;
                        }
                    }
                }

                if (reset)
                {
                    me.CreatedSim.mAutonomy.RecreateAllMotives();
                }

                foreach (CommodityKind kind in sCommodities)
                {
                    ForceSetMax(me.CreatedSim.Motives, kind);
                }
            }
            return true;
        }
    }
}
