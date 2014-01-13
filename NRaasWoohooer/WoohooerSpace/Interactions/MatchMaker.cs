using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Selection;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Pools;
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

namespace NRaas.WoohooerSpace.Interactions
{
    public class MatchMaker : RabbitHole.RabbitHoleInteraction<Sim, DaySpa>, Common.IAddInteraction
    {
        static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<DaySpa>(Singleton);
        }

        protected static int OnSort(Pair<int, SimDescription> a, Pair<int, SimDescription> b)
        {
            try
            {
                return a.First.CompareTo(b.First);
            }
            catch (Exception e)
            {
                Common.Exception(a.Second.FullName + " " + b.Second.FullName, e);
            }

            return 0;
        }

        public override void ConfigureInteraction()
        {
            base.ConfigureInteraction();

            TimedStage stage = new TimedStage(GetInteractionName(), 30, false, true, true);
            Stages = new List<Stage>(new Stage[] { stage });
            ActiveStage = stage;
        }

        public override bool InRabbitHole()
        {
            try
            {
                if (!AcceptCancelDialog.Show(Common.Localize("Matchmaker:Prompt", Actor.IsFemale, new object[] { Woohooer.Settings.mMatchmakerCost })))
                {
                    return false;
                }

                List<SimDescription> actors = new List<SimDescription>();
                actors.Add(Actor.SimDescription);
                actors.AddRange(Households.Pets(Actor.Household));

                SimDescription actor = Actor.SimDescription;
                if (actors.Count > 0)
                {
                    actor = new ActorSelection(Common.Localize("Matchmaker:Choices"), actor, actors).SelectSingle();
                    if (actor == null) return false;
                }

                SimSelection selection = new SimSelection(Common.Localize("Matchmaker:MenuName"), actor, SimListing.GetResidents(false).Values, SimSelection.Type.Matchmaker, 0);
                if (selection.IsEmpty)
                {
                    Common.Notify(Common.Localize("Matchmaker:NoChoices", Actor.IsFemale));
                    return false;
                }

                SimDescription choice = selection.SelectSingle();
                if (choice == null)
                {
                    Common.Notify(Common.Localize("Matchmaker:NoSelect", Actor.IsFemale));
                    return false;
                }

                if (!CelebrityManager.TryModifyFundsWithCelebrityDiscount(Actor, Target, Woohooer.Settings.mMatchmakerCost, true))
                {
                    Common.Notify(Common.Localize("Matchmaker:CannotPay", Actor.IsFemale));
                    return false;
                }

                Relationship relation = Relationship.Get(actor, choice, true);
                if (relation != null)
                {
                    relation.MakeAcquaintances();
                }

                if (actor.IsHuman)
                {
                    Common.Notify(choice.CreatedSim, Common.Localize("Matchmaker:Success", Actor.IsFemale, choice.IsFemale, new object[] { choice }));
                }
                else
                {
                    SimDescription owner = null;
                    if (!choice.Household.IsSpecialHousehold)
                    {
                        owner = SimTypes.HeadOfFamily(choice.Household);
                    }

                    if (owner == null)
                    {
                        owner = choice;
                    }

                    relation = Relationship.Get(Actor.SimDescription, owner, true);
                    if (relation != null)
                    {
                        relation.MakeAcquaintances();
                    }

                    Common.Notify(choice.CreatedSim, Common.Localize("Matchmaker:SuccessPet", Actor.IsFemale, choice.IsFemale, new object[] { choice }));
                }
                return true;
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

        public class Definition : InteractionDefinition<Sim, DaySpa, MatchMaker>
        {
            public override string GetInteractionName(Sim actor, DaySpa target, InteractionObjectPair iop)
            {
                return Common.Localize("MatchMaker:MenuName", actor.IsFemale);
            }

            public override bool Test(Sim a, DaySpa target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!a.SimDescription.TeenOrAbove)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Too Young");
                    return false;
                }

                if (a.FamilyFunds < Woohooer.Settings.mMatchmakerCost)
                {
                    greyedOutTooltipCallback = delegate
                    {
                        return Common.Localize("Matchmaker:Failure", a.IsFemale, new object[] { Woohooer.Settings.mMatchmakerCost });
                    };
                    return false;
                }

                return true;
            }
        }

        public class ActorSelection : ProtoSimSelection<SimDescription>
        {
            public ActorSelection(string title, SimDescription me, ICollection<SimDescription> sims)
                : base(title, me, sims, true, false)
            { }
        }
    }
}
