using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    [Persistable]
    public class PartyGuest : SelectionOption, IDoesNotNeedSpeciesFilter
    {
        Dictionary<ulong,bool> mValidChoices = null;

        public override string GetTitlePrefix()
        {
            return "Criteria.PartyGuests";
        }

        public override void Reset()
        {
            mValidChoices = null;

 	        base.Reset();
        }

        protected Dictionary<ulong, bool> ValidChoices
        {
            get
            {
                if (mValidChoices == null)
                {
                    mValidChoices = new Dictionary<ulong, bool>();

                    List<PhoneSimPicker.SimPickerInfo> choices = Phone.Call.GetAllValidCalleesForParty(Sim.ActiveActor, HouseParty.kLTRMinToInvite, false);
                    if (choices != null)
                    {
                        foreach (PhoneSimPicker.SimPickerInfo choice in choices)
                        {
                            IMiniSimDescription sim = choice.SimDescription as IMiniSimDescription;
                            if (sim == null) continue;

                            if (mValidChoices.ContainsKey(sim.SimDescriptionId)) continue;
                            mValidChoices.Add(sim.SimDescriptionId, true);
                        }
                    }
                }

                return mValidChoices;
            }
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            return ValidChoices.ContainsKey(me.SimDescriptionId);
        }
        protected override bool Allow(MiniSimDescription me, IMiniSimDescription actor)
        {
            return ValidChoices.ContainsKey(me.SimDescriptionId);
        }
    }
}
