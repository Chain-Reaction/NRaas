using NRaas.CommonSpace.Options;
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
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace
{
    public interface INestingOption : IOptionItem
    { }

    public abstract class NestingOptionItem<TOption> : OptionItem, INestingOption, INotPersistableOption
        where TOption : class, IOptionItem
    {
        public NestingOptionItem()
        { }

        protected override string GetLocalizationValueKey()
        {
            return null;
        }

        protected override bool IsFemaleLocalization()
        {
            return GetManager().IsFemaleLocalization();
        }

        public abstract StoryProgressionObject GetManager();

        public override string GetUIValue(bool pure)
        {
            return "";
        }

        public sealed override object PersistValue
        {
            get
            {
                return null;
            }
            set
            { }
        }

        public override bool ShouldDisplay()
        {
            if (GetManager() == null) return false;

            if (GetOptions().Count == 0) return false;

            return true;
        }

        protected virtual bool Allow(TOption option)
        {
            return true;
        }

        public List<TOption> GetOptions()
        {
            List<TOption> options = new List<TOption>();
            GetManager().GetOptions<TOption>(options, true, Allow);

            return options;
        }

        protected override bool PrivatePerform()
        {
            while (true)
            {
                List<TOption> allOptions = GetOptions();

                bool okayed = false;
                List<TOption> selection = OptionItem.ListOptions(allOptions, Name, 1, out okayed);
                if ((selection == null) || (selection.Count == 0))
                {
                    return okayed;
                }

                foreach (IOptionItem option in selection)
                {
                    if (option.Perform())
                    {
                        if (option is INestingOption) return true;
                    }
                }
            }
        }
    }
}

