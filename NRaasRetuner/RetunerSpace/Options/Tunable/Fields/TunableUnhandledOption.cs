using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.RetunerSpace.Helpers.FieldInfos;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace.Options.Tunable.Fields
{
    public class TunableUnhandledOption : OperationSettingOption<GameObject>, ITunableFieldOption
    {
        TunableFieldInfo mField;

        public TunableUnhandledOption(TunableFieldInfo field)
            : base(field.Name)
        {
            mField = field;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override string DisplayValue
        {
            get { return Common.Localize("Type:Unhandled"); }
        }

        public void Export(Common.StringBuilder result)
        {
            result += Common.NewLine + mField.ToXMLString();
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            string msg = Common.Localize("Unhandled:Notice");

            msg += Common.NewLine + Common.Localize("Unhandled:ParentType") + mField.Field.DeclaringType;
            msg += Common.NewLine + Common.Localize("Unhandled:FieldName") + mField.Name;
            msg += Common.NewLine + Common.Localize("Unhandled:FieldType") + mField.Field.FieldType;

            SimpleMessageDialog.Show(Name, msg);

            Common.WriteLog(msg);

            return OptionResult.Failure;
        }
    }
}
