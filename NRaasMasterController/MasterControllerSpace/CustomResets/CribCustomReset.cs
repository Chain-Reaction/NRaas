using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.CustomResets
{
    public class CribCustomReset : GenericCustomReset<Crib>
    {
        protected override bool PrivatePerform(Crib obj)
        {
            Sim child = obj.Child;
            if (child != null)
            {
                if (child.Inventory == null)
                {
                    IStuffedAnimal stuffedAnimal = obj.StuffedAnimal;
                    if (stuffedAnimal != null)
                    {
                        stuffedAnimal.UnParent();
                        stuffedAnimal.RemoveFromWorld();
                        stuffedAnimal.FadeIn();
                        obj.mContainedStuffedAnimal = ObjectGuid.InvalidObjectGuid;
                    }
                }
            }

            obj.SetObjectToReset();
            return true;
        }
    }
}
