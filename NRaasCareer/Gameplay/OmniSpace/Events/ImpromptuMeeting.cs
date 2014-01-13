using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.OmniSpace.Events
{
    public class ImpromptuMeeting : Career.EventDaily
    {
        // Methods
        public ImpromptuMeeting()
        {
        }

        public ImpromptuMeeting(XmlDbRow row, Dictionary<string, Dictionary<int, CareerLevel>> careerLevels, string careerName) 
            : base(row, careerLevels, Career.Event.DisplayTypes.TNS)
        {
        }

        public static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, name, "Gameplay/Careers/Business/ImpromptuMeeting:" + name, parameters);
        }

        public override void RunEvent(Career c)
        {
            ObjectGuid simObjectId = new ObjectGuid();
            if (c.OwnerDescription.CreatedSim != null)
            {
                simObjectId = c.OwnerDescription.CreatedSim.ObjectId;
            }
            base.Display(LocalizeString(c.OwnerDescription, "ImpromptuMeetingEvent", new object[] { c.OwnerDescription }), simObjectId, c);

            Business business = OmniCareer.Career<Business>(c);
            if (business != null)
            {
                business.MeetingsHeldToday++;
            }
        }
    }
}
