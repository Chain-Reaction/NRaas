using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Skills;
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
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class WoohooBuffs : Common.IWorldLoadFinished
    {
        static BuffNames sLikeFriendWithBenefits = unchecked((BuffNames)ResourceUtils.HashString64("NRaasLikeFriendWithBenefits"));
        static BuffNames sDislikeFriendWithBenefits = unchecked((BuffNames)ResourceUtils.HashString64("NRaasDislikeFriendWithBenefits"));

        static BuffNames sLikeOneNightStand = unchecked((BuffNames)ResourceUtils.HashString64("NRaasLikeOneNightStand"));
        static BuffNames sDislikeOneNightStand = unchecked((BuffNames)ResourceUtils.HashString64("NRaasDislikeOneNightStand"));

        static BuffNames sLikePartner = unchecked((BuffNames)ResourceUtils.HashString64("NRaasLikePartnerWoohoo"));
        static BuffNames sWorryAboutWoohoo = unchecked((BuffNames)ResourceUtils.HashString64("NRaasWorryAboutWoohoo"));

        static BuffNames sLikeRisky = unchecked((BuffNames)ResourceUtils.HashString64("NRaasLikeRiskyWoohoo"));
        static BuffNames sDislikeRisky = unchecked((BuffNames)ResourceUtils.HashString64("NRaasDislikeRiskyWoohoo"));

        static BuffNames sLikeAdultery = unchecked((BuffNames)ResourceUtils.HashString64("NRaasLikeAdultery"));
        static BuffNames sDislikeAdultery = unchecked((BuffNames)ResourceUtils.HashString64("NRaasDislikeAdultery"));

        static BuffNames sWitnessed = unchecked((BuffNames)ResourceUtils.HashString64("NRaasWitnessed"));
        static BuffNames sVoyeur = unchecked((BuffNames)ResourceUtils.HashString64("NRaasVoyeur"));

        public static Origin sWoohooOrigin = unchecked((Origin)ResourceUtils.HashString64("NRaasWoohoo"));
        public static Origin sBookOrigin = unchecked((Origin)ResourceUtils.HashString64("NRaasFromNovel"));

        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kReadBook, OnAddBookBuff);
        }

        private static void OnAddBookBuff(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null) return;

            BookWritten target = e.TargetObject as BookWritten;
            if (target == null) return;

            BookWrittenData data = target.Data as BookWrittenData;
            if (data == null) return;

            if (actor.SimDescription.ChildOrBelow) return;

            if (actor.SimDescription.Teen)
            {
                if (!Woohooer.Settings.AllowTeen(true)) return;
            }

            if (actor.BuffManager == null) return;

            switch (data.Genre)
            {
                case BookData.BookGenres.Romance:
                //case BookData.BookGenres.Poetry:
                case BookData.BookGenres.Trashy:
                    actor.BuffManager.AddElement(BuffNames.InTheMood, sBookOrigin);
                    break;
            }
        }

        public static void Apply(Sim actor, Sim target, bool risky)
        {
            if (!Woohooer.Settings.mApplyBuffs) return;

            bool useTraitScoring = Woohooer.Settings.UsingTraitScoring;

            if ((actor == null) || (target == null)) return;

            Relationship relation = Relationship.Get(actor, target, false);
            if (relation == null) return;

            bool witnessed = false;

            if (useTraitScoring)
            {
                string reason;

                foreach (Sim sim in actor.LotCurrent.GetAllActors())
                {
                    if (!sim.IsHuman) continue;

                    if ((sim == actor) || (sim == target)) continue;

                    if (sim.RoomId != actor.RoomId) continue;

                    if ((!CommonSocials.CanGetRomantic(sim.SimDescription, actor.SimDescription, out reason)) && (!CommonSocials.CanGetRomantic(sim.SimDescription, target.SimDescription, out reason))) continue;

                    if (ScoringLookup.GetScore("LikeWatching", sim.SimDescription) > 0)
                    {
                        sim.BuffManager.AddElement(sVoyeur, sWoohooOrigin);

                        witnessed = true;
                    }
                    else
                    {
                        sim.BuffManager.AddElement(BuffNames.Embarrassed, sWoohooOrigin);
                    }
                }
            }

            Sim[] sims = new Sim[] { actor, target };

            foreach (Sim sim in sims)
            {
                if (!sim.IsHuman) continue;

                if (useTraitScoring)
                {
                    if (KamaSimtra.GetSkillLevel(sim.SimDescription) < 4)
                    {
                        if (ScoringLookup.GetScore("WorryAboutWoohoo", sim.SimDescription) > 0)
                        {
                            sim.BuffManager.AddElement(sWorryAboutWoohoo, sWoohooOrigin);
                        }
                    }
                }

                if (witnessed)
                {
                    sim.BuffManager.AddElement(sWitnessed, sWoohooOrigin);
                }

                if (useTraitScoring)
                {
                    if ((risky) && (!Woohooer.Settings.ReplaceWithRisky))
                    {
                        int score = ScoringLookup.GetScore("LikeRisky", sim.SimDescription);
                        if (score > 0)
                        {
                            sim.BuffManager.RemoveElement(sDislikeRisky);
                            sim.BuffManager.AddElement(sLikeRisky, sWoohooOrigin);
                        }
                        else if (score < 0)
                        {
                            sim.BuffManager.RemoveElement(sLikeRisky);
                            sim.BuffManager.AddElement(sDislikeRisky, sWoohooOrigin);
                        }
                    }
                }

                if (actor.Partner == target.SimDescription)
                {
                    sim.BuffManager.AddElement(sLikePartner, sWoohooOrigin);
                }
                else if (sim.Partner != null)
                {
                    if (useTraitScoring)
                    {
                        int score = ScoringLookup.GetScore("Monogamous", sim.SimDescription);
                        if (score > 0)
                        {
                            sim.BuffManager.RemoveElement(sLikeAdultery);
                            sim.BuffManager.AddElement(sDislikeAdultery, sWoohooOrigin);
                        }
                        else if (score < 0)
                        {
                            sim.BuffManager.RemoveElement(sDislikeAdultery);
                            sim.BuffManager.AddElement(sLikeAdultery, sWoohooOrigin);
                        }
                    }
                }

                if ((useTraitScoring) && (!relation.AreRomantic()))
                {
                    if (relation.LTR.Liking > 75)
                    {
                        int score = ScoringLookup.GetScore("LikeFriendWithBenefits", sim.SimDescription);
                        if (score > 0)
                        {
                            sim.BuffManager.RemoveElement(sDislikeFriendWithBenefits);
                            sim.BuffManager.AddElement(sLikeFriendWithBenefits, sWoohooOrigin);
                        }
                        else if (score < 0)
                        {
                            sim.BuffManager.RemoveElement(sLikeFriendWithBenefits);
                            sim.BuffManager.AddElement(sDislikeFriendWithBenefits, sWoohooOrigin);
                        }
                    }
                    else
                    {
                        int score = ScoringLookup.GetScore("LikeOneNightStand", sim.SimDescription);
                        if (score > 0)
                        {
                            sim.BuffManager.RemoveElement(sDislikeOneNightStand);
                            sim.BuffManager.AddElement(sLikeOneNightStand, sWoohooOrigin);
                        }
                        else if (score < 0)
                        {
                            sim.BuffManager.RemoveElement(sLikeOneNightStand);
                            sim.BuffManager.AddElement(sDislikeOneNightStand, sWoohooOrigin);
                        }
                    }
                }
            }
        }
    }
}
