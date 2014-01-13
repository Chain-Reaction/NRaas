using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckDataDrivenObject : Check<IDataDrivenObject>
    {
        protected override bool PrePerform(IDataDrivenObject obj, bool postLoad)
        {
            return true;
        }

        protected override bool PostPerform(IDataDrivenObject obj, bool postLoad)
        {
            if (ErrorTrap.Loading)
            {
                ErrorTrap.AddObjectOfInterest(obj);
            }

            return true;
        }
    }
}
