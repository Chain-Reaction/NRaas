using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims
{
    public class MoveIn : SimFromList, ISimOption
    {
        Household mHouse = null;

        public MoveIn(Household house)
        {
            mHouse = house;
        }

        public override string GetTitlePrefix()
        {
            return "MoveIn";
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.Household != null) 
            {
                if (me.Household == mHouse) return false;
            }

            if (me.Household == Household.ActiveHousehold)
            {
                if ((me.TeenOrAbove) && (me.IsHuman))
                {
                    bool foundAdult = false, foundChild = false;

                    foreach (SimDescription sim in CommonSpace.Helpers.Households.All(me.Household))
                    {
                        if (sim == me) continue;

                        if ((sim.TeenOrAbove) && (sim.IsHuman))
                        {
                            foundAdult = true;
                            break;
                        }
                        else
                        {
                            foundChild = true;
                        }
                    }

                    if (foundChild)
                    {
                        if (!foundAdult) return false;
                    }
                }
            }

            return true;
        }
        protected override bool PrivateAllow(MiniSimDescription me)
        {
            //if (!base.Allow(me)) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return true;
        }
    }
}
