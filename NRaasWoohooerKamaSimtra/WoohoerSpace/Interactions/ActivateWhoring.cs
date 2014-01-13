using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using NRaas.WoohooerSpace.Skills;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class ActivateWhoring : Computer.ComputerInteraction, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Computer>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                StandardEntry();
                if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                {
                    StandardExit();
                    return false;
                }

                Target.StartVideo(Computer.VideoType.Browse);
                AnimateSim("GenericTyping");

                KamaSimtra skill = KamaSimtra.EnsureSkill(Actor);
                if (skill != null)
                {
                    skill.WhoringActive = !skill.WhoringActive;
                }

                Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                StandardExit();
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

        public sealed class Definition : InteractionDefinition<Sim, Computer, ActivateWhoring>
        {
            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                KamaSimtra skill = actor.SkillManager.GetSkill<KamaSimtra>(KamaSimtra.StaticGuid);
                if (skill == null) return "";

                if (skill.WhoringActive)
                {
                    return Common.Localize("DisableWhoring:MenuName");
                }
                else
                {
                    return Common.Localize("EnableWhoring:MenuName");
                }
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                using (WoohooTuningControl control = new WoohooTuningControl(parameters.InteractionObjectPair.Tuning, Woohooer.Settings.mAllowTeenWoohoo))
                {
                    return base.Test(ref parameters, ref greyedOutTooltipCallback);
                }
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!KamaSimtra.Settings.mShowRegisterInteraction) return false;

                if (!target.IsComputerUsable(a, true, false, isAutonomous)) return false;

                KamaSimtra skill = a.SkillManager.GetSkill<KamaSimtra>(KamaSimtra.StaticGuid);
                if (skill == null) return false;

                return skill.CanWhore();
            }
        }
    }
}
