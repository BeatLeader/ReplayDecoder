using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayDecoder
{
    public enum ScoringType
    {
        Default,
        Ignore,
        NoScore,
        Normal,
        SliderHead,
        SliderTail,
        BurstSliderHead,
        BurstSliderElement
    }

    public class NoteParams
    {
        public ScoringType scoringType;
        public int lineIndex;
        public int noteLineLayer;
        public int colorType;
        public int cutDirection;

        public NoteParams(int noteId)
        {
            int id = noteId;
            if (id < 100000) {
                scoringType = (ScoringType)(id / 10000);
                id -= (int)scoringType * 10000;

                lineIndex = id / 1000;
                id -= lineIndex * 1000;

                noteLineLayer = id / 100;
                id -= noteLineLayer * 100;

                colorType = id / 10;
                cutDirection = id - colorType * 10;
            } else {
                scoringType = (ScoringType)(id / 10000000);
                id -= (int)scoringType * 10000000;

                lineIndex = id / 1000000;
                id -= lineIndex * 1000000;

                noteLineLayer = id / 100000;
                id -= noteLineLayer * 100000;

                colorType = id / 10;
                cutDirection = id - colorType * 10;
            }
        }
    }
}
