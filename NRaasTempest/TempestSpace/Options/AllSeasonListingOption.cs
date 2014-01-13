using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options
{
    public abstract class AllSeasonListingOption<T> : InteractionOptionList<T, GameObject>, IPrimaryOption<GameObject>
        where T : class, IInteractionOptionItem<IActor, GameObject, GameHitParameters< GameObject>>
    {
        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected abstract T GetSeasonOption(Season season);

        public override List<T> GetOptions()
        {
            List<T> results = base.GetOptions();

            foreach (Season season in Enum.GetValues(typeof(Season)))
            {
                results.Add(GetSeasonOption(season));
            }

            return results;
        }
    }
}
