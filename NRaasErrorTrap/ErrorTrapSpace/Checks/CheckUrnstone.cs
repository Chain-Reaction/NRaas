using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckUrnstone : Check<Urnstone>
    {
        protected override bool PrePerform(Urnstone book, bool postLoad)
        {
            ErrorTrap.CheckTravelData();
            return true;
        }
    }
}
