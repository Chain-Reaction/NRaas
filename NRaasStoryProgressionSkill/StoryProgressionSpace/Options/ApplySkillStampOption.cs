using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    // this is not fully implemented
    // I need to find an elegant way to do this that doesn't cause lag
    // check should probably go in SPS\SimData\NonPersistSimData\SimCastes
    public class ApplySkillStampOption : GenericOptionBase.ListedOptionItem<string, string>, IReadSimLevelOption, IWriteSimLevelOption, IHouseLevelSimOption, IDebuggingOption
    {
        static Common.MethodStore sGetSkillStamps = new Common.MethodStore("NRaasMasterController", "NRaas.MasterController", "GetSkillStamps", new Type[] { typeof(Dictionary<string, Dictionary<SkillNames, int>>) });

        public ApplySkillStampOption()
            : base(new List<string>(), new List<string>())
        { }

        public override string GetTitlePrefix()
        {
            return "ApplySkillStamp";
        }

        protected override string ValuePrefix
        {
            get { return "YesNo"; }
        }

        protected override string ConvertFromString(string value)
        {            
            return value;
        }

        protected override string ConvertToValue(string value, out bool valid)
        {
            valid = true;
            return value;
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override string GetLocalizedValue(string value, ref ThumbnailKey icon)
        {
            return value;
        }

        protected override string GetLocalizationValueKey()
        {
            return "ApplySkillStamp";
        }

        public override bool ShouldDisplay()
        {
            if (!GetValue<AllowSkillOption, bool>()) return false;

            if (!sGetSkillStamps.Valid) return false;

            return base.ShouldDisplay();
        }

        protected override IEnumerable<string> GetOptions()
        {
            Dictionary<string, Dictionary<SkillNames, int>> stamps = new Dictionary<string, Dictionary<SkillNames, int>>();
            sGetSkillStamps.Invoke<Dictionary<string, Dictionary<SkillNames, int>>>(new object[] { stamps });

            return new List<string>(stamps.Keys);
        }
    }
}