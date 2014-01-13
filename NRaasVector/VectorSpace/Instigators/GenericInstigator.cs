using NRaas.CommonSpace.Booters;
using NRaas.VectorSpace.Booters;
using NRaas.VectorSpace.Tests;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Instigators
{
    public class GenericInstigator : InstigatorBooter.Data
    {
        public GenericInstigator(XmlDbRow row)
            : base(row, new GenericTest(row))
        {}
    }
}
