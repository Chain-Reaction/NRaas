using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CareerSpace.Skills;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class Bribe : RabbitHole.RabbitHoleInteraction<Sim, RabbitHole>, Common.IAddInteraction
    {
        // Fields
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<PoliceStation>(Singleton);
            interactions.Add<CityHall>(Singleton);
        }

        // Methods
        public override void ConfigureInteraction()
        {
            base.ConfigureInteraction();
            TimedStage stage = new TimedStage(GetInteractionName(), RabbitHole.InvestInRabbithole.kTimeToSpendInside, false, false, true);
            Stages = new List<Stage>(new Stage[] { stage });
            ActiveStage = stage;
        }

        public override bool InRabbitHole()
        {
            try
            {
                Assassination skill = Actor.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
                if (skill == null) return false;

                if (skill.GetNetAggression() == 0) return false;

                StartStages();
                BeginCommodityUpdates();
                bool succeeded = false;

                try
                {
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                if (!succeeded)
                {
                    return succeeded;
                }

                int owed = (int)(skill.GetNetAggression() * (1 + Assassination.Settings.mRabbitHoleCorruption));

                int funds = Actor.FamilyFunds;
                if (funds > owed)
                {
                    funds = owed;
                }

                string text = StringInputDialog.Show(Common.Localize("Bribe:MenuName", Actor.IsFemale), Common.Localize("Bribe:Prompt", Actor.IsFemale, new object[] { Actor, Assassination.Settings.mRabbitHoleCorruption * 100, owed }), funds.ToString());
                if ((text == null) || (text == "")) return false;

                if (!int.TryParse(text, out funds)) return false;

                if (funds < 0) return false;

                if (funds > owed)
                {
                    funds = owed;
                }

                Actor.ModifyFunds(-funds);

                skill.Bribe((int)(funds * (1f - Assassination.Settings.mRabbitHoleCorruption)));
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

        // Nested Types
        public sealed class Definition : InteractionDefinition<Sim, RabbitHole, Bribe>
        {
            // Methods
            public Definition()
            { }

            public override string GetInteractionName(Sim a, RabbitHole target, InteractionObjectPair interaction)
            {
                return Common.LocalizeEAString(a.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasAssassinBribe", new object[0]);
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a.SkillManager == null) return false;

                Assassination skill = a.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
                if (skill == null) return false;

                if (skill.GetNetAggression() == 0) return false;

                return true;
            }
        }

        public static bool OnCallbackTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (actor.Household == target.Household) return false;

                if (actor.SkillManager == null) return false;

                Assassination skill = actor.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
                if (skill == null) return false;

                if (skill.GetNetAggression() == 0) return false;

                Occupation career = target.Occupation;
                if (career == null) return false;

                if (career is LawEnforcement) return true;

                if (career is Political) return true;

                return false;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnScoreAwkward(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                bool witnessed = false;

                foreach (Sim sim in target.LotCurrent.GetSims())
                {
                    if (sim == actor) continue;

                    if (sim == target) continue;

                    if (sim.RoomId == actor.RoomId)
                    {
                        witnessed = true;
                        break;
                    }
                }

                return witnessed;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnScoreInsulting(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (target.HasTrait(TraitNames.Good)) return true;

                return false;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void OnAccepted(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                Assassination skill = actor.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
                if (skill == null) return;

                if (skill.GetNetAggression() == 0) return;

                int owed = skill.GetNetAggression();

                int funds = actor.FamilyFunds;
                if (funds > owed)
                {
                    funds = owed;
                }

                if (i.Autonomous)
                {
                    funds = RandomUtil.GetInt(owed);
                }
                else
                {
                    string text = StringInputDialog.Show(Common.Localize("Bribe:MenuName", actor.IsFemale), Common.Localize("Bribe:SocialPrompt", actor.IsFemale, new object[] { actor, owed }), funds.ToString());
                    if ((text == null) || (text == "")) return;

                    if (!int.TryParse(text, out funds)) return;
                }

                if (funds < 0) return;

                if (funds > owed)
                {
                    funds = owed;
                }

                actor.ModifyFunds(-funds);

                target.ModifyFunds(funds);

                skill.Bribe(funds);
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
