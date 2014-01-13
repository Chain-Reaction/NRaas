using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.SimDataElement
{
    public class RetainedInventorySimData : OnDemandSimData
    {
        public List<GameObject> mInventory = new List<GameObject>();

        public RetainedInventorySimData()
        { }

        public override bool Delayed
        {
            get { return false; }
        }

        public override string ToString()
        {
            string text = base.ToString();

            foreach (GameObject obj in mInventory)
            {
                text += Common.NewLine + obj.CatalogName;
            }

            return text;
        }
    }
}

