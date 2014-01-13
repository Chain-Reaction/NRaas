using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class AskToJoinSkinnyDippingEx : Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Woohooer.InjectAndReset<Sim, HotTubBase.AskToJoinHotTub.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.SetFlags(Availability.FlagField.DisallowedIfPregnant, false);
            }

            sOldSingleton = AskToJoinSkinnyDipping.Singleton;
            AskToJoinSkinnyDipping.Singleton = new Definition();
        }

        public class Definition : AskToJoinSkinnyDipping.Definition
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                using (WoohooTuningControl control = new WoohooTuningControl(parameters.InteractionObjectPair.Tuning, Woohooer.Settings.mAllowTeenSkinnyDip))
                {
                    return base.Test(ref parameters, ref greyedOutTooltipCallback);
                }
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (a == target)
                    {
                        return false;
                    }

                    if (!Woohooer.Settings.mAllowTeenSkinnyDip)
                    {
                        if (target.SimDescription.TeenOrBelow)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (target.SimDescription.ChildOrBelow)
                        {
                            return false;
                        }
                    }

                    if (!CelebrityManager.CanSocialize(a, target))
                    {
                        return false;
                    }

                    if (target.SimDescription.IsVisuallyPregnant)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(target.IsFemale, "Gameplay/Actors/Sim:PregnantFailure", new object[0]));
                        return false;
                    }

                    SwimmingInPool posture = a.Posture as SwimmingInPool;
                    SwimmingInPool pool2 = target.Posture as SwimmingInPool;
                    if (posture == null)
                    {
                        return false;
                    }

                    if ((pool2 != null) && (pool2.ContainerPool == posture.ContainerPool))
                    {
                        return false;
                    }

                    if (!Pool.SimOutfitSupportsSkinnyDipping(target, ref greyedOutTooltipCallback))
                    {
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}
