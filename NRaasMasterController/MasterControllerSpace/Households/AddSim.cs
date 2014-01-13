using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Households
{
    public class AddSim : HouseholdFromList, IHouseholdOption
    {
        Sim mActor;

        GameObject mTarget;

        public override string GetTitlePrefix()
        {
            return "AddSimHouse";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mActor = parameters.mActor as Sim;

            mTarget = parameters.mTarget;

            if (parameters.mTarget is Sim)
            {
                if (parameters.mActor != parameters.mTarget) return false;
            }

            return base.Allow(parameters);
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            return true;
        }

        public static bool TestForRemainingActive(List<IMiniSimDescription> movers)
        {
            bool foundChild = false;

            foreach (SimDescription active in CommonSpace.Helpers.Households.All(Household.ActiveHousehold))
            {
                if (movers.Contains(active)) continue;

                if ((active.TeenOrAbove) && (active.IsHuman))
                {
                    return true;
                }
                else
                {
                    foundChild = true;
                }
            }

            return !foundChild;
        }

        public static int GetSpace(Household me)
        {
            int maxSelection = 8 + 6;
            if (NRaas.MasterController.Settings.mAllowOverStuffed)
            {
                maxSelection = int.MaxValue;
            }
            else if (me != null)
            {
                maxSelection -= CommonSpace.Helpers.Households.NumSimsIncludingPregnancy(me);
            }

            return maxSelection;
        }

        protected override bool Allow(Lot lot, Household house)
        {
            if (GetSpace(house) <= 0) return false;

            if ((lot != null) && (lot.IsCommunityLot) && (!lot.IsBaseCampLotType))
            {
                if (!MasterController.Settings.mCommunityAddSim) return false;
            }

            return base.Allow(lot, house);
        }

        protected override OptionResult Run(Lot lot, Household me)
        {
            int maxSelection = GetSpace(me);
            if (maxSelection <= 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("AddSimHouse:TooMany"));
                return OptionResult.Failure;
            }

            SimDescription targetSim = mActor.SimDescription;
            if (mTarget is Sim)
            {
                targetSim = (mTarget as Sim).SimDescription;
            }
            else if ((me != null) && (CommonSpace.Helpers.Households.NumSims(me) > 0))
            {
                if (Sim.ActiveActor != null)
                {
                    targetSim = Sim.ActiveActor.SimDescription;
                }
                else
                {
                    targetSim = me.AllSimDescriptions[0];
                }
            }

            bool okayed = false;
            List<IMiniSimDescription> sims = new Sims.MoveIn(me).GetSelection(targetSim, Name, SelectionOption.List, maxSelection, CanApplyAll(), out okayed);
            if ((sims == null) || (sims.Count == 0))
            {
                if (okayed)
                {
                    return OptionResult.SuccessClose;
                }
                else
                {
                    return OptionResult.Failure;
                }
            }

            return PrivatePerform(lot, me, sims);
        }

        protected virtual OptionResult PrivatePerform(Lot lot, Household me, List<IMiniSimDescription> sims)
        {
            return Perform(lot, me, sims);
        }

        public static OptionResult Perform(Lot lot, Household me, List<IMiniSimDescription> sims)
        {
            if (!TestForRemainingActive(sims))
            {
                Common.Notify(Common.Localize("AddSim:ActiveFail"));
                return OptionResult.Failure;
            }

            if ((me == null) && (lot != null))
            {
                me = Household.Create();

                try
                {
                    lot.MoveIn(me);
                }
                catch
                {
                    me.Dispose();
                    return OptionResult.Failure;
                }
            }
            else
            {
                EventTracker.SendEvent(new HouseholdUpdateEvent(EventTypeId.kHouseholdMerged, me));
            }

            int maxSelection = GetSpace(me);

            AddSims addSim = new AddSims(me, sims, ((maxSelection > 0) && (maxSelection < int.MaxValue)), true, MasterController.Settings.mDreamCatcher);

            addSim.SendEvents();

            return OptionResult.SuccessClose;
        }
    }
}
