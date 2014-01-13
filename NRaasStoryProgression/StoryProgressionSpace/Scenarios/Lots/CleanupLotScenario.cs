using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
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
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public class CleanupLotScenario : LotScenario
    {
        public static bool sExpandedInstalled = false;

        public CleanupLotScenario(Lot lot)
            : base (lot)
        { }
        protected CleanupLotScenario(CleanupLotScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "CleanupLot";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<NightlyLotScenario.Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(Lot lot)
        {
            if (Household.ActiveHousehold == null)
            {
                IncStat("No Active");
                return false;
            }

            return base.Allow(lot);
        }

        public PropertyData FindVenueProperty(RealEstateManager ths, Lot l)
        {
            Predicate<PropertyData> match = delegate(PropertyData d)
            {
                if ((d.PropertyType != RealEstatePropertyType.Venue) && (d.PropertyType != RealEstatePropertyType.Resort))
                {
                    return false;
                }
                return d.LotId == l.LotId;
            };

            return ths.mAllProperties.Find(match);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool isActiveLot = false;

            if (Sim.ActiveActor != null)
            {
                isActiveLot = Lot.CanSimTreatAsHome(Sim.ActiveActor);
                
                if ((!isActiveLot) && (Household.ActiveHousehold.RealEstateManager != null))
                {
                    PropertyData data = Household.ActiveHousehold.RealEstateManager.FindProperty(Lot);
                    if ((data != null) && (data.PropertyType == RealEstatePropertyType.Resort))
                    {
                        isActiveLot = true;
                    }
                }
            }

            if (GetValue<CleanOption, bool>())
            {
                if ((!isActiveLot) && (!Lot.IsActive) && (!Occupation.DoesLotHaveAnyActiveJobs(Lot)))
                {
                    bool cleaner = false;
                    if ((Lot.Household != null) && (!Lot.IsBaseCampLotType))
                    {
                        bool child = false;

                        foreach (SimDescription sim in HouseholdsEx.All(Lot.Household))
                        {
                            if (AddScoring("Neat", sim) > 0)
                            {
                                cleaner = true;
                            }

                            if (sim.ToddlerOrBelow)
                            {
                                child = true;
                            }
                        }

                        if ((!child) && (Babysitter.Instance != null))
                        {
                            Babysitter.Instance.MakeServiceRequest(Lot, false, ObjectGuid.InvalidObjectGuid);
                        }
                    }
                    else
                    {
                        cleaner = true;

                        // Disable all services applied to this lot
                        Service[] serviceArray = new Service[] { Babysitter.Instance, Maid.Instance, PizzaDelivery.Instance, Repairman.Instance, SocialWorkerAdoption.Instance, Police.Instance };
                        foreach (Service service in serviceArray)
                        {
                            if (service == null) continue;

                            service.MakeServiceRequest(Lot, false, ObjectGuid.InvalidObjectGuid);
                        }
                    }

                    foreach (GameObject obj in Lot.GetObjects<GameObject>())
                    {
                        if (obj.LotCurrent == null)
                        {
                            IncStat("Bogus Location " + obj.CatalogName + " (" + Lot.Name + ", " + Lot.Address + ")", Common.DebugLevel.High);
                            continue;
                        }

                        if (obj is IVelvetRopes)
                        {
                            if (!Bartending.IsBarVenue(Lot))
                            {
                                (obj as IVelvetRopes).NightlyCleanUp();
                            }
                        }

                        try
                        {
                            if (!Lot.DoesObjectNeedCleaning(obj)) continue;
                        }
                        catch (Exception e)
                        {
                            Common.Exception(obj, e);
                            continue;
                        }

                        if (cleaner)
                        {
                            if (obj.IsCleanable)
                            {
                                obj.Cleanable.ForceClean();
                            }

                            IThrowAwayable awayable = obj as IThrowAwayable;
                            if (awayable != null)
                            {
                                awayable.ThrowAwayImmediately();
                            }

                            Book book = obj as Book;
                            if ((book != null) && (!(book.Data is BookToddlerData)))
                            {
                                if (!book.InInventory)
                                {
                                    if (!Lots.PutAwayBook(this, book, Lot))
                                    {
                                        if (!Lot.IsResidentialLot)
                                        {
                                            book.FadeOut(false, true);
                                        }
                                    }
                                }
                            }
                        }

                        if ((obj is IDestroyOnMagicalCleanup) && ((!Lot.IsResidentialLot) || (!sExpandedInstalled)))
                        {
                            obj.FadeOut(false, true);
                        }
                    }

                    if (!Lot.IsResidentialLot)
                    {
                        Lot.FireManager.RestoreObjects();
                    }

                    LotLocation[] puddles = World.GetPuddles(Lot.LotCurrent.LotId, LotLocation.Invalid);
                    if (puddles.Length > 0)
                    {
                        foreach (LotLocation location in puddles)
                        {
                            PuddleManager.RemovePuddle(Lot.LotCurrent.LotId, location);
                        }
                    }

                    LotLocation[] burntTiles = World.GetBurntTiles(Lot.LotCurrent.LotId, LotLocation.Invalid);
                    if (burntTiles.Length > 0)
                    {
                        foreach (LotLocation location2 in burntTiles)
                        {
                            World.SetBurnt(Lot.LotId, location2, false);
                        }
                    }

                    foreach (ICatPrey prey in Sims3.Gameplay.Queries.GetObjects<ICatPrey>(Lot))
                    {
                        if (prey.IsUnconscious)
                        {
                            prey.Destroy();
                        }
                    }

                    if (!Lot.IsResidentialLot)
                    {
                        Lot.RepairAllObjects();
                    }
                }
            }

            if (GetValue<SeasonOption, bool>())
            {
                if ((!isActiveLot) || (Lot.IsBaseCampLotType) || (Lot.HasVirtualResidentialSlots))
                {
                    SeasonalLotMarker[] objects = Lot.GetObjects<SeasonalLotMarker>();
                    if ((objects.Length > 0x0) && objects[0x0].IsKickNeeded())
                    {
                        TimeUnit units = TimeUnit.Minutes;

                        if (Lot.IsCommunityLot)
                        {
                            foreach (Sim sim in Lot.GetAllActors())
                            {
                                Sim.MakeSimGoHome(sim, true, new InteractionPriority(InteractionPriorityLevel.High));

                                units = TimeUnit.Hours;
                            }
                        }
                        Lot.mGoHomeForSeasonChangeTimer = Lot.AlarmManager.AddAlarm(1f, units, Lot.ChangeSeason, "timer just to make it yieldable", AlarmType.DeleteOnReset, Lot);
                        Lot.AlarmManager.AlarmWillYield(Lot.mGoHomeForSeasonChangeTimer);
                    }
                }
                else
                {
                    Lot.MagicallyChangeSeason();
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new CleanupLotScenario(this);
        }

        public class CleanOption : BooleanManagerOptionItem<ManagerLot>, IDebuggingOption
        {
            public CleanOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CleanupLotClean";
            }
        }

        public class SeasonOption : BooleanManagerOptionItem<ManagerLot>, IDebuggingOption
        {
            public SeasonOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CleanupLotSeason";
            }
        }
    }
}
