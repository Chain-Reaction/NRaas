using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
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
    public class ChooseLifetimeWishScenario : AgeUpBaseScenario
    {
        public ChooseLifetimeWishScenario()
        { }
        public ChooseLifetimeWishScenario(SimDescription sim)
            : base (sim)
        { }
        protected ChooseLifetimeWishScenario(ChooseLifetimeWishScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ChooseLifetimeWish";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return sim.IsHuman;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Not Resident");
                return false;
            }
            else if (!sim.TeenOrAbove)
            {
                IncStat("Too Young");
                return false;
            }
            else if ((sim.Teen) && (!GetValue<AllowTeenOption, bool>()))
            {
                IncStat("Teen Denied");
                return false;
            }
            else
            {
                DreamNodeInstance instance = null;
                DreamsAndPromisesManager.sMajorWishes.TryGetValue(sim.LifetimeWish, out instance);
                if (instance != null)
                {
                    IncStat("Has LTW");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        public bool Run(Manager manager)
        {
            if (Sim == null) return false;

            Manager = manager;

            return PrivatePerform();
        }

        public uint ChooseRandomLifetimeWish(DreamsAndPromisesManager ths)
        {
            List<IInitialMajorWish> topMajorDreamMatches = DreamsAndPromisesManager.GetTopMajorDreamMatches(ths.mActor.SimDescription);

            for (int i = topMajorDreamMatches.Count - 1; i >= 0; i--)
            {
                if (HasValue<DisallowLTWOption, LifetimeWant>((LifetimeWant)topMajorDreamMatches[i].PrimitiveId))
                {
                    topMajorDreamMatches.RemoveAt(i);
                }
            }

            if (topMajorDreamMatches.Count > 0x0)
            {
                return RandomUtil.GetRandomObjectFromList<IInitialMajorWish>(topMajorDreamMatches).PrimitiveId;
            }

            return 0x0;
        }

        protected bool PrivatePerform()
        {
            if (!Sims.Instantiate(Sim, null, false)) return false;

            if (Sim.CreatedSim == null) return false;

            bool DnP = (Sim.CreatedSim.DreamsAndPromisesManager != null);

            bool success = false;

            if (!DnP)
            {
                bool hadWish = Sim.HasLifetimeWish;

                try
                {
                    DreamsAndPromisesManager.CreateAndIntitForSim(Sim.CreatedSim);
                }
                catch (Exception e)
                {
                    Common.DebugException(Sim, e);
                }

                if (!hadWish)
                {
                    Sim.LifetimeWish = ChooseRandomLifetimeWish(Sim.CreatedSim.DreamsAndPromisesManager);
                    if (Sim.HasLifetimeWish)
                    {
                        Sim.CreatedSim.DreamsAndPromisesManager.TryAddLifetimeWish();
                    }
                    success = true;
                }
            }

            if ((Sims.MatchesAlertLevel(Sim)) && (GetValue<PromptOption, bool>()))
            {
                if ((AcceptCancelDialog.Show(ManagerSim.GetPersonalInfo(Sim, Common.Localize("ChooseLifetimeWish:Prompt", Sim.IsFemale)))) && (LifetimeWants.SetLifetimeWant(Sim)))
                {
                    success = true;
                }
            }
            
            if (!success)
            {
                Sim.LifetimeWish = ChooseRandomLifetimeWish(Sim.CreatedSim.DreamsAndPromisesManager);
                if (Sim.HasLifetimeWish)
                {
                    Sim.CreatedSim.DreamsAndPromisesManager.TryAddLifetimeWish();
                }
            }

            IncStat("Set");

            if (!DnP)
            {
                Sim.CreatedSim.NullDnPManager();
            }

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            return PrivatePerform ();
        }

        public override Scenario Clone()
        {
            return new ChooseLifetimeWishScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSim, ChooseLifetimeWishScenario>, ManagerSim.ITraitsLTWOption
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerSim main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    ImmigrantMoveInScenario.OnAfterMoveIn += OnRun;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "SetInactiveLTW";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                ImmigrantMoveInScenario s = scenario as ImmigrantMoveInScenario;
                if (s == null) return;

                foreach (SimDescription sim in s.Movers)
                {
                    scenario.Add(frame, new ChooseLifetimeWishScenario(sim), ScenarioResult.Start);
                }
            }
        }

        public class AllowTeenOption : BooleanManagerOptionItem<ManagerSim>, ManagerSim.ITraitsLTWOption
        {
            public AllowTeenOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowTeenLTW";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class PromptOption : BooleanManagerOptionItem<ManagerSim>, ManagerSim.ITraitsLTWOption
        {
            public PromptOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "PromptForLifetimeWant";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class DisallowLTWOption : MultiEnumManagerOptionItem<ManagerSim, LifetimeWant>, ManagerSim.ITraitsLTWOption
        {
            public DisallowLTWOption()
                : base(new List<LifetimeWant>())
            { }

            public override string GetTitlePrefix()
            {
                return "DisallowLTW";
            }

            public override string ValuePrefix
            {
                get { return "Disallowed"; }
            }

            protected override bool PersistCreate(ref LifetimeWant defValue, string value)
            {
                if (base.PersistCreate(ref defValue, value)) return true;

                long result;
                if (!long.TryParse(value, out result)) return false;

                defValue = (LifetimeWant)result;
                return true;
            }

            protected override List<IGenericValueOption<LifetimeWant>> GetAllOptions()
            {
                List<IGenericValueOption<LifetimeWant>> results = new List<IGenericValueOption<LifetimeWant>>();

                if (Sims3.Gameplay.Actors.Sim.ActiveActor != null)
                {
                    List<IInitialMajorWish> allMajorDreamMatches = DreamsAndPromisesManager.GetAllMajorDreamMatches(Sims3.Gameplay.Actors.Sim.ActiveActor.SimDescription);

                    foreach (IInitialMajorWish ltw in allMajorDreamMatches)
                    {
                        results.Add(new ListItem(this, ltw));
                    }
                }

                return results;
            }

            public class ListItem : BaseListItem<DisallowLTWOption>
            {
                IInitialMajorWish mWant = null;

                public ListItem(DisallowLTWOption option, IInitialMajorWish ltw)
                    : base(option, (LifetimeWant)ltw.PrimitiveId)
                {
                    mWant = ltw;

                    SetThumbnail(ltw.PrimaryIconKey);
                }

                public override string Name
                {
                    get
                    {
                        return mWant.GetMajorWishName(Sims3.Gameplay.Actors.Sim.ActiveActor.SimDescription);
                    }
                }
            }
        }
    }
}
