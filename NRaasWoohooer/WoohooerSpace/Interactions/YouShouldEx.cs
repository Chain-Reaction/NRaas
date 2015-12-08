using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Skills;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class YouShouldEx : Common.IPreLoad, Common.IAddInteraction
    {
        public void OnPreLoad()
        {
            Tunings.Inject<Sim, YouShould.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP3)) return;

            interactions.AddCustom(new CustomInjector());
        }

        public class CustomInjector : Common.InteractionNoDupTestInjector<Sim>
        {
            public CustomInjector()
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                for (int i = obj.Interactions.Count - 1; i >= 0; i--)
                {
                    if (obj.Interactions[i].InteractionDefinition.GetType() == typeof(YouShould.Definition))
                    {                        
                        obj.Interactions.RemoveAt(i);
                    }
                }

                base.Perform(obj, new YouShouldEx.Definition("NRaasWooHoo", "Gameplay/Excel/Socializing/Action:NRaasWooHoo"), existing); 
                base.Perform(obj, new YouShouldEx.Definition("NRaasTryForBaby", "Gameplay/Excel/Socializing/Action:NRaasTryForBaby"), existing); 
                base.Perform(obj, new YouShouldEx.Definition("NRaasRiskyWooHoo", "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo"), existing); 

                int length = YouShould.kYouShouldSocials.GetLength(0x0);
                for (int i = 0x0; i < length; i++)
                {
                    base.Perform(obj, new YouShouldEx.Definition(YouShould.kYouShouldSocials[i, 0x0], YouShould.kYouShouldSocials[i, 0x1]), existing);
                }

                return true;
            }
        }

        public class Definition : YouShould.Definition
        {
            public Definition()
            { }
            public Definition(string pushedSocialActionKey, string interactionNameLocalizationKey)
                : base(pushedSocialActionKey, interactionNameLocalizationKey)
            { }

            public override List<Sim> GetYouShouldTargets(Sim thisActor, Sim pushedSocialActor)
            {
                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);
                SocialInteractionA.Definition pushedSocialDefinition = new SocialInteractionA.Definition(mPushedSocialActionKey, new string[0x0], null, false);
                pushedSocialDefinition.ChecksToSkip = mChecksToSkip;

                List<Sim> list = new List<Sim>();
                foreach (Sim sim in pushedSocialActor.LotCurrent.GetAllActors())
                {
                    if (sim == thisActor) continue;
                    
                    if (sim == pushedSocialActor) continue;
                    
                    if (sim.SimDescription.HasActiveRole) continue;

                    GreyedOutTooltipCallback tooltipCallback = null;
                    if (!PushedSocialTest(pushedSocialActor, sim, pushedSocialDefinition, priority, tooltipCallback))
                    {
                        continue;
                    }

                    ActionData data = ActionData.Get(mPushedSocialActionKey);
                    if ((data != null) && ((data.IntendedCommodityString == CommodityTypes.Amorous) || (data.IntendedCommodityString == AmorousCommodity.sAmorous2)))
                    {
                        // Custom Function
                        string reason;
                        if (!CommonSocials.CanGetRomantic(sim.SimDescription, pushedSocialActor.SimDescription, false, true, true, ref tooltipCallback, out reason))
                        {
                            continue;
                        }
                    }
                    
                    list.Add(sim);
                }

                return list;
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                InteractionTestResult result = base.Test(ref parameters, ref greyedOutTooltipCallback);
                if (result == InteractionTestResult.Def_TestFailed)
                {
                    Sim actor = parameters.Actor as Sim;
                    Sim target = parameters.Target as Sim;

                    if (!actor.SimDescription.IsVampire)
                    {
                        return InteractionTestResult.Def_TestFailed;
                    }
                    else if (!actor.Posture.AllowsNormalSocials() || !target.Posture.AllowsNormalSocials())
                    {
                        return InteractionTestResult.GenericFail;
                    }
                    else if ((actor.Posture is SwimmingInPool) || (target.Posture is SwimmingInPool))
                    {
                        return InteractionTestResult.Social_TargetInPool;
                    }
                    else if (Relationship.AreStrangers(actor, target) || target.NeedsToBeGreeted(actor))
                    {
                        return InteractionTestResult.Social_TargetIsUngreetedOnCurrentLot;
                    }
                    else if (!CelebrityManager.CanSocialize(actor, target))
                    {
                        return InteractionTestResult.Social_TargetCannotBeSocializedWith;
                    }
                    else if (GetYouShouldTargets(actor, target).Count == 0x0)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(YouShould.LocalizeString("InteractionUnavailable", new object[0x0]));
                        return InteractionTestResult.GenericFail;
                    }

                    return InteractionTestResult.Pass;
                }

                return result;
            }
        }
    }
}
