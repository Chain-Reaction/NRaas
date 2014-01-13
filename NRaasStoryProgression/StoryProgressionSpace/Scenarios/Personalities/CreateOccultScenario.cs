using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
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
    public abstract class CreateOccultScenario : RelationshipScenario, IHasPersonality
    {
        OccultTypes mOccult;

        bool mAllowHybrid;

        public CreateOccultScenario(int delta, OccultTypes type)
            : base (delta)
        {
            mOccult = type;
            mAllowHybrid = false;
        }
        protected CreateOccultScenario(CreateOccultScenario scenario)
            : base (scenario)
        {
            mOccult = scenario.mOccult;
            mAllowHybrid = scenario.mAllowHybrid;
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected override bool CheckBusy
        {
            get { return base.ShouldPush; }
        }

        public SimPersonality Personality
        {
            get { return Manager as SimPersonality; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return GetFilteredSims(Personalities.GetClanLeader(Manager));
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return GetFilteredTargets(sim);
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mOccult = row.GetEnum<OccultTypes>("Occult", OccultTypes.None);
            if (mOccult == OccultTypes.None)
            {
                error = "Occult Fail: " + row.GetString("Occult");
                return false;
            }

            mAllowHybrid = row.GetBool("AllowHybrid");

            return base.Parse(row, ref error);
        }

        protected override bool Allow()
        {
            if (!Sims.SatisfiesTownOccultRatio(mOccult))
            {
                IncStat("Too Many");
                return false;
            }

            return base.Allow();
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Personalities.Allow(this, sim))
            {
                IncStat("Personality User Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.OccultManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (!sim.OccultManager.HasOccultType(mOccult))
            {
                IncStat("Not " + mOccult);
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if (target.OccultManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (target.OccultManager.HasOccultType(mOccult))
            {
                IncStat("Already " + mOccult);
                return false;
            }
            else if ((!mAllowHybrid) && (target.OccultManager.HasAnyOccultType()))
            {
                IncStat("Already Other Occult");
                return false;
            }
            else if (!GetValue<AllowPersonalityOccultOption,bool>(target))
            {
                IncStat("Occult Denied");
                return false;
            }

            return base.TargetAllow(target);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!base.PrivateUpdate(frame)) return false;

            OccultTypeHelper.Add(Target, mOccult, false, true);

            return true;
        }

        public static bool DrinkFirstAction(GoToLotSituation parent, GoToLotSituation.MeetUp meetUp)
        {
            try
            {
                NRaas.StoryProgression.Main.Situations.IncStat("First Action: Vampire Drink");

                meetUp.ForceSituationSpecificInteraction(parent.mSimB, parent.mSimA, new SocialInteractionA.Definition("Vampire Drink", null, null, false), null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed);
                return true;
            }
            catch (Exception e)
            {
                Common.DebugException(parent.mSimA, parent.mSimB, e);
                return false;
            }
        }

        public static bool BiteFirstAction(GoToLotSituation parent, GoToLotSituation.MeetUp meetUp)
        {
            try
            {
                NRaas.StoryProgression.Main.Situations.IncStat("First Action: Werewolf Drink");

                meetUp.ForceSituationSpecificInteraction(parent.mSimB, parent.mSimA, new SocialInteractionA.Definition("Werewolf Cursed Bite", null, null, false), null, meetUp.OnSocialSucceeded, meetUp.OnSocialFailed);
                return true;
            }
            catch (Exception e)
            {
                Common.DebugException(parent.mSimA, parent.mSimB, e);
                return false;
            }
        }

        protected override bool Push()
        {
            GoToLotSituation.FirstActionDelegate firstAction = ManagerFriendship.FriendlyFirstAction;
            switch(mOccult)
            {
                case OccultTypes.Vampire:
                    firstAction = DrinkFirstAction;
                    break;
                case OccultTypes.Werewolf:
                    firstAction = BiteFirstAction;
                    break;
            }

            return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Either, firstAction);
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Occult: " + mOccult;

            return text;
        }
    }
}
