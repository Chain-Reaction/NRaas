using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
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
    public class VisitRabbitHoleEx : RabbitHole.VisitRabbitHoleBase<VisitRabbitHoleEx>, ITakeSimToWorkLocation, Common.IPreLoad, Common.IAddInteraction
    {
        private bool mbTakingSimToWorkLocation;

        public bool mImpregnate;
        public CommonWoohoo.WoohooStyle mStyle;

        static Dictionary<RabbitHoleType, InteractionParameters> sParameters = null;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddCustom(new CustomInjector());
        }

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, RabbitHole.VisitRabbitHole.Definition, Definition>(false);
        }

        public static Dictionary<RabbitHoleType, InteractionParameters> Parameters
        {
            get
            {
                if (sParameters == null)
                {
                    sParameters = new Dictionary<RabbitHoleType, InteractionParameters>();

                    sParameters.Add(RabbitHoleType.Arboretum, new InteractionParameters("Gameplay/Objects/RabbitHoles/Arboretum:", "VisitInteraction", Arboretum.kVisitRabbitHoleTuning, Origin.FromArboretum));
                    sParameters.Add(RabbitHoleType.Bookstore, new InteractionParameters("", "VisitInteractionName", CityHall.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
                    sParameters.Add(RabbitHoleType.BusinessAndJournalism, new InteractionParameters("", "VisitInteractionName", CityHall.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
                    sParameters.Add(RabbitHoleType.CityHall, new InteractionParameters("Gameplay/Objects/RabbitHoles/CityHall:", "VisitInteraction", CityHall.kVisitRabbitHoleTuning, Origin.FromCityHall));
                    sParameters.Add(RabbitHoleType.ComboBookstoreDaySpa, new InteractionParameters("", "VisitInteractionName", CityHall.kVisitRabbitHoleTuning, Origin.FromDaySpa));
                    sParameters.Add(RabbitHoleType.ComboBusinessRestaurant, new InteractionParameters("", "VisitInteractionName", Theatre.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
                    sParameters.Add(RabbitHoleType.ComboCityhallPoliceMilitary, new InteractionParameters("", "VisitInteractionName", MilitaryBase.kVisitRabbitHoleTuning, Origin.FromMilitaryBase));
                    sParameters.Add(RabbitHoleType.DaySpa, new InteractionParameters("", "VisitInteractionName", CityHall.kVisitRabbitHoleTuning, Origin.FromDaySpa));
                    sParameters.Add(RabbitHoleType.EquestrianCenter, new InteractionParameters("", "VisitInteractionName", CityHall.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
                    sParameters.Add(RabbitHoleType.Grocery, new InteractionParameters("", "VisitInteractionName", CityHall.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
                    sParameters.Add(RabbitHoleType.Hideout, new InteractionParameters("", "VisitInteractionName", Mausoleum.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
                    sParameters.Add(RabbitHoleType.Hospital, new InteractionParameters("", "VisitInteractionName", CityHall.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
                    sParameters.Add(RabbitHoleType.Mausoleum, new InteractionParameters("Gameplay/Objects/RabbitHoles/Mausoleum:", "VisitInteractionName", Mausoleum.kVisitRabbitHoleTuning, Origin.FromMausoleum));
                    sParameters.Add(RabbitHoleType.MilitaryBase, new InteractionParameters("Gameplay/Objects/RabbitHoles/MilitaryBase:", "VisitInteractionName", MilitaryBase.kVisitRabbitHoleTuning, Origin.FromMilitaryBase));
                    sParameters.Add(RabbitHoleType.MovieSet, new InteractionParameters("Gameplay/Objects/RabbitHoles/MovieSet:", "VisitInteraction", MovieSet.kVisitRabbitHoleTuning, Origin.FromMovieSet));
                    sParameters.Add(RabbitHoleType.PoliceStation, new InteractionParameters("", "VisitInteractionName", MilitaryBase.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
					sParameters.Add(RabbitHoleType.Restaurant, new InteractionParameters("", "VisitInteractionName", Theatre.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
                    sParameters.Add(RabbitHoleType.School, new InteractionParameters("", "VisitInteractionName", CityHall.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
                    sParameters.Add(RabbitHoleType.ScienceLab, new InteractionParameters("Gameplay/Objects/RabbitHoles/ScienceLab:", "VisitInteractionName", ScienceLab.kVisitRabbitHoleTuning, Origin.FromScienceLab));
                    sParameters.Add(RabbitHoleType.Stadium, new InteractionParameters("", "VisitInteractionName", Theatre.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
                    sParameters.Add(RabbitHoleType.Subway, new InteractionParameters("", "VisitInteractionName", CityHall.kVisitRabbitHoleTuning, Origin.FromVisitingLocation));
                    sParameters.Add(RabbitHoleType.Theatre, new InteractionParameters("Gameplay/Objects/RabbitHoles/Theatre:", "VisitInteractionName", Theatre.kVisitRabbitHoleTuning, Origin.FromTheatre));
                    sParameters.Add(RabbitHoleType.VaultOfAntiquity, new InteractionParameters("Gameplay/Objects/RabbitHoles/VaultOfAntiquity:", "VisitInteractionName", VaultOfAntiquity.kVisitRabbitHoleTuning, Origin.FromVaultOfAntiquity));

                    sParameters.Add(RabbitHoleType.AdminstrationCenter, new InteractionParameters("Gameplay/Objects/RabbitHoles/AdminstrationCenter:", "VisitInteractionName", AdminstrationCenter.kVisitRabbitHoleTuning, Origin.FromAdminstrationCenterRabbitHole));
                    sParameters.Add(RabbitHoleType.Annex, new InteractionParameters("Gameplay/Objects/RabbitHoles/Annex:", "VisitInteractionName", Annex.kVisitRabbitHoleTuning, Origin.FromAnnexRabbitHole));
                    sParameters.Add(RabbitHoleType.CollegeOfArts, new InteractionParameters("Gameplay/Objects/RabbitHoles/CollegeOfArts:", "VisitInteractionName", CollegeOfArts.kVisitRabbitHoleTuning, Origin.FromCollegeOfArtsRabbitHole));
                    sParameters.Add(RabbitHoleType.CollegeOfBusiness, new InteractionParameters("Gameplay/Objects/RabbitHoles/CollegeOfBusiness:", "VisitInteractionName", CollegeOfBusiness.kVisitRabbitHoleTuning, Origin.FromCollegeOfBusinessRabbitHole));
                    sParameters.Add(RabbitHoleType.CollegeOfScience, new InteractionParameters("Gameplay/Objects/RabbitHoles/CollegeOfScience:", "VisitInteractionName", CollegeOfScience.kVisitRabbitHoleTuning, Origin.FromCollegeOfScienceRabbitHole));

                    sParameters.Add(RabbitHoleType.ServoBotArena, new InteractionParameters("Gameplay/Objects/RabbitHoles/ServoBotArena:", "VisitInteractionName", ServoBotArena.kVisitRabbitHoleTuning, Origin.FromServoBotArena));
                    sParameters.Add(RabbitHoleType.StellarObservatory, new InteractionParameters("Gameplay/Objects/RabbitHoles/StellarObservatory:", "VisitInteractionName", StellarObservatory.kVisitRabbitHoleTuning, Origin.FromStellarObservatory));
                }
                return sParameters;
            }
        }

        public bool IsTakingSimToWork()
        {
            return mbTakingSimToWorkLocation;
        }

        public void SetTakingSimToWork()
        {
            mbTakingSimToWorkLocation = true;
        }

        public override string GetInteractionName()
        {
            if (IsGettingItOn)
            {
                switch (mStyle)
                {
                    case CommonWoohoo.WoohooStyle.Safe:
                        return Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
                    case CommonWoohoo.WoohooStyle.Risky:
                        return Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[] { Woohooer.Settings.mRiskyBabyMadeChanceV2[PersistedSettings.GetSpeciesIndex(Actor)] });
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        return Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
                }
            }

            Definition definition = InteractionDefinition as Definition;
            if (definition != null)
            {
                return definition.GetInteractionName(Actor, Target, InteractionObjectPair);
            }
            else
            {
                return base.GetInteractionName();
            }
        }

        public override bool InRabbitHole()
        {
            try
            {
                Definition definition = InteractionDefinition as Definition;

                StartStages();
                BeginCommodityUpdates();

                bool succeeded = false;
                try
                {
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, RabbitHole>.InsideLoopFunction(LoopDel), null);
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                if (IsGettingItOn)
                {
					GettingItOnInRabbitHole(this, WooHooer, WooHooee, RomanticType, definition.VisitTuning, mStyle, CommonWoohoo.WoohooLocation.RabbitHole, mImpregnate);
                    /*if (Actor == WooHooer)
                    {
                        switch (RomanticType)
                        {
                            case RabbitHoleRomanticType.TryForBaby:
                            case RabbitHoleRomanticType.WooHoo:
                                CommonWoohoo.RunPostWoohoo(WooHooer, WooHooee, Target, mStyle, CommonWoohoo.WoohooLocation.RabbitHole, true);
                                break;
                        }

                        Relationship relationship = Relationship.Get(WooHooer, WooHooee, true);
                        if (relationship != null)
                        {
                            relationship.UpdateSTCFromOutsideConversation(WooHooer, WooHooee, definition.VisitTuning.WooHooingCommodity, definition.VisitTuning.WooHooingSTCIncrement);
                        }
                    }

                    if ((mImpregnate) && (CommonPregnancy.IsSuccess(WooHooer, WooHooee, Autonomous, mStyle)))
                    {
                        CommonPregnancy.Impregnate(WooHooer, WooHooee, Autonomous, mStyle);
                    }

                    Target.RabbitHoleProxy.TurnOffWooHooEffect();*/
                }

                if (Actor.HasExitReason(ExitReason.StageComplete) || (Actor.HasExitReason(ExitReason.Finished) && !IsGettingItOn))
                {
					GiveVisitingBuffs(Actor, Target, definition.VisitTuning, definition.VisitBuffOrigin);
                    /*Career occupationAsCareer = Actor.OccupationAsCareer;
                    if ((occupationAsCareer != null) && (occupationAsCareer.CareerLoc.Owner == Target))
                    {
                        Actor.BuffManager.AddElement(BuffNames.Bored, definition.VisitBuffOrigin);
                        return succeeded;
                    }

                    float visitBoredomChanceAdult = definition.VisitTuning.VisitBoredomChanceAdult;
                    if (Actor.SimDescription.Teen)
                    {
                        visitBoredomChanceAdult = definition.VisitTuning.VisitBoredomChanceTeen;
                    }
                    else if (Actor.SimDescription.Child)
                    {
                        visitBoredomChanceAdult = definition.VisitTuning.VisitBoredomChanceChild;
                    }

                    if (RandomUtil.RandomChance(visitBoredomChanceAdult))
                    {
                        Actor.BuffManager.AddElement(BuffNames.Bored, definition.VisitBuffOrigin);
                        return succeeded;
                    }

                    BuffNames[] namesArray = new BuffNames[] { BuffNames.Excited, BuffNames.Fascinated, BuffNames.Intrigued, BuffNames.Impressed, BuffNames.Educated };
                    Actor.BuffManager.AddElement(namesArray[RandomUtil.GetInt(0x4)], definition.VisitBuffOrigin);*/
                }
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

		public static void GettingItOnInRabbitHole(InteractionInstance instance, Sim wooHooer, Sim wooHooee, RabbitHoleRomanticType romanticType, RabbitHole.VisitRabbitHoleTuningClass visitTuning, CommonWoohoo.WoohooStyle style, CommonWoohoo.WoohooLocation location, bool impregnate)
		{
			RabbitHole target = instance.Target as RabbitHole;

			if (instance.InstanceActor == wooHooer)
			{
				switch (romanticType)
				{
				case RabbitHoleRomanticType.TryForBaby:
				case RabbitHoleRomanticType.WooHoo:
					CommonWoohoo.RunPostWoohoo(wooHooer, wooHooee, target, style, location, true);
					break;
				}

				Relationship relationship = Relationship.Get(wooHooer, wooHooee, true);
				if (relationship != null)
				{
					relationship.UpdateSTCFromOutsideConversation(wooHooer, wooHooee, visitTuning.WooHooingCommodity, visitTuning.WooHooingSTCIncrement);
				}
			}

			if ((impregnate) && (CommonPregnancy.IsSuccess(wooHooer, wooHooee, instance.Autonomous, style)))
			{
				CommonPregnancy.Impregnate(wooHooer, wooHooee, instance.Autonomous, style);
			}

			target.RabbitHoleProxy.TurnOffWooHooEffect();
		}

		public static void GiveVisitingBuffs(Sim actor, RabbitHole target, RabbitHole.VisitRabbitHoleTuningClass visitTuning, Origin visitBuffOrigin)
		{
			Career occupationAsCareer = actor.OccupationAsCareer;
			if ((occupationAsCareer != null) && (occupationAsCareer.CareerLoc.Owner == target))
			{
				actor.BuffManager.AddElement(BuffNames.Bored, visitBuffOrigin);
				return;
			}

			float visitBoredomChanceAdult = visitTuning.VisitBoredomChanceAdult;
			if (actor.SimDescription.Teen)
			{
				visitBoredomChanceAdult = visitTuning.VisitBoredomChanceTeen;
			}
			else if (actor.SimDescription.Child)
			{
				visitBoredomChanceAdult = visitTuning.VisitBoredomChanceChild;
			}

			if (RandomUtil.RandomChance(visitBoredomChanceAdult))
			{
				actor.BuffManager.AddElement(BuffNames.Bored, visitBuffOrigin);
				return;
			}

			BuffNames[] namesArray = new BuffNames[] { BuffNames.Excited, BuffNames.Fascinated, BuffNames.Intrigued, BuffNames.Impressed, BuffNames.Educated };
			actor.BuffManager.AddElement(namesArray[RandomUtil.GetInt(0x4)], visitBuffOrigin);
		}

		public abstract class BaseDefinition<TInteraction> : RabbitHole.VisitRabbitHoleBase<TInteraction>.BaseDefinition, IWooHooDefinition where TInteraction : InteractionInstance, new()
		{
			public bool IsGroupAddition;

			public BaseDefinition()
			{ }
			public BaseDefinition(string interactionName, RabbitHole.VisitRabbitHoleTuningClass visitTuning, Origin visitBuffOrigin)
				: base(interactionName, visitTuning, visitBuffOrigin, 0f)
			{ }

			public Sim ITarget(InteractionInstance paramInteraction)
			{
				RabbitHole.VisitRabbitHoleBase<TInteraction> interaction = paramInteraction as RabbitHole.VisitRabbitHoleBase<TInteraction>;
				if (interaction == null) return null;

				return interaction.WooHooee;
			}

			public abstract CommonWoohoo.WoohooLocation GetLocation (IGameObject obj);

			public abstract CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction);

			public int Attempts
			{
				set { }
			}

			public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
			{
				if (isAutonomous && a.IsInGroupingSituation())
				{
					return false;
				}

				if (a.GetSituationOfType<GroupingSituation>() != null)
				{
					return IsGroupAddition;
				}

				if ((a.Posture != null) && (a.Posture.Container == target))
				{
					return false;
				}

				return true;
			}

			public InteractionDefinition ProxyClone(Sim target)
			{
				throw new NotImplementedException();
			}
		}

		public class Definition : BaseDefinition<VisitRabbitHoleEx> //RabbitHole.VisitRabbitHoleBase<VisitRabbitHoleEx>.BaseDefinition, IWooHooDefinition
        {
            public Definition(InteractionParameters parameters)
                : base(parameters.mPrefix + parameters.mVisitName, parameters.mTuning, parameters.mOrigin)
            { }
            public Definition(string interactionName, RabbitHole.VisitRabbitHoleTuningClass visitTuning, Origin visitBuffOrigin)
                : base(interactionName, visitTuning, visitBuffOrigin)
            { }

            public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return CommonWoohoo.WoohooLocation.RabbitHole;
            }

            public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
            {
                VisitRabbitHoleEx interaction = paramInteraction as VisitRabbitHoleEx;
                if (interaction == null) return CommonWoohoo.WoohooStyle.Safe;

                return interaction.mStyle;
            }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                if (InteractionName.StartsWith("VisitInteraction"))
                {
					return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Core/VisitCommunityLot:VisitNamedLot", new object[] { target.GetLocalizedName() });
                }
                else
                {
                    return base.GetInteractionName(actor, target, iop);
                }
            }
        }

        public class InteractionParameters
        {
            public readonly string mPrefix;
            public readonly string mVisitName;
            public readonly RabbitHole.VisitRabbitHoleTuningClass mTuning;
            public readonly Origin mOrigin;

            public InteractionParameters(string prefix, string visitName, RabbitHole.VisitRabbitHoleTuningClass tuning, Origin origin)
            {
                mPrefix = prefix;
                mVisitName = visitName;
                mTuning = tuning;
                mOrigin = origin;
            }
        }

        public class CustomInjector : Common.InteractionInjector<RabbitHole>
        {
            public CustomInjector()
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                RabbitHole hole = obj as RabbitHole;
				if (hole == null || hole is EiffelTower) return false;

                InteractionParameters parameters;
                if (Parameters.TryGetValue(hole.Guid, out parameters))
                {
                    if (base.Perform(obj, new Definition(parameters), existing))
                    {
                        Type type = typeof(RabbitHole.VisitRabbitHole.Definition);

                        Common.RemoveInteraction(obj, type);
                        existing.Remove(type);

                        return true;
                    }
                }

                return false;
            }
        }
    }
}
