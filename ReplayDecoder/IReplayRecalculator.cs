using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayDecoder
{
    public interface IReplayRecalculator
    {
        public Task<(int?, Replay)> RecalculateReplay(Replay replay);
    }
}
