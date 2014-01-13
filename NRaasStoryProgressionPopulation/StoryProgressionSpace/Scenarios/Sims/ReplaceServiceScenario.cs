using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
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
    public class ReplaceServiceScenario : SimEventScenario<Event>, ISimFromBinManager
    {
        public ReplaceServiceScenario()
        { }
        protected ReplaceServiceScenario(ReplaceServiceScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ReplaceService";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override int ContinueChance
        {
            get { return 25; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return HouseholdsEx.All(Household.NpcHousehold);
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimInstantiated);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return sim.IsHuman;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            // Delayed to allow for Role Manager to assign a role
            Add(frame, new DelayedScenario(Sim), ScenarioResult.Start);
            return true;
        }

        public override Scenario Clone()
        {
            return new ReplaceServiceScenario(this);
        }

        public class DelayedScenario : SimScenario
        {
            public DelayedScenario(SimDescription sim)
                : base(sim)
            { }
            protected DelayedScenario(DelayedScenario scenario)
                : base(scenario)
            { }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "ReplaceServiceDelayed";
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override bool Progressed
            {
                get { return false; }
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected static bool IsValidSim(SimDescription sim)
            {
                if ((SimTypes.InServicePool(sim)) || (sim.AssignedRole != null))
                {
                    return true;
                }
                /* Needs option
                else if (Household.RoommateManager.IsNPCRoommate(sim))
                {
                    return true;
                }
                */

                return false;
            }

            protected override bool Allow(SimDescription sim)
            {
                if (sim.LotHome != null)
                {
                    IncStat("Resident");
                    return false;
                }
                else if (!IsValidSim(sim))
                {
                    IncStat("Invalid");
                    return false;
                }
                else if ((!SimTypes.InServicePool(sim)) && (sim.AssignedRole == null))
                {
                    IncStat("Not Service");
                    return false;
                }
                else if (sim.IsZombie)
                {
                    IncStat("Zombie");
                    return false;
                }
                else if ((sim.AssignedRole is RoleTourist) || (sim.AssignedRole is RoleExplorer))
                {
                    IncStat("Tourist");
                    return false;
                }
                else if (SimTypes.IsSkinJob(sim))
                {
                    IncStat("Skinjob");
                    return false;
                }
                else if (GetValue<ReplacedServiceOption, bool>(sim))
                {
                    IncStat("Already Replaced");
                    return false;
                }

                return base.Allow(sim);
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                SimDescription newSim = null;

                using (SimFromBin<ManagerSim> bin = new SimFromBin<ManagerSim>(this, Sims))
                {
                    CASAgeGenderFlags gender = Sim.Gender;

                    switch (GetValue<GenderOption, BabyGenderScenario.FirstBornGender>())
                    {
                        case BabyGenderScenario.FirstBornGender.Male:
                            gender = CASAgeGenderFlags.Male;
                            break;
                        case BabyGenderScenario.FirstBornGender.Female:
                            gender = CASAgeGenderFlags.Female;
                            break;
                    }

                    newSim = bin.CreateNewSim(Sim.Age, gender, CASAgeGenderFlags.Human);
                    if (newSim == null)
                    {
                        IncStat("Creation Fail");
                        return false;
                    }
                }

                bool genderChanged = (Sim.Gender != newSim.Gender);

                bool result = FacialBlends.CopyGenetics(newSim, Sim, false, false);

                Sim.VoiceVariation = newSim.VoiceVariation;
                Sim.VoicePitchModifier = newSim.VoicePitchModifier;

                Sim.FirstName = newSim.FirstName;

                if (genderChanged)
                {
                    Sim.Gender = newSim.Gender;

                    SavedOutfit.Cache cache = new SavedOutfit.Cache(newSim);

                    Dictionary<OutfitCategories, bool> replaced = new Dictionary<OutfitCategories, bool>();

                    Sim.RemoveOutfits(OutfitCategories.Career, true);

                    SimOutfit geneOutfit = CASParts.GetOutfit(Sim, CASParts.sPrimary, false);

                    foreach (SavedOutfit.Cache.Key outfit in cache.Outfits)
                    {
                        using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(Sim, outfit.mKey, geneOutfit))
                        {
                            builder.Builder.Gender = Sim.Gender;

                            outfit.Apply(builder, false, null, null);

                            if (!replaced.ContainsKey(outfit.Category))
                            {
                                replaced.Add(outfit.Category, true);

                                CASParts.RemoveOutfits(Sim, outfit.Category, false);
                            }
                        }
                    }

                    if (Sim.CreatedSim != null)
                    {
                        Sim.CreatedSim.UpdateOutfitInfo();

                        Sim.CreatedSim.RefreshCurrentOutfit(true);

                        SwitchOutfits.SwitchNoSpin(Sim.CreatedSim, new CASParts.Key(OutfitCategories.Everyday, 0));
                    }
                }
                else
                {
                    new SavedOutfit.Cache(Sim).PropagateGenetics(Sim, CASParts.sPrimary);
                }

                if (newSim.OccultManager.CurrentOccultTypes != OccultTypes.None)
                {
                    if (Instantiation.PerformOffLot(Sim, Household.ActiveHousehold.LotHome, null) != null)
                    {
                        List<OccultTypes> occults = OccultTypeHelper.CreateList(newSim, true);

                        foreach (OccultTypes occult in occults)
                        {
                            switch (occult)
                            {
                                case OccultTypes.Frankenstein:
                                    Sim.TraitManager.AddElement(TraitNames.Brave);
                                    Sim.TraitManager.AddElement(TraitNames.Hydrophobic);
                                    break;
                            }
                        }

                        Sims.ApplyOccultChance(this, Sim, occults, 100, int.MaxValue);
                    }

                    if (Sim.GetOutfitCount(OutfitCategories.Everyday) > 1)
                    {
                        Sim.RemoveOutfit(OutfitCategories.Everyday, 1, true);
                    }

                    SimOutfit currentOutfit = Sim.GetOutfit(OutfitCategories.Everyday, 0);
                    if (currentOutfit != null)
                    {
                        try
                        {
                            ThumbnailManager.GenerateHouseholdSimThumbnail(currentOutfit.Key, currentOutfit.Key.InstanceId, 0x0, ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge | ThumbnailSizeMask.Medium | ThumbnailSizeMask.Small, ThumbnailTechnique.Default, true, false, Sim.AgeGenderSpecies);
                        }
                        catch (Exception e)
                        {
                            Common.Exception(Sim, e);
                        }
                    }
                }

                Deaths.CleansingKill(newSim, true);

                if (!result) return false;

                if (Common.kDebugging)
                {
                    Common.DebugNotify(GetTitlePrefix(PrefixType.Pure) + ": " + Sim.FullName, Sim.CreatedSim);
                }

                SpeedTrap.Sleep();

                SetValue<ReplacedServiceOption, bool>(Sim, true);
                return true;
            }

            public override Scenario Clone()
            {
                return new DelayedScenario(this);
            }
        }

        public class Controller : SimFromBinController
        {
            public Controller(ManagerSim manager)
                : base(manager)
            { }

            public override bool ShouldDisplayImmigrantOptions()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return true;
            }
        }

        public class Option : BooleanScenarioOptionItem<ManagerSim, ReplaceServiceScenario>, ManagerSim.IImmigrationEmigrationOption
        {
            public Option()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ReplaceService";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool Install(ManagerSim main, bool initial)
            {
                if (!base.Install(main, initial)) return false;
            
                SimFromBin<ManagerSim>.Install(new Controller(Manager), main, initial);
                return true;
            }
        }

        public class EventOption : BooleanEventOptionItem<ManagerSim, ReplaceServiceScenario>, ManagerSim.IImmigrationEmigrationOption, IDebuggingOption
        {
            public EventOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ReplaceServiceEvent";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class ReplacedServiceOption : SimBooleanOption, IDebuggingOption, INotCasteLevelOption
        {
            public ReplacedServiceOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ReplacedService";
            }
        }

        public class GenderOption : BabyGenderScenario.FirstBornGenderOptionBase<ManagerSim>, ManagerSim.IImmigrationEmigrationOption
        {
            public GenderOption()
            { }

            public override string GetTitlePrefix()
            {
                return "ServiceImmigrantGender";
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
    }
}
