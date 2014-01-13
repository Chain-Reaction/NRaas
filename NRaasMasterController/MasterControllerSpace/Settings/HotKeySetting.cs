using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public class HotKeySetting : OptionItem, ISettingOption, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "HotKeys";
        }

        public void Import(Persistence.Lookup settings)
        {
            string value = settings.GetString(GetTitlePrefix());
            if (value == null) return;

            MasterController.Settings.mHotkeys = new StringToStringList().Convert(value);
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add(GetTitlePrefix(), new ListToString<string>().Convert(MasterController.Settings.mHotkeys));
        }

        public string PersistencePrefix
        {
            get { return GetTitlePrefix(); }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return new HotkeyOption.HotkeyOptionList(Name).Perform(parameters);
        }

        public static bool ToggleState(string name)
        {
            if (MasterController.Settings.mHotkeys.Contains(name))
            {
                MasterController.Settings.mHotkeys.Remove(name);
            }
            else
            {
                MasterController.Settings.mHotkeys.Add(name);
            }

            HotkeyInteraction.CommonDefinition<HotkeyInteraction>.UnloadPopupOptions();

            return true;
        }

        public class HotkeyOption : BooleanSettingOption<GameObject>
        {
            IOptionItem mOption;

            public HotkeyOption(IOptionItem option)
            {
                mOption = option;
            }

            public override string PersistencePrefix
            {
                // Disables persistence
                get { return null; }
            }

            protected override bool Value
            {
                get
                {
                    return MasterController.Settings.mHotkeys.Contains(mOption.HotkeyID);
                }
                set
                {
                    ToggleState(mOption.HotkeyID);
                }
            }

            public override string Name
            {
                get { return mOption.Name; }
            }

            public override string DisplayValue
            {
                get
                {
                    if (mOption is IInteractionOptionList<GameObject>) return null;

                    return base.DisplayValue;
                }
            }

            public override string GetTitlePrefix()
            {
                return mOption.GetTitlePrefix();
            }

            protected override string GetPrompt()
            {
                return null;
            }

            public override ITitlePrefixOption ParentListingOption
            {
                get { return null; }
            }

            protected override OptionResult Run(GameHitParameters<GameObject> parameters)
            {
                IInteractionOptionList<GameObject> list = mOption as IInteractionOptionList<GameObject>;
                if (list != null)
                {
                    return new HotkeyOptionList(list).Perform(parameters); 
                }
                else
                {
                    return base.Run(parameters);
                }
            }

            public class HotkeyOptionList : InteractionOptionList<HotkeyOption, GameObject>
            {
                List<HotkeyOption> mOptions = new List<HotkeyOption>();

                public HotkeyOptionList(string name)
                    : base(name)
                {
                    foreach (IPrimaryOption<GameObject> paramOption in CommonOptionList<IPrimaryOption<GameObject>>.AllOptions())
                    {
                        IOptionItem option = paramOption as IOptionItem;
                        if (option == null) continue;

                        mOptions.Add(new HotkeyOption(option));
                    }
                }
                public HotkeyOptionList(IInteractionOptionList<GameObject> options)
                    : base(options.ToString())
                {
                    foreach (IInteractionOptionItem<IActor,GameObject,GameHitParameters<GameObject>> paramOption in options.IOptions())
                    {
                        IOptionItem option = paramOption as IOptionItem;
                        if (option == null) continue;

                        if (!(option is IOptionList))
                        {
                            if (string.IsNullOrEmpty(option.HotkeyID)) continue;
                        }

                        mOptions.Add(new HotkeyOption(option));
                    }
                }

                public override string GetTitlePrefix()
                {
                    return null;
                }

                public override ITitlePrefixOption ParentListingOption
                {
                    get { return null; }
                }

                public override List<HotkeyOption> GetOptions()
                {
                    return mOptions;
                }
            }
        }
    }
}
