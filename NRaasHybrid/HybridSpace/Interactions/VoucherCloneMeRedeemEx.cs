using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Interactions
{
    public class VoucherCloneMeRedeemEx : VoucherCloneMe.Redeem, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<VoucherCloneMe, VoucherCloneMe.Redeem.Definition, Definition>(false);

            sOldSingleton = VoucherCloneMe.Redeem.Singleton;
            VoucherCloneMe.Redeem.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<VoucherCloneMe, VoucherCloneMe.Redeem.Definition>(VoucherCloneMe.Redeem.Singleton);
        }

        public new class Definition : VoucherCloneMe.Redeem.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new VoucherCloneMeRedeemEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, VoucherCloneMe target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, VoucherCloneMe target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                target.TargetScienceLab = Voucher.FindNearestScienceLab(a);
                if (target.TargetScienceLab == null)
                {
                    return false;
                }

                /*
                if (!Household.ActiveHousehold.CanAddSpeciesToHousehold(a.SimDescription.Species))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.IsFemale, "Gameplay/Objects/RabbitHoles/ScienceLab:HouseholdTooLarge", new object[0x0]));
                    return false;
                }

                if (a.OccultManager.HasAnyOccultType())
                {
                    return false;
                }

                if (a.SimDescription.IsGhost)
                {
                    return false;
                }
                */

                if (GameUtils.IsOnVacation())
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(a.IsFemale, "Ui/Tooltip/Vacation/GreyedoutTooltip:InteractionNotValidOnVacation", new object[0x0]));
                    return false;
                }

                return true;
            }
        }
    }
}
