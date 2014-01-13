using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class HotkeyInteraction : CommonInteraction<HotkeyInteraction.HotkeyOption,GameObject>, Common.IWorldLoadFinished
    {
        public static readonly InteractionDefinition Singleton = new CommonDefinition<HotkeyInteraction>(true, false);

        public void OnWorldLoadFinished()
        {
            CommonDefinition<HotkeyInteraction>.UnloadPopupOptions();
        }

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<GameObject>(Singleton);
        }

        protected override OptionResult Perform(IActor actor, GameObject target, GameObjectHit hit)
        {
            throw new NotImplementedException();
        }

        protected override bool Test(IActor actor, GameObject target, GameObjectHit hit, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return PrimaryInteraction<IOptionItem>.PublicTest(actor, target, hit, ref greyedOutTooltipCallback);
        }

        public class HotkeyOption : OperationSettingOption<GameObject>, ICommonOptionListProxy<HotkeyOption>
        {
            public OptionItem mOption;

            public HotkeyOption()
            { }
            protected HotkeyOption(OptionItem option)
            {
                mOption = option;
            }

            protected static Dictionary<string, OptionItem> sLookup = null;

            protected static Dictionary<string, OptionItem> Lookup
            {
                get
                {
                    if (sLookup == null)
                    {
                        sLookup = new Dictionary<string, OptionItem>();

                        List<OptionItem> actions = Common.DerivativeSearch.Find<OptionItem>();

                        foreach (OptionItem action in actions)
                        {
                            string name = action.HotkeyID;
                            if (string.IsNullOrEmpty(name)) continue;

                            if (sLookup.ContainsKey(name)) continue;

                            sLookup.Add(name, action);
                        }
                    }

                    return sLookup;
                }
            }

            public static List<OptionItem> GetHotkeyOptions()
            {
                List<OptionItem> hotKeys = new List<OptionItem>();

                foreach (string hotkey in MasterController.Settings.mHotkeys)
                {
                    OptionItem option;
                    if (!Lookup.TryGetValue(hotkey, out option)) continue;

                    hotKeys.Add(option.Clone ());
                }

                return hotKeys;
            }

            public override string Name
            {
                get { return mOption.Name; }
            }

            public override string GetTitlePrefix()
            {
                return mOption.GetTitlePrefix();
            }

            protected override bool Allow(GameHitParameters<GameObject> parameters)
            {
                return mOption.Test(parameters);
            }

            protected override OptionResult Run(GameHitParameters<GameObject> parameters)
            {
                return mOption.Perform(parameters);
            }

            public void GetOptions(List<HotkeyOption> items)
            {
                List<OptionItem> options = GetHotkeyOptions();

                foreach (OptionItem option in options)
                {
                    items.Add(new HotkeyOption(option));
                }
            }
        }
    }
}
