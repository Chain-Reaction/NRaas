using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class Species : SelectionTestableOptionList<Species.Item, CASAgeGenderFlags, CASAgeGenderFlags>, IDoesNotNeedSpeciesFilter
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Species";
        }

        public class Item : TestableOption<CASAgeGenderFlags, CASAgeGenderFlags>
        {
            public Item()
            { }
            public Item(CASAgeGenderFlags species, int count)
                : base(species, GetName(species), count)
            { }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<CASAgeGenderFlags,CASAgeGenderFlags> results)
            {
                results[me.Species] = me.Species;
                return true;
            }
            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<CASAgeGenderFlags, CASAgeGenderFlags> results)
            {
                results[me.Species] = me.Species;
                return true;
            }

            public override void SetValue(CASAgeGenderFlags value, CASAgeGenderFlags storeType)
            {
                mValue = value;

                mName = GetName(value);
            }

            public static string GetName(CASAgeGenderFlags species)
            {
                return Common.Localize("Species:" + species);
            }
        }

        public Species()
        {}
        public Species(IEnumerable<CASAgeGenderFlags> species)
            : base(CreateList(species))
        { }

        protected static List<Item> CreateList(IEnumerable<CASAgeGenderFlags> species)
        {
            List<Item> results = new List<Item>();

            foreach (CASAgeGenderFlags specie in species)
            {
                results.Add(new Item(specie, 1));
            }

            return results;
        }

        public static List<CASAgeGenderFlags> GetSpecies()
        {
            List<CASAgeGenderFlags> results = new List<CASAgeGenderFlags>();

            foreach (CASAgeGenderFlags species in Enum.GetValues(typeof(CASAgeGenderFlags)))
            {
                if ((species & CASAgeGenderFlags.SpeciesMask) == 0) continue;

                switch (species)
                {
                    case CASAgeGenderFlags.SpeciesMask:
                    case CASAgeGenderFlags.LargeBird:
                    case CASAgeGenderFlags.SimLeadingHorse:
                    case CASAgeGenderFlags.SimWalkingDog:
                    case CASAgeGenderFlags.SimWalkingLittleDog:
                    case CASAgeGenderFlags.HouseboatLarge:
                    case CASAgeGenderFlags.HouseboatMedium:
                    case CASAgeGenderFlags.HouseboatSmall:
                    case CASAgeGenderFlags.Paddleboat:
                    case CASAgeGenderFlags.Rowboat:
                    case CASAgeGenderFlags.Sailboat:
                    case CASAgeGenderFlags.Shark:
                    case CASAgeGenderFlags.Speedboat:
                    case CASAgeGenderFlags.WaterScooter:
                    case CASAgeGenderFlags.WindsurfBoard:
                        continue;
                }

                results.Add(species);
            }

            return results;
        }
        public static List<Item> GetSpeciesChoices()
        {
            List<Item> results = new List<Item>();

            foreach (CASAgeGenderFlags species in GetSpecies())
            {
                results.Add(new Item(species, 0));
            }

            return results;
        }
    }
}
