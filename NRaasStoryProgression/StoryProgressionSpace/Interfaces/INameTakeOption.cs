using Sims3.Gameplay.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interfaces
{
    public interface INameTakeOption
    {
        NameTakeType Value
        {
            get;
        }

        string GetNameTakeLocalizationValueKey();
    }
}

