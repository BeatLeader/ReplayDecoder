namespace ReplayDecoder
{
    public enum ScoringType
    {
        Default,
        Ignore,
        NoScore,
        Normal,
        ArcHead,
        ArcTail,
        ChainHead,
        ChainLink,
        ArcHeadArcTail,
        ChainHeadArcTail,
        ChainLinkArcHead,
        ChainHeadArcHead,
        ChainHeadArcHeadArcTail,
    }

    public class NoteScoreDefinition
    {
        public readonly int maxCenterDistanceCutScore;
        public readonly int minBeforeCutScore;
        public readonly int maxBeforeCutScore;
        public readonly int minAfterCutScore;
        public readonly int maxAfterCutScore;
        public readonly int fixedCutScore;

        public int maxCutScore => this.maxCenterDistanceCutScore + this.maxBeforeCutScore + this.maxAfterCutScore + this.fixedCutScore;

        public int executionOrder => this.maxCutScore;

        public bool accApplicable => this.maxCenterDistanceCutScore > 0 && this.fixedCutScore == 0;
        public bool beforeCutApplicable => this.minBeforeCutScore < this.maxBeforeCutScore;
        public bool afterCutApplicable => this.minAfterCutScore > this.maxAfterCutScore;

        public NoteScoreDefinition(
            int maxCenterDistanceCutScore,
            int minBeforeCutScore,
            int maxBeforeCutScore,
            int minAfterCutScore,
            int maxAfterCutScore,
            int fixedCutScore)
        {
            this.maxCenterDistanceCutScore = maxCenterDistanceCutScore;
            this.minBeforeCutScore = minBeforeCutScore;
            this.maxBeforeCutScore = maxBeforeCutScore;
            this.minAfterCutScore = minAfterCutScore;
            this.maxAfterCutScore = maxAfterCutScore;
            this.fixedCutScore = fixedCutScore;
        }
    }

    public static class ScoringExtensions
    {
        public static readonly Dictionary<ScoringType, NoteScoreDefinition> ScoreDefinitions = new Dictionary<ScoringType, NoteScoreDefinition>()
        {
            {
                ScoringType.Ignore,
                (NoteScoreDefinition) null
            },
            {
                ScoringType.NoScore,
                new NoteScoreDefinition(0, 0, 0, 0, 0, 0)
            },
            {
                ScoringType.Normal,
                new NoteScoreDefinition(15, 0, 70, 0, 30, 0)
            },
            {
                ScoringType.ArcHead,
                new NoteScoreDefinition(15, 0, 70, 30, 30, 0)
            },
            {
                ScoringType.ArcTail,
                new NoteScoreDefinition(15, 70, 70, 0, 30, 0)
            },
            {
                ScoringType.ChainHead,
                new NoteScoreDefinition(15, 0, 70, 0, 0, 0)
            },
            {
                ScoringType.ChainLink,
                new NoteScoreDefinition(0, 0, 0, 0, 0, 20)
            },
            {
                ScoringType.ArcHeadArcTail,
                new NoteScoreDefinition(15, 70, 70, 30, 30, 0)
            },
            {
                ScoringType.ChainHeadArcTail,
                new NoteScoreDefinition(15, 70, 70, 30, 30, 0)
            },
            {
                ScoringType.ChainLinkArcHead,
                new NoteScoreDefinition(0, 0, 0, 0, 0, 20)
            },
            {
                ScoringType.ChainHeadArcHead,
                new NoteScoreDefinition(15, 0, 70, 30, 30, 0)
            },
            {
                ScoringType.ChainHeadArcHeadArcTail,
                new NoteScoreDefinition(15, 70, 70, 30, 30, 0)
            }
        };
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
