using NRaas.CommonSpace.Helpers;
using NRaas.OnceReadSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OnceReadSpace.Interactions
{
    public class PickUpTabletForAudioEx : Tablet.PickUpTabletForAudio, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Tablet, Tablet.PickUpTabletForAudio.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Tablet, Tablet.PickUpTabletForAudio.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public new class Definition : Tablet.PickUpTabletForAudio.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PickUpTabletForAudioEx();
                na.Init(ref parameters);
                return na;
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                // Custom
                TabletEx.PopulateAudioPrograms(parameters.Actor as Sim, ref parameters, out listObjs, out headers, out NumSelectableRows);
            }
        }
    }
}


