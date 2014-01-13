using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace
{
    public interface IHasSpeciesSubOption<TOption>
    {
        TOption GetSubOption(CASAgeGenderFlags species);
    }

    public interface IBySpeciesOptionItem<TBaseType> : IGenericValueOption<TBaseType>
    {
        TBaseType GetValue(CASAgeGenderFlags species);
    }

    public abstract class BySpeciesManagerOptionItem<TManager, TOption, TBaseType> : NestingManagerOptionItem<TManager, TOption>, IBySpeciesOptionItem<TBaseType>, IHasSpeciesSubOption<TOption>, IGeneralOption
        where TManager : StoryProgressionObject
        where TOption : GenericOptionItem<TBaseType>, IInstallable<TManager>, ISpeciesOption, INameableOption, new()
    {
        static CASAgeGenderFlags[] sSpecies = new CASAgeGenderFlags[] { CASAgeGenderFlags.Human, CASAgeGenderFlags.LittleDog, CASAgeGenderFlags.Dog, CASAgeGenderFlags.Cat, CASAgeGenderFlags.Horse };

        TOption mDefault = new TOption();

        public BySpeciesManagerOptionItem()
        { }
        public BySpeciesManagerOptionItem(TManager manager)
            : base (manager)
        { }

        public override string GetTitlePrefix()
        {
            return mDefault.GetTitlePrefix();
        }

        public string LocalizedSettingPath
        {
            get
            {
                return GetManager().GetLocalizedName();
            }
        }

        public TBaseType Value
        {
            get
            {
                List<TOption> options = GetOptions();

                TBaseType type = default(TBaseType);

                foreach (TOption option in options)
                {
                    type = Combine(type, option.Value);
                }

                return type;
            }
        }

        public TOption GetSubOption(CASAgeGenderFlags species)
        {
            mDefault.Species = species;
            return Manager.GetOption<TOption>(mDefault.GetStoreKey());
        }

        public TBaseType GetValue(CASAgeGenderFlags species)
        {
            TOption option = GetSubOption(species);
            if (option == null) return default(TBaseType);

            return option.Value;
        }

        protected abstract TBaseType Combine(TBaseType a, TBaseType b);

        public override bool Install(TManager manager, bool initial)
        {
            if (!HasRequiredVersion()) return false;

            if (GameUtils.IsInstalled(ProductVersion.EP5))
            {
                foreach (CASAgeGenderFlags species in sSpecies)
                {
                    TOption option = new TOption();
                    option.Species = species;

                    Installer<TManager>.Install(manager, option, initial);
                }
            }
            else
            {
                TOption option = new TOption();
                option.Species = CASAgeGenderFlags.Human;

                Installer<TManager>.Install(manager, option, initial);
            }

            return base.Install(manager, initial);
        }
    }
}

