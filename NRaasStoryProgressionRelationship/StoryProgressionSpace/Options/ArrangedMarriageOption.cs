using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
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

namespace NRaas.StoryProgressionSpace.Options
{
    public class ArrangedMarriageOption : GenericOptionBase.ChoiceOptionItem<ulong>, IReadSimLevelOption, IWriteSimLevelOption, INotCasteLevelOption
    {
        public ArrangedMarriageOption()
            : base(0, 0)
        { }

        public override string GetTitlePrefix()
        {
            return "ArrangedMarriage";
        }

        protected override string ValuePrefix
        {
            get { return "YesNo"; }
        }

        public override object PersistValue
        {
            set
            {
                if (value is string)
                {
                    ulong result;
                    if (ulong.TryParse(value as string, out result))
                    {
                        SetValue(result);
                    }
                }
                else
                {
                    SetValue((ulong)value);
                }
            }
        }

        public override string GetUIValue(bool pure)
        {
            SimDescription sim = ManagerSim.Find(Value);
            if (sim == null) return null;

            return sim.FirstName;
        }

        protected override IEnumerable<ulong> GetOptions()
        {
            List<ulong> results = new List<ulong>();

            ManagerFlirt manager = StoryProgression.Main.Flirts;

            SimDescription me = Manager.SimDescription;

            foreach (SimDescription other in StoryProgression.Main.Sims.All)
            {
                if (other.LotHome == null) continue;

                if (other.IsMarried) continue;

                if (string.IsNullOrEmpty(other.FirstName)) continue;

                ulong otherArranged = StoryProgression.Main.GetValue<ArrangedMarriageOption, ulong>(other);
                if ((otherArranged != 0) && (otherArranged != me.SimDescriptionId))
                {
                    continue;
                }

                if (!manager.CanHaveAutonomousRomance(manager, other, me, false)) continue;

                results.Add(other.SimDescriptionId);
            }

            return results;
        }

        protected override string GetLocalizationValueKey()
        {
            return null;
        }

        protected override string GetLocalizedValue(ulong value, ref bool matches, ref ThumbnailKey icon)
        {
            matches = (Value == value);

            SimDescription sim = ManagerSim.Find(value);
            if (sim == null) return null;

            icon = sim.GetThumbnailKey(ThumbnailSize.Large, 0);

            return sim.LastName + ", " + sim.FirstName;
        }

        protected override bool PrivatePerform()
        {
            if (!base.PrivatePerform()) return false;

            ulong otherSim = Value;
            if (otherSim != 0)
            {
                SimDescription other = ManagerSim.Find(otherSim);

                StoryProgression.Main.SetValue<ArrangedMarriageOption, ulong>(other, Manager.SimDescription.SimDescriptionId);
            }

            return true;
        }
    }
}

