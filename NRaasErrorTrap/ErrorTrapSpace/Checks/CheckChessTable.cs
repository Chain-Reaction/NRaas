using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ErrorTrapSpace.Checks
{
    public class CheckChessTable : Check<ChessTable>
    {
        protected override bool PrePerform(ChessTable table, bool postLoad)
        {
            if ((table.mSimsPlayingForChessRank != null) && (table.mSimsPlayingForChessRank.Length != 2))
            {
                table.mSimsPlayingForChessRank = new Sim[2];
            }

            return true;
        }
    }
}
