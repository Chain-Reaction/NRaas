using NRaas.CommonSpace.Helpers;
using NRaas.RelativitySpace.Alterations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.ChildrenObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Helpers
{
    public class TuningAlterations : Common.IDelayedWorldLoadFinished, Common.IWorldQuit
    {
        public readonly static CASAgeGenderFlags[] sSpecies = new CASAgeGenderFlags[] { CASAgeGenderFlags.Human, CASAgeGenderFlags.Horse, CASAgeGenderFlags.Dog, CASAgeGenderFlags.LittleDog, CASAgeGenderFlags.Cat };

        public readonly static CASAgeGenderFlags[] sHumanAges = new CASAgeGenderFlags[] { CASAgeGenderFlags.Baby, CASAgeGenderFlags.Toddler, CASAgeGenderFlags.Child, CASAgeGenderFlags.Teen, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };
        public readonly static CASAgeGenderFlags[] sAnimalAges = new CASAgeGenderFlags[] { CASAgeGenderFlags.Child, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };

        public readonly static OccultTypes[] sHumanOccults = null;

        public readonly static OccultTypes[] sHorseOccults = new OccultTypes[] { OccultTypes.Unicorn };

        public readonly static OccultTypes[] sOtherOccults = new OccultTypes[0];

        public readonly static CommodityKind[] sCommodities = new CommodityKind[] { CommodityKind.VampireThirst, CommodityKind.Hunger, CommodityKind.Energy, CommodityKind.Fun, CommodityKind.Hygiene, CommodityKind.Social, CommodityKind.Bladder, CommodityKind.HorseExercise, CommodityKind.HorseThirst, CommodityKind.CatScratch, CommodityKind.DogDestruction, CommodityKind.AlienBrainPower };

        static bool sInitial = true;

        static List<IAlteration> sAlterations = new List<IAlteration>();

        static Dictionary<MotiveTuning, float> sPreviousDecayFactor = new Dictionary<MotiveTuning, float>();

        static List<InteractionTuningChange> sPreviousTuning = new List<InteractionTuningChange>();

        static TuningAlterations()
        {
            List<OccultTypes> humanTypes = new List<OccultTypes>();
            foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
            {
                if (type == OccultTypes.Unicorn) continue;

                humanTypes.Add(type);
            }

            sHumanOccults = humanTypes.ToArray();
        }

        public void OnDelayedWorldLoadFinished()
        {
            Apply();
        }

        public void OnWorldQuit()
        {
            Revert();
        }

        public static void Apply()
        {
            float relativeFactor = 1f;
            if (PersistedSettings.sRelativeFactor != 0)
            {
                relativeFactor = 1f / PersistedSettings.sRelativeFactor;
            }

            if (Relativity.Settings.mShowNotice)
            {
                if ((!sInitial) || (relativeFactor != 1f))
                {
                    Common.Notify(Common.Localize("Speed:Change", false, new object[] { relativeFactor }));
                }

                sInitial = false;
            }

            Revert();

            if (!Relativity.Settings.mPerformRelativeTuningAlterations)
            {
                relativeFactor = 1f;
            }

            if (relativeFactor != 1f)
            {
                sAlterations.Add(new RelativeAlteration<Autonomy>("kMaxTimeForLowLODInteractionDuringTheDay", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Autonomy>("kMaxTimeForLowLODInteractionDuringTheNight", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Sim>("kWaitForExitReasonDefaultTime", relativeFactor));

                sAlterations.Add(new RelativeAlteration<RabbitHole.CollectMoney>("kTimeToSpendInside", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Toilet>("kMaxLengthPlayInToilet", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Toilet>("kMaxLengthDrinkFromToilet", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Bathtub>("kMinimumTimeInBath", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Weeds.Weed>("kSimMinutesToWeed", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.RabbitHoles.Hospital.GetAllergyShot>("kSimMinutesForAllergyShot", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.RabbitHoles.Hospital.GetFluShot>("kSimMinutesForFluShot", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.RabbitHoles.Hospital.GetMedicalAdvice>("kSimMinutesForAdvice", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.RabbitHoles.Hospital.GetTherapy>("kSimMinutesForTherapy", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.ActorSystems.Pregnancy>("kSimMinutesReactToContraction", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.ObjectComponents.CatHuntingComponent.PetEatPrey>("kSimMinutesForCatToEatPrey", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.ObjectComponents.CatHuntingComponent.PetEatPrey>("kSimMinutesForDogToEatPrey", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.Pets.BoxStall>("kSimMinutesToCleanDirtyPoint", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.RabbitHoles.BusinessAndJournalismRabbitHole>("kSimMinutesSpentInRabbitHoleToPrank", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Careers.GoToSchoolInRabbitHole>("kSimMinutesBetweenLTRUpdates", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Actors.Sim.TellGhostStory>("kSimMinutesBetweenLTRUpdates", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.ActorSystems.Children.PlayPeekAboo>("kSimMinutesBetweenLTRUpdates", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.Decorations.Mirror.AdmireSelf>("kInteractionTimeLength", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.Decorations.Mirror.GussyUp>("kInteractionTimeLength", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.Decorations.Mirror.CheckSelfOutInMirror>("kInteractionTimeLength", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.Decorations.Mirror.PlayWithMirror>("kInteractionTimeLength", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.HobbiesSkills.ChessTable>("kTimeToCompleteStage", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.CookingObjects.CuttingBoard>("ChopSpreadLoopLength", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.CookingObjects.BowlLarge>("kStirMinutesLowSkill", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.CookingObjects.BowlLarge>("kStirMinutesMedSkill", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.CookingObjects.BowlLarge>("kStirMinutesHighSkill", relativeFactor));

                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.CookingObjects.Food>("kBaseCookMinutesSingleServing", relativeFactor));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Objects.CookingObjects.Food>("kBaseCookMinutesGroupServing", relativeFactor));

                // You write faster if the speed is slower
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Skills.Writing>("kRateBasePPM", 1f / relativeFactor, 0.1f));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Skills.Writing>("kRateBookWormBonusPPM", 1f / relativeFactor, 0.1f));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Skills.Writing>("kRateMaxBooksReadPPM", 1f / relativeFactor, 0.1f));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Skills.Writing>("kRateMaxWritingSkillPPM", 1f / relativeFactor, 0.1f));
                sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.Skills.Writing>("kRateMaxBooksWrittenPPM", 1f / relativeFactor, 0.1f));

                //sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.CelebritySystem.PayOffPaparazzi>("kSimMinutesInRabbitHole", relativeFactor));
                //sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.CelebritySystem.DeflectScandal>("kSimMinutesInRabbitHole", relativeFactor));
                //sAlterations.Add(new RelativeAlteration<Sims3.Gameplay.CelebritySystem.SueForSlander>("kSimMinutesInRabbitHole", relativeFactor));

                sAlterations.Add(new HarvestAlteration(relativeFactor));
                sAlterations.Add(new WaterPlantAlteration(relativeFactor));
            }

            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Entertainment.KaraokeMachine.kUpgradeTuningSingingSynth, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Appliances.Dishwasher.kUpgradeMakeSilentTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Appliances.FortuneCookieMaker.kUpgradeAlwaysGoodFortuneTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Appliances.Microwave.kFasterCookingTuning.UpgradeTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Appliances.Stove.kUpgradeMakeFireproofTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Appliances.Stove.kUpgradeImprovedCookingQualityTuning.UpgradeTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Appliances.WashingMachineCheap.kTuning.kUpgradePolymerInjectionTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Appliances.WashingMachineCheap.kTuning.kUpgradeUnbreakableTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Appliances.WashingMachineExpensive.kTuning.kUpgradePolymerInjectionTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Appliances.WashingMachineExpensive.kTuning.kUpgradeUnbreakableTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Appliances.Mimics.WasherVenueLaundry.kTuning.kUpgradePolymerInjectionTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Appliances.Mimics.WasherVenueLaundry.kTuning.kUpgradeUnbreakableTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Electronics.Computer.kUpgradeGraphicsTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Electronics.StereoCheap.kUpgradeSoupUpSpeakersTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Electronics.StereoCheap.kUpgradeWireHouseWithSpeakersTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Electronics.StereoCommunity.kUpgradeSoupUpSpeakersTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Electronics.StereoCommunity.kUpgradeWireHouseWithSpeakersTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Electronics.StereoExpensive.kUpgradeSoupUpSpeakersTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Electronics.StereoExpensive.kUpgradeWireHouseWithSpeakersTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Electronics.StereoMediocre.kUpgradeSoupUpSpeakersTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Electronics.StereoMediocre.kUpgradeWireHouseWithSpeakersTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Electronics.TV.kUpgradeBoostChannelTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Electronics.WallMountedSpeaker.kUpgradeImproveSpeakersTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Entertainment.ElectroDanceSphere.kUpgradeTuningStabilizer, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Entertainment.ElectroDanceSphere.kUpgradeTuningDimensionalGate, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Environment.SolarPanel.kUpgradeMoreEfficentTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Environment.Sprinkler.kUpgradeAutoWaterTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Environment.Windmill.kUpgradeMoreEfficentTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.FireFightingObjects.FireStationAlarm.kImprovedFireAlarmTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Environment.Mimics.FirePitCommunity.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Environment.Mimics.FirePitPatio.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Environment.Mimics.FirePitPatioSquare.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Environment.Mimics.FirepitRanchStones.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Environment.Mimics.FirepitSleekSop.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Environment.Mimics.FirePitValue.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceContemporary.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceHearthPatio.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceHearthPatio2x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceHearthRanch2x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceHearthRanchRetreat2x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceHearthValue.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleAntique2x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleCeleb3x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleColonial3x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleDarkLux2x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleDarkLuxMirror2x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleFederal2x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleFederalColumns2x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleFrance2x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleMission.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleModern.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleModern2x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleRanchGothic.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeFireplaceAlteration(Sims3.Gameplay.Objects.Fireplaces.Mimics.FireplaceMantleRomantic2x1.kFireplaceTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.HobbiesSkills.Dartboard.kNeverLoseTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.HobbiesSkills.NectarMaker.kUpgradeFlavorEnhancement, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.HobbiesSkills.NectarMaker.kUpgradeImprovedPressing, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.HobbiesSkills.StylingStation.kLookGoodMirrosUpgradeTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.HobbiesSkills.TattooChairReal.kTattooChairTuning.kUpgradeInkinizationTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.HobbiesSkills.TattooChairReal.kTattooChairTuning.kUpgradeUnbreakableTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.HobbiesSkills.TattooChairAdvanced.kTattooChairTuning.kUpgradeInkinizationTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.HobbiesSkills.TattooChairAdvanced.kTattooChairTuning.kUpgradeUnbreakableTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Miscellaneous.TrashCompactor.kUpgradeImprovedCrushingTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Pets.WaterTrough.kUpgradeAutoFillTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Plumbing.HotTubBase.kUpgradeImprovedJetsTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Rewards.FoodReplicator.kUpgradeImproveMemoryTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Rewards.Teleporter.kUpgradePerfectTeleportation, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Umbrella.kUpgradeGlowTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Vehicles.CarUFO.kUpgradeSpaceTravelTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Gameplay.Objects.Vehicles.CarUFO.kUpgradeLaserCannonsTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Store.Objects.RoutineMachine.UpgradePreventBreakage.kUpgradeUnbreakableTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Store.Objects.RoutineMachine.UpgradeSpeakers.kAddSpeakersTuning, relativeFactor));
            sAlterations.Add(new UpgradeAlteration(Sims3.Store.Objects.RoutineMachine.UpgradeTransporter.kUpgradeTransporterTuning, relativeFactor));

            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Entertainment.KaraokeMachine.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Appliances.FortuneCookieMaker.kRepairableComponentTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.StereoCheap.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.StereoCommunity.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.StereoExpensive.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.StereoMediocre.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.WallMountedSpeaker.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Entertainment.ElectroDanceSphere.kRepairTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.HobbiesSkills.NectarMaker.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Miscellaneous.TrashCompactor.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Rewards.Teleporter.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Umbrella.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.HobbiesSkills.TattooChairReal.kTattooChairTuning.kRepairableComponentTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.HobbiesSkills.TattooChairAdvanced.kTattooChairTuning.kRepairableComponentTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Appliances.DishwasherCheap.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Appliances.DishwasherExpensive.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Appliances.WashingMachineCheap.kTuning.kRepairableComponentTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Appliances.WashingMachineExpensive.kTuning.kRepairableComponentTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Appliances.Mimics.WasherVenueLaundry.kTuning.kRepairableComponentTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.ComputerCheap.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.ComputerExpensive.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.ComputerLaptop.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.ComputerLaptopVenue.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.JukeboxCheap.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.JukeboxExpensive.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.JukeboxModerate.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.TVCheap.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.TVExpensive.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.TVModerate.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.TVModerateFlatscreen.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.TVWall.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.VideoPokerMachine.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Electronics.WallMountedSpeaker.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.HobbiesSkills.Inventing.TimeMachine.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.CornerBathtub.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.HotTub4Seated.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.HotTubGrotto.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.HotTubGrottoCeleb.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubClawfoot.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubDarkLux.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubModern.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRanch.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRectangle.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRomantic.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerModern.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerSeasonChic.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubValue.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerBasic.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerGen.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerHETech.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerLoft.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerModern.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerRanch.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerRomantic.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletCheap.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletDarkLux.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletDive.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletEgyptAncient.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletExpensive.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletModerate.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletModern.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletRanch.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletRomantic.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletStall.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.ShowerCheap.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.ShowerExpensive.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Rewards.Teleporter.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Umbrella.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Store.Objects.FootMassageChair.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Store.Objects.GumballMachine.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Store.Objects.PetFeedingStation.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Store.Objects.PinballMachine.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Store.Objects.RoutineMachine.kRepairableComponentTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Store.Objects.SleepPodFuture.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Store.Objects.UberToilet.kRepairableTuning, relativeFactor));

            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterCeleb.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterCheap.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterExpensive.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterKitchen.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterModerate.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterPatio.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterQuaint.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedastalVenueLaundry.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalDarkLux.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalDive.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalExpensive.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalHETech.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalLounge.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalModerate.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalRanch.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalTraditional.kRepairableTuning, relativeFactor));
            sAlterations.Add(new RepairAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkRomantic.kRepairableTuning, relativeFactor));

            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterCeleb.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterCheap.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterExpensive.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterKitchen.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterModerate.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterPatio.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterQuaint.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedastalVenueLaundry.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalDarkLux.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalDive.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalExpensive.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalHETech.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalLounge.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalModerate.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalRanch.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalTraditional.kSinkTuning, relativeFactor));
            sAlterations.Add(new SinkAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkRomantic.kSinkTuning, relativeFactor));

            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Appliances.Mimics.StoveAntiqueDecoMod.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Appliances.Mimics.StoveCeleb.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Appliances.Mimics.StoveCheap.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Appliances.Mimics.StoveCountry.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Appliances.Mimics.StoveExpensive.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Appliances.Mimics.StoveModern.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Appliances.Mimics.StovePatio.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Appliances.Mimics.StoveRanch.kCleanableTuning, relativeFactor));

            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Counters.Counter.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Decorations.BirdCage.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Decorations.FishTankModern.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.MinorPets.TerrariumLizard.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.MinorPets.TerrariumRodent.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.MinorPets.TerrariumSnake.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.MinorPets.TerrariumTurtle.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Miscellaneous.SnackBowl.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Pets.LitterBoxCheap.kCleanableTuning, relativeFactor));

            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubClawfoot.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubDarkLux.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubModern.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRanch.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRectangle.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubRomantic.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerModern.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubShowerSeasonChic.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.BathtubValue.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerBasic.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerGen.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerHETech.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerLoft.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerModern.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerRanch.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ShowerRomantic.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletCheap.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletDarkLux.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletDive.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletEgyptAncient.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletExpensive.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletModerate.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletModern.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletRanch.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletRomantic.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.ToiletStall.kCleanableTuning, relativeFactor));

            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.ShowerCheap.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.ShowerExpensive.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.UrinalHandFlush.kCleanableTuning, relativeFactor));

            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Seating.HighChair.kCleanableTuning, relativeFactor));

            sAlterations.Add(new CleaningAlteration(Sims3.Store.Objects.ChocolateFountain.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Store.Objects.DeepFryerVegas.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Store.Objects.IceCreamMaker.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Store.Objects.UberToilet.kCleanableTuning, relativeFactor));

            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterCeleb.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterCheap.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterExpensive.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterKitchen.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterModerate.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterPatio.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkCounterQuaint.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedastalVenueLaundry.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalDarkLux.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalDive.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalExpensive.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalHETech.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalLounge.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalModerate.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalRanch.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkPedestalTraditional.kCleanableTuning, relativeFactor));
            sAlterations.Add(new CleaningAlteration(Sims3.Gameplay.Objects.Plumbing.Mimics.SinkRomantic.kCleanableTuning, relativeFactor));

            sAlterations.Add(new JungleGymAlteration(Sims3.Gameplay.Objects.Environment.JungleGymLookOutTower.kJungleGymTuning, relativeFactor));
            sAlterations.Add(new JungleGymAlteration(Sims3.Gameplay.Objects.Environment.JungleGymSlide.kJungleGymTuning, relativeFactor));

            sAlterations.Add(new SkillBookAlteration());

            sAlterations.Add(new BoardBreakAlteration());

            sAlterations.Add(new SkillAlteration<BoardBreaker>("kAthleticSkillGainRateWhileBoardBreaking", SkillNames.Athletic));
            sAlterations.Add(new SkillAlteration<Criminal.PracticeIllicitActivities>("kSkillGainRate", SkillNames.Athletic));
            sAlterations.Add(new SkillAlteration<LawEnforcement.WorkoutTone>("kAthleticSkillGainRate", SkillNames.Athletic));
            sAlterations.Add(new SkillAlteration<SwimmingInPool>("kSkillGainRatePerMinute", SkillNames.Athletic));

            sAlterations.Add(new SkillAlteration<Music.StudyMusicTheoryTone>("kGuitarSkillGainRate", SkillNames.Guitar));
            sAlterations.Add(new SkillAlteration<Music.StudyMusicTheoryTone>("kLogicSkillGainRate", SkillNames.Logic));

            sAlterations.Add(new SkillAlteration<Telescope>("kLogicSkillGainRateWhileSearchingGalaxy", SkillNames.Logic));

            sAlterations.Add(new SkillAlteration<PottyChair>("kPottyTrainToddlerGainRate", SkillNames.PottyTrain));

            sAlterations.Add(new SkillAlteration<TeachToTalk>("kSkillGainPerSimMinute", SkillNames.LearnToTalk));
            sAlterations.Add(new SkillAlteration<TeachToWalk>("kSkillGainPerSimMinute", SkillNames.LearnToWalk));

            sAlterations.Add(new MotiveDecayAlteration<Motive>("kAdultFunLossPerSimHour", CommodityKind.Fun));
            sAlterations.Add(new MotiveDecayAlteration<Motive>("kChildFunLossPerSimHour", CommodityKind.Fun));

            sAlterations.Add(new MotiveAlteration<Motive>("kAdultStressLossPerSimHour", CommodityKind.Fun));
            sAlterations.Add(new MotiveAlteration<Motive>("kChildStressLossPerSimHour", CommodityKind.Fun));

            sAlterations.Add(new MotiveAlteration<Sims3.Gameplay.Objects.CookingObjects.EatHeldFood>("kHungerValuePerSipForJuice", CommodityKind.Hunger));
            sAlterations.Add(new MotiveAlteration<Sims3.Gameplay.Objects.CookingObjects.EatHeldFood>("kHungerValuePerBiteForSnack", CommodityKind.Hunger));
            sAlterations.Add(new MotiveAlteration<Sims3.Gameplay.Objects.CookingObjects.EatHeldFood>("kHungerValuePerBiteForMeal", CommodityKind.Hunger));

            sAlterations.Add(new InverseMotiveAlteration<Sims3.Gameplay.Objects.Plumbing.Toilet>("kMaxLengthUseToilet", CommodityKind.Bladder, Alteration.sDefaultMinimum));

            // If the value becomes too small, energy will start to increase rather than decay
            //sAlterations.Add(new FloatAlteration<Motives>("kEnergyHoursInDay", 1f / Relativity.Settings.GetMotiveDecayFactor(null, CommodityKind.Energy)));

            // Elements from Motive:UpdateMotiveWithDecay()
            sAlterations.Add(new MotiveDecayAlteration<Sims3.Gameplay.ActorSystems.SimTemperature>("kTemperatureDeltaFrostyFace", CommodityKind.Temperature));

            sAlterations.Add(new MotiveDecayAlteration<Sims3.Gameplay.Careers.Career>("kWorkaholicTraitMissingWorkStressPerHour", CommodityKind.Fun));
            sAlterations.Add(new MotiveDecayAlteration<Sims3.Gameplay.ActorSystems.BuffTooManyPeople>("kTooManyPeopleFunPenalty", CommodityKind.Fun));
            sAlterations.Add(new MotiveDecayAlteration<Sims3.Gameplay.ActorSystems.BuffCryingBaby>("kStressPerHour", CommodityKind.Fun));
            sAlterations.Add(new MotiveDecayAlteration<Sims3.Gameplay.ActorSystems.BuffPestilencePlague>("kExtraHygieneMotiveDecayPerHour", CommodityKind.Hygiene));
            sAlterations.Add(new MotiveDecayAlteration<Sims3.Gameplay.ActorSystems.BuffPestilencePlague>("kExtraEnergyMotiveDecayPerHour", CommodityKind.Energy));
            sAlterations.Add(new MotiveDecayAlteration<Sims3.Gameplay.ActorSystems.BuffTooMuchSun>("kTooMuchSunHungerPerHour", CommodityKind.VampireThirst));
            sAlterations.Add(new MotiveDecayAlteration<Motive>("kSwimmingInPoolFatigueAmount", CommodityKind.Fatigue));
            sAlterations.Add(new MotiveDecayAlteration<Sims3.Gameplay.ActorSystems.HorseManager>("kFoalAwayFromMotherSocialDecayPerHour", CommodityKind.Social));
            sAlterations.Add(new MotiveDecayAlteration<Sims3.Gameplay.TuningValues.TraitTuning>("kSocialButterflySocialDecayIncrease", CommodityKind.Social));

            sAlterations.Add(new MotiveAlteration<Sims3.Gameplay.ActorSystems.BuffGoingWild>("kFunMultiplier", CommodityKind.Fun));
            sAlterations.Add(new MotiveAlteration<Motive>("kRelaxingDestressAmount", CommodityKind.Fun));
            sAlterations.Add(new MotiveAlteration<Motive>("kSwimmingInPoolFunAmount", CommodityKind.Fun));
            sAlterations.Add(new MotiveAlteration<Sims3.Gameplay.TuningValues.TraitTuning>("kCarefreeFunGainMultiplier", CommodityKind.Fun));
            sAlterations.Add(new MotiveAlteration<Motive>("kHorseExerciseIncreaseWalking", CommodityKind.HorseExercise));
            sAlterations.Add(new MotiveAlteration<Motive>("kHorseExerciseIncreaseTrotting", CommodityKind.HorseExercise));
            sAlterations.Add(new MotiveAlteration<Motive>("kHorseExerciseIncreaseGalloping", CommodityKind.HorseExercise));

            foreach (IAlteration alteration in sAlterations)
            {
                alteration.Store();
            }

            Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>> found = new Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>>();

            foreach (InteractionTuning tuning in InteractionTuning.sAllTunings.Values)
            {
                sPreviousTuning.Add(new InteractionTuningChange(tuning));
            }

            AdjustExistingTuning(found);

            CloneNewTuning(found);

            try
            {
                LotManager.UpdateAllSimDesires();
            }
            catch (Exception)
            {
                UpdateAllSimDesires();

                //Common.Exception("TuningAlterations:Apply", e);
            }
        }

        public static void UpdateAllSimDesires()
        {
            foreach (Sim sim in LotManager.sActorList)
            {
                if ((sim != null) && (sim.Autonomy != null))
                {
                    UpdateMotivesWhenTuningsChange(sim.Autonomy);
                }
            }
        }

        public static void UpdateDesiresWhenTuningsChange(InteractionScorer ths)
        {
            ths.mActor.UpdateMotiveTunings();
            bool flag = false;
            foreach (Desire desire in ths.mDesires)
            {
                if ((desire.Commodity == CommodityKind.None) || (MotiveTuning.GetTuning(desire.Commodity) == null))
                {
                    flag = true;
                    break;
                }

                try
                {
                    desire.UpdateTuning(ths.mActor);
                }
                catch (Exception e)
                {
                    Common.Exception(ths.Actor, null, desire.Commodity.ToString(), e);
                }
            }

            ths.mUseValuesDirty = true;
            if (flag)
            {
                ths.mActor.Autonomy.Motives.RecreateMotives(ths.mActor);
            }
        }

        public static void UpdateMotivesWhenTuningsChange(Autonomy ths)
        {
            UpdateDesiresWhenTuningsChange(ths.InteractionScorer);
            ths.Motives.UpdateWhenTuningsChange();
        }

        protected static void AdjustExistingTuning(Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>> found)
        {
            Common.StringBuilder msg = new Common.StringBuilder("AdjustExistingTuning");

            foreach (KeyValuePair<int, List<MotiveTuning>> pair in MotiveTuning.sTuning)
            {
                CommodityKind kind = (CommodityKind)pair.Key;

                foreach (MotiveTuning tuning in pair.Value)
                {
                    OccultTypes occult = OccultTypes.None;

                    if (tuning.TraitSpecificity != null)
                    {
                        foreach (TraitNames trait in tuning.TraitSpecificity)
                        {
                            occult = OccultTypeHelper.OccultFromTrait(trait);
                            if (occult != OccultTypes.None)
                            {
                                break;
                            }
                        }
                    }

                    CASAgeGenderFlags ageSpecies = tuning.AgeSpeciesSpecificity;
                    if (ageSpecies == CASAgeGenderFlags.AgeMask)
                    {
                        ageSpecies = CASAgeGenderFlags.None;
                    }

                    MotiveKey key = new MotiveKey(ageSpecies, occult, kind);

                    key.SetMotiveFactor(found, 1f);

                    float decayFactor = Relativity.Settings.GetMotiveDecayFactor(key);
                    if (ApplyDecayFactor(tuning, decayFactor))
                    {
                        msg.Append(Common.NewLine + key + ":" + decayFactor);

                        sPreviousDecayFactor[tuning] = decayFactor;
                    }
                }
            }

            Common.DebugWriteLog(msg);
        }

        protected static void CloneNewTuning(Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>> found)
        {
            Common.StringBuilder msg = new Common.StringBuilder("CloneNewTuning");

            bool tuningAdded = false;

            foreach (CASAgeGenderFlags species in sSpecies)
            {
                CASAgeGenderFlags[] ages = null;
                OccultTypes[] occults = null;
                switch (species)
                {
                    case CASAgeGenderFlags.Human:
                        ages = sHumanAges;
                        occults = sHumanOccults;
                        break;
                    case CASAgeGenderFlags.Horse:
                        ages = sAnimalAges;
                        occults = sHorseOccults;
                        break;
                    default:
                        ages = sAnimalAges;
                        occults = sOtherOccults;
                        break;
                }

                foreach (CASAgeGenderFlags age in ages)
                {
                    foreach (OccultTypes occult in occults)
                    {
                        foreach (CommodityKind kind in sCommodities)
                        {
                            MotiveKey key = new MotiveKey(age | species, occult, kind);

                            float decayFactor = Relativity.Settings.GetMotiveDecayFactor(key);
                            if ((decayFactor == 1f) || (decayFactor == 0f)) continue;

                            // Check if Motive Tuning already exists
                            bool exactMatch = false;
                            if ((key.GetMotiveFactor(found, out exactMatch) != 0f) && (exactMatch)) continue;

                            List<MotiveTuning> tuning;
                            if (MotiveTuning.sTuning.TryGetValue((int)kind, out tuning))
                            {
                                MotiveTuning bestTuning = GetBestTuning(tuning, key);

                                MotiveTuning newTuning = CloneTuning(bestTuning);
                                if (newTuning != null)
                                {
                                    newTuning.AgeSpeciesSpecificity = key.mAgeSpecies;

                                    TraitNames trait = OccultTypeHelper.TraitFromOccult(key.mOccult);
                                    if (trait != TraitNames.Unknown)
                                    {
                                        newTuning.TraitSpecificity = new List<TraitNames>();
                                        newTuning.TraitSpecificity.Add(trait);
                                    }
                                    else
                                    {
                                        newTuning.TraitSpecificity = null;
                                    }

                                    float previousFactor;
                                    if (!sPreviousDecayFactor.TryGetValue(bestTuning, out previousFactor))
                                    {
                                        previousFactor = 1f;
                                    }

                                    if (ApplyDecayFactor(newTuning, decayFactor / previousFactor))
                                    {
                                        msg.Append(Common.NewLine + key + ":" + decayFactor);

                                        tuning.Add(newTuning);

                                        sPreviousDecayFactor[newTuning] = decayFactor;

                                        tuningAdded = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (tuningAdded)
            {
                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim.Autonomy == null) continue;

                    if (sim.Autonomy.Motives == null) continue;

                    sim.UpdateMotiveTunings();

                    sim.Autonomy.Motives.UpdateWhenTuningsChange();
                }
            }

            Common.DebugWriteLog(msg);
        }

        protected static MotiveTuning GetBestTuning(List<MotiveTuning> allTunings, MotiveKey key)
        {
            MotiveTuning tuning = null;
            float minValue = float.MinValue;

            foreach (MotiveTuning tuning2 in allTunings)
            {
                float num2 = SimEx.ScoreMotiveTuning(tuning2, key);
                if (num2 > minValue)
                {
                    minValue = num2;
                    tuning = tuning2;
                }
            }

            return tuning;
        }

        protected static MotiveTuning CloneTuning(MotiveTuning srcTuning)
        {
            if (srcTuning == null) return null;

            return new MotiveTuning(
                srcTuning.mCommodity,
                srcTuning.Universal,
                srcTuning.Insatiable,
                srcTuning.AgeSpeciesSpecificity,
                srcTuning.WorldRestrictionType,
                Common.CloneList(srcTuning.WorldRestrictionWorldTypes),
                Common.CloneList(srcTuning.WorldRestrictionWorldNames),
                Common.CloneList(srcTuning.TraitSpecificity),
                CloneDesireCurve(srcTuning.mCurve),
                srcTuning.mDecayType,
                srcTuning.mDecayValue,
                CloneCurve(srcTuning.mAutoSatisfyCurve),
                CloneCurve(srcTuning.mMotiveDecayCurve),
                CloneCurve(srcTuning.mMotiveMoodContributionCurve),
                srcTuning.mHasDefaultValue,
                srcTuning.mInitialMin,
                srcTuning.mInitialMax,
                srcTuning.mTimeRandomness,
                CloneBuffTriggers(srcTuning.mBuffTriggers),
                srcTuning.AddIfUniversalOnLoadFixUp
            );
        }

        protected static DesireCurve CloneDesireCurve(DesireCurve curve)
        {
            DesireCurve result = new DesireCurve(new Vector2[0]);

            result.mPoints = Common.CloneList(curve.mPoints).ToArray();
            result.mDYDX = Common.CloneList(curve.mDYDX).ToArray();

            return result;
        }

        protected static TYPE CloneCurve<TYPE>(TYPE curve)
            where TYPE : Curve, new()
        {
            TYPE result = new TYPE();

            result.m_loops = curve.m_loops;
            result.m_maxX = curve.m_maxX;
            result.m_maxY = curve.m_maxY;
            result.m_minX = curve.m_minX;
            result.m_minY = curve.m_minY;
            result.m_name = curve.m_name;
            result.m_points = Common.CloneList(curve.m_points);
            result.m_x = curve.m_x;
            result.m_xAfterWhichCurveIsFlat = curve.m_xAfterWhichCurveIsFlat;
            result.m_xAxisLabel = curve.m_xAxisLabel;
            result.m_yAxisLabel = curve.m_yAxisLabel;

            return result;
        }

        protected static List<MotiveTuning.MotiveBuffTrigger> CloneBuffTriggers(List<MotiveTuning.MotiveBuffTrigger> srcList)
        {
            List<MotiveTuning.MotiveBuffTrigger> result = new List<MotiveTuning.MotiveBuffTrigger>();

            foreach (MotiveTuning.MotiveBuffTrigger srcTrigger in srcList)
            {
                MotiveTuning.MotiveBuffTrigger trigger = new MotiveTuning.MotiveBuffTrigger();

                trigger.mAddBuff = srcTrigger.mAddBuff;
                trigger.mCustomClass = srcTrigger.mCustomClass;
                trigger.mDecay = srcTrigger.mDecay;
                trigger.mRemoveBuff = Common.CloneList(srcTrigger.mRemoveBuff);
                trigger.mTriggerValueEnd = srcTrigger.mTriggerValueEnd;
                trigger.mTriggerValueStart = srcTrigger.mTriggerValueStart;

                result.Add(trigger);
            }

            return result;
        }

        public static void Revert()
        {
            foreach (IAlteration alteration in sAlterations)
            {
                alteration.Revert();
            }

            sAlterations.Clear();

            foreach (KeyValuePair<int, List<MotiveTuning>> pair in MotiveTuning.sTuning)
            {
                foreach (MotiveTuning tuning in pair.Value)
                {
                    float decayFactor;
                    if (!sPreviousDecayFactor.TryGetValue(tuning, out decayFactor)) continue;

                    ApplyDecayFactor(tuning, 1f / decayFactor);
                }
            }

            sPreviousDecayFactor.Clear();

            foreach (InteractionTuningChange tuning in sPreviousTuning)
            {
                tuning.Dispose();
            }

            sPreviousTuning.Clear();
        }

        protected static void ApplyDecayCurve(Curve curve, float factor)
        {
            List<Vector2> newPoints = new List<Vector2>();
            foreach (Vector2 point in curve.m_points)
            {
                newPoints.Add(new Vector2(point.x, point.y * factor));
            }

            curve.m_points = newPoints;
            curve.m_minY *= factor;
            curve.m_maxY *= factor;
        }

        protected static bool ApplyDecayFactor(MotiveTuning tuning, float factor)
        {
            if ((factor == 1f) || (factor == 0f)) return false;

            // DecayValue is the divisor when calculating decay, so smaller values produce bigger decays
            tuning.mDecayValue /= factor;

            ApplyDecayCurve(tuning.mAutoSatisfyCurve, factor);
            ApplyDecayCurve(tuning.mMotiveDecayCurve, factor);

            foreach (MotiveTuning.MotiveBuffTrigger trigger in tuning.mBuffTriggers)
            {
                trigger.mDecay *= factor;
            }

            return true;
        }

        public class InteractionTuningChange : IDisposable
        {
            List<Pair<CommodityChange, float>> mPriorValues = new List<Pair<CommodityChange, float>>();

            public InteractionTuningChange(InteractionTuning tuning)
            {
                foreach (CommodityChange change in tuning.mTradeoff.mOutputs)
                {
                    if (CommodityTest.IsSkill(change.Commodity))
                    {
                        SkillNames skill;
                        if (SkillManager.SkillCommodityMap.TryGetValue(change.Commodity, out skill))
                        {
                            mPriorValues.Add(new Pair<CommodityChange, float>(change, change.mActualValue));

                            change.mActualValue *= Relativity.Settings.GetConstantSkillFactor(skill);
                        }
                    }
                }
            }

            public void Dispose()
            {
                foreach (Pair<CommodityChange, float> change in mPriorValues)
                {
                    change.First.mActualValue = change.Second;
                }
            }
        }
    }
}
