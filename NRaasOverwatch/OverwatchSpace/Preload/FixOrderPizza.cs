using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Preload
{
    public class FixOrderPizza : PreloadOption
    {
        public FixOrderPizza()
        { }

        public override string GetTitlePrefix()
        {
            return "FixOrderPizza";
        }

        public override void OnPreLoad()
        {
            Overwatch.Log(GetTitlePrefix());

            Tunings.Inject<BarAdvanced, BarAdvanced.OrderFood.Definition, BarAdvanced.OrderPizza.Definition>(false);
        }
    }
}