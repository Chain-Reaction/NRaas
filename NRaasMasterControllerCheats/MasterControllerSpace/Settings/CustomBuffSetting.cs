using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public class CustomBuffSetting : OptionItem, ISettingOption, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "CustomBuffs";
        }

        public void Import(Persistence.Lookup settings)
        {
            MasterController.Settings.mCustomBuffs = new StringToStringList().Convert(settings.GetString(GetTitlePrefix()));
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add(GetTitlePrefix (), new ListToString<string>().Convert(MasterController.Settings.mCustomBuffs));
        }

        public string PersistencePrefix
        {
            get { return GetTitlePrefix(); }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<Moodlet.Item> options = new List<Moodlet.Item>();
            foreach (BuffInstance instance in BuffManager.BuffDictionary.Values)
            {
                BuffNames guid = (BuffNames)instance.mBuffGuid;

                int value = 0;
                if (MasterController.Settings.mCustomBuffs.Contains(guid.ToString()))
                {
                    value = 1;
                }

                options.Add(new Moodlet.Item(instance, value));
            }

            CommonSelection<Moodlet.Item>.Results choices = new CommonSelection<Moodlet.Item>(Name, options).SelectMultiple();
            if ((choices == null) || (choices.Count == 0)) return OptionResult.Failure;

            foreach (Moodlet.Item item in choices)
            {
                string buff = item.Value.ToString();

                if (item.Count > 0)
                {
                    MasterController.Settings.mCustomBuffs.Remove(buff);
                }
                else
                {
                    MasterController.Settings.mCustomBuffs.Add(buff);
                }
            }

            return OptionResult.SuccessRetain;
        }
    }
}
