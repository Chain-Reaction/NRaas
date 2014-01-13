using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class PurgeGeneticHair : AlarmOption
    {
        public PurgeGeneticHair()
        { }

        public override string GetTitlePrefix()
        {
            return "PurgeGeneticHair";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mPurgeGeneticHair;
            }
            set
            {
                NRaas.Overwatch.Settings.mPurgeGeneticHair = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log("Purge Genetic Hair");

                if ((!prompt) || (AcceptCancelDialog.Show(Common.Localize("PurgeGeneticHair:Prompt"))))
                {
                    Dictionary<SimDescription, bool> completed = new Dictionary<SimDescription, bool>();

                    int count = 0;

                    List<SimDescription> list = new List<SimDescription>(SimDescription.GetSimDescriptionsInWorld());
                    foreach (SimDescription sim in list)
                    {
                        if (sim.GeneticHairstyleKey != ResourceKey.kInvalidResourceKey)
                        {
                            sim.GeneticHairstyleKey = ResourceKey.kInvalidResourceKey;
                            count++;
                        };
                    }

                    if (prompt)
                    {
                        SimpleMessageDialog.Show(Name, Common.Localize("PurgeGeneticHair:Result", false, new object[] { count }));
                    }
                    else if (count > 0)
                    {
                        Overwatch.AlarmNotify(Common.Localize("PurgeGeneticHair:Result", false, new object[] { count }));
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Name, exception);
            }
        }
    }
}
