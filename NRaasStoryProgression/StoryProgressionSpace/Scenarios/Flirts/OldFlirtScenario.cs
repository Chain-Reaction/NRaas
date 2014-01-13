using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Flirts
{
    public class OldFlirtScenario : FlirtBaseScenario
    {
        int mPregnancyChance = -1;

        ManagerRomance.AffairStory mAffairStory = ManagerRomance.AffairStory.All;

        public OldFlirtScenario(SimDescription sim, SimDescription target, bool report, ManagerRomance.AffairStory affairStory, int pregnancyChance)
            : base(sim, target, "OldFlirt", report)
        {
            mPregnancyChance = pregnancyChance;
            mAffairStory = affairStory;
        }
        protected OldFlirtScenario(OldFlirtScenario scenario)
            : base (scenario)
        {
            mPregnancyChance = scenario.mPregnancyChance;
            mAffairStory = scenario.mAffairStory;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "PregnancyChance=" + mPregnancyChance;
            text += Common.NewLine + "DetailedAffair=" + mAffairStory;

            return text;
        }

        public ManagerRomance.AffairStory AffairStory
        {
            get { return mAffairStory; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (row.Exists("ChanceOfPregnancy"))
            {
                mPregnancyChance = row.GetInt("ChanceOfPregnancy");
            }

            if (row.Exists("AffairStory"))
            {
                if (!ParserFunctions.TryParseEnum<ManagerRomance.AffairStory>(row.GetString("AffairStory"), out mAffairStory, ManagerRomance.AffairStory.None))
                {
                    error = "AffairStory unknown";
                    return false;
                }
            }

            return base.Parse(row, ref error);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            Relationship relationship = ManagerSim.GetRelationship(Sim, Target);
            if (relationship == null)
            {
                IncStat("Bad Relation");
                return false;
            }
            else if (!relationship.AreRomantic())
            {
                IncStat("Not Romantic");
                return false;
            }

            LongTermRelationship LTR = relationship.LTR;
            if (LTR == null)
            {
                IncStat("Bad LTR");
                return false;
            }
            else if (LTR.Liking <= Sims3.Gameplay.Actors.Sim.kRomanceUseLikingGate)
            {
                IncStat("Too Low");
                return false;
            }

            if (mAffairStory == ManagerRomance.AffairStory.None)
            {
                if (!Romances.AllowAffair(this, Sim, Target, Managers.Manager.AllowCheck.None))
                {
                    IncStat("Affair Denied");
                    return false;
                }
            }

            return base.TargetAllow(sim);
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Flirts.FindExistingFor(this, sim, true);
        }

        public static event UpdateDelegate OnRomanceAffairScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Relationship relationship = ManagerSim.GetRelationship(Sim, Target);
            if (relationship == null) return false;

            LongTermRelationship LTR = relationship.LTR;
            if (LTR == null) return false;

            if (Sim.TraitManager.HasElement(TraitNames.GreatKisser) || Target.TraitManager.HasElement(TraitNames.GreatKisser))
            {
                if (Sim.CreatedSim != null)
                {
                    ManagerSim.AddBuff(this, Sim, BuffNames.GreatKisser, Origin.FromSocialization);
                }
                if (Target.CreatedSim != null)
                {
                    ManagerSim.AddBuff(this, Target, BuffNames.GreatKisser, Origin.FromSocialization);
                }
            }

            if (SimID.Matches(Flirts.PreviousLoveLoss, Sim))
            {
                Flirts.PreviousLoveLoss = null;
            }

            if (LTR.Liking > Sims3.Gameplay.Actors.Sim.kWooHooUseLikingGate)
            {
                if (!Sim.HadFirstWooHoo)
                {
                    Sim.SetFirstWooHoo();

                    if ((Sim.CreatedSim != null) && (Target.CreatedSim != null))
                    {
                        EventTracker.SendEvent(EventTypeId.kHadFirstWoohoo, Sim.CreatedSim, Target.CreatedSim);
                    }
                }

                if (!Target.HadFirstWooHoo)
                {
                    Target.SetFirstWooHoo();

                    if ((Sim.CreatedSim != null) && (Target.CreatedSim != null))
                    {
                        EventTracker.SendEvent(EventTypeId.kHadFirstWoohoo, Target.CreatedSim, Sim.CreatedSim);
                    }
                }

                LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Kissed);
                LTR.AddInteractionBit(LongTermRelationship.InteractionBits.WooHoo);

                if (Sim.IsHuman)
                {
                    Sim.SetFirstKiss(Target);
                    Target.SetFirstKiss(Sim);
                }
                else
                {
                    if ((Sim.CreatedSim != null) && (Target.CreatedSim != null))
                    {
                        EventTracker.SendEvent(EventTypeId.kPetWooHooed, Sim.CreatedSim, Target.CreatedSim);
                        EventTracker.SendEvent(EventTypeId.kPetWooHooed, Target.CreatedSim, Sim.CreatedSim);
                    }
                }

                if (mPregnancyChance != 0)
                {
                    Add(frame, new UnexpectedPregnancyScenario(Sim, Target, mPregnancyChance), ScenarioResult.Start);
                }
                else
                {
                    Romances.AddWoohooerNotches(Sim, Target, false, false);
                }
            }

            if (OnRomanceAffairScenario != null)
            {
                OnRomanceAffairScenario(this, frame);
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new OldFlirtScenario(this);
        }
    }
}
