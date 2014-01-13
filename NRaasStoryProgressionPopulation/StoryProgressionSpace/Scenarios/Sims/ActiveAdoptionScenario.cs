using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class ActiveAdoptionScenario : SimEventScenario<Event>
    {
        public ActiveAdoptionScenario()
        { }
        protected ActiveAdoptionScenario(ActiveAdoptionScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ActiveAdoption";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Task; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kAdoptedChild);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            foreach (Situation situation in Situation.sAllSituations)
            {
                SocialWorkerAdoptionSituation adoption = situation as SocialWorkerAdoptionSituation;
                if (adoption == null) continue;

                if (adoption.mAdoptingParent != Event.mActor) continue;

                SocialWorkerAdoptionSituation.InstantiateNewKid childSit = adoption.Child as SocialWorkerAdoptionSituation.InstantiateNewKid;
                if (childSit == null) continue;

                Sim oldChild = childSit.mAdoptedChild;

                using (SimFromBin<ManagerLot> simBin = new SimFromBin<ManagerLot>(this, Lots))
                {
                    SimDescription newChild = simBin.CreateNewSim(oldChild.SimDescription.Age, oldChild.SimDescription.Gender, CASAgeGenderFlags.Human);
                    if (newChild == null)
                    {
                        IncStat("Creation Failure");
                        return false;
                    }

                    newChild.WasAdopted = true;

                    adoption.mAdoptingParent.Household.Remove(oldChild.SimDescription);

                    adoption.mAdoptingParent.Household.Add(newChild);
                    adoption.mAdoptingParent.Genealogy.AddChild(newChild.Genealogy);

                    newChild.FirstName = oldChild.FirstName;
                    newChild.LastName = oldChild.LastName;

                    childSit.mAdoptedChild = newChild.Instantiate(Vector3.Zero);

                    Deaths.CleansingKill(oldChild.SimDescription, true);

                    return true;
                }
            }

            IncStat("Situation Not Found");
            return false;
        }

        public override Scenario Clone()
        {
            return new ActiveAdoptionScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSim, ActiveAdoptionScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ActiveAdoption";
            }
        }
    }
}
