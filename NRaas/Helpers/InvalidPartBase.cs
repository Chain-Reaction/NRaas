using NRaas.CommonSpace.Converters;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NRaas.CommonSpace.Helpers
{
    [Persistable]
    public abstract class InvalidPartBase
    {
        public enum Reason
        {
            None,
            ImproperAge,
            ValidFail,
            Age,
            Gender,
            Species,
            Category,
            Extended,
            World,
            TuningMatch,
        }

        OutfitCategories mCategories;
        OutfitCategoriesExtended mExtended;

        List<WorldType> mWorldTypes;

        CASAgeGenderFlags mAge;
        CASAgeGenderFlags mGender;
        CASAgeGenderFlags mSpecies;

        public InvalidPartBase() // Required for persistence
        {
            mWorldTypes = new List<WorldType>();
        }

        public void Set(OutfitCategories categories, List<WorldType> worldTypes, OutfitCategoriesExtended extended, CASAgeGenderFlags age, CASAgeGenderFlags gender, CASAgeGenderFlags species)
        {
            mCategories = categories;
            mExtended = extended;
            mAge = age;
            mGender = gender;
            mSpecies = species;
            mWorldTypes = worldTypes;
        }

        public bool Allow(CASAgeGenderFlags age, CASAgeGenderFlags gender, CASAgeGenderFlags species, OutfitCategories category, OutfitCategoriesExtended extended, out Reason reason)
        {
            if (mWorldTypes.Count > 0)
            {
                if (!mWorldTypes.Contains(GameUtils.GetCurrentWorldType()))
                {
                    reason = Reason.World;
                    return true;
                }
            }

            if ((mAge & age) != age)
            {
                reason = Reason.Age;
                return true;
            }

            if ((mGender & gender) != gender)
            {
                reason = Reason.Gender;
                return true;
            }

            if (mSpecies != species)
            {
                reason = Reason.Species;
                return true;
            }

            if ((mCategories & category) != category)
            {
                reason = Reason.Category;
                return true;
            }

            if (mExtended != OutfitCategoriesExtended.ValidForRandom)
            {
                if ((mExtended & extended) != mExtended)
                {
                    reason = Reason.Extended;
                    return true;
                }
            }

            reason = Reason.TuningMatch;
            return false;
        }

        public bool Import(Persistence.Lookup settings)
        {
            mAge = settings.GetEnum<CASAgeGenderFlags>("Age", CASAgeGenderFlags.None);

            mGender = settings.GetEnum<CASAgeGenderFlags>("Gender", CASAgeGenderFlags.None);

            mSpecies = settings.GetEnum<CASAgeGenderFlags>("Species", CASAgeGenderFlags.None);

            mCategories = settings.GetEnum<OutfitCategories>("Categories", OutfitCategories.None);

            mExtended = settings.GetEnum<OutfitCategoriesExtended>("Extended", (OutfitCategoriesExtended)0x0);

            mWorldTypes = new ToWorldType().Convert(settings.GetString("WorldTypes"));

            return true;
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("Age", mAge.ToString());
            settings.Add("Gender", mGender.ToString());
            settings.Add("Species", mSpecies.ToString());
            settings.Add("Categories", mCategories.ToString());
            settings.Add("Extended", mExtended.ToString());
            settings.Add("WorldTypes", GetWorldTypeString());
        }

        protected string GetWorldTypeString()
        {
            return new ListToString<WorldType>().Convert(mWorldTypes);
        }

        public string ToXML(ResourceKey key, string indent)
        {
            string result = indent + "<InvalidParts>";
            result += Common.NewLine + indent + "  <Instance>0x" + key.InstanceId.ToString("X16") + "</Instance>";
            result += Common.NewLine + indent + "  <Group>0x" + key.GroupId.ToString("X8") + "</Group>";
            result += Common.NewLine + ToXML(indent + "  ");
            result += Common.NewLine + indent + "</InvalidParts>";

            return result;
        }
        public string ToXML(string indent)
        {
            string result = indent + "<Age>" + mAge + "</Age>";

            result += Common.NewLine + indent + "<Gender>" + mGender + "</Gender>";
            result += Common.NewLine + indent + "<Species>" + mSpecies + "</Species>";
            result += Common.NewLine + indent + "<Categories>" + mCategories + "</Categories>";

            if (mExtended != (OutfitCategoriesExtended)0)
            {
                result += Common.NewLine + indent + "<Extended>" + mExtended + "</Extended>";
            }

            if (mWorldTypes.Count > 0)
            {
                result += Common.NewLine + indent + "<WorldTypes>" + GetWorldTypeString() + "</WorldTypes>";
            }

            return result;
        }

        public override string ToString()
        {
            return ToXML(" ");
        }

        public class ToWorldType : StringToList<WorldType>
        {
            protected override bool PrivateConvert(string value, out WorldType result)
            {
                return ParserFunctions.TryParseEnum<WorldType>(value, out result, WorldType.Undefined);
            }
        }
    }
}


