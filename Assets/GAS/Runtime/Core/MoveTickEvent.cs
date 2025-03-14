using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GAS.Runtime.Core
{
    public class MoveTickEvent
    {
        public MoveTickEvent(int preMove, int curMove) 
        { 
            PreMove = preMove;
            CurMove = curMove;
        }

        public int PreMove;
        public int CurMove;
    }
}
