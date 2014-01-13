using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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
    public abstract class OccupationManagerOptionItem<TManager> : MultiEnumManagerOptionItem<TManager, OccupationNames>
        where TManager : StoryProgressionObject
    {
        public OccupationManagerOptionItem(OccupationNames[] defValue)
            : base(defValue)
        { }

        protected override bool PersistCreate(ref OccupationNames defValue, string value)
        {
            if (!ParserFunctions.TryParseEnum<OccupationNames>(value, out defValue, OccupationNames.Undefined))
            {
                ulong guid;
                if (ulong.TryParse(value, out guid))
                {
                    defValue = (OccupationNames)guid;

                    if (CareerManager.GetStaticOccupation(defValue) == null)
                    {
                        defValue = OccupationNames.Undefined;
                        return false;
                    }
                }
            }

            return true;
        }

        protected override string LocalizeValue(OccupationNames value)
        {
            Occupation career = CareerManager.GetStaticOccupation(value);
            if (career != null)
            {
                return career.GetLocalizedCareerName(false);
            }
            else
            {
                return base.LocalizeValue(value);
            }
        }

        protected override List<IGenericValueOption<OccupationNames>> GetAllOptions()
        {
            List<IGenericValueOption<OccupationNames>> results = new List<IGenericValueOption<OccupationNames>>();

            foreach (Occupation career in CareerManager.OccupationList)
            {
                if (Allow(career.Guid))
                {
                    results.Add(new EnumListItem(this, career.Guid));
                }
            }

            return results;
        }
    }
}
