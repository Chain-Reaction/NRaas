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
    public class NetRatioOption : HouseIntegerOption, IReadHouseLevelOption, INotPersistableOption, IDebuggingOption
    {
        public NetRatioOption()
            : base(0)
        { }

        public override string GetTitlePrefix()
        {
            return "NetRatio";
        }

        public override int Value
        {
            get
            {
                int assets = GetValue<AssetsOption,int>();
                if (assets <= 0) return 100;

                int ratio = (GetValue<DebtOption, int>() * 100 / assets);
                if (ratio > 100)
                {
                    return 100;
                }
                else
                {
                    return ratio;
                }
            }
        }
    }
}

