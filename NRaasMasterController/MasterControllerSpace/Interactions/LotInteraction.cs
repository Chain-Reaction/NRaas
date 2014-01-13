using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Object;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class LotInteraction : PrimaryInteraction<ILotOption>
    {
        public static InteractionDefinition Singleton = new CommonDefinition<LotInteraction>();

        public override void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddCustom(new CustomInjector());
        }

        public class CustomInjector : Common.InteractionInjector<GameObject>
        {
            public CustomInjector()
                : base(Singleton)
            { }

            public override List<Type> GetTypes()
            {
                List<Type> list = new List<Type>();
                list.Add(typeof(Lot));
                list.Add(typeof(Terrain));
                list.Add(typeof(BuildableShell));
                return list;
            }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                if (obj is Lot)
                {
                    Lot lot = obj as Lot;
                    if (!lot.IsBaseCampLotType)
                    {
                        return base.Perform(obj, definition, existing);
                    }
                }
                else if (obj is Terrain)
                {
                    return base.Perform(obj, definition, existing);
                }
                else if (obj is BuildableShell)
                {
                    return base.Perform(obj, definition, existing);
                }

                return false;
            }
        }
    }
}
