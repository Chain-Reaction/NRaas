using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace
{
    public abstract class BooleanEventOptionItem<TManager, TScenario> : BooleanManagerOptionItem<TManager>, IScenarioOptionItem
        where TManager : Manager
        where TScenario : Scenario, IEventScenario, new()
    {
        public BooleanEventOptionItem(bool value)
            : base(value)
        { }

        public Scenario GetScenario()
        {
            return new TScenario();
        }

        public StoryProgressionObject GetManager()
        {
            return Manager;
        }

        protected void SetupListener()
        {
            Manager.AddListener(new TScenario());
        }

        public override bool Install(TManager main, bool initial)
        {
            if (!base.Install(main, initial)) return false;

            SetupListener();
            return true;
        }
    }
}
