using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.Pools;
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

namespace NRaas.CommonSpace.Interactions
{
    public abstract class CommonSkillTutor<TDefinition> : TutorSkillInteraction, Common.IPreLoad, Common.IAddInteraction
        where TDefinition : TutorSkillInteraction.Definition, new()
    {
        public abstract void AddInteraction(Common.InteractionInjectorList interactions);

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, TutorSkillInteraction.Definition, TDefinition>(false);
        }

        public override void PreSocialLoop()
        {
            try
            {
                base.PreSocialLoop();

                TargetSimSkill.StopSkillGain();

                float rate = 2.5f / 60f;
                if (this.ActorLogicSkill.IsSkillTeacherExtraordinaire)
                {
                    rate *= LogicSkill.TutorRateBonusForSkillTeacherExtraordinaire;
                }
                TargetSimSkill.StartSkillGain(rate, Actor.IsSelectable);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        public abstract class BaseDefinition<TInteraction> : TutorSkillInteraction.Definition
            where TInteraction : CommonSkillTutor<TDefinition>, new ()
        {
            protected BaseDefinition()
            { }
            protected BaseDefinition(SkillNames skillName)
                : base("Tutor Sim in Skill", Common.LocalizeEAString("Gameplay/Skills/Logic:TutorSkill"), skillName)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                SocialInteractionA na = new TInteraction();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                results.Add(new InteractionObjectPair(new TDefinition(), target));
            }

            public override bool Test(Sim initiatingSim, Sim targetSim, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    LogicSkill skill = initiatingSim.SkillManager.GetSkill<LogicSkill>(SkillNames.Logic);
                    if ((skill != null) && (skill.SkillLevel < skill.MaxSkillLevel))
                    {
                        return false;
                    }

                    return base.Test(initiatingSim, targetSim, isAutonomous, ref greyedOutTooltipCallback);
                }
                catch (Exception e)
                {
                    Common.Exception(initiatingSim, targetSim, e);
                }

                return false;
            }
        }
    }
}
