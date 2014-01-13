using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace
{
    public interface IMagicalInteraction : ISpellPointInteraction
    {
        Sim Actor
        {
            get;
        }

        Sim Target
        {
            get;
        }

        OccultTypes IntendedType
        {
            get;
        }

        int SpellLevel
        {
            get;
        }

        void StandardEntry();
        void StandardExit();

        void BeginCommodityUpdates();
        void EndCommodityUpdates(bool succeeded);

        bool EpicFailureAllowed();

        bool OnPreFailureCheck();
        bool OnPostFailureCheck();

        void OnSpellSuccess();
        void OnSpellEpicFailure();
        void OnSpellFailure();

        bool SpellCastingSucceeded
        {
            get;
            set;
        }

        bool SpellCastingEpiclyFailed
        {
            get;
            set;
        }
    }
}
