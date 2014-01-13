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
    public class KillSim : SocialInteraction, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        protected bool OnPerform()
        {
            Definition interactionDefinition = InteractionDefinition as Definition;

            Target.Kill(interactionDefinition.mDeathType);
            Target.InteractionQueue.RemoveInteractionByRef(this);

            Sim selectedObject = GetSelectedObject() as Sim;
            if (selectedObject == null) return false;

            Target.SimDescription.GetRelationship(selectedObject.SimDescription, true).LTR.SetLiking(-100f);
            return true;
        }

        public override bool Run()
        {
            try
            {
                Actor.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                Target.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                Target.InteractionQueue.CancelAllInteractions();
                if (BeginSocialInteraction(new SocialInteractionB.Definition(null, Common.Localize("KillSim:MenuName", Actor.IsFemale), false), false, 3f, true))
                {
                    if (MagicWand.PerformAnimation(Actor, OnPerform))
                    {
                        Magic.EnsureSkill(Actor).IncreaseEvilSpellCount();
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

        [DoesntRequireTuning]
        public sealed class Definition : InteractionDefinition<Sim, Sim, KillSim>
        {
            public SimDescription.DeathType mDeathType;
            string mMenuText;

            public Definition()
            { }
            public Definition(string text, SimDescription.DeathType deathType)
            {
                mMenuText = text;
                mDeathType = deathType;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                foreach (SimDescription.DeathType deathType in Enum.GetValues(typeof(SimDescription.DeathType)))
                {
                    if (deathType == SimDescription.DeathType.None) continue;

                    if (deathType == SimDescription.DeathType.Meteor) continue;

                    results.Add(new InteractionObjectPair(new Definition(deathType.ToString(), deathType), iop.Target));
                }
            }

            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return mMenuText;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.Localize("KillBy:MenuName", isFemale) };
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (((!target.SimDescription.IsGhost && (a != target)) && (a.SimDescription.TeenOrAbove && target.SimDescription.TeenOrAbove)) && a.Inventory.ContainsType(typeof(MagicWand), 1))
                {
                    if (Magic.GetSkillLevel(a) >= Magic.Settings.mKillSimLevel)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}

