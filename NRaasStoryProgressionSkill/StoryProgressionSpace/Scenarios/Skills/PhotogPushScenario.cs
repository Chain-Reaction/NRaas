using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Scenarios.Money;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class PhotogPushScenario : SimSingleProcessScenario, IHasSkill
    {
        static List<PhotographSize> sSizes = new List<PhotographSize>();

        static List<PaintingStyle> sStyles = new List<PaintingStyle>();

        int mNet = 0;

        static PhotogPushScenario()
        {
            foreach (PhotographSize size in Enum.GetValues(typeof(PhotographSize)))
            {
                if (size == PhotographSize.NumSizes) continue;

                sSizes.Add(size);
            }

            foreach (PaintingStyle style in Enum.GetValues(typeof(PaintingStyle)))
            {
                if (style == PaintingStyle.NumStyles) continue;

                sStyles.Add(style);
            }
        }
        public PhotogPushScenario()
        { }
        protected PhotogPushScenario(PhotogPushScenario scenario)
            : base (scenario)
        {
            mNet = scenario.mNet;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "PhotogPush";
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Photography };
            }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (GetValue<Option, int>() <= 0) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return new SimScoringList(this, "Photography", Sims.All, false).GetBestByMinScore(1);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("No Home");
                return false;
            }
            else if (sim.SkillManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (!Skills.Allow(this, sim))
            {
                IncStat("Skill Denied");
                return false;
            }
            else if (!Money.Allow(this, sim))
            {
                IncStat("Money Denied");
                return false;
            }
            else if ((sim.IsEP11Bot) && (!sim.HasTrait(TraitNames.ArtisticAlgorithmsChip)))
            {
                IncStat("Chip Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected static void UpdateRecords(Photography skill, Sims3.Gameplay.Objects.HobbiesSkills.Camera camera, SubjectDefinition subject, PaintingStyle style, PhotographSize size, out bool newSubject, out bool firstShotTodayOfSubject, out bool collectionComplete)
        {
            newSubject = false;
            firstShotTodayOfSubject = false;
            collectionComplete = false;

            Photography.SubjectRecord record;

            string str = subject.Key;
            string str2 = subject.Collection.Key;

            if (str != "Nothing")
            {
                if (!skill.mSubjectRecords.TryGetValue(str, out record))
                {
                    record = new Photography.SubjectRecord();
                    record.SetUIState(Photography.UIState.NewlyDiscovered);
                    skill.OnNewSubjectCaptured(skill.SkillOwner.CreatedSim, subject, camera);
                    skill.mSubjectRecords[str] = record;
                }
                firstShotTodayOfSubject = Photography.WouldBeNewToday(record);
                Photography.UpdateRecord(record, null);
                newSubject = record.mCaptureCount == 0x1;
                if (!skill.mCollectionRecords.TryGetValue(str2, out record))
                {
                    record = new Photography.SubjectRecord();
                    skill.mCollectionRecords[str2] = record;
                }
                if (record.mCaptureCount == 0x0)
                {
                    record.SetUIState(Photography.UIState.NewlyDiscovered);
                    skill.OnNewCollectionDiscovered(skill.SkillOwner.CreatedSim, subject.Collection, subject, camera);
                }
                Photography.UpdateRecord(record, null);

                collectionComplete = skill.HasCompletedCollection(str2);
                if (newSubject && collectionComplete)
                {
                    record.SetUIState(Photography.UIState.NewlyCompleted);
                    skill.mCollectionsCompleted++;
                }
            }
            if (!skill.mStyleRecords.TryGetValue(style, out record))
            {
                record = new Photography.SubjectRecord();
                skill.mStyleRecords[style] = record;
            }
            Photography.UpdateRecord(record, null);
            if (!skill.mSizeRecords.TryGetValue(size, out record))
            {
                record = new Photography.SubjectRecord();
                skill.mSizeRecords[size] = record;
            }
            Photography.UpdateRecord(record, null);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            mNet = 0;

            Camera camera = ManagedBuyProduct<Camera>.Purchase(Sim, 0, this, UnlocalizedName, null, BuildBuyProduct.eBuyCategory.kBuyCategoryElectronics, BuildBuyProduct.eBuySubCategory.kBuySubCategoryHobbiesAndSkills);
            if (camera == null)
            {
                return false;
            }

            Photography skill = Sim.SkillManager.GetSkill<Photography>(SkillNames.Photography);
            if (skill == null)
            {
                skill = Sim.SkillManager.AddElement(SkillNames.Photography) as Photography;
            }

            List<PhotographSize> sizes = new List<PhotographSize>();
            foreach (PhotographSize size in sSizes)
            {
                if (Photography.SizeUnlocked(skill, size))
                {
                    sizes.Add(size);
                }
            }

            if (sizes.Count == 0)
            {
                IncStat("No Sizes");
                return false;
            }

            List<PaintingStyle> styles = new List<PaintingStyle>();
            foreach (PaintingStyle style in sStyles)
            {
                if (Photography.StyleUnlocked(skill, style))
                {
                    styles.Add(style);
                }
            }

            if (styles.Count == 0)
            {
                IncStat("No Styles");
                return false;
            }

            List<SubjectDefinition> subjects = new List<SubjectDefinition>();
            foreach (CollectionDefinition collection in PhotographySubjects.sCollectionDefinitions.Values)
            {
                if (!collection.AvailableForSkill(skill)) continue;

                if (collection.Location != WorldName.UserCreated) continue;

                foreach (SubjectDefinition definition in collection.Subjects)
                {
                    try
                    {
                        if (!definition.AvailableForSkill(skill)) continue;
                    }
                    catch
                    {
                        continue;
                    }

                    subjects.Add(definition);
                }
            }

            if (subjects.Count == 0)
            {
                IncStat("No Subjects");
                return false;

            }

            int additional = AddScoring("AdditionalPhoto", Sim);

            int total = Sims3.Gameplay.Core.RandomUtil.GetInt(GetValue<Option, int>() + additional);
            if (total <= 0)
            {
                total = 1;
            }

            int totalFunds = 0;

            for (int i = 0; i < total; i++)
            {
                PhotographSize size = Sims3.Gameplay.Core.RandomUtil.GetRandomObjectFromList(sizes);
                PaintingStyle style = Sims3.Gameplay.Core.RandomUtil.GetRandomObjectFromList(styles);

                int cost = Photography.GetCostToTakePhoto(skill, camera, size, style);
                if (Sim.FamilyFunds < cost) continue;

                float x = 0f;
                float num3 = 1f;
                float num5 = 0f;
                float num6 = 1f;

                SubjectDefinition subject = Sims3.Gameplay.Core.RandomUtil.GetRandomObjectFromList(subjects);

                skill.ApplyMultiplier(ref num6, camera.CameraTuning.ValueMultiplier, "Value", "CameraTuning");

                bool newSubject, firstShotTodayOfSubject, collectionComplete;
                UpdateRecords(skill, camera, subject, style, size, out newSubject, out firstShotTodayOfSubject, out collectionComplete);

                if (newSubject)
                {
                    skill.ApplyBonus(ref x, (float)subject.SkillPoints, "Skill Gain", "Subject Tuning (New Subject)");
                    skill.ApplyBonus(ref num5, (float)subject.Value, "Value", "Subject Tuning (New Subject)");
                }
                else if (firstShotTodayOfSubject)
                {
                    skill.ApplyBonus(ref x, subject.SkillPoints * Photography.kFirstShotOfTheDaySkillGainMultiplier, "Skill Gain", "Subject Tuning (First Time Today)");
                    skill.ApplyBonus(ref num5, subject.Value * Photography.kFirstShotOfTheDayValueMultiplier, "Value", "Subject Tuning (First Time Today)");
                }

                if (Sims3.Gameplay.Core.RandomUtil.RandomChance((skill.MaxSkillLevel - (skill.SkillLevel - 1)) * 10))
                {
                    skill.ApplyMultiplier(ref num6, Photography.kBadPhotoValueMultiplier, "Value", "Bad Photo (kBadPhotoValueMultiplier)");
                }

                skill.ApplyMultiplier(ref num6, Photography.sValueMultipliers[skill.SkillLevel], "Value", "Sim's Skill Level");
                skill.ApplyMultiplier(ref num6, Photography.sSizeData[size].ValueMultiplier, "Value", "Photo Size");
                skill.ApplyMultiplier(ref num6, Photography.sStyleData[style].ValueMultiplier, "Value", "Photo Style");

                if (collectionComplete)
                {
                    skill.ApplyMultiplier(ref num6, subject.Collection.ValueMultiplierWhenComplete, "Value", "Collection Tuning (Complete)");
                }
                else
                {
                    skill.ApplyMultiplier(ref num6, subject.Collection.ValueMultiplier, "Value", "Collection Tuning");
                }

                if (Sim.TraitManager.HasElement(TraitNames.PhotographersEye))
                {
                    skill.ApplyMultiplier(ref num6, TraitTuning.PhotographersEyePhotographValueMultiplier, "Value", "Photographer's Eye (TraitTuning)");
                }

                skill.ApplyMultiplier(ref num3, camera.CameraTuning.SkillGainMultiplier, "Skill Gain", "CameraTuning");

                int funds = (int)((Photography.kBaseValuePerPhoto + num5) * num6);

                if ((funds < cost) && (additional > 0))
                {
                    IncStat("Try Again");
                    total++;
                    additional--;
                }

                skill.EarnedMoneyFromPhotography((uint)funds);
                skill.UpdateXpForEarningMoney(funds);

                totalFunds += funds;

                AddStat("Subject: " + subject.Name, funds);

                AddStat("Funds", funds);

                Money.AdjustFunds(Sim, "Photography", -cost);

                AddStat("Cost", cost);

                skill.SpentMoneyOnPhotography((uint)cost);

                AddScoring("Net", funds - cost);

                mNet += (funds - cost);

                float points = (Photography.kBaseSkillGainPerPhoto + x) * num3;

                AddStat("Skill", points);

                skill.AddPoints(points);
                skill.CheckForCompletedOpportunities();
            }

            Money.AdjustFunds(Sim, "Photography", totalFunds);

            return (mNet > 0);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, mNet };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        protected override bool Push()
        {
            Situations.PushVisit(this, Sim, Lots.GetCommunityLot(Sim.CreatedSim, null, false));
            return true;
        }

        public override Scenario Clone()
        {
            return new PhotogPushScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerSkill, PhotogPushScenario>
        {
            public Option()
                : base(2)
            { }

            public override string GetTitlePrefix()
            {
                return "PhotogPush";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP1);
            }

            public override int Value
            {
                get
                {
                    if (!ShouldDisplay()) return 0;

                    return base.Value;
                }
            }
        }
    }
}
