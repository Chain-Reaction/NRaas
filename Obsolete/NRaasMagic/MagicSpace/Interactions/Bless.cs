using NRaas.MagicSpace.Skills;
using Sims3.Gameplay.Objects.KolipokiMod;
using Sims3.Gameplay;
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
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.RouteDestinations;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NRaas.MagicSpace.Interactions
{
    public class Bless : SocialInteraction, Common.IAddInteraction
    {
        static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        public bool OnPerform()
        {
            Definition interactionDefinition = InteractionDefinition as Definition;

            Target.TraitManager.AddElement(interactionDefinition.mTrait);

            return true;
        }

        public override bool Run()
        {
            try
            {
                Actor.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                Target.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                Target.InteractionQueue.CancelAllInteractions();
                if (BeginSocialInteraction(new SocialInteractionB.Definition(null, Common.Localize("BlessCreativity:MenuName", Actor.IsFemale), false), false, 3f, true))
                {
                    if (MagicWand.PerformAnimation(Actor, OnPerform))
                    {
                        Magic.EnsureSkill(Actor).IncreaseGoodSpellCount();
                    }

                    return true;
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

        public bool TestFunction(IGameObject obj, object customData)
        {
            return !base.Autonomous;
        }

        public sealed class Definition : InteractionDefinition<Sim, Sim, Bless>
        {
            public readonly int mLevel;
            public readonly TraitNames mTrait;

            public Definition()
            { }

            public Definition(TraitNames trait, int level)
            {
                mTrait = trait;
                mLevel = level;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                results.Add(new InteractionObjectPair(new Definition(TraitNames.ExtraCreative, Magic.Settings.mBlessExtraCreativeLevel), iop.Target));
                results.Add(new InteractionObjectPair(new Definition(TraitNames.FastLearner, Magic.Settings.mBlessFastLearnerLevel), iop.Target));
                results.Add(new InteractionObjectPair(new Definition(TraitNames.FertilityTreatment, Magic.Settings.mBlessFertilityTreatmentLevel), iop.Target));
                results.Add(new InteractionObjectPair(new Definition(TraitNames.Attractive, Magic.Settings.mBlessAttractiveLevel), iop.Target));
                results.Add(new InteractionObjectPair(new Definition(TraitNames.Observant, Magic.Settings.mBlessObservantLevel), iop.Target));
            }

            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return Common.Localize("Bless" + mTrait, a.IsFemale);
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.Localize("BlessWith:MenuName", isFemale) };
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
               if (((!target.TraitManager.HasElement(mTrait) && (a != target)) && (a.SimDescription.ChildOrAbove && target.SimDescription.ToddlerOrAbove)) && a.Inventory.ContainsType(typeof(MagicWand), 1))
                {
                    if (Magic.GetSkillLevel(a) >= mLevel)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}

