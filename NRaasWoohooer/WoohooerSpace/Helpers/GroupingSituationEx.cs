using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class GroupingSituationEx
    {
        public static bool CanGoOnDate(SimDescription a, SimDescription b)
        {
            Relationship relationship = Relationship.Get(a, b, false);
            if (relationship == null) return false;
            
            if (!relationship.AreRomantic() && ((relationship.STC == null) || !relationship.STC.IsRomantic))
            {
                return false;
            }

            string reason;
            GreyedOutTooltipCallback callback = null;
            if (!CommonSocials.CanGetRomantic(a, b, true, false, true, ref callback, out reason))
            {
                return false;
            }

            return true;
        }
    }
}
