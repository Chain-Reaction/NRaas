using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OnceReadSpace.Interactions
{
    public class ReadWhileRockingEx : Common.IPreLoad, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddCustom(new CustomInjector());
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Book, ReadWhileRocking.Definition, Definition>(false);

            ReadWhileRocking.Singleton = Singleton;
        }

        public class Definition : ReadWhileRocking.Definition
        {
            public override bool Test(Sim a, Book target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target is SheetMusic) return false;

                if (isAutonomous)
                {
                    if (a.LotCurrent.IsWorldLot)
                    {
                        return false;
                    }
                    else if (ReadBookData.HasSimFinishedBook(a, target.Data.ID))
                    {
                        return false;
                    }
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }

        public class CustomInjector : Common.InteractionReplacer<Book, ReadWhileRocking.Definition>
        {
            public CustomInjector()
                : base(Singleton, true)
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                if (obj is BookToddler) return false;

                return base.Perform(obj, definition, existing);
            }
        }
    }
}


