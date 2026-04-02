using NRaas.CommonSpace.Options;
using NRaas.ChemistrySpace.Tasks;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.ChemistrySpace.Options.DebuggingLevel
{
    public abstract class DebuggingLevelSetting<TObject> : ListedSettingOption<AttractionCoreTask.DebuggingLevel, TObject>
        where TObject : class, IGameObject
    {
        public override string GetLocalizedValue(AttractionCoreTask.DebuggingLevel value)
        {
            return Common.Localize("DebuggingLevel:" + value);
        }

        protected override bool Allow(AttractionCoreTask.DebuggingLevel value)
        {
            if (value == AttractionCoreTask.DebuggingLevel.None) return false;

            return base.Allow(value);
        }

        public override bool ConvertFromString(string value, out AttractionCoreTask.DebuggingLevel newValue)
        {
            return ParserFunctions.TryParseEnum<AttractionCoreTask.DebuggingLevel>(value, out newValue, AttractionCoreTask.DebuggingLevel.None);
        }

        public override string ConvertToString(AttractionCoreTask.DebuggingLevel value)
        {
            return value.ToString();
        }
    }
}
