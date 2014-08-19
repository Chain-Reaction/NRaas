using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using NRaas.StoryProgressionSpace.Booters;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options.Immigration;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using Sims3.Gameplay;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.StoryProgression;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class SimFromBinEvents
    {
        public delegate void SimFromBinUpdate(Common.IStatGenerator stats, SimDescription sim, SimDescription mom, SimDescription dad, Manager manager);

        public static SimFromBinUpdate OnGeneticSkinBlend;
        public static SimFromBinUpdate OnSimFromBinUpdate;
    }

    public abstract class SimFromBinController
    {
        Manager mManager;

        public SimFromBinController(Manager manager)
        {
            mManager = manager;
        }

        public Manager Manager
        {
            get { return mManager; }
        }

        public abstract bool ShouldDisplayImmigrantOptions();
    }

    public class SimFromBin<TManager> : Manager.DisposableManagerToggle<TManager,bool>, ISimFromBin
        where TManager : Manager, ISimFromBinManager
    {
        Common.IStatGenerator mStats;

        Dictionary<ExportBinContents, bool> mLoaded = new Dictionary<ExportBinContents, bool>();

        List<SimDescription> mDispose = new List<SimDescription>();

        List<string> mDisallowed = null;

        int mRandomChance;
        int mBinChance;

        public SimFromBin(Common.IStatGenerator stats, TManager manager)
            : base(manager, false)
        {
            mStats = stats;

            mDisallowed = Manager.GetValue<DisallowHouseholdOption<TManager>,List<string>>();

            mRandomChance = Manager.GetValue<SimFromRandomChanceOption<TManager>, int>();
            mBinChance = Manager.GetValue<SimFromBinChanceOption<TManager>, int>();

            BinModel.Singleton.PopulateExportBin();
        }

        public static void Install(SimFromBinController controller, TManager manager, bool initial)
        {
            List<ISimFromBinOption<TManager>> options = new List<ISimFromBinOption<TManager>>();

            options.Add(new AllowAlienHouseholdOption<TManager>());
            options.Add(new BustRangeOption<TManager>());
            options.Add(new ChanceOfHybridOption<TManager>());
            options.Add(new ChanceOfOccultOption<TManager>());
            options.Add(new CustomNamesOnlyOption<TManager>());
            options.Add(new DisallowHouseholdOption<TManager>());
            options.Add(new EqualBinChanceOption<TManager>());
            options.Add(new FatRangeOption<TManager>());
            options.Add(new FitRangeOption<TManager>());
            options.Add(new ImmigrantOccultOption<TManager>());
            options.Add(new ImmigrantWorldOption<TManager>());
            options.Add(new MaxCelebrityLevelOption<TManager>());
            options.Add(new MaximumBinHouseSizeOption<TManager>());
            options.Add(new MuscleRangeOption<TManager>());
            options.Add(new MutationSetRangeOption<TManager>());
            options.Add(new MutationUnsetRangeOption<TManager>());
            options.Add(new NameTakeOption<TManager>());
            options.Add(new SimFromBinChanceOption<TManager>());
            options.Add(new SimFromRandomChanceOption<TManager>());
            options.Add(new SkinToneRangeOption<TManager>());
            options.Add(new MaximumOccultOption<TManager>());

            foreach (ISimFromBinOption<TManager> option in options)
            {
                Installer<TManager>.Install(manager, option, initial);
                option.Controller = controller;
            }
        }

        public override void Dispose ()
        {
            foreach (ExportBinContents content in mLoaded.Keys)
            {
                try
                {
                    if (content.Household != null)
                    {
                        foreach (SimDescription sim in new List<SimDescription> (content.Household.AllSimDescriptions))
                        {
                            Manager.Deaths.CleansingKill(sim, true);
                        }

                        mStats.IncStat("SimFromBin Household Disposed");

                        content.Household.Destroy();
                        content.Household.Dispose();
                    }

                    if (content.mHouseholdContents != null)
                    {
                        ExportBinContentsEx.Flush(content);
                    }
                }
                catch (Exception e)
                {
                    Common.DebugException(content.HouseholdName, e);
                }
            }

            foreach (SimDescription sim in mDispose)
            {
                sim.Dispose();
            }

            mStats.IncStat("SimFromBin Clear");

            BinModel.Singleton.ClearExportBin();            
        }

        protected bool ValidContent(ExportBinContents content)
        {
            if (mDisallowed.Contains(content.PackageName)) return false;

            if ((content.HouseholdSims == null) || (content.HouseholdSims.Count == 0))
            {
                return false;
            }

            if (content.HouseholdSims.Count > Manager.GetValue<MaximumBinHouseSizeOption<TManager>, int>())
            {
                return false;
            }

            return true;
        }
        protected bool ValidContent(UISimInfo sim, CASAgeGenderFlags species, SimDescription exclude)
        {
            if (sim == null) return false;

            if (sim.SimName == null) return false;

            if (sim.Species != species) return false;

            if (exclude != null)
            {
                if (sim.SimName.Contains(exclude.LastName)) return false;
            }

            return true;
        }

        protected SimDescription GetRandomSim(CASAgeGenderFlags gender, CASAgeGenderFlags species)
        {
            SimDescription result = null;

            if (species == CASAgeGenderFlags.Human)
            {
                SimUtils.SimCreationSpec sim = new SimUtils.SimCreationSpec();

                Vector2 fatRange = Manager.GetValue<FatRangeOption<TManager>, Vector2>();
                Vector2 fitRange = Manager.GetValue<FitRangeOption<TManager>, Vector2>();

                WorldName worldName = GetGeneticWorld();

                sim.Gender = gender;
                sim.Age = CASAgeGenderFlags.Adult;
                sim.Species = species;
                sim.Weight = RandomUtil.GetFloat(fatRange.x, fatRange.y);
                sim.Fitness = RandomUtil.GetFloat(fitRange.x, fitRange.y);
                sim.Description = "";
                sim.GivenName = SimUtils.GetRandomFamilyName(worldName);

                result = sim.Instantiate(worldName, uint.MaxValue);

                result.VoiceVariation = (VoiceVariationType)RandomUtil.GetInt(0, 2);
                result.VoicePitchModifier = RandomUtil.GetFloat(0, 1f);

                Vector2 skintToneRange = Manager.GetValue<SkinToneRangeOption<TManager>, Vector2>();

                result.SkinToneIndex = RandomUtil.GetFloat(skintToneRange.x, skintToneRange.y);
            }
            else
            {
                result = GeneticsPet.MakeRandomPet(CASAgeGenderFlags.Adult, gender, species, 1f);
            }

            SetBustMuscleSliders(result);

            FacialBlends.RandomizeBlends(mStats.AddStat, result, Manager.GetValue<MutationSetRangeOption<TManager>, Vector2>(), true, Manager.GetValue<MutationSetRangeOption<TManager>, Vector2>(), false, Manager.GetValue<AllowAlienHouseholdOption<TManager>, bool>());

            mDispose.Add(result);

            mStats.IncStat("Immigrant: Random Sim");

            return result;
        }

        protected SimDescription GetSim(CASAgeGenderFlags gender, CASAgeGenderFlags species, SimDescription exclude)
        {
            if (RandomUtil.RandomChance (mBinChance))
            {
                List<ExportBinContents> contents = new List<ExportBinContents>();
                foreach (IExportBinContents iContent in BinModel.Singleton.ExportBinContents)
                {
                    ExportBinContents content = iContent as ExportBinContents;
                    if (content == null) continue;

                    contents.Add(content);
                }

                Dictionary<UISimInfo, ExportBinContents> uiSims = new Dictionary<UISimInfo, ExportBinContents>();

                if (Manager.GetValue<EqualBinChanceOption<TManager>, bool>())
                {
                    foreach (ExportBinContents content in contents)
                    {
                        if (!ValidContent(content)) continue;

                        foreach (UISimInfo sim in content.HouseholdSims)
                        {
                            if (!ValidContent(sim, species, exclude)) continue;

                            if (uiSims.ContainsKey(sim)) continue;

                            uiSims.Add(sim, content);
                        }
                    }
                }
                else
                {
                    RandomUtil.RandomizeListOfObjects(contents);
                    foreach(ExportBinContents content in contents)
                    {
                        if (!ValidContent (content)) continue;

                        foreach (UISimInfo sim in content.HouseholdSims)
                        {
                            if (!ValidContent(sim, species, exclude)) continue;

                            uiSims.Add(sim, content);
                        }

                        if (uiSims.Count > 0)
                        {
                            break;
                        }
                    }
                }

                if (uiSims.Count > 0)
                {
                    UISimInfo choice = RandomUtil.GetRandomObjectFromList(new List<UISimInfo>(uiSims.Keys));

                    ExportBinContents content = uiSims[choice];

                    if (!content.IsLoaded())
                    {
                        Dictionary<string, List<News.NewsTuning.ArticleTuning>> namedArticles = News.sNewsTuning.mNamedArticles;

                        bool fail = false;
                        try
                        {
                            // Doing so stops Marriage notices of imported sims from appearing in the newspaper
                            News.sNewsTuning.mNamedArticles = new Dictionary<string, List<News.NewsTuning.ArticleTuning>>();

                            ExportBinContentsEx.Import(content, false);

                            mStats.AddStat("Import Size", content.HouseholdSims.Count);
                        }
                        catch (Exception e)
                        {
                            Common.DebugException(content.HouseholdName, e);
                            fail = true;
                        }
                        finally
                        {
                            News.sNewsTuning.mNamedArticles = namedArticles;
                        }

                        mLoaded[content] = true;

                        if (fail) return null;
                    }

                    if ((content.Household != null) && (content.Household.AllSimDescriptions.Count > 0))
                    {
                        List<SimDescription> choices = new List<SimDescription>();

                        foreach (SimDescription sim in Households.All(content.Household))
                        {
                            if (SimTypes.IsSkinJob(sim)) continue;

                            if (SimTypes.IsOccult(sim, OccultTypes.ImaginaryFriend)) continue;

                            mStats.IncStat("Immigrant: Bin Sim");

                            if (sim.Species == choice.Species)
                            {
                                choices.Add(sim);

                                if (sim.FullName == choice.SimName)
                                {
                                    return sim;
                                }
                            }
                        }

                        if (choices.Count > 0)
                        {
                            return RandomUtil.GetRandomObjectFromList(choices);
                        }
                    }
                }
            }

            if ((RandomUtil.RandomChance(mRandomChance)) || (Manager.Sims.All.Count <= 20))
            {
                return GetRandomSim(gender, species);
            }

            List<SimDescription> sims = new List<SimDescription>();

            bool allowAlien = Manager.GetValue<AllowAlienHouseholdOption<TManager>,bool>();

            foreach (SimDescription sim in Manager.Sims.All)
            {
                if (SimTypes.InServicePool(sim, ServiceType.GrimReaper)) continue;

                if (sim.Species != species) continue;

                if (!allowAlien)
                {
                    if (SimTypes.IsServiceAlien(sim))
                    {
                        continue;
                    }
                }

                if (exclude != null)
                {
                    if (exclude == sim) continue;

                    if (exclude.LastName == sim.LastName) continue;
                }

                sims.Add(sim);
            }

            if (sims.Count == 0)
            {
                return null;
            }
            else
            {
                mStats.IncStat("Immigrant: Town Sim");

                return RandomUtil.GetRandomObjectFromList(sims);
            }
        }

        public SimDescription CreateNewSim(CASAgeGenderFlags age, CASAgeGenderFlags gender, CASAgeGenderFlags species)
        {
            SimDescription dad = GetSim(CASAgeGenderFlags.Male, species, null);
            if (dad == null)
            {
                dad = GetSim(CASAgeGenderFlags.Female, species, null);
                if (dad == null)
                {
                    return null;
                }
            }

            SimDescription mom = GetSim(CASAgeGenderFlags.Female, species, dad);
            if (mom == null)
            {
                mom = GetSim(CASAgeGenderFlags.Male, species, dad);
                if (mom == null)
                {
                    return null;
                }
            }

            return CreateNewSim(mom, dad, age, gender, species, false);
        }

        protected void SetBustMuscleSliders(SimDescription newSim)
        {
            bool changed = false;

            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(newSim, CASParts.sPrimary))
            {
                if (!builder.OutfitValid) return;

                if (newSim.IsFemale)
                {
                    Vector2 bustRange = Manager.GetValue<BustRangeOption<TManager>, Vector2>();
                    if (bustRange.x != bustRange.y)
                    {
                        float value = RandomUtil.GetFloat(bustRange.x, bustRange.y);

                        NormalMap.ApplyBustValue(builder.Builder, value);
                        changed = true;

                        mStats.AddScoring("Bust 100s", (int)(value * 100));
                    }
                }
                else
                {
                    Vector2 muscleRange = Manager.GetValue<MuscleRangeOption<TManager>, Vector2>();
                    if (muscleRange.x != muscleRange.y)
                    {
                        float value = RandomUtil.GetFloat(muscleRange.x, muscleRange.y);

                        NormalMap.ApplyMuscleValue(builder.Builder, value);
                        changed = true;

                        mStats.AddScoring("Muscle 100s", (int)(value * 100));
                    }
                }
            }

            if (changed)
            {
                new SavedOutfit.Cache(newSim).PropagateGenetics(newSim, CASParts.sPrimary);
            }
        }

        protected WorldName GetGeneticWorld()
        {
            WorldName worldName = WorldName.UserCreated;

            List<WorldName> worldNames = Manager.GetValue<ImmigrantWorldOption<TManager>, List<WorldName>>();
            if (worldNames.Count > 0)
            {
                worldName = RandomUtil.GetRandomObjectFromList(worldNames);
            }

            if (worldName == WorldName.UserCreated)
            {
                worldName = GameUtils.GetCurrentWorld();
            }

            return worldName;
        }

        public SimDescription CreateNewSim(SimDescription mom, SimDescription dad, CASAgeGenderFlags ages, CASAgeGenderFlags genders, CASAgeGenderFlags species, bool updateGenealogy)
        {
            if (dad == null)
            {
                dad = GetSim(CASAgeGenderFlags.Male, species, mom);
                if (dad == null)
                {
                    dad = GetSim(CASAgeGenderFlags.Female, species, mom);
                    if (dad == null)
                    {
                        return null;
                    }
                }
            }

            CASAgeGenderFlags gender = (CASAgeGenderFlags)RandomUtil.SelectOneRandomBit((uint)(genders));
            CASAgeGenderFlags age = (CASAgeGenderFlags)RandomUtil.SelectOneRandomBit((uint)(ages));

            if (dad.CelebrityManager == null)
            {
                dad.Fixup();
            }

            if (mom.CelebrityManager == null)
            {
                mom.Fixup();
            }

            SimDescription newSim = null;

            try
            {
                if (mom.Species == CASAgeGenderFlags.Human)
                {
                    newSim = Genetics.MakeDescendant(dad, mom, age, gender, 100, new Random(), false, updateGenealogy, true, GetGeneticWorld(), false);

                    newSim.HomeWorld = GameUtils.GetCurrentWorld();
                }
                else
                {
                    newSim = GeneticsPet.MakePetDescendant(dad, mom, age, gender, mom.Species, new Random(), updateGenealogy, GeneticsPet.SetName.SetNameNonInteractive, 0, OccultTypes.None);
                }
            }
            catch (Exception e)
            {
                Common.Exception(dad, mom, e);
            }

            if (newSim == null)
            {
                return null;
            }

            if (!updateGenealogy)
            {
                FacialBlends.RandomizeBlends(mStats.AddStat, newSim, new Vector2(0f, 0f), true, Manager.GetValue<MutationUnsetRangeOption<TManager>, Vector2>(), true, Manager.GetValue<AllowAlienHouseholdOption<TManager>, bool>());
            }

            List<OccultTypes> occults = new List<OccultTypes>();

            occults.AddRange(OccultTypeHelper.CreateList(mom.OccultManager.CurrentOccultTypes, true));
            occults.AddRange(OccultTypeHelper.CreateList(dad.OccultManager.CurrentOccultTypes, true));

            if (updateGenealogy)
            {
                Manager.Sims.ApplyOccultChance(Manager, newSim, occults, Manager.GetValue<ChanceOfHybridOption<TManager>, int>(), Manager.GetValue<MaximumOccultOption<TManager>,int>());
            }
            else if (species == CASAgeGenderFlags.Human)
            {
                int maxCelebrityLevel = Manager.GetValue<MaxCelebrityLevelOption<TManager>, int>();

                if (maxCelebrityLevel >= 0)
                {
                    newSim.CelebrityManager.mOwner = newSim;

                    Skill scienceSkill = SkillManager.GetStaticSkill(SkillNames.Science);
                    if (scienceSkill != null)
                    {
                        scienceSkill.mNonPersistableData.SkillCategory |= SkillCategory.Hidden;
                    }

                    try
                    {
                        newSim.CelebrityManager.ForceSetLevel((uint)RandomUtil.GetInt(maxCelebrityLevel));
                    }
                    finally
                    {
                        if (scienceSkill != null)
                        {
                            scienceSkill.mNonPersistableData.SkillCategory &= ~SkillCategory.Hidden;
                        }
                    }
                }

                occults.AddRange(Manager.GetValue<ImmigrantOccultOption<TManager>, List<OccultTypes>>());
                if (occults.Count > 0)
                {
                    if (RandomUtil.RandomChance(Manager.GetValue<ChanceOfOccultOption<TManager>, int>()))
                    {
                        OccultTypeHelper.Add(newSim, RandomUtil.GetRandomObjectFromList(occults), false, false);
                    }
                }
            }

            Manager.Sims.ApplyOccultChance(Manager, newSim, occults, Manager.GetValue<ChanceOfHybridOption<TManager>, int>(), Manager.GetValue<MaximumOccultOption<TManager>, int>());

            OccultTypeHelper.ValidateOccult(newSim, null);

            OccultTypeHelper.TestAndRebuildWerewolfOutfit(newSim);

            newSim.FirstName = Manager.Sims.EnsureUniqueName(newSim);

            List<Trait> traits = new List<Trait>(newSim.TraitManager.List);
            foreach (Trait trait in traits)
            {
                if (trait.IsHidden)
                {
                    newSim.TraitManager.RemoveElement(trait.Guid);
                }
            }

            List<Trait> choices = AgingManager.GetValidTraits(newSim, true, true, true);
            if (choices.Count > 0)
            {
                while (!newSim.TraitManager.TraitsMaxed())
                {
                    Trait choice = RandomUtil.GetRandomObjectFromList<Trait>(choices);
                    if (!newSim.TraitManager.AddElement(choice.Guid))
                    {
                        break;
                    }
                }
            }

            if (SimFromBinEvents.OnGeneticSkinBlend != null)
            {
                SimFromBinEvents.OnGeneticSkinBlend(mStats, newSim, mom, dad, Manager);
            }

            FixInvisibleTask.Perform(newSim, true);

            if (SimFromBinEvents.OnSimFromBinUpdate != null)
            {
                SimFromBinEvents.OnSimFromBinUpdate(mStats, newSim, mom, dad, Manager);
            }

            INameTakeOption nameTake = Manager.GetOption<NameTakeOption<TManager>>();

            if ((!updateGenealogy) && (nameTake != null))
            {
                bool wasEither;
                newSim.LastName = Manager.Sims.HandleName(nameTake, mom, dad, out wasEither);
            }
            else if (mom != null)
            {
                newSim.LastName = mom.LastName;
            }
            else if (dad != null)
            {
                newSim.LastName = dad.LastName;
            }

            if (!updateGenealogy && Manager.GetValue<CustomNamesOnlyOption<TManager>, bool>())
            {
                newSim.LastName = LastNameListBooter.GetRandomName(!Manager.GetValue<CustomNamesOnlyOption<TManager>, bool>(), newSim.Species, newSim.IsFemale);
            }

            return newSim;
        }
    }
}

