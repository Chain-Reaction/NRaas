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
    [Persistable]
    public class LotOptions : GenericOptionBase
    {
        private ulong mLotId = 0;

        public LotOptions() // Required for persistence
        { }
        public LotOptions(Lot lot)
        {
            mLotId = lot.LotId;
        }

        public override string Name
        {
            get { return "Lot: " + Lot.Name; }
        }

        public override SimDescription SimDescription
        {
            get 
            {
                return null;
            }
        }

        public override Household House
        {
            get 
            {
                Lot lot = Lot;
                if (lot == null) return null;

                return lot.Household;
            }
        }

        public override Lot Lot
        {
            get 
            { 
                return LotManager.GetLot(mLotId); 
            }
        }

        public ulong LotId
        {
            get { return mLotId; }
        }

        public R GetValue<T, R>()
            where T : GenericOptionItem<R>, IReadLotLevelOption, new()
        {
            return GetInternalValue<T, R>();
        }

        public bool SetValue<T, R>(R value)
            where T : GenericOptionItem<R>, IWriteLotLevelOption, new()
        {
            return SetInternalValue<T, R>(value);
        }

        public TType AddValue<T, TType>(TType value)
            where T : OptionItem, IGenericAddOption<TType>, IWriteLotLevelOption, new()
        {
            return AddInternalValue<T, TType>(value);
        }

        public void RemoveValue<T, TType>(TType value)
            where T : OptionItem, IGenericRemoveOption<TType>, IWriteLotLevelOption, new()
        {
            RemoveInternalValue<T, TType>(value);
        }

        public bool HasValue<T, TType>(TType value)
            where T : OptionItem, IGenericHasOption<TType>, IReadLotLevelOption, new()
        {
            return HasInternalValue<T, TType>(value);
        }

        public bool AllowCastes(Common.IStatGenerator stats, SimDescription sim)
        {
            List<SimDescription> sims = new List<SimDescription>();
            sims.Add(sim);
            return AllowCastes(stats, sims);
        }
        public bool AllowCastes(Common.IStatGenerator stats, IEnumerable<SimDescription> sims)
        {
            if (sims == null) return true;

            RequireCasteLotOption requireCastes = GetInternalOption<RequireCasteLotOption>();
            DisallowCasteLotOption disallowCastes = GetInternalOption<DisallowCasteLotOption>();

            if ((requireCastes == null) && (disallowCastes == null)) return true;

            foreach (SimDescription sim in sims)
            {
                SimData simData = StoryProgression.Main.GetData(sim);

                if ((disallowCastes != null) && (disallowCastes.Count > 0))
                {
                    if (disallowCastes.Contains(simData.Castes))
                    {
                        stats.IncStat("Find Lot: Disallow Caste Denied");
                        return false;
                    }
                }

                if ((requireCastes != null) && (requireCastes.Count > 0))
                {
                    if (!requireCastes.Contains(simData.Castes))
                    {
                        stats.IncStat("Find Lot: Require Caste Denied");
                        return false;
                    }
                }
            }

            return true;
        }

        public override void UpdateInheritors(IGenericLevelOption option)
        {
            base.UpdateInheritors(option);

            if (House == null) return;

            StoryProgression.Main.GetHouseOptions(House).UpdateInheritors(option);
        }

        public override bool IsValidOption(IGenericLevelOption option)
        {
            if (option is IReadLotLevelOption) return true;

            if (option is IWriteLotLevelOption) return true;

            return false;
        }

        protected override void GetParentOptions(List<ParentItem> options, DefaultingLevel level)
        {
            // Global options
            options.Add(new ParentItem(StoryProgression.Main.Options, level));
        }

        public override string ToString()
        {
            Common.StringBuilder text = new Common.StringBuilder("<LotOptions>");

            Lot lot = Lot;
            if (lot != null)
            {
                text.AddXML("Lot", lot.Name);
            }
            else
            {
                text.AddXML("Lot", "Null");
            }

            text += base.ToString();

            text += Common.NewLine + "</LotOptions>";

            return text.ToString();
        }
    }
}

