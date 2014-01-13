using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings.SkillStamps
{
    public class AddStampSetting : OptionItem, ISkillStampOption, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "AddSkillStamp";
        }

        public void Import(Persistence.Lookup settings)
        {
            MasterController.Settings.SkillStamps.Clear();
            MasterController.Settings.SkillStamps.AddRange(settings.GetList<SkillStamp>(""));

            MasterController.Settings.UpdateStamp();
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("", MasterController.Settings.SkillStamps);
        }

        public string PersistencePrefix
        {
            get { return GetTitlePrefix(); }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            string name = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt"), EAText.GetNumberString(MasterController.Settings.SkillStamps.Count + 1));
            if (string.IsNullOrEmpty(name)) return OptionResult.Failure;

            MasterController.Settings.SkillStamps.Add(new SkillStamp(name));

            MasterController.Settings.UpdateStamp();

            return OptionResult.SuccessRetain;
        }
    }
}
