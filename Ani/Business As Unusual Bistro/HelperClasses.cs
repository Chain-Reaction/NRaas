using Sims3.Gameplay.CAS;
using System.Collections.Generic;
using Sims3.SimIFace;

namespace ani_BistroSet
{
    [Persistable]
    public class Shift
    {
        public Employee Cheff;
        public List<Employee> Waiters;

        public float StarWork;
        public float EndWork;

        public Shift()
        {
            Waiters = new List<Employee>();
        }
    }

    [Persistable]
    public class Employee
    {
        public ulong DescriptionId;
        public int Wage;       

        public Employee()
        {
        }
    }
}
