using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Helpers
{
    public class InteractionControl
    {
        public static bool Run(IMagicalInteraction ths, OccultTypes intendedType)
        {
            bool succeeded = true;

            if (!ths.OnPreFailureCheck()) return false;

            bool epicFailure;
            ths.SpellCastingSucceeded = MagicPointControl.IsFailure(ths, intendedType, out epicFailure);
            if (!ths.SpellCastingSucceeded)
            {
                ths.SpellCastingEpiclyFailed = epicFailure;
            }

            ths.StandardEntry();
            ths.BeginCommodityUpdates();

            try
            {
                if (!ths.OnPostFailureCheck()) return false;

                MagicPointControl.UsePoints(ths.Actor, ths.SpellPoints, intendedType);

                if (ths.SpellCastingSucceeded)
                {
                    ths.OnSpellSuccess();
                }
                else if ((ths.EpicFailureAllowed()) && (ths.SpellCastingEpiclyFailed))
                {
                    succeeded = false;
                    ths.OnSpellEpicFailure();
                }
                else
                {
                    succeeded = false;
                    ths.OnSpellFailure();
                }

                EventTracker.SendEvent(EventTypeId.kCastSpell, ths.Actor);
            }
            finally
            {
                ths.EndCommodityUpdates(succeeded);
                ths.StandardExit();
            }

            return succeeded;
        }
    }
}
