using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.RetunerSpace.Booters
{
    public class InteractionBooter : BySeasonBooter<SeasonSettings.ITUNSettings>
    {
        public InteractionBooter()
            : base("ITUN", "RetunerFile", "Retuners")
        { }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            string interactionName = row.GetString("InteractionName");
            if (string.IsNullOrEmpty(interactionName))
            {
                BooterLogger.AddError("InteractionName missing");
                return;
            }

            string objectName = row.GetString("ObjectName");
            if (string.IsNullOrEmpty(objectName))
            {
                BooterLogger.AddError("ObjectName missing");
                return;
            }

            Add(ParseKey(row), new SeasonSettings.ITUNSettings(Retuner.TuningName(objectName, interactionName), row));
        }
    }
}
