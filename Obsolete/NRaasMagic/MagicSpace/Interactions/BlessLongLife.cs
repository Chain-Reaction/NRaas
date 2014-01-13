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
    public class BlessLongLife : SocialInteraction, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        protected bool OnPerform()
        {
            AgingManager.Singleton.CancelAgingAlarmsForSim(Target);
            if (Target.SimDescription.Elder)
            {
                Target.SimDescription.AgingState.ExtendElderLifeSpan(20f);
            }
            else
            {
                Target.SimDescription.AgingState.ExtendAgingState(20f);
            }

            HudModel hudModel = (HudModel)Sims3.Gameplay.UI.Responder.Instance.HudModel;
            if (hudModel != null)
            {
                hudModel.OnSimAgeChanged(Target.ObjectId);
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
                if (BeginSocialInteraction(new SocialInteractionB.Definition(null, Common.Localize("BlessLongLife:MenuName", Actor.IsFemale), false), false, 3f, true))
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

        [DoesntRequireTuning]
        public sealed class Definition : InteractionDefinition<Sim, Sim, BlessLongLife>
        {
            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return Common.Localize("BlessLongLife:MenuName", a.IsFemale);
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.Localize("BlessWith:MenuName", isFemale) };
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (((a != target) && a.SimDescription.ChildOrAbove) && (target.SimDescription.ToddlerOrAbove && a.Inventory.ContainsType(typeof(MagicWand), 1)))
                {
                    if (Magic.GetSkillLevel(a) >= Magic.Settings.mBlessLongLifeLevel)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}

