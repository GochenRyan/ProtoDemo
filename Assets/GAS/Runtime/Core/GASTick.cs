using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GAS.Runtime.Core
{
    public class GASTick
    {
        public event EventHandler<TurnTickEvent> TurnTick;
        public event EventHandler<MoveTickEvent> MoveTick;

        private GASTick() { }

        private static GASTick _instance;

        public static GASTick Instance 
        {
            get 
            {
                if ( _instance == null )
                    _instance = new GASTick();
                return _instance;
            } 
        }

        public void NextMove(int preMove, int curMove)
        {
            var e = new MoveTickEvent(preMove, curMove);
            MoveTick.Invoke(this, e);
        }

        public void NextTurn(int preTurn, int curTurn)
        {
            var e = new TurnTickEvent(preTurn, curTurn);
            TurnTick.Invoke(this, e);
        }
    }
}
