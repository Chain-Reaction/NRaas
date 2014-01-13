using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class CleanseTheDead : MausoleumBase
    {
        public override string GetTitlePrefix()
        {
            return "CleanseTheDead";
        }

        protected override OptionResult Run(IActor actor, IMausoleum mausoleum)
        {
            Dictionary<SimDescription, Pair<IMausoleum, Urnstone>> urnstones = new Dictionary<SimDescription, Pair<IMausoleum, Urnstone>>();
            CommonSpace.Helpers.CleanseTheDead.Retrieve(mausoleum, urnstones);

            if ((urnstones == null) || (urnstones.Count == 0))
            {
                SimpleMessageDialog.Show(Name, Common.Localize (GetTitlePrefix () + ":Empty"));
                return OptionResult.Failure;
            }

            Sim sim = actor as Sim;

            Selection.Results choices = new Selection(Name, sim.SimDescription, urnstones.Keys).SelectMultiple();
            if (choices.Count == 0) return OptionResult.Failure;

            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[] { choices.Count })))
            {
                return OptionResult.Failure;
            }

            CommonSpace.Helpers.CleanseTheDead.Cleanse(choices, urnstones, false, null);

            return OptionResult.SuccessClose;
        }

        public class Selection : ProtoSimSelection<SimDescription>
        {
            public Selection(string title, SimDescription me, ICollection<SimDescription> sims)
                : base(title, me, sims, true, true)
            { }
        }
    }
}
