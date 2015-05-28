using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Tasks;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Settings;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class GeneticBlendScenario : SimScenario
    {
        SimDescription mPriorBaby;

        public GeneticBlendScenario(SimDescription sim, SimDescription priorBaby)
            : base(sim)
        {
            mPriorBaby = priorBaby;
        }
        protected GeneticBlendScenario(GeneticBlendScenario scenario)
            : base (scenario)
        {
            mPriorBaby = scenario.mPriorBaby;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "SkinBlend";
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!sim.Baby)
            {
                IncStat("Not Baby");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SimDescription mom = null;
            SimDescription dad = null;

            Relationships.GetParents(Sim, out mom, out dad);

            return PrivatePerform(mom, dad, mPriorBaby);
        }

        public bool Run(Manager manager, SimDescription a, SimDescription b)
        {
            if (Sim == null) return false;

            Manager = manager;

            return PrivatePerform(a, b, mPriorBaby);
        }

        protected void ApplyBlend(SimBuilder target, SimOutfit.BlendInfo blend)
        {
            float mutation = 0;
            if (RandomUtil.RandomChance(GetValue<BlendMutationOption, int>()))
            {
                Vector2 range = GetValue<BlendMutationTravelOption,Vector2>();

                mutation = RandomUtil.GetFloat(range.x, range.y);

                AddScoring("Mutation", (int)(mutation * 100));
            }

            target.SetFacialBlend(blend.key, blend.amount + mutation);
        }

        protected void InheritFacialBlends(SimBuilder target, List<SimDescription> parentSims, Random rnd)
        {
            if (!GetValue<AdvancedGeneticsOption, bool>()) return;

            if ((parentSims != null) && (parentSims.Count != 2))
            {
                List<SimOutfit> grandParents = new List<SimOutfit>();

                int grandParentChance = GetValue<GrandparentInheritanceOption, int>();
                if (grandParentChance > 0)
                {
                    foreach (SimDescription parent in parentSims)
                    {
                        foreach (SimDescription grandParent in Relationships.GetParents(parent))
                        {
                            SimOutfit outfit = grandParent.GetOutfit(OutfitCategories.Everyday, 0);
                            if (outfit != null)
                            {
                                grandParents.Add(outfit);
                            }
                        }
                    }
                }

                List<uint> grandParentList = new List<uint>();

                if (grandParents.Count > 0)
                {
                    foreach (uint num2 in Enum.GetValues(typeof(FacialBlend.FacialRegions)))
                    {
                        if (RandomUtil.RandomChance(grandParentChance))
                        {
                            SimOutfit outfit = RandomUtil.GetRandomObjectFromList(grandParents);

                            foreach (SimOutfit.BlendInfo grandBlend in outfit.Blends)
                            {
                                FacialBlend blend = new FacialBlend(grandBlend.key);

                                if (blend.GetRegionFlags() == num2)
                                {
                                    ApplyBlend(target, grandBlend);

                                    grandParentList.Add(num2);

                                    IncStat("Grandparent Blended");
                                }
                            }
                        }
                    }
                }

                SimOutfit parentOutfit1 = parentSims[0].GetOutfit(OutfitCategories.Everyday, 0);
                SimOutfit parentOutfit2 = parentSims[1].GetOutfit(OutfitCategories.Everyday, 0);
                if ((parentOutfit1 != null) && (parentOutfit2 != null))
                {
                    List<uint> list = new List<uint>();

                    foreach (uint num2 in Enum.GetValues(typeof(FacialBlend.FacialRegions)))
                    {
                        if (grandParentList.Contains(num2)) continue;

                        if (RandomUtil.CoinFlip())
                        {
                            list.Add(num2);
                        }
                    }

                    // Blend Parent 1
                    foreach (SimOutfit.BlendInfo parentBlend in parentOutfit1.Blends)
                    {
                        FacialBlend blend = new FacialBlend(parentBlend.key);

                        if (list.Contains(blend.GetRegionFlags()))
                        {
                            ApplyBlend(target, parentBlend);
                        }
                    }

                    // Blend Parent 2
                    foreach (SimOutfit.BlendInfo parentBlend in parentOutfit2.Blends)
                    {
                        FacialBlend blend = new FacialBlend(parentBlend.key);

                        if (!list.Contains(blend.GetRegionFlags()))
                        {
                            ApplyBlend(target, parentBlend);
                        }
                    }
                }
            }
        }

        protected static Vector3 GetWingColor(SimDescription sim, out bool success)
        {
            success = false;

            OccultFairy occultType = sim.OccultManager.GetOccultType(OccultTypes.Fairy) as OccultFairy;
            if (occultType != null)
            {
                CASFairyData fairyData = occultType.FairyData;
                if (fairyData != null)
                {
                    success = true;
                    return fairyData.WingColor;
                }
            }

            return new Vector3();
        }

        protected bool PrivatePerform(SimDescription a, SimDescription b, SimDescription priorBaby)
        {
            if ((priorBaby != null) && (RandomUtil.RandomChance(GetValue<ChanceOfIdenticalTwinOption, int>())))
            {
                FacialBlends.CopyGenetics(priorBaby, Sim, false, false);

                new SavedOutfit.Cache(Sim).PropagateGenetics(Sim, CASParts.sPrimary);

                IncStat("Identical");
                return true;
            }
            else
            {
                List<SimDescription> parents = new List<SimDescription>();

                List<WorldName> worlds = new List<WorldName>();
                if (a != null)
                {
                    SimDescription sim = Relationships.GetSim(a.Genealogy);
                    if (sim != null)
                    {
                        parents.Add(sim);

                        worlds.Add(a.HomeWorld);
                    }
                }

                if (b != null)
                {
                    SimDescription sim = Relationships.GetSim(b.Genealogy);
                    if (sim != null)
                    {
                        parents.Add(sim);

                        worlds.Add(b.HomeWorld);
                    }
                }

                if (GetValue<SkinOption, bool>())
                {
                    AlterSkinBlend(this, Sim, parents);
                }

                if (parents.Count == 2)
                {
                    List<SimDescription> allParents = new List<SimDescription>(parents);
                    foreach (SimDescription parent in parents)
                    {
                        foreach (SimDescription grandParent in Relationships.GetParents(parent))
                        {
                            allParents.Add(grandParent);
                        }
                    }

                    BlendWings(allParents);
                    
                    using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(Sim, CASParts.sPrimary))
                    {
                        builder.Builder.Species = CASAgeGenderFlags.Human;
                        builder.Builder.UseCompression = true;

                        if (!builder.OutfitValid)
                        {
                            IncStat("Skin Blend: Outfit Fail");
                            return false;
                        }
                        else
                        {
                            InheritFacialBlends(builder.Builder, parents, new Random());

                            builder.Builder.SkinToneIndex = Sim.SkinToneIndex;
                            builder.Builder.SkinTone = Sim.SkinToneKey;
                        }
                    }

                    new SavedOutfit.Cache(Sim).PropagateGenetics(Sim, CASParts.sPrimary);

                    IncStat("Blended");
                    return true;
                }
                else
                {
                    IncStat("Not Two");
                    return false;
                }
            }
        }

        protected void BlendWings(List<SimDescription> parents)
        {
            try
            {
                int wingRate = GetValue<BlendFairyWingsOption, int>();
                if (wingRate > 0)
                {
                    OccultFairy occultType = Sim.OccultManager.GetOccultType(OccultTypes.Fairy) as OccultFairy;
                    if (occultType != null)
                    {
                        CASFairyData fairyData = occultType.FairyData;
                        if (fairyData != null)
                        {
                            List<Vector3> parentColors = new List<Vector3>();
                            foreach (SimDescription parent in parents)
                            {
                                bool success;
                                Vector3 color = GetWingColor(parent, out success);
                                if (success)
                                {
                                    IncStat("Wing Color: " + color);

                                    parentColors.Add(color);
                                }
                            }

                            if (parentColors.Count >= 2)
                            {
                                RandomUtil.RandomizeListOfObjects(parentColors);

                                Vector3 ahsv = CompositorUtil.ColorShifter.ColorToHsv(parentColors[0]);
                                Vector3 bhsv = CompositorUtil.ColorShifter.ColorToHsv(parentColors[1]);

                                IncStat("Wing ChoiceA: " + ahsv);
                                IncStat("Wing ChoiceB: " + bhsv);

                                Vector3 resultHSV = new Vector3();
                                resultHSV.y = RandomUtil.CoinFlip() ? ahsv.y : bhsv.y;
                                resultHSV.z = RandomUtil.CoinFlip() ? ahsv.z : bhsv.z;

                                float difference = (ahsv.x - bhsv.x) * RandomUtil.GetFloat(wingRate / 100f);
                                if (difference == 0)
                                {
                                    difference = RandomUtil.GetFloat(GetValue<BlendFairyWingsMutateOption, int>() / 100f);
                                    difference = Math.Max(0,Math.Min(1f, difference));
                                }

                                AddStat("Wing Difference", difference);

                                float min = ahsv.x;
                                float max = bhsv.x;
                                if (ahsv.x > bhsv.x)
                                {
                                    min = bhsv.x;
                                    max = ahsv.x;
                                }

                                if (RandomUtil.CoinFlip())
                                {
                                    resultHSV.x = min + difference;
                                }
                                else
                                {
                                    resultHSV.x = max - difference;
                                }

                                IncStat("Wing Result: " + resultHSV);

                                fairyData.WingColor = CompositorUtil.ColorShifter.HsvToColor(resultHSV);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(Sim, e);
            }
        }

        protected static void AlterSkinBlend(Common.IStatGenerator stats, SimDescription sim, List<SimDescription> immediateParents)
        {
            List<SimDescription> choices = new List<SimDescription>(immediateParents);

            foreach (SimDescription parent in immediateParents)
            {
                if (parent.IsAlien)
                {
                    return;
                }

                foreach (SimDescription grand in Relationships.GetParents(parent))
                {
                    if (!grand.IsAlien)
                    {
                        choices.Add(grand);
                    }
                }
            }

            stats.AddStat("Blend Choices", choices.Count);

            if (choices.Count >= 2)
            {
                sim.SkinToneKey = FixInvisibleTask.ValidateSkinTone(RandomUtil.GetRandomObjectFromList(choices).SkinToneKey);

                RandomUtil.RandomizeListOfObjects(choices);

                float num4 = choices[0].SkinToneIndex;
                float num5 = choices[1].SkinToneIndex;
                if (num4 > num5)
                {
                    float num6 = num5;
                    num5 = num4;
                    num4 = num6;
                }
                float num8 = num5 - num4;

                float newIndex = 0;

                float num7 = RandomUtil.GetFloat(Sims3.Gameplay.CAS.Genetics.kSkinColorMaxPercentTravel / 100f);
                if (RandomUtil.CoinFlip())
                {
                    newIndex = num4 + (num8 * num7);
                }
                else
                {
                    newIndex = num5 - (num8 * num7);
                }

                sim.SkinToneIndex = newIndex;
            }
        }

        public override Scenario Clone()
        {
            return new GeneticBlendScenario(this);
        }

        public class BlendMutationOption : IntegerManagerOptionItem<ManagerPregnancy>
        {
            public BlendMutationOption()
                : base(5)
            { }

            public override string GetTitlePrefix()
            {
                return "BlendMutation";
            }

            public override int Value
            {
                get
                {
                    if (!ShouldDisplay()) return 0;

                    return base.Value;
                }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<AdvancedGeneticsOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class BlendMutationTravelOption : RangeManagerOptionItem<ManagerPregnancy>
        {
            public BlendMutationTravelOption()
                : base(new Vector2(-0.1f, 0.1f), new Vector2(-1,1))
            { }

            public override string GetTitlePrefix()
            {
                return "BlendMutationTravel";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<BlendMutationOption, int>() <= 0) return false;

                return base.ShouldDisplay();
            }
        }

        public class GrandparentInheritanceOption : IntegerManagerOptionItem<ManagerPregnancy>
        {
            public GrandparentInheritanceOption()
                : base(15)
            { }

            public override string GetTitlePrefix()
            {
                return "GrandparentInheritance";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<AdvancedGeneticsOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class AdvancedGeneticsOption : BooleanManagerOptionItem<ManagerPregnancy>
        {
            public AdvancedGeneticsOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AdvancedGenetics";
            }

            public override bool Value
            {
                get
                {
                    if (!ShouldDisplay()) return false;

                    return base.Value;
                }
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

        public class BlendFairyWingsOption : IntegerManagerOptionItem<ManagerPregnancy>
        {
            public BlendFairyWingsOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "BlendFairyWings";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP7);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class BlendFairyWingsMutateOption : IntegerManagerOptionItem<ManagerPregnancy>
        {
            public BlendFairyWingsMutateOption()
                : base(10)
            { }

            public override string GetTitlePrefix()
            {
                return "MutateFairyWings";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP7);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class Option : BooleanManagerOptionItem<ManagerPregnancy>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerPregnancy main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    BirthScenario.OnBirthScenario += OnRun;

                    SimFromBinEvents.OnGeneticSkinBlend += OnFromBin;

                    TestSim.OnPerform += OnTest;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "GeneticBlending";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            protected static void OnTest(Sim target)
            {
                GeneticBlendScenario scenario = new GeneticBlendScenario(target.SimDescription, null);
                scenario.Manager = StoryProgression.Main.Pregnancies;

                for (int i = 0; i < 100; i++)
                {
                    scenario.BlendWings(Relationships.GetParents(target.SimDescription));
                }
            }

            protected static void OnFromBin(Common.IStatGenerator stats, SimDescription sim, SimDescription mom, SimDescription dad, Manager manager)
            {
                if (manager.GetValue<Option, bool>())
                {
                    AlterSkinBlend(stats, sim, new List<SimDescription>(new SimDescription[] { mom, dad }));
                }
            }

            protected static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                BirthScenario s = scenario as BirthScenario;
                if (s == null) return;

                SimDescription priorBaby = null;

                foreach (SimDescription baby in s.Babies)
                {
                    scenario.Add(frame, new GeneticBlendScenario(baby, priorBaby), ScenarioResult.Start);
                    priorBaby = baby;
                }
            }
        }

        public class SkinOption : BooleanManagerOptionItem<ManagerPregnancy>
        {
            public SkinOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "GeneticSkinBlending";
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

        public class ChanceOfIdenticalTwinOption : IntegerManagerOptionItem<ManagerPregnancy>
        {
            public ChanceOfIdenticalTwinOption()
                : base(10)
            {}

            public override string GetTitlePrefix()
            {
                return "ChanceOfIdenticalTwin";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
