using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class AttendShowEx : ShowVenue.AttendShow, Common.IPreLoad, Common.IAddInteraction
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<ShowVenue, ShowVenue.AttendShow.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<ShowVenue>(Singleton);
        }

        protected static int CalculateShowPriceEx(Sim a, ShowVenue target)
        {
            int showPrice = target.ShowPrice;
            if (a.HasTrait(TraitNames.ComplimentaryEntertainment))
            {
                showPrice = 0x0;
            }

            Music music = OmniCareer.Career<Music>(a.Occupation);

            if ((music != null) && (music.CurLevelBranchName == "Symphonic") && (music.CareerLevel >= Music.FreeTheatreShowLevel))
            {
                showPrice = 0x0;
            }

            foreach (Sim sim in a.Household.Sims)
            {
                if ((target.ActorsUsingMe.Contains(sim) && (sim.CurrentInteraction != null)) && (sim.CurrentInteraction.InteractionDefinition == ShowVenue.PerformConcert.Singleton))
                {
                    return 0x0;
                }
            }
            return showPrice;
        }

        protected new static string GetInteractionNameBase(Sim a, ShowVenue target)
        {
            Journalism journalism = OmniCareer.Career<Journalism>(a.Occupation);

            int num = CalculateShowPriceEx(a, target);
            switch (target.ShowType)
            {
                case ShowVenue.ShowTypes.kMovie:
                    if ((journalism == null) || !CanReviewRabbitHole(a, target))
                    {
                        if (num == 0)
                        {
                            return (LocalizeString(a.IsFemale, "AttendFreeMovie", new object[0]) + Localization.Ellipsis);
                        }
                        return (LocalizeString("AttendMovie", new object[] { num }) + Localization.Ellipsis);
                    }
                    return (LocalizeString(a.IsFemale, "ReviewMovie", new object[0]) + Localization.Ellipsis);

                case ShowVenue.ShowTypes.kPlay:
                    if ((journalism == null) || !CanReviewRabbitHole(a, target))
                    {
                        if (num == 0)
                        {
                            return LocalizeString(a.IsFemale, "AttendFreePlayRandom", new object[] { LocalizeString(ShowVenue.kPlayNames[ShowVenue.sRandomPlay], new object[0]) });
                        }
                        return LocalizeString(a.IsFemale, "AttendPlayRandom", new object[] { LocalizeString(ShowVenue.kPlayNames[ShowVenue.sRandomPlay], new object[0]), num });
                    }
                    return LocalizeString("ReviewPlayRandom", new object[] { LocalizeString(ShowVenue.kPlayNames[ShowVenue.sRandomPlay], new object[0]) });

                case ShowVenue.ShowTypes.kSymphony:
                    if ((journalism == null) || !CanReviewRabbitHole(a, target))
                    {
                        if (num == 0)
                        {
                            return LocalizeString(a.IsFemale, "AttendFreeSymphonyRandom", new object[] { LocalizeString(ShowVenue.kSymphonyNames[ShowVenue.sRandomSymphony], new object[0]) });
                        }
                        return LocalizeString(a.IsFemale, "AttendSymphonyRandom", new object[] { LocalizeString(ShowVenue.kSymphonyNames[ShowVenue.sRandomSymphony], new object[0]), num });
                    }
                    return LocalizeString(a.IsFemale, "ReviewSymphonyRandom", new object[] { LocalizeString(ShowVenue.kSymphonyNames[ShowVenue.sRandomSymphony], new object[0]) });

                case ShowVenue.ShowTypes.kConcert:
                case ShowVenue.ShowTypes.kPlayerConcert:
                    if ((journalism == null) || !CanReviewRabbitHole(a, target))
                    {
                        if (num == 0)
                        {
                            return LocalizeString(a.IsFemale, "AttendFreeConcert", new object[0]);
                        }
                        return LocalizeString(a.IsFemale, "AttendConcert", new object[] { num });
                    }
                    return LocalizeString(a.IsFemale, "ReviewConcert", new object[0]);

                case ShowVenue.ShowTypes.kEquestrianCenterRace:
                    if (num != 0)
                    {
                        return LocalizeString(a.IsFemale, "AttendEquestrianCompetition", new object[] { num });
                    }
                    return LocalizeString(a.IsFemale, "AttendFreeEquestrianCompetition", new object[0]);
            }
            return target.ShowTimeText();
        }

        public static bool CanReviewRabbitHole(Sim sim, RabbitHole rabbitHole)
        {
            Journalism journalism = OmniCareer.Career<Journalism>(sim.Occupation);

            if (!OmniCareer.HasMetric<MetricStoriesAndReviews>(sim.Occupation))
            {
                return false;
            }
            foreach (Journalism.ReviewedRabbitHole hole in journalism.RabbitHolesReviewed)
            {
                if (hole.EventLocation == rabbitHole)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool InRabbitHole()
        {
            try
            {
                SetMovieGenre();

                if (Actor.IsSelectable)
                {
                    Target.PlayMovieGenreMusic(mMovieGenre);
                }

                Journalism journalism = OmniCareer.Career<Journalism>(Actor.Occupation);

                bool isReviewing = false;
                if (journalism != null)
                {
                    isReviewing = journalism.CanReviewRabbitHole(Target);
                }

                if (!InRabbitholePreLoop(isReviewing))
                {
                    return false;
                }

                StartStages();
                BeginCommodityUpdates();
                bool succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                EndCommodityUpdates(succeeded);
                if (succeeded)
                {
                    Target.PostAttendShow(Actor);
                    if (isReviewing)
                    {
                        journalism.RabbitHolesReviewed.Add(new Journalism.ReviewedRabbitHole(Target, Target.ShowType));
                    }
                }

                ShowVenue.ApplyMovieBuff(mMovieGenre, Actor);
                return succeeded;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : ShowVenue.AttendShow.Definition
        {
            public Definition()
            { }
            public Definition(string parent, string child, ShowVenue.MovieGenres genre)
                : base(parent, child, genre)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new AttendShowEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim a, ShowVenue target, InteractionObjectPair interaction)
            {
                if (Name != string.Empty)
                {
                    return Name;
                }
                return AttendShowEx.GetInteractionNameBase(a, target);
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, ShowVenue target, List<InteractionObjectPair> results)
            {
                ShowVenue.SetUpRandomNames();
                if (target.ShowType == ShowVenue.ShowTypes.kMovie)
                {
                    string interactionNameBase = AttendShowEx.GetInteractionNameBase(actor, target);
                    results.Add(new InteractionObjectPair(new Definition(interactionNameBase, ShowVenue.AttendShow.LocalizeString(actor.IsFemale, ShowVenue.kActionMovieNames[ShowVenue.sRandomActionMovie], new object[0]), ShowVenue.MovieGenres.Action), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition(interactionNameBase, ShowVenue.AttendShow.LocalizeString(actor.IsFemale, ShowVenue.kThrillerMovieNames[ShowVenue.sRandomThrillerMovie], new object[0]), ShowVenue.MovieGenres.Thriller), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition(interactionNameBase, ShowVenue.AttendShow.LocalizeString(actor.IsFemale, ShowVenue.kSciencefictionMovieNames[ShowVenue.sRandomSciencefictionMovie], new object[0]), ShowVenue.MovieGenres.Sciencefiction), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition(interactionNameBase, ShowVenue.AttendShow.LocalizeString(actor.IsFemale, ShowVenue.kBromanceMovieNames[ShowVenue.sRandomBromanceMovie], new object[0]), ShowVenue.MovieGenres.Bromance), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition(interactionNameBase, ShowVenue.AttendShow.LocalizeString(actor.IsFemale, ShowVenue.kChickflickMovieNames[ShowVenue.sRandomChickflickMovie], new object[0]), ShowVenue.MovieGenres.Chickflick), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition(interactionNameBase, ShowVenue.AttendShow.LocalizeString(actor.IsFemale, ShowVenue.kComedyMovieNames[ShowVenue.sRandomComedyMovie], new object[0]), ShowVenue.MovieGenres.Comedy), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition(interactionNameBase, ShowVenue.AttendShow.LocalizeString(actor.IsFemale, ShowVenue.kForeignMovieNames[ShowVenue.sRandomForeignMovie], new object[0]), ShowVenue.MovieGenres.Foreign), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition(interactionNameBase, ShowVenue.AttendShow.LocalizeString(actor.IsFemale, ShowVenue.kIndieMovieNames[ShowVenue.sRandomIndieMovie], new object[0]), ShowVenue.MovieGenres.Indie), iop.Target));
                }
                else
                {
                    base.AddInteractions(iop, actor, target, results);
                }
            }

            public override bool Test(Sim a, ShowVenue target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!(a.Occupation is OmniCareer))
                {
                    return false;
                }

                if ((!isAutonomous || !a.IsSelectable) || (AutonomyRestrictions.GetLevel() >= AutonomyLevel.Max))
                {
                    if (a.IsInGroupingSituation())
                    {
                        return false;
                    }

                    Journalism journalism = OmniCareer.Career<Journalism>(a.Occupation);

                    int num = CalculateShowPriceEx(a, target);
                    if (((target.ShowType != ShowVenue.ShowTypes.kNoShow) && target.DoorsOpen) && ((a.FamilyFunds >= num) && ((journalism == null) || !journalism.BadReviewWrittenOnRabbitHole(target))))
                    {
                        return true;
                    }

                    greyedOutTooltipCallback = delegate
                    {
                        if ((journalism != null) && journalism.BadReviewWrittenOnRabbitHole(target))
                        {
                            return LocalizeString("NotWelcomeFromBadReview", new object[0x0]);
                        }

                        if (target.ShowType != ShowVenue.ShowTypes.kNoShow)
                        {
                            if (target.DoorsOpen)
                            {
                                return LocalizeString("InsufficientFunds", new object[0x0]);
                            }
                            if (target.ShowInProgress)
                            {
                                return LocalizeString("DoorsClosed", new object[0x0]);
                            }
                        }
                        return ShowVenue.AttendShow.LocalizeString(target.NoShowTooltip, new object[0]);
                    };
                }
                return false;
            }
        }
    }
}
