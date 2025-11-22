using System.Collections.Generic;
using Verse;

namespace VirtuAwake
{
    public class MatrixEventChainExtension : DefModExtension
    {
        public string chainId;
        public int stage;
        public int ticksToNextStage = 1200;
        public int minParticipants = 2;
        public int maxParticipants = 3;
        public List<string> nextEventDefs;
        public bool startNewChain = true;
        public bool chainOnly;
        public float syncLucidityLerp;
        public float syncInstabilityLerp;
        public bool wakeAll;
        public bool forceEject;

        public bool IsChainStart => this.startNewChain || this.stage <= 0;
    }
}
