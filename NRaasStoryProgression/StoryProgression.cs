using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace;
using NRaas.StoryProgressionSpace.Booters;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class StoryProgression : Common, Common.IWorldLoadFinished, Common.IWorldQuit
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static Main sMain = null;

        public static Main Main
        {
            get
            {
                return sMain;
            }
        }

        static StoryProgression()
        {
            sEnableLoadLog = true;

            StatValueCount.sFullLog = true;

            Bootstrap();

            BooterHelper.Add(new FirstNameListBooter());
            BooterHelper.Add(new LastNameListBooter());

            // Load in specific order
            BooterHelper.Add(new ScoringBooter("MethodFile", "NRaas.StoryProgressionModule", false));
            BooterHelper.Add(new ScoringBooter("MethodFile", "NRaas.BootStrap", false));

            BooterHelper.Add(new PersonalityLookup("NRaas.StoryProgressionModule"));
            BooterHelper.Add(new PersonalityLookup("NRaas.BootStrap"));
        }

        public void OnWorldLoadFinished()
        {
            DreamCatcher.OnWorldLoadFinishedDreams();

            if (!World.IsEditInGameFromWBMode())
            {
                sMain = new Main();

                Common.FunctionTask.Perform(sMain.InitialStartup);
            }
        }

        public void OnWorldQuit()
        {
            if (sMain != null)
            {
                sMain.Shutdown();
                sMain = null;
            }
        }

        public static void Reset()
        {
            if (sMain == null) return;

            sMain.Shutdown();
            sMain = null;

            sMain = new Main();
            sMain.Startup(null);

            SimpleMessageDialog.Show(StoryProgression.Localize("Reset:Title"), StoryProgression.Localize("Reset:Prompt"));

            throw new TotalResetException();
        }

        // Externalized to Consigner
        public static bool AdjustFunds(SimDescription sim, string key, int funds)
        {
            try
            {
                if (sMain == null) return false;

                if (funds == 0) return false;

                // It is expected that the calling routine will have applied the funds directly already
                if (funds > 0)
                {
                    sMain.Money.AdjustAccounting(sim.Household, "Income", -funds);
                }
                else
                {
                    sMain.Money.AdjustAccounting(sim.Household, "Expense", -funds);
                }

                sMain.Money.AdjustAccounting(sim.Household, key, funds);

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return true;
            }
        }

        // Externalized to Consigner
        public static bool MatchesAlertLevel(string managerName, SimDescription sim)
        {
            try
            {
                if (sMain == null) return true;

                Manager manager = Main.GetManager<Manager>(managerName);
                if (manager == null) return true;

                return manager.MatchesAlertLevel(sim);
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return true;
            }
        }

        // Externalized to Woohooer
        public static string AllowImpregnation(SimDescription sim, bool autonomous)
        {
            try
            {
                if (sMain == null) return null;

                Stats stats = new Stats(Main.Pregnancies);
                if (autonomous)
                {
                    int maximum = Main.GetValue<MaximumNumberOfChildrenOption, int>(sim);
                    if (maximum > 0)
                    {
                        if (Relationships.GetChildren(sim).Count >= maximum)
                        {
                            return "Maximum Children";
                        }
                    }

                    if (!Main.Pregnancies.TestCooldown(stats, sim))
                    {
                        return stats.ToString();
                    }
                }

                if (!Main.Pregnancies.AllowImpregnation(stats, sim, autonomous ? Manager.AllowCheck.None : Manager.AllowCheck.UserDirected))
                {
                    return stats.ToString();
                }

                return null;
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return null;
            }
        }

        // Externalized to Woohooer
        public static string AllowPregnancy(SimDescription actor, SimDescription target, bool autonomous)
        {
            try
            {
                if (sMain == null) return null;

                Stats stats = new Stats(Main.Pregnancies);

                if (!Main.Pregnancies.Allow(stats, actor, target, autonomous ? Manager.AllowCheck.None : Manager.AllowCheck.UserDirected))
                {
                    return stats.ToString();
                }

                return null;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return null;
            }
        }

        // Externalized to Woohooer
        public static string AllowBreakup(SimDescription sim, bool autonomous)
        {
            try
            {
                if (sMain == null) return null;

                Stats stats = new Stats(Main.Romances);
                if (!Main.Romances.AllowBreakup(stats, sim, autonomous ? Manager.AllowCheck.None : Manager.AllowCheck.UserDirected))
                {
                    return stats.ToString();
                }

                if (sim.Partner != null)
                {
                    if (!Main.Romances.AllowBreakup(stats, sim.Partner, autonomous ? Manager.AllowCheck.None : Manager.AllowCheck.UserDirected))
                    {
                        return stats.ToString();
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return null;
            }
        }

        // Externalized to Woohooer
        public static string AllowRomance(SimDescription sim, bool autonomous)
        {
            try
            {
                if (sMain == null) return null;

                Stats stats = new Stats(Main.Romances);
                if (!Main.Romances.Allow(stats, sim, autonomous ? Manager.AllowCheck.None : Manager.AllowCheck.UserDirected))
                {
                    return stats.ToString();
                }

                return null;
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return null;
            }
        }

        /* Not sure how to sync this, since scenarios are delayed
        // Externalized to Careers
        public static bool AboutToDie(SimDescription sim)
        {
            try
            {
                TODO
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return true;
            }
        }
        */        

        // Externalized to Careers
        public static bool AllowDeath(SimDescription sim)
        {
            try
            {
                if (sMain == null) return true;

                return (Main.GetValue<PushDeathChanceOption, int>(sim) > 0);
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return true;
            }
        }

        // Externalized to GoHere
        public static bool AllowPushToLot(SimDescription sim, Lot lot)
        {
            try
            {
                if (sMain == null) return true;

                if (!Main.Situations.AllowPush(Main.Situations, sim)) return false;

                if (lot != null)
                {
                    return Main.Lots.AllowSim(Main.Lots, sim.CreatedSim, lot);
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return true;
            }
        }

        // Externalized to Woohooer
        public static string CanFriendInteract(SimDescription actor, SimDescription target, bool autonomous)
        {
            try
            {
                if (sMain == null) return null;

                Stats stats = new Stats(Main.Friends);
                if (!Main.Friends.AllowFriend(stats, actor, target, autonomous ? Manager.AllowCheck.None : Manager.AllowCheck.UserDirected))
                {
                    return stats.ToString();
                }

                if (!Main.Friends.AllowEnemy(stats, actor, target, autonomous ? Manager.AllowCheck.None : Manager.AllowCheck.UserDirected))
                {
                    return stats.ToString();
                }

                return null;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return null;
            }
        }

        // Externalized to Woohooer
        public static string CanRomanticInteract(SimDescription actor, SimDescription target, bool autonomous)
        {
            try
            {
                if (sMain == null) return null;

                Stats stats = new Stats(Main.Flirts);
                if (!Main.Flirts.Allow(stats, actor, target, autonomous ? Manager.AllowCheck.None : Manager.AllowCheck.UserDirected))
                {
                    return stats.ToString();
                }

                return null;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return null;
            }
        }

        // Externalized to Woohooer
        public static string AllowSteady(SimDescription actor, SimDescription target, bool autonomous)
        {
            try
            {
                if (sMain == null) return null;

                Stats stats = new Stats(Main.Romances);
                if (!Main.Romances.AllowPartner(stats, actor, target, autonomous ? Manager.AllowCheck.None : Manager.AllowCheck.UserDirected))
                {
                    return stats.ToString();
                }

                return null;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return null;
            }
        }
   
        // Externalized to Woohooer
        public static string AllowMarriage(SimDescription actor, SimDescription target, bool autonomous)
        {
            try
            {
                if (sMain == null) return null;

                Stats stats = new Stats(Main.Romances);
                if (!Main.Romances.AllowMarriage(Main.Romances, actor, target, autonomous ? Manager.AllowCheck.None : Manager.AllowCheck.UserDirected))
                {
                    return stats.ToString();
                }

                return null;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return null;
            }
        }

        // Externalized to Mover
        public static bool HandleMarriageName(SimDescription actor, SimDescription target)
        {
            try
            {
                MarriageNameTask.Perform(actor, target, false);
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return true;
            }
        }

        public class Stats : IScoringGenerator
        {
            string mResult;

            Manager mManager;

            public Stats(Manager manager)
            {
                mManager = manager;
            }

            public Common.DebugLevel DebuggingLevel
            {
                get { return DebugLevel.Stats; }
            }

            public float AddStat(string stat, float val)
            {
                mResult = stat + ": " + val;
                return mManager.AddStat(stat, val);
            }
            public float AddStat(string stat, float val, Common.DebugLevel minLevel)
            {
                mResult = stat + ": " + val;
                return mManager.AddStat(stat, val, minLevel);
            }

            public int AddScoring(string stat, int score)
            {
                mResult = stat + ": " + score;
                return mManager.AddScoring(stat, score);
            }
            public int AddScoring(string stat, int score, Common.DebugLevel minLevel)
            {
                mResult = stat + ": " + score;
                return mManager.AddScoring(stat, score, minLevel);
            }

            public int AddStat(string stat, int val)
            {
                mResult = stat + ": " + val;
                return mManager.AddStat(stat, val);
            }
            public int AddStat(string stat, int val, Common.DebugLevel minLevel)
            {
                mResult = stat + ": " + val;
                return mManager.AddStat(stat, val, minLevel);
            }

            public void IncStat(string stat)
            {
                mManager.IncStat(stat);
                mResult = stat;
            }
            public void IncStat(string stat, Common.DebugLevel minLevel)
            {
                mManager.IncStat(stat, minLevel);
                mResult = stat;
            }

            public override string ToString()
            {
                return mResult;
            }

            public int AddScoring(string scoring, SimDescription sim)
            {
                mResult = scoring;
                return mManager.AddScoring(scoring, sim);
            }

            public int AddScoring(string scoring, SimDescription sim, DebugLevel minLevel)
            {
                mResult = scoring;
                return mManager.AddScoring(scoring, sim, minLevel);
            }

            public int AddScoring(string scoring, SimDescription sim, SimDescription other)
            {
                mResult = scoring;
                return mManager.AddScoring(scoring, sim, other);
            }

            public int AddScoring(string scoring, SimDescription sim, SimDescription other, DebugLevel minLevel)
            {
                mResult = scoring;
                return mManager.AddScoring(scoring, sim, other, minLevel);
            }

            public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim)
            {
                mResult = scoring;
                return mManager.AddScoring(scoring, option, type, sim);
            }

            public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, DebugLevel minLevel)
            {
                mResult = scoring;
                return mManager.AddScoring(scoring, option, type, sim, minLevel);
            }

            public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, SimDescription other)
            {
                mResult = scoring;
                return mManager.AddScoring(scoring, option, type, sim, other);
            }

            public int AddScoring(string scoring, int option, ScoringLookup.OptionType type, SimDescription sim, SimDescription other, DebugLevel minLevel)
            {
                mResult = scoring;
                return mManager.AddScoring(scoring, option, type, sim, other, minLevel);
            }
        }

        public class MarriageNameTask : FunctionTask
        {
            SimDescription mActor;
            SimDescription mTarget;

            bool mActorOnEither;

            protected MarriageNameTask(SimDescription actor, SimDescription target, bool actorOnEither)
            {
                mActor = actor;
                mTarget = target;
                mActorOnEither = actorOnEither;
            }

            public static void Perform(SimDescription actor, SimDescription target, bool actorOnEither)
            {
                new MarriageNameTask(actor, target, actorOnEither).AddToSimulator();
            }

            protected override void OnPerform()
            {
                if (sMain == null) return;

                Main.Romances.HandleMarriageName(mActor, mTarget, mActorOnEither);
            }
        }

        public class TotalResetException : Exception
        { }
    }
}
