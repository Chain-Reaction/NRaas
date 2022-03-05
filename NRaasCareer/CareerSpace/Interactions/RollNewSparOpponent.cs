using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using System;

namespace NRaas.CareerSpace.Interactions
{
    public class RollNewSparOpponent : Computer.ComputerInteraction, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Computer>(Singleton);
        }

        public RollNewSparOpponent()
        { }

        public class Affinity
        {
            MartialArts mSkill;

            public Affinity(MartialArts skill)
            {
                mSkill = skill;
            }

            public int GetAffinity(SimDescription simDescription)
            {
                int num = 0x0;
                if (mSkill.mSkillOwner.Household != simDescription.Household)
                {
                    num++;
                }

                // Custom
                if (simDescription.AssignedRole != null)
                {
                    return 0x0;
                }

                int num2 = MartialArts.kMartialArtsSkillForOpponentPerTournamentRank[mSkill.mTournamentRank];
                SkillManager skillManager = simDescription.SkillManager;
                if (skillManager != null)
                {
                    MartialArts skill = skillManager.GetSkill<MartialArts>(SkillNames.MartialArts);
                    if ((skill == null) || (!skill.CanParticipateInTournaments))
                    {
                        MiniSimDescription miniSim = simDescription.GetMiniSimForProtection();
                        if ((GameUtils.GetCurrentWorld() == WorldName.China) || ((miniSim != null) && (miniSim.ProtectionFlags == MiniSimDescription.ProtectionFlag.None)))
                        {
                            num += mSkill.MaxSkillLevel - num2;
                        }
                        else
                        {
                            return 0x0;
                        }
                    }
                    else
                    {
                        int num3 = Math.Abs((int)(num2 - skill.SkillLevel));
                        num += mSkill.MaxSkillLevel - num3;
                    }

                    if (skillManager.HasElement(SkillNames.Athletic))
                    {
                        num += MartialArts.kIncreaseInAffinityForOpponentsWithAthleticSkill;
                    }
                }
                return num;
            }
        }

        private static SimDescription FindSparTournamentOpponent(MartialArts ths, SimDescription presentChallenger)
        {
            if ((ths.SkillOwner == null) || ((ths.SkillOwner.Household != null) && !ths.SkillOwner.Household.IsActive))
            {
                return null;
            }

            SimDescription description = TournamentManagement.FindSuitableOpponent(ths.mSkillOwner, Household.sHouseholdList, presentChallenger, new Affinity (ths).GetAffinity);
            if (description != null)
            {
                Household household = description.Household;
                if (!household.IsActive && !household.IsTravelHousehold)
                {
                    MartialArts arts = description.SkillManager.AddElement(SkillNames.MartialArts) as MartialArts;
                    if (arts != null)
                    {
                        int num = MartialArts.kMartialArtsSkillForOpponentPerTournamentRank[ths.mTournamentRank];
                        if (arts.SkillLevel < num)
                        {
                            if ((GameUtils.GetCurrentWorld() == WorldName.China) || (description.GetMiniSimForProtection().ProtectionFlags == MiniSimDescription.ProtectionFlag.None))
                            {
                                int num2 = num;
                                num2 += RandomUtil.GetInt(0x0, MartialArts.kMaximumVarienceInSkillLevelForSparTournametOpponent);
                                num2 = MathUtils.Clamp(num2, MartialArts.kMartialArtsSkillForOpponentPerTournamentRank[0x1], arts.MaxSkillLevel);
                                arts.ForceSkillLevelUp(num2);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return description;
        }

        public static bool Perform(Sim actor)
        {
            MartialArts skill = actor.SkillManager.GetSkill<MartialArts>(SkillNames.MartialArts);
            if (skill == null) return false;

            SimDescription sim = FindSparTournamentOpponent(skill, skill.TournamentChallenger);
            if (sim == null)
            {
                Common.Notify(Common.Localize("NewSparOpponent:NoResult", actor.IsFemale, new object[] { actor }), actor.ObjectId);

                skill.mTournamentChallenger = 0;
                return false;
            }

            skill.mTournamentChallenger = sim.SimDescriptionId;
            return true;
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

                AnimateSim("GenericTyping");

                if (Perform(Actor))
                {
                    Common.Notify(Common.Localize("NewSparOpponent:Result", Actor.IsFemale, new object[] { Actor }), Actor.ObjectId);
                }

                Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                StandardExit();
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        // Nested Types
        private sealed class Definition : InteractionDefinition<Sim, Computer, RollNewSparOpponent>
        {
            // Methods
            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                return Common.Localize("NewSparOpponent:MenuName", actor.IsFemale, new object[0]);
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;

                if (!target.IsComputerUsable(a, true, false, isAutonomous)) return false;

                MartialArts skill = a.SkillManager.GetSkill<MartialArts>(SkillNames.MartialArts);
                if (skill == null) return false;

                return skill.CanParticipateInTournaments;
            }
        }
    }
}
