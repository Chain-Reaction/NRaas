using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.SelectorSpace
{
    [Persistable]
    public class PersistedSettings
    {
        [Tunable, TunableComment("Whether to show the 'No Interactions' tooltip")]
        protected static bool kShowNoInteractions = true;

        [Tunable, TunableComment("Whether to move all immediate interactions to the Shift-Click Cheat menu")]
        protected static bool kMoveInteractionsToShift = false;

        [Tunable, TunableComment("Whether to allow switching to inactive by right clicking on them")]
        protected static bool kDreamCatcher = true;

        [Tunable, TunableComment("Whether to display debugging messages")]
        protected static bool kDebugging = false;

        public bool mShowNoInteractions = kShowNoInteractions;

        public bool mMoveInteractionsToShift = kMoveInteractionsToShift;

        public bool mDreamCatcher = kDreamCatcher;

        private bool mDebugging = kDebugging;

        public bool Debugging
        {
            get
            {
                return mDebugging;
            }
            set
            {
                mDebugging = value;

                Common.kDebugging = value;
            }
        }
    }
}
