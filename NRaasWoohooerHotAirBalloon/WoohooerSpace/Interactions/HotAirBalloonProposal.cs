using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Controller;
using Sims3.Store.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class HotAirBalloonProposal : HotairBalloon.Proposal, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<HotairBalloon, HotairBalloon.Proposal.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<HotairBalloon, HotairBalloon.Proposal.Definition>(Singleton);
        }

        public static bool ProposalTest(HotairBalloon ths, Sim a, Sim b, bool autonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if ((ths.mCurrentHeight == HotairBalloon.BalloonHeight.OnGround) || (ths.mTargetHeight == HotairBalloon.BalloonHeight.OnGround))
            {
                return false;
            }

            if (a == b)
            {
                return false;
            }

            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(HotairBalloon.LocalizeString(a.IsFemale, "ProposalTooltip", new object[0x0]));
            if ((a.SimDescription.IsGhost && !a.SimDescription.IsPlayableGhost) || (b.SimDescription.IsGhost && !b.SimDescription.IsPlayableGhost))
            {
                return false;
            }

            // Custom
            return CommonSocials.TestProposeMarriage(a, b, null, autonomous, ref greyedOutTooltipCallback);
        }

        public new class Definition : HotairBalloon.Proposal.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new HotAirBalloonProposal();
                result.Init(ref parameters);
                return result;
            }

            public override bool Test(Sim a, HotairBalloon target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!(a.Posture is HotairBalloon.InBalloonPosture))
                    {
                        return false;
                    }

                    Sim otherSim = target.GetOtherSim(a);
                    if (otherSim == null)
                    {
                        return false;
                    }

                    // Custom
                    return ProposalTest(target, a, otherSim, isAutonomous, ref greyedOutTooltipCallback);
                }
                catch (ResetException)
                {
                    throw;
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
