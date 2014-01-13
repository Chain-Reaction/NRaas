using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public abstract class CareerBase<TOption> : SelectionTestableOptionList<TOption, OccupationNames, OccupationNames>
        where TOption : CareerBase<TOption>.ItemBase, new()
    {
        public abstract class ItemBase : TestableOption<OccupationNames, OccupationNames>
        {
            public ItemBase()
            { }
            public ItemBase(OccupationNames value, string name, int count)
                : base(value, name, count)
            { }

            public override void SetValue(OccupationNames value, OccupationNames storeType)
            {
                mValue = value;

                if (value == OccupationNames.Undefined)
                {
                    mName = Common.Localize("Criteria.Career:Unemployed");
                }
                else
                {
                    Occupation occupation = CareerManager.GetStaticOccupation(mValue);
                    if (occupation != null)
                    {
                        mName = occupation.CareerName;

                        SetThumbnail(occupation.CareerIconColored, ProductVersion.BaseGame);
                    }
                    else
                    {
                        mName = mValue.ToString();
                    }
                }
            }
        }
    }
}
