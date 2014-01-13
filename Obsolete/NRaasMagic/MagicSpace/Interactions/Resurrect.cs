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
    public class Resurrect : SocialInteraction, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();
        
        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        protected bool OnPerform()
        {
            Urnstone urnstone = Urnstone.FindGhostsGrave(Target);
            if (urnstone != null)
            {
                urnstone.GhostToSim(Target, false, true);
                return true;
            }

            return false;
        }

        public override bool Run()
        {
            try
            {
                Actor.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                Target.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                Target.InteractionQueue.CancelAllInteractions();
                if (!BeginSocialInteraction(new SocialInteractionB.Definition(null, Common.Localize("Resurrect:MenuName", Actor.IsFemale), false), false, 3f, true))
                {
                    return false;
                }

                if (MagicWand.PerformAnimation(Actor, OnPerform))
                {
                    Magic.EnsureSkill(Actor).IncreaseGoodSpellCount();
                }
                return true;
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

        public sealed class Definition : InteractionDefinition<Sim, Sim, Resurrect>
        {
            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return Common.Localize("Resurrect:MenuName", a.IsFemale);
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (((a != target) && a.SimDescription.ChildOrAbove) && ((target.SimDescription.DeathStyle > SimDescription.DeathType.None) && a.Inventory.ContainsType(typeof(MagicWand), 1)))
                {
                    Magic magic = a.SkillManager.GetSkill<Magic>(Magic.StaticGuid);
                    if ((magic != null) && (magic.IsMasterWizard()))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}

