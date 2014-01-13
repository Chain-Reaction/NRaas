using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public abstract class HaveBabyBaseScenario : DualSimScenario
    {
        public HaveBabyBaseScenario()
        { }
        public HaveBabyBaseScenario (SimDescription sim, SimDescription target)
            : base(sim, target)
        { }
        protected HaveBabyBaseScenario(HaveBabyBaseScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            if (sim.Partner == null) return null;

            List<SimDescription> list = new List<SimDescription>();
            list.Add(sim.Partner);
            return list;
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (Sims.HasEnough(this, sim))
            {
                IncStat("Maximum Reached");
                return false;
            }
            else if (sim.IsPregnant)
            {
                IncStat("Couple Pregnant");
                return false;
            }
            else if (SimTypes.InServicePool(sim, ServiceType.GrimReaper))
            {
                IncStat("Reaper");
                return false;
            }
            else if (sim.IsMummy)
            {
                IncStat("Mummy");
                return false;
            }
            
            return base.CommonAllow(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!base.Allow(sim)) return false;

            if (SimTypes.IsDead(sim))
            {
                IncStat("Dead");
                return false;
            }
            else if (SimTypes.IsTourist(sim))
            {
                IncStat("Tourist");
                return false;
            }
            else if ((sim.AgingState != null) && (sim.AgingState.IsAgingInProgress()))
            {
                IncStat("Aging");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("No Household");
                return false;
            }
            else if (HouseholdHasAgelessBaby(sim.Household))
            {
                IncStat("Unaging Baby");
                return false;
            }
            else if (HouseholdsEx.IsFull(this, sim.Household, sim.Species, 0, true, true))
            {
                IncStat("House Full");
                return false;
            }
            else if ((sim.CreatedSim != null) && (sim.CreatedSim.BuffManager.HasTransformBuff ()))
            {
                IncStat("Transformed");
                return false;
            }
            else if (!Pregnancies.AllowImpregnation(this, sim, Managers.Manager.AllowCheck.Active))
            {
                IncStat("User Denied");
                return false;
            }

            return true;
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if (target.IsFrankenstein)
            {
                IncStat("Partner Simbot");
                return false;
            }
            else if ((Sim.CreatedSim != null) && (Stylist.IsStyleeJobTargetOfAnyStyler(Sim.CreatedSim)))
            {
                IncStat("Stylee");
                return false;
            }
            else if (!Pregnancies.Allow(this, Sim, Target, Managers.Manager.AllowCheck.Active))
            {
                return false;
            }

            return base.TargetAllow(target);
        }

        protected bool HouseholdHasAgelessBaby(Household house)
        {
            if (house == null) return false;

            foreach (SimDescription sim in HouseholdsEx.All(house))
            {
                if ((sim.ToddlerOrBelow) && (!Sims.AllowAging(this, sim)))
                {
                    return true;
                }
            }

            return false;
        }

        public static event UpdateDelegate OnGatheringScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Sim.CreatedSim == null)
            {
                Sims.Instantiate(Sim, Sim.LotHome, false);
            }

            if (Target.CreatedSim == null)
            {
                Sims.Instantiate(Target, Target.LotHome, false);
            }

            if ((Sim.CreatedSim == null) || (Target.CreatedSim == null))
            {
                IncStat("Uninstantiated");
                return false;
            }
            else
            {
                if (Sim.IsRobot)
                {
                    if ((Sim.LotHome == null) || 
                        ((Sim.LotHome.CountObjects<BotMakingStation> () + Sim.LotHome.CountObjects<InventionWorkbench>()) == 0))
                    {
                        IncStat("No Workbench");
                        return false;
                    }

                    SimDescription child = null;
                    if (Sim.IsFrankenstein)
                    {
                        bool reward = Sim.OccultManager.mIsLifetimeReward;

                        if ((Target.IsFrankenstein) && (RandomUtil.CoinFlip()))
                        {
                            reward = Target.OccultManager.mIsLifetimeReward;
                        }

                        Sim childSim = OccultFrankenstein.CreateFrankenStein(Sim.CreatedSim, CASAgeGenderFlags.None, reward);
                        if (childSim == null)
                        {
                            IncStat("Creation Fail");
                            return false;
                        }

                        child = childSim.SimDescription;
                    }
                    else
                    {
                        child = OccultRobot.MakeRobot(CASAgeGenderFlags.Adult, CASAgeGenderFlags.None, RobotForms.MaxType);
                        if (child == null)
                        {
                            IncStat("Creation Fail");
                            return false;
                        }

                        CASRobotData supernaturalData = child.SupernaturalData as CASRobotData;
                        if (supernaturalData != null)
                        {
                            supernaturalData.CreatorSim = Sim.SimDescriptionId;

                            int quality = 0;
                            int count = 0;

                            CASRobotData parentData = Sim.SupernaturalData as CASRobotData;
                            if (parentData != null)
                            {
                                quality = parentData.BotQualityLevel;
                                count++;
                            }

                            parentData = Target.SupernaturalData as CASRobotData;
                            if (parentData != null)
                            {
                                quality += parentData.BotQualityLevel;
                                count++;
                            }

                            if (count == 2)
                            {
                                quality /= count;
                            }

                            supernaturalData.BotQualityLevel = quality;
                        }
                    }

                    if (child.Genealogy.Parents.Count == 0)
                    {
                        Sim.Genealogy.AddChild(child.Genealogy);
                    }

                    Target.Genealogy.AddChild(child.Genealogy);

                    if (!Households.MoveSim(child, Sim.Household))
                    {
                        IncStat("Move Fail");

                        Deaths.CleansingKill(child, true);

                        return false;
                    }

                    return true;
                }
                else if (Target.IsRobot)
                {
                    IncStat("Simbot Partner");
                    return false;
                }
                else
                {
                    if (CommonSpace.Helpers.Pregnancies.Start(Sim.CreatedSim, Target, false) != null)
                    {
                        ManagerSim.ForceRecount();

                        if (Sim.IsHuman)
                        {
                            if (OnGatheringScenario != null)
                            {
                                OnGatheringScenario(this, frame);
                            }
                        }

                        if ((!Sim.IsHuman) && (Sim.Partner != Target))
                        {
                            if ((GetValue<AllowMarriageOption, bool>(Sim)) && (GetValue<AllowMarriageOption, bool>(Target)))
                            {
                                if ((Romances.AllowBreakup(this, Sim, Managers.Manager.AllowCheck.None)) && (Romances.AllowBreakup(this, Target, Managers.Manager.AllowCheck.None)))
                                {
                                    RemoveAllPetMateFlags(Sim);
                                    RemoveAllPetMateFlags(Target);
                                    Relationship.Get(Sim, Target, false).LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Marry);
                                }
                            }
                        }
                    }
                    return true;
                }
            }
        }

        public static void RemoveAllPetMateFlags(SimDescription pet)
        {
            foreach (Relationship relationship in Relationship.Get(pet))
            {
                if (relationship.IsPetToPetRelationship)
                {
                    relationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Marry);
                }
            }
        }
    }
}
