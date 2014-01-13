using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Tutorial;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerDeath : Manager
    {
        public enum Inheritors : int
        {
            Children = 0x01,
            Relatives = 0x02,
            Friends = 0x04,
        }

        Dictionary<ulong, bool> mCleansedSims = new Dictionary<ulong, bool>();

        public ManagerDeath(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Deaths";
        }

        private ListenerAction OnDisposed(Event e)
        {
            SimDescription simDesc = null;

            try
            {
                if (!GameStates.IsWorldShuttingDown)
                {
                    SimDescriptionEvent simDescEvent = e as SimDescriptionEvent;
                    if (simDescEvent != null)
                    {
                        simDesc = simDescEvent.SimDescription;
                        if ((simDesc != null) && (simDesc.SimDescriptionId != 0))
                        {
                            string name = simDesc.FullName.Trim();
                            if (!string.IsNullOrEmpty(name))
                            {
                                //Common.DebugStackLog(name);

                                SimDisposedScenario.Task.Perform(this, simDesc);
                            }
                            else
                            {
                                //Common.DebugStackLog(name);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception(simDesc, exception);
            }

            return ListenerAction.Keep;
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerDeath>(this).Perform(initial);
        }

        protected override string IsOnActiveLot(SimDescription sim, bool testActiveLot)
        {
            return null;
        }

        public bool Allow(IScoringGenerator stats, Sim sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IScoringGenerator stats, Sim sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, SimData settings, AllowCheck check)
        {
            if (!base.PrivateAllow(stats, sim, settings, check)) return false;

            if (SimTypes.IsDead(sim))
            {
                stats.IncStat("Allow: Dead");
                return false;
            }

            if (sim.ChildOrBelow)
            {
                stats.IncStat("Allow: Too Young");
                return false;
            }

            if (sim.IsPregnant)
            {
                stats.IncStat("Allow: Pregnant");
                return false;
            }

            if (!sim.AgingEnabled)
            {
                stats.IncStat("Allow: Aging Disabled");
                return false;
            }

            if (settings.GetValue<PushDeathChanceOption, int>() <= 0)
            {
                stats.IncStat("Allow: Sim Push Death Denied");
                return false;
            }

            return true;
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if (initialPass)
            {
                // This must be initiated AFTER the completion of OnWorldLoadFinished or we pick up a pile of load disposals
                AddListener(EventTypeId.kSimDescriptionDisposed, OnDisposed);
            }

            if (Autonomy.sOnceADayAlarm != AlarmHandle.kInvalidHandle)
            {
                AlarmManager.Global.RemoveAlarm (Autonomy.sOnceADayAlarm);
                Autonomy.sOnceADayAlarm = AlarmHandle.kInvalidHandle;

                IncStat("sOnceADayAlarm Dropped");
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public bool WasCleansed(SimDescription sim)
        {
            return mCleansedSims.ContainsKey(sim.SimDescriptionId);
        }

        public void SetCleansed(SimDescription sim)
        {
            if (WasCleansed(sim)) return;

            mCleansedSims.Add(sim.SimDescriptionId, true);
        }

        public static string ParseNotification(string key, Household houshold, SimDescription sim)
        {
            string str;
            if (sim.Household != null)
            {
                if (sim != null)
                {
                    str = Common.LocalizeEAString(sim.IsFemale, "Gameplay/Notifications/" + key, new object[] { sim, sim.Household.Name });
                }
                else
                {
                    str = Common.LocalizeEAString(sim.IsFemale, "Gameplay/Notifications/" + key, new object[] { sim.Household.Name });
                }
            }
            else if (sim != null)
            {
                str = Common.LocalizeEAString(sim.IsFemale, "Gameplay/Notifications/" + key, new object[] { sim });
            }
            else
            {
                str = Common.LocalizeEAString(sim.IsFemale, "Gameplay/Notifications/" + key, new object[0x0]);
            }
            return str;
        }

        public bool IsDying(SimDescription sim)
        {
            if (sim == null) return false;

            DyingSimData data = GetData<DyingSimData>(sim);

            if (data.Saved) return false;

            if (sim.Elder)
            {
                if (sim.AgingState == null) return false;

                if (sim.AgingState.AgeTransitionWithoutCakeAlarm == AlarmHandle.kInvalidHandle) return false;
            }
            else
            {
                if (!data.Testing) return false;
            }

            return true;
        }

        protected void GetInheritors(List<Genealogy> possibles, float defFraction, Dictionary<SimDescription, float> inheritors)
        {
            List<Genealogy> children = new List<Genealogy>(possibles);

            Dictionary<SimDescription, float> fractions = new Dictionary<SimDescription, float>();

            int index = 0;
            while (index < children.Count)
            {
                Genealogy childGene = children[index];
                index++;

                SimDescription child = Relationships.GetSim(childGene);

                if (child == null) continue;

                float fFraction;
                if (!fractions.TryGetValue(child, out fFraction))
                {
                    fFraction = defFraction;
                }

                // No inheritance for dead children
                if (child.DeathStyle != SimDescription.DeathType.None)
                {
                    foreach (Genealogy grandchildGene in childGene.Children)
                    {
                        SimDescription grandchild = Relationships.GetSim(grandchildGene);
                        if (grandchild == null) continue;

                        children.Add(grandchildGene);

                        if (!fractions.ContainsKey(grandchild))
                        {
                            fractions.Add(grandchild, fFraction / childGene.Children.Count);
                        }
                    }
                }
                else if (!inheritors.ContainsKey(child))
                {
                    inheritors.Add(child, fFraction);
                }
            }
        }
        public Dictionary<SimDescription, float> GetInheritors(SimDescription sim, Inheritors potentials, bool allowPets)
        {
            Dictionary<SimDescription, float> inheritors = new Dictionary<SimDescription, float>();

            if ((potentials & Inheritors.Children) == Inheritors.Children)
            {
                GetInheritors(sim.Genealogy.Children, 1f, inheritors);
            }

            if ((inheritors.Count == 0) && ((potentials & Inheritors.Relatives) == Inheritors.Relatives))
            {
                List<Genealogy> siblings = new List<Genealogy>();
                foreach (IGenealogy iSibling in sim.Genealogy.ISiblings)
                {
                    Genealogy sibling = iSibling as Genealogy;
                    if (sibling == null) continue;

                    siblings.Add(sibling);
                }

                GetInheritors(siblings, 0.25f, inheritors);
            }

            AddStat("Inheritors: Family", inheritors.Count);

            if ((inheritors.Count == 0) && ((potentials & Inheritors.Friends) == Inheritors.Friends))
            {
                foreach (Relationship relation in Relationship.GetRelationships(sim))
                {
                    if (relation.LTR.Liking < 25) continue;

                    SimDescription other = relation.GetOtherSimDescription(sim);
                    if (other == null) continue;

                    if (!allowPets)
                    {
                        if (!other.IsHuman) continue;
                    }

                    if (other.LotHome == null) continue;

                    if (!inheritors.ContainsKey(other))
                    {
                        inheritors.Add(other, relation.LTR.Liking / 100f);
                    }
                }

                AddStat("Inheritors: Friends", inheritors.Count);
            }

            return inheritors;
        }

        public static SimDescription.DeathType GetRandomDeathType()
        {
            List<SimDescription.DeathType> deathTypes = new List<SimDescription.DeathType>();

            foreach(SimDescription.DeathType type in Enum.GetValues(typeof(SimDescription.DeathType)))
            {
                if (type == SimDescription.DeathType.None) continue;

                deathTypes.Add(type);
            }

            return RandomUtil.GetRandomObjectFromList(deathTypes);
        }

        public bool CleansingKill(SimDescription sim, bool cleanse)
        {
            return CleansingKill(sim, SimDescription.DeathType.None, cleanse);
        }
        public bool CleansingKill(SimDescription sim, SimDescription.DeathType deathType, bool cleanse)
        {
            AddTry("CleansingKill");

            if (!GameStates.IsLiveState)
            {
                IncStat("CleansingKill Mode Save");
                return false;
            }

            string name = sim.FullName;

            if (cleanse)
            {
                Annihilation.CleanseGenealogy(sim);

                SetCleansed(sim);
            }

            bool bSuccess = false;

            Sim createdSim = sim.CreatedSim;

            if (createdSim != null)
            {
                try
                {
                    if (createdSim.InteractionQueue != null)
                    {
                        createdSim.InteractionQueue.CancelAllInteractions();

                        SpeedTrap.Sleep();
                    }

                    if ((deathType != SimDescription.DeathType.None) || (cleanse))
                    {
                        if (createdSim.Kill(deathType))
                        {
                            bSuccess = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.DebugException(createdSim, e);
                }
            }

            if (!bSuccess)
            {
                if (createdSim == PlumbBob.SelectedActor)
                {
                    IntroTutorial.ForceExitTutorial();
                }

                if (Sims == null)
                {
                    AddSuccess("CleansingKill: Destruct Fail");
                    return false;
                }

                if (!Annihilation.Perform(sim, cleanse))
                {
                    Main.RemoveSim(sim.SimDescriptionId);
                }
                else
                {
                    if (!cleanse)
                    {
                        sim.Dispose(true, false, true);
                    }

                    // Special case to stop HangWithCoworker bounces
                    Careers.RemoveSim(sim.SimDescriptionId);
                }

                bSuccess = true;
            }

            if (bSuccess)
            {
                AddSuccess("CleansingKill: Success");

                IncStat("Killed: " + name, Common.DebugLevel.High);

                return true;
            }
            else
            {
                return false;
            }
        }

        public class Updates : AlertLevelOption<ManagerDeath>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        protected class DebugOption : DebugLevelOption<ManagerDeath>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        protected class SpeedOption : SpeedBaseOption<ManagerDeath>
        {
            public SpeedOption()
                : base(1000, false)
            { }
        }

        protected class TicksPassedOption : TicksPassedBaseOption<ManagerDeath>
        {
            public TicksPassedOption()
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerDeath>
        {
            public DumpScoringOption()
            { }
        }

        public class DyingSimData : ElementalSimData
        {
            bool mDying = true;

            bool mTesting = false;

            bool mNotified = false;

            public DyingSimData()
            { }

            public bool Saved
            {
                get 
                {
                    if (!mTesting) return false;

                    return !mDying; 
                }
                set { mDying = false; }
            }

            public bool Notified
            {
                get { return mNotified; }
                set { mNotified = value; }
            }

            public bool Testing
            {
                get { return mTesting; }
                set 
                { 
                    mTesting = value;
                    mDying = true;
                }
            }

            public override string ToString()
            {
                Common.StringBuilder text = new Common.StringBuilder(base.ToString());
                text.AddXML("Testing", mTesting.ToString());
                text.AddXML("Dying", mDying.ToString());
                text.AddXML("Notified", mNotified.ToString());
                return text.ToString();
            }
        }
    }
}
