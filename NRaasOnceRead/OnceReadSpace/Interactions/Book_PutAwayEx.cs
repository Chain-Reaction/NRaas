using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OnceReadSpace.Interactions
{
    public class Book_PutAwayEx : Common.IAddInteraction, Common.IPreLoad
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Book, Book_PutAway.Definition, Definition>(false);

            sOldSingleton = Book_PutAway.Singleton;
            Book_PutAway.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Book, Book_PutAway.Definition>(Book_PutAway.Singleton);
        }

        public class Definition : Book_PutAway.Definition
        {
            public override string GetInteractionName(Sim actor, Book target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Book target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target is BookToddler)
                {
                    if (!a.Inventory.Contains(target))
                    {
                        if (isAutonomous) return false;
                    }

                    if (a.LotHome != a.LotCurrent) return false;
                }
                else
                {
                    if (a.Inventory.Contains(target) && (a.LotCurrent != a.LotHome))
                    {
                        return false;
                    }
                }

                if (target.InUse || (Bookshelf.FindClosestBookshelf(a, target, a.Inventory.Contains(target)) == null))
                {
                    return false;
                }

                if (!target.IsServiceableBySim(a))
                {
                    return false;
                }

                return true;
            }
        }
    }
}
