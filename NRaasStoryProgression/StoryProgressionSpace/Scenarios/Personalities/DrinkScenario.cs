using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Personalities
{
    public class DrinkScenario : CohesionScenario
    {
        public DrinkScenario()
        { }
        protected DrinkScenario(DrinkScenario scenario)
            : base (scenario)
        {}

        protected override bool Allow(SimDescription sim)
        {
            if (sim.OccultManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (!sim.OccultManager.HasOccultType(OccultTypes.Vampire))
            {
                IncStat("Not Vampire");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim.OccultManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (sim.OccultManager.HasAnyOccultType())
            {
                IncStat("Already Occult");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            Drink(Sim.CreatedSim, Target.CreatedSim);

            return true;
        }

        public static void Drink(Sim sim, Sim target)
        {
            if (sim != null)
            {
                sim.BuffManager.RemoveElement(BuffNames.HeatingUp);
                sim.BuffManager.RemoveElement(BuffNames.TooMuchSun);

                if (sim.Motives.IsFullEnoughForStuffedBuff())
                {
                    sim.BuffManager.AddElement(BuffNames.Stuffed, Origin.FromCarnivorousBehavior);
                }

                sim.Motives.SetMax(CommodityKind.VampireThirst);

                sim.BuffManager.AddElement(BuffNames.SanguineSnack, Origin.FromReceivingVampireNutrients);
            }

            if (target != null)
            {
                target.BuffManager.AddElement(BuffNames.Weakened, Origin.FromProvidingVampireNutrients);
            }
        }

        public static bool DrinkFirstAction(GoToLotSituation parent, GoToLotSituation.MeetUp meetUp)
        {
            try
            {
                NRaas.StoryProgression.Main.Situations.IncStat("First Action: Drink");

                meetUp.ForceSituationSpecificInteraction(parent.mSimB, parent.mSimA, new SocialInteractionA.Definition("Vampire Drink", null, null, false), null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed);
                return true;
            }
            catch (Exception e)
            {
                Common.DebugException(parent.mSimA, parent.mSimB, e);
                return false;
            }
        }

        protected override GoToLotSituation.FirstActionDelegate FirstAction
        {
            get { return DrinkFirstAction; }
        }

        public override Scenario Clone()
        {
            return new DrinkScenario(this);
        }
    }
}
