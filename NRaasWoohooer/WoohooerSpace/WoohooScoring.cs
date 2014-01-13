using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using NRaas.WoohooerSpace.Skills;
using NRaas.WoohooerSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.TombObjects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace
{
    public sealed class WoohooScoring : Common.IStatGenerator
    {
        static StatBin sStats = new StatBin("Woohooer");

        public enum ScoreTestResult
        {
            Success,
            Jealousy,
            InterestInRomance,
            CustomScoring,
            Attraction,
            LikingGate
        }

        static WoohooScoring()
        {
            ScoringLookup.Stats = new WoohooScoring();
        }

        public static void Dump(bool empty)
        {
            sStats.Dump(null, empty);
        }

        public static int Count
        {
            get
            {
                return sStats.Count;
            }
        }

        public Common.DebugLevel DebuggingLevel
        {
            get 
            {
                if (Common.kDebugging)
                {
                    return Common.DebugLevel.Stats;
                }
                else
                {
                    return Common.DebugLevel.Quiet;
                }
            }
        }

        public int AddScoring(string scoring, SimDescription sim)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, sim));
        }
        public int AddScoring(string scoring, SimDescription sim, Common.DebugLevel minLevel)
        {
            return AddScoring(scoring, ScoringLookup.GetScore(scoring, sim));
        }

        public int AddScoring(string stat, int score)
        {
            return (int)sStats.AddScoring(stat, score);
        }
        public int AddScoring(string stat, int score, Common.DebugLevel minLevel)
        {
            return (int)sStats.AddScoring(stat, score);
        }

        public int AddStat(string stat, int val)
        {
            return (int)sStats.AddStat(stat, val);
        }
        public int AddStat(string stat, int val, Common.DebugLevel minLevel)
        {
            return (int)sStats.AddStat(stat, val);
        }

        public float AddStat(string stat, float val)
        {
            return sStats.AddStat(stat, val);
        }
        public float AddStat(string stat, float val, Common.DebugLevel minLevel)
        {
            return sStats.AddStat(stat, val);
        }

        public void IncStat(string stat)
        {
            sStats.IncStat(stat);
        }
        public void IncStat(string stat, Common.DebugLevel minLevel)
        {
            sStats.IncStat(stat);
        }

        public static ScoreTestResult ScoreActor(string logName, Sim actor, Sim target, bool isAutonomous, string scoring, bool woohoo)
        {
            if ((!isAutonomous) || (!Woohooer.Settings.UsingTraitScoring))
            {
                return ScoreTestResult.Success;
            }

            ScoringLookup.IncStat("OnScoreActor " + logName + " Try");

            if (!IsSafeFromJealousy(actor, target, woohoo))
            {
                ScoringLookup.IncStat("OnScoreActor " + logName + " Jealousy");
                return ScoreTestResult.Jealousy;
            }

            if (!TestScoringNormal(actor, target, "Attraction", false))
            {
                ScoringLookup.IncStat("OnScoreActor " + logName + " Attraction");
                return ScoreTestResult.Attraction;
            }

            bool success = false;
            if ((actor.Partner == target.SimDescription) || (target.Partner == actor.SimDescription))
            {
                success = TestScoringAbsolute(actor, target, "InterestInRomance", true);
            }
            else
            {
                success = TestScoringNormal(actor, target, "InterestInRomance", true);
            }

            if (!success)
            {
                ScoringLookup.IncStat("OnScoreActor " + logName + " InterestInRomance");
                return ScoreTestResult.InterestInRomance;
            }

            if (scoring != "InterestInRomance")
            {
                /*
                Relationship relation = Relationship.Get(actor, target, false);
                if ((relation != null) && ((relation.CurrentSTC == ShortTermContextTypes.Hot) || (relation.CurrentSTC == AmorousCommodity.sHot2)))
                {
                    success = TestScoringAbsolute(actor, target, scoring, true);
                }
                else
                */
                {
                    success = TestScoringNormal(actor, target, scoring, true);
                }

                if (!success)
                {
                    ScoringLookup.IncStat("OnScoreActor " + logName + " " + scoring);
                    return ScoreTestResult.CustomScoring;
                }
            }

            ScoringLookup.IncStat("OnScoreActor " + logName + " Success");
            return ScoreTestResult.Success;
        }

        public static ScoreTestResult ScoreTarget(string logName, Sim actor, Sim target, bool isAutonomous, string scoring, bool woohoo)
        {
            return ScoreTarget(logName, actor, target, isAutonomous, scoring, woohoo, Woohooer.Settings.mLikingGateForUserDirected);
        }
        public static ScoreTestResult ScoreTarget(string logName, Sim actor, Sim target, bool isAutonomous, string scoring, bool woohoo, bool checkLiking)
        {
            if ((isAutonomous) || (checkLiking))
            {
                if (!CommonSocials.SatisfiesLikingGate(actor, target, woohoo))
                {
                    return ScoreTestResult.LikingGate;
                }
            }

            if (!Woohooer.Settings.UsingTraitScoring)
            {
                return ScoreTestResult.Success;
            }

            if (!isAutonomous)
            {
                if (!Woohooer.Settings.TraitScoringForUserDirected)
                {
                    return ScoreTestResult.Success;
                }
            }

            ScoringLookup.IncStat("OnScoreTarget " + logName + " Try");

            if (!IsSafeFromJealousy(actor, target, woohoo))
            {
                ScoringLookup.IncStat("OnScoreTarget " + logName + " Jealousy");
                return ScoreTestResult.Jealousy;
            }

            bool success = false;
            if ((actor.Partner == target.SimDescription) || (target.Partner == actor.SimDescription))
            {
                success = TestScoringAbsolute(actor, target, "InterestInRomance", true);
            }
            else
            {
                success = TestScoringNormal(actor, target, "InterestInRomance", true);
            }

            if (!success)
            {
                ScoringLookup.IncStat("OnScoreTarget " + logName + " InterestInRomance");
                return ScoreTestResult.InterestInRomance;
            }

            if (scoring != "InterestInRomance")
            {
                /*
                Relationship relation = Relationship.Get(actor, target, false);
                if ((relation != null) && ((relation.CurrentSTC == ShortTermContextTypes.Hot) || (relation.CurrentSTC == AmorousCommodity.sHot2)))
                {
                    success = TestScoringAbsolute(actor, target, scoring, true);
                }
                else
                */
                {
                    success = TestScoringNormal(actor, target, scoring, true);
                }

                if (!success)
                {
                    ScoringLookup.IncStat("OnScoreTarget " + logName + " " + scoring);
                    return ScoreTestResult.CustomScoring;
                }
            }

            ScoringLookup.IncStat("OnScoreTarget " + logName + " Success");
            return ScoreTestResult.Success;
        }

        public static bool ScoreFriendly(Sim actor, Sim target, bool isAutonomous, string scoring)
        {
            ScoringLookup.IncStat("OnScoreFriendly Try");

            if (!isAutonomous)
            {
                if (!Woohooer.Settings.TraitScoringForUserDirected)
                {
                    ScoringLookup.IncStat("OnScoreFriendly Not User TraitScored");
                    return true;
                }
            }

            if (!Woohooer.Settings.UsingTraitScoring)
            {
                ScoringLookup.IncStat("OnScoreFriendly Not TraitScored");
                return true;
            }

            // Both sims interested and attracted to each other

            if (!TestScoringAbsolute(target, actor, scoring, true))
            {
                ScoringLookup.IncStat("OnScoreFriendly " + scoring);
                return false;
            }

            if (!TestScoringAbsolute(target, actor, "Attraction", false))
            {
                ScoringLookup.IncStat("OnScoreFriendly Attraction");
                return false;
            }

            if (!IsSafeFromJealousy(target, actor, true))
            {
                ScoringLookup.IncStat("OnScoreFriendly Jealousy");
                return false;
            }

            ScoringLookup.IncStat("OnScoreFriendly Success");

            return true;
        }

        public static bool ReactsToJealousy(Sim actor)
        {
            if (actor.SimDescription.ToddlerOrBelow) return false;

            if (KamaSimtra.IsWhoring(actor)) return false;

            if (actor.HasTrait(TraitNames.InappropriateButInAGoodWay)) return false;

            if (!Woohooer.Settings.UsingTraitScoring) return true;

            if (Woohooer.Settings.mReactToJealousyBaseChanceScoring <= 0) return false;

            return TestScoringAbsolute(actor, null, "ReactsToJealousy", false);
        }

        public static bool IsSafeFromJealousy(Sim actor, Sim target, bool woohoo)
        {
            if (actor.TraitManager == null) return false;

            if (actor.HasTrait(TraitNames.NoJealousy)) return true;

            if (!Woohooer.Settings.UsingTraitScoring) return true;

            if (actor.LotCurrent == null) return true;

            bool found = false;
            foreach (Sim sim in actor.LotCurrent.GetAllActors ())
            {
                if (sim == null) continue;

                if (sim == target) continue;

                if (!woohoo)
                {
                    if (sim.RoomId != actor.RoomId) continue;
                }

                Relationship relation = Relationship.Get(sim, actor, false);
                if (relation == null) continue;

                if (relation.AreRomantic())
                {
                    found = true;
                    break;
                }
            }

            if (!found) return true;

            return !TestScoringNormal(actor, target, "Sneakiness", false);
        }

        public static bool ScoreBoring(Sim actor, Sim target, bool isAutonomous, string scoring)
        {
            if (actor.Partner != target.SimDescription) return false;

            return ScoreCreepy(actor, target, isAutonomous, scoring, "OnScoreBoring");
        }

        public static bool ScoreAwkward(Sim actor, Sim target, bool isAutonomous, string scoring)
        {
            ScoringLookup.IncStat("OnScoreAwkward Try");

            if (!isAutonomous)
            {
                if (!Woohooer.Settings.TraitScoringForUserDirected)
                {
                    ScoringLookup.IncStat("OnScoreAwkward Not User TraitScored");
                    return false;
                }
            }

            if (!Woohooer.Settings.UsingTraitScoring)
            {
                ScoringLookup.IncStat("OnScoreAwkward Not TraitScored");
                return false;
            }

            // Target is not interested

            if (!TestScoringAbsolute(target, actor, scoring, true))
            {
                ScoringLookup.IncStat("OnScoreAwkward " + scoring);
                return true;
            }

            /*
            if (!TestScoringAbsolute(target, actor, "Attraction", false))
            {
                ScoringLookup.IncStat("OnScoreAwkward Attraction");
                return true;
            }
            */

            ScoringLookup.IncStat("OnScoreAwkward Fail");
            return false;
        }

        public static bool ScoreCreepy(Sim actor, Sim target, bool isAutonomous, string scoring)
        {
            return ScoreCreepy(actor, target, isAutonomous, scoring, "OnScoreCreepy");
        }
        public static bool ScoreCreepy(Sim actor, Sim target, bool isAutonomous, string scoring, string title)
        {
            ScoringLookup.IncStat(title + " Try");

            if (!isAutonomous)
            {
                if (!Woohooer.Settings.TraitScoringForUserDirected)
                {
                    ScoringLookup.IncStat(title + " Not User TraitScored");
                    return false;
                }
            }

            if (!Woohooer.Settings.UsingTraitScoring)
            {
                ScoringLookup.IncStat(title + " Not TraitScored");
                return false;
            }

            // Target is not attracted to Actor

            /*
            if (!TestScoringAbsolute(target, actor, scoring, true)) 
            {
                ScoringLookup.IncStat(title + " " + scoring);
                return true;
            }
            */

            if (!TestScoringAbsolute(target, actor, "Attraction", false))
            {
                ScoringLookup.IncStat(title + " Attraction");
                return true;
            }

            ScoringLookup.IncStat(title + " Fail");
            return false;
        }

        public static bool ScoreInsulting(Sim actor, Sim target, bool isAutonomous, string scoring)
        {
            ScoringLookup.IncStat("OnScoreInsulting Try");

            if (!isAutonomous)
            {
                if (!Woohooer.Settings.TraitScoringForUserDirected)
                {
                    ScoringLookup.IncStat("OnScoreInsulting Not User TraitScored");
                    return false;
                }
            }

            if (!Woohooer.Settings.UsingTraitScoring)
            {
                ScoringLookup.IncStat("OnScoreInsulting Not TraitScored");
                return false;
            }

            // Target is not interested and is not attracted to sim

            if (!TestScoringAbsolute(target, actor, scoring, true))
            {
                if (!TestScoringAbsolute(target, actor, "Attraction", false))
                {
                    ScoringLookup.IncStat("OnScoreInsulting Attraction");
                    return true;
                }
            }

            ScoringLookup.IncStat("OnScoreInsulting Fail");
            return false;
        }

        public static bool TestScoringNormal(Sim actor, Sim target, string scoringName, bool checkAffair)
        {
            int score = GetScoring(actor, target, scoringName, checkAffair, false);

            ScoringLookup.AddStat("Test Normal " + scoringName, score);

            return (score > 0);
        }
        public static bool TestScoringAbsolute(Sim actor, Sim target, string scoringName, bool checkAffair)
        {
            int score = GetScoring(actor, target, scoringName, checkAffair, true);

            ScoringLookup.AddStat("Test Absolute " + scoringName, score);

            return (score > 0);
        }
        public static int GetScoring(Sim actor, Sim target, string scoringName, bool checkAffair, bool absolute)
        {
            if (actor == target) return 0;

            int score = 0;
            if (scoringName == "Attraction")
            {
                score = (int)RelationshipEx.GetAttractionScore(actor.SimDescription, target.SimDescription, false);
            }
            else
            {
                IListedScoringMethod scoring = null;

                if (checkAffair)
                {
                    if ((actor.Partner != null) && ((target == null) || (actor.Partner != target.SimDescription)))
                    {
                        scoring = ScoringLookup.GetScoring(scoringName + "Affair");
                    }
                    else
                    {
                        scoring = ScoringLookup.GetScoring(scoringName + "Single");
                    }
                }
                else
                {
                    scoring = ScoringLookup.GetScoring(scoringName);
                }

                if (scoring == null) return 1;

                if (scoring is DualSimListedScoringMethod)
                {
                    score = scoring.IScore(new DualSimScoringParameters(target.SimDescription, actor.SimDescription, absolute));
                }
                else
                {
                    score = scoring.IScore(new SimScoringParameters(actor.SimDescription, absolute));
                }
            }

            return score;
        }
    }
}
