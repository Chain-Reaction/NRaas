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
//using Sims3.Gameplay.Pools;
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
	public class TakeElevatorToTopEx : RabbitHole.VisitRabbitHoleBase<TakeElevatorToTopEx>, ITakeSimToWorkLocation, Common.IPreLoad, Common.IAddInteraction
	{
		private bool mbTakingSimToWorkLocation;

		public bool mImpregnate;
		public CommonWoohoo.WoohooStyle mStyle;

		//static Dictionary<RabbitHoleType, InteractionParameters> sParameters = null;

		public Dictionary<Sim, bool> SimArrivalStatus;

		public AlarmHandle AddSoreBuffAlarmHandle = AlarmHandle.kInvalidHandle;

		public AlarmHandle AddWondrousViewAlarmHandle = AlarmHandle.kInvalidHandle;

		public void AddInteraction(Common.InteractionInjectorList interactions)
        {
			//interactions.Replace<EiffelTower, EiffelTower.TakeElevatorToTop.ElevatorDefinition>(new ElevatorDefinition (new VisitRabbitHoleEx.InteractionParameters ("Gameplay/Objects/RabbitHoles/EiffelTower:", "TakeElevatorToTop", EiffelTower.kVisitRabbitHoleTuning, Origin.FromEiffelTower)));
			//interactions.Replace<EiffelTower, EiffelTower.TakeElevatorToTop.StairsDefinition>(new StairsDefinition (new VisitRabbitHoleEx.InteractionParameters ("Gameplay/Objects/RabbitHoles/EiffelTower:", "TakeStairsToTop", EiffelTower.kVisitRabbitHoleTuning, Origin.FromEiffelTower)));

			interactions.AddCustom(new TakeElevatorToTopEx.CustomInjector());
        }

		public void OnPreLoad()
        {
			//Tunings.Inject<RabbitHole, RabbitHole.VisitRabbitHole.Definition, Definition>(false);
			//Tunings.Inject<EiffelTower, EiffelTower.TakeElevatorToTop.ElevatorDefinition, ElevatorDefinition>(false);
			Tunings.Inject<EiffelTower, EiffelTower.TakeElevatorToTop.ElevatorDefinition, ElevatorDefinition>(false);
			Tunings.Inject<EiffelTower, EiffelTower.TakeElevatorToTop.StairsDefinition, StairsDefinition>(false);

        }

		/*public static Dictionary<RabbitHoleType, InteractionParameters> Parameters
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
        }*/

        public bool IsTakingSimToWork()
        {
            return mbTakingSimToWorkLocation;
        }

        public void SetTakingSimToWork()
        {
            mbTakingSimToWorkLocation = true;
        }

		public void AddSoreBuffCallback ()
		{
			Actor.AddSoreBuff = true;
			AddSoreBuffAlarmHandle = AlarmHandle.kInvalidHandle;
		}

		public void AddWondrousViewBuffCallback ()
		{
			Actor.BuffManager.AddElement (BuffNames.WondrousView, Origin.FromEiffelTower);
			AddWondrousViewAlarmHandle = AlarmHandle.kInvalidHandle;
		}

		public override void BeforeExitRabbitHoleAndRouteAway (Sim actor)
		{
			if (base.InteractionDefinition is TakeElevatorToTopEx.StairsDefinition)
			{
				Actor.ResetShapeDelta ();
				Actor.SkillManager.StopSkillGain (SkillNames.Athletic);
			}
			base.BeforeExitRabbitHoleAndRouteAway (actor);
		}

		public void ChargeBill ()
		{
			int num = 0;
			Household household = null;
			if (SimArrivalStatus == null)
			{
				num = EiffelTower.TakeElevatorToTop.kElevatorPricePerPerson;
				household = Actor.Household;
			}
			else
			{
				household = Household.ActiveHousehold;
				foreach (Sim current in SimArrivalStatus.Keys)
				{
					if (current.Household == household)
					{
						num += EiffelTower.TakeElevatorToTop.kElevatorPricePerPerson;
					}
				}
			}
			if (num <= household.FamilyFunds)
			{
				household.ModifyFamilyFunds (-num);
				return;
			}
			household.UnpaidBills += num;
			DisplayLackOfFundsTNS ();
		}

		public override void Cleanup ()
		{
			if (base.InteractionDefinition is TakeElevatorToTopEx.StairsDefinition)
			{
				Actor.ResetShapeDelta ();
				Actor.SkillManager.StopSkillGain (SkillNames.Athletic);
			}
			base.Cleanup ();
		}

		public void DisplayLackOfFundsTNS ()
		{
			Sim sim = Actor;
			if (SimArrivalStatus != null && SimArrivalStatus.Count > 1)
			{
				foreach (Sim current in SimArrivalStatus.Keys)
				{
					if (current.IsInActiveHousehold)
					{
						current.BuffManager.AddElement (BuffNames.Embarrassed, Origin.FromUnpaidBills);
						sim = current;
					}
				}
			}
			string message = Localization.LocalizeString ("Gameplay/Objects/RabbitHoles/EiffelTower/TakeElevatorToTop:InsufficientFunds", new object[]
				{
					sim.Household.Name
				});
			sim.ShowTNSIfSelectable (message, StyledNotification.NotificationStyle.kGameMessageNegative);
		}

		public void DoChargeCheck (bool setArrivalStatus)
		{
			if (base.InteractionDefinition is TakeElevatorToTopEx.ElevatorDefinition)
			{
				if (SimArrivalStatus != null)
				{
					if (setArrivalStatus)
					{
						SimArrivalStatus [Actor] = true;
					}
					if (IsEveryoneInside ())
					{
						ChargeBill ();
						return;
					}
				}
				else
				{
					if (setArrivalStatus)
					{
						ChargeBill ();
					}
				}
			}
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
				if (definition is ElevatorDefinition)
				{
					return (definition as ElevatorDefinition).GetInteractionName(Actor, Target, InteractionObjectPair);
				}
				return (definition as StairsDefinition).GetInteractionName(Actor, Target, InteractionObjectPair);
            }
            return base.GetInteractionName();
        }

        public override bool InRabbitHole()
        {
			DoChargeCheck(true);
			if (base.InteractionDefinition is TakeElevatorToTopEx.StairsDefinition) 
			{
				AddWondrousViewAlarmHandle = Target.AddAlarm ((float)EiffelTower.TakeElevatorToTop.kTimeUntilWondrousViewBuff * 2, TimeUnit.Minutes, new AlarmTimerCallback (AddWondrousViewBuffCallback), "AddWondrousViewBuffAlarm", AlarmType.DeleteOnReset);
				AddSoreBuffAlarmHandle = Target.AddAlarm ((float)EiffelTower.TakeElevatorToTop.kTimeUntilSoreBuff, TimeUnit.Minutes, new AlarmTimerCallback (AddSoreBuffCallback), "AddSoreBuffAlarm", AlarmType.DeleteOnReset);
				Actor.AddCardioDelta (EiffelTower.TakeElevatorToTop.kDaysToReachCardioShape);
				Actor.SkillManager.StartGainWithoutSkillMeter (SkillNames.Athletic, EiffelTower.TakeElevatorToTop.kAthleticSkillGainRate, true);
			} 
			else 
			{
				AddWondrousViewAlarmHandle = Target.AddAlarm ((float)EiffelTower.TakeElevatorToTop.kTimeUntilWondrousViewBuff, TimeUnit.Minutes, new AlarmTimerCallback (AddWondrousViewBuffCallback), "AddWondrousViewBuffAlarm", AlarmType.DeleteOnReset);
			}
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
                    if (Actor == WooHooer)
                    {
                        switch (RomanticType)
                        {
                            case RabbitHoleRomanticType.TryForBaby:
                            case RabbitHoleRomanticType.WooHoo:
							CommonWoohoo.RunPostWoohoo(WooHooer, WooHooee, Target, mStyle, CommonWoohoo.WoohooLocation.EiffelTower, true);
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

                    Target.RabbitHoleProxy.TurnOffWooHooEffect();
                }

                if (Actor.HasExitReason(ExitReason.StageComplete) || (Actor.HasExitReason(ExitReason.Finished) && !IsGettingItOn))
                {
                    Career occupationAsCareer = Actor.OccupationAsCareer;
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
                    Actor.BuffManager.AddElement(namesArray[RandomUtil.GetInt(0x4)], definition.VisitBuffOrigin);
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

		public bool IsEveryoneInside ()
		{
			using (Dictionary<Sim, bool>.ValueCollection.Enumerator enumerator = SimArrivalStatus.Values.GetEnumerator ())
			{
				while (enumerator.MoveNext ())
				{
					if (!enumerator.Current)
					{
						return false;
					}
				}
			}
			return true;
		}

		public class Definition : RabbitHole.VisitRabbitHoleBase<TakeElevatorToTopEx>.BaseDefinition, IWooHooDefinition, IZombieAllowedDefinition
		{
			public bool IsGroupAddition;
			public int Attempts
			{
				set
				{
				}
			}
			public Definition()
			{
			}
			public Definition(VisitRabbitHoleEx.InteractionParameters parameters) : base(parameters.mPrefix + parameters.mVisitName, parameters.mTuning, parameters.mOrigin, 0f)
			{
			}
			public Definition(string interactionName, RabbitHole.VisitRabbitHoleTuningClass visitTuning, Origin visitBuffOrigin) : base(interactionName, visitTuning, visitBuffOrigin, 0f)
			{
			}
			public Sim ITarget(InteractionInstance paramInteraction)
			{
				TakeElevatorToTopEx visitRabbitHoleEx = paramInteraction as TakeElevatorToTopEx;
				if (visitRabbitHoleEx == null)
				{
					return null;
				}
				return visitRabbitHoleEx.WooHooee;
			}
			public CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
			{
				return CommonWoohoo.WoohooLocation.EiffelTower;
			}
			public CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
			{
				TakeElevatorToTopEx visitRabbitHoleEx = paramInteraction as TakeElevatorToTopEx;
				if (visitRabbitHoleEx == null)
				{
					return CommonWoohoo.WoohooStyle.Safe;
				}
				return visitRabbitHoleEx.mStyle;
			}
			/*public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
			{
				if (this.InteractionName.StartsWith("VisitInteraction"))
				{
					return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Core/VisitCommunityLot:VisitNamedLot", new object[]
						{
							target.CatalogName
						});
				}
				return base.GetInteractionName(actor, target, iop);
			}*/
			public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
			{
				if (isAutonomous && a.IsInGroupingSituation())
				{
					return false;
				}
				if (a.GetSituationOfType<GroupingSituation>() != null)
				{
					return this.IsGroupAddition;
				}
				return a.Posture == null || a.Posture.Container != target;
			}
			public InteractionDefinition ProxyClone(Sim target)
			{
				throw new NotImplementedException();
			}
		}

		public class ElevatorDefinition : TakeElevatorToTopEx.Definition
        {
			public ElevatorDefinition()
            { }
			public ElevatorDefinition(VisitRabbitHoleEx.InteractionParameters parameters) : base(parameters)
            { }
			public ElevatorDefinition(string interactionName, RabbitHole.VisitRabbitHoleTuningClass visitTuning, Origin visitBuffOrigin) : base(interactionName, visitTuning, visitBuffOrigin)
            { }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
				return Localization.LocalizeString (actor.IsFemale, "Gameplay/Objects/RabbitHoles/EiffelTower:TakeElevatorToTopWithCost", new object[]
				{
					EiffelTower.TakeElevatorToTop.kElevatorPricePerPerson
				});
            }
        }

		public class StairsDefinition : TakeElevatorToTopEx.Definition
		{
			public StairsDefinition ()
			{ }
			public StairsDefinition(VisitRabbitHoleEx.InteractionParameters parameters) : base(parameters)
			{ }
			public StairsDefinition (string interactionName, RabbitHole.VisitRabbitHoleTuningClass visitTuning, Origin visitBuffOrigin) : base (interactionName, visitTuning, visitBuffOrigin)
			{ }
		}

		/*public class InteractionParameters
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
        }*/

		public class CustomInjector : Common.InteractionInjector<EiffelTower>
        {
            public CustomInjector()
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
				EiffelTower tower = obj as EiffelTower;
				if (tower == null) return false;

				RemoveInteractionCustom<EiffelTower.TakeElevatorToTop.ElevatorDefinition>(obj);
				RemoveInteractionCustom<EiffelTower.TakeElevatorToTop.StairsDefinition>(obj);

				//Common.RemoveInteraction<VisitRabbitHoleEx.Definition>(obj);
				base.Perform (obj, new ElevatorDefinition (new VisitRabbitHoleEx.InteractionParameters ("Gameplay/Objects/RabbitHoles/EiffelTower:", "TakeElevatorToTop", EiffelTower.kVisitRabbitHoleTuning, Origin.FromEiffelTower)), existing);
				base.Perform (obj, new StairsDefinition (new VisitRabbitHoleEx.InteractionParameters("Gameplay/Objects/RabbitHoles/EiffelTower:", "TakeStairsToTop", EiffelTower.kVisitRabbitHoleTuning, Origin.FromEiffelTower)), existing);
				return true;
                
            }

			public static void RemoveInteractionCustom<DEF>(GameObject obj) where DEF : InteractionDefinition
			{
				if (obj.Interactions != null)
				{
					for (int i = 0; i < obj.Interactions.Count; i++)
					{
						InteractionObjectPair interactionObjectPair = obj.Interactions[i];
						if (interactionObjectPair.InteractionDefinition is DEF)
						{
							obj.Interactions.RemoveAt(i);
						}
					}
				}
			}
		}
    }
}
