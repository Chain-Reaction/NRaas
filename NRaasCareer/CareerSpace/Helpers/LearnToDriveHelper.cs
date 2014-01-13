using NRaas.CommonSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CareerSpace.Helpers
{
    public class LearnToDriveHelper : Common.IAddInteraction
    {
        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddNoDupTest<SchoolRabbitHole>(new RabbitHole.AttendClassInRabbitHole.Definition(SkillNames.LearnToDrive, SchoolRabbitHole.kClassPaintingCost, SchoolRabbitHole.kClassPaintingDuration));
        }
    }
}
