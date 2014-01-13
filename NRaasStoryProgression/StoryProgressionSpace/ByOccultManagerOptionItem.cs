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
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace
{
    public interface IHasOccultSubOption<TOption>
    {
        TOption GetSubOption(OccultTypes type);
    }

    public interface IByOccultOptionItem<TBaseType> : IGenericValueOption<TBaseType>
    {
        TBaseType GetValue(OccultTypes type);
    }

    public abstract class ByOccultManagerOptionItem<TManager, TOption, TBaseType> : NestingManagerOptionItem<TManager, TOption>, IByOccultOptionItem<TBaseType>, IHasOccultSubOption<TOption>, IGeneralOption
        where TManager : StoryProgressionObject
        where TOption : GenericOptionItem<TBaseType>, IInstallable<TManager>, IOccultOption, INameableOption, new()
    {
        TOption mDefault = new TOption();

        public ByOccultManagerOptionItem()
        { }
        public ByOccultManagerOptionItem(TManager manager)
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

        public TOption GetSubOption(OccultTypes type)
        {
            mDefault.Occult = type;
            return Manager.GetOption<TOption>(mDefault.GetStoreKey());
        }

        public TBaseType GetValue(OccultTypes type)
        {
            TOption option = GetSubOption(type);
            if (option == null) return default(TBaseType);

            return option.Value;
        }

        protected abstract TBaseType Combine(TBaseType a, TBaseType b);

        public override bool Install(TManager manager, bool initial)
        {
            if (!HasRequiredVersion()) return false;

            foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
            {
                switch (type)
                {
                    case OccultTypes.None:
                    case OccultTypes.Ghost:
                        continue;
                }

                TOption option = new TOption();
                option.Occult = type;

                Installer<TManager>.Install(manager, option, initial);
            }

            return base.Install(manager, initial);
        }
    }
}

