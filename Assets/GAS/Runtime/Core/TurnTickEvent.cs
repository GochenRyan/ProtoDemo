using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GAS.Runtime.Core
{
    public class TurnTickEvent : EventArgs
    {
        public TurnTickEvent(int preTurn, int curTurn) 
        { 
            PreTurn = preTurn;
            CurTurn = curTurn;
        }

        public int PreTurn;
        public int CurTurn;
    }
}
