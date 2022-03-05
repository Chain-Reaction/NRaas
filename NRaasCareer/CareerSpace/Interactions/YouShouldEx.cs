using NRaas.CareerSpace.Skills;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class YouShouldEx : SocialInteractionA, Common.IPreLoad, Common.IAddInteraction
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

        public override string GetInteractionName()
        {
            return Definition.GetLocalizedYouShouldPieMenuName();
        }

        public override bool Run()
        {
            try
            {
                Sim selectedObject = GetSelectedObject() as Sim;
                if (selectedObject != null)
                {
                    base.Run();
                    if (mResultCode != SocialInteraction.SocialResultCode.Succeeded)
                    {
                        return false;
                    }

                    SocialRule targetEffect = mTargetEffect;
                    if (targetEffect != null)
                    {
                        if (targetEffect.RHS.STEffectCommodity != CommodityTypes.Friendly)
                        {
                            return false;
                        }

                        Definition interactionDefinition = InteractionDefinition as Definition;
                        InteractionPriority priority = GetPriority();
                        InteractionDefinition definition2 = interactionDefinition.GetPushDefintion(Target, selectedObject, priority);
                        if (definition2 == null)
                        {
                            return false;
                        }

                        // Never could figure out why adding routing pushes here just does nothing...

                        Kill instance = Kill.Singleton.CreateInstance(selectedObject, Target, priority, false, true) as Kill;
                        Kill.Definition def = instance.InteractionDefinition as Kill.Definition;
                        def.mType = interactionDefinition.mType;
                        def.mDirect = false;
                        if (!Target.InteractionQueue.PushAsContinuation(instance, true))
                        {
                            return false;
                        }

                        Assassination skill = Assassination.EnsureSkill(Target);

                        skill.AddJob(Actor, selectedObject);

                        return true;
                    }
                }
                return false;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public class CustomInjector : Common.InteractionNoDupTestInjector<Sim>
        {
            public CustomInjector()
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                foreach (SimDescription.DeathType type in Assassination.Types.Keys)
                {
                    base.Perform(obj, new Definition("NRaas Assassin " + type, "Gameplay/Excel/Socializing/Action:NRaasAssassin" + type, type), existing);
                }

                return true;
            }
        }

        public new class Definition : YouShould.Definition
        {
            public Definition()
            { }
            public Definition(string pushedSocialActionKey, string interactionNameLocalizationKey, SimDescription.DeathType type)
                : base(pushedSocialActionKey, interactionNameLocalizationKey)
            {
                mType = type;            
            }

            public SimDescription.DeathType mType;

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new YouShouldEx();
                na.Init(ref parameters);
                return na;
            }

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

                    GreyedOutTooltipCallback tooltipCallback = null;
                    if (!PushedSocialTest(pushedSocialActor, sim, pushedSocialDefinition, priority, tooltipCallback))
                    {
                        continue;
                    }

                    GreyedOutTooltipCallback callback = null;
                    if (!Assassination.CanBeKilled(sim, ref callback))
                    {
                        continue;
                    }
                    
                    list.Add(sim);
                }

                return list;
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {          
                if (a.FamilyFunds < Assassination.Settings.mHiringCost)
                {                    
                    greyedOutTooltipCallback = Common.DebugTooltip("Funds Fail");
                    return false;
                }

                if (!Assassination.Allow(a, target, mType, isAutonomous, true, false, ref greyedOutTooltipCallback))
                {
                    return false;
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
           
            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                InteractionTestResult r = base.Test(ref parameters, ref greyedOutTooltipCallback);
                
                // EA, GENIUS!
                if (r == InteractionTestResult.Def_TestFailed && this.Test(parameters.Actor as Sim, parameters.Target as Sim, parameters.Autonomous, ref greyedOutTooltipCallback))
                {
                    r = InteractionTestResult.Pass;
                }

                if (r == InteractionTestResult.Social_TargetCannotBeSocializedWith)
                {
                    r = InteractionTestResult.Pass;
                }

                return r;
            }
        }
    }
}
