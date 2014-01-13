using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class StatusPetPool : SelectionTestableOptionList<StatusPetPool.Item, PetPoolType, PetPoolType>, IDoesNotNeedSpeciesFilter
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.StatusPetPool";
        }

        public class Item : TestableOption<PetPoolType,PetPoolType>, IApplyOptionItem
        {
            public Item()
            { }
            public Item(PetPoolType value, int count)
                : base(value, GetName(value), count)
            { }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<PetPoolType, PetPoolType> results)
            {
                PetPoolType type = SimTypes.GetPetPool(me);
                if (type == PetPoolType.None) return false;

                results[type] = type;
                return true;
            }

            public override void SetValue(PetPoolType value, PetPoolType storeType)
            {
                mValue = value;

                mName = GetName(value);
            }

            public static string GetName(PetPoolType pool)
            {
                if (pool == PetPoolType.None) return null;

                return Common.Localize("PetPool:" + pool);
            }

            public bool Apply(SimDescription me)
            {
                if (me.CreatedSim == Sim.ActiveActor)
                {
                    LotManager.SelectNextSim();
                }

                Household house = me.Household;
                if (house != null)
                {
                    house.Remove(me, !house.IsServiceNpcHousehold);
                }

                try
                {
                    if (PetPoolManager.AddPet(Value, me)) return true;
                }
                catch(Exception e)
                {
                    Common.Exception(me, e);

                    if ((house != null) && (me.Household == null))
                    {
                        house.Add(me);
                    }
                }

                return false;
            }
        }
    }
}
