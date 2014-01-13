using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class InheritCASPartsScenario : AgeUpBaseScenario
    {
        CASAgeGenderFlags[] sAges = new CASAgeGenderFlags[] { CASAgeGenderFlags.Toddler, CASAgeGenderFlags.Child, CASAgeGenderFlags.Teen, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Adult };

        public InheritCASPartsScenario()
        { }
        protected InheritCASPartsScenario(InheritCASPartsScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "InheritCASParts";
        }

        protected override bool Allow()
        {
            if (GetValue<Option, int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return sim.IsHuman;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            List<CASParts.PartPreset> parts = new List<CASParts.PartPreset>();

            foreach (SimDescription parent in Relationships.GetParents(Sim))
            {
                SimOutfit outfit = CASParts.GetOutfit(parent, new CASParts.Key(OutfitCategories.Everyday, 0), false);

                foreach (CASPart part in outfit.Parts)
                {
                    if (CASParts.GeneticBodyTypes.Contains(part.BodyType))
                    {
                        CASAgeGenderFlags ages = part.Age;

                        if ((ages & Sim.Age) == CASAgeGenderFlags.None) continue;

                        IncStat("Found: " + part.BodyType);

                        bool found = false;
                        foreach (CASAgeGenderFlags priorAge in sAges)
                        {
                            if (priorAge >= Sim.Age) break;

                            if ((ages & priorAge) == priorAge)
                            {
                                found = true;
                                break;
                            }
                        }

                        // This part would have been inherited in an earlier age-up
                        if (found) continue;

                        parts.Add(new CASParts.PartPreset(part, outfit));
                    }
                }
            }

            AddStat("Choices", parts.Count);

            bool adjusted = false;
            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(Sim, new CASParts.Key(OutfitCategories.Everyday, 0)))
            {
                foreach (CASParts.PartPreset part in parts)
                {
                    if (RandomUtil.RandomChance(GetValue<Option, int>()))
                    {
                        builder.ApplyPartPreset(part);
                        adjusted = true;

                        IncStat("Inherited: " + part.mPart.BodyType);
                    }
                }
            }

            if (adjusted)
            {
                SavedOutfit.Cache cache = new SavedOutfit.Cache(Sim);
                cache.PropagateGenetics(Sim, new CASParts.Key(OutfitCategories.Everyday, 0));
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new InheritCASPartsScenario(this);
        }

        public class Option : IntegerEventOptionItem<ManagerPregnancy, InheritCASPartsScenario>
        {
            public Option()
                : base(25)
            { }

            public override string GetTitlePrefix()
            {
                return "InheritCASParts";
            }
        }
    }
}
