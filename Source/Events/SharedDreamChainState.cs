using System.Collections.Generic;
using Verse;

namespace VirtuAwake
{
    public class SharedDreamChainState : IExposable
    {
        public string chainId;
        public List<Pawn> participants = new List<Pawn>();
        public MatrixEventDef currentEvent;
        public int stageIndex;
        public int ticksUntilNext;

        public bool HasParticipants => this.participants != null && this.participants.Count > 0;

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.chainId, "chainId");
            Scribe_Collections.Look(ref this.participants, "participants", LookMode.Reference);
            Scribe_Defs.Look(ref this.currentEvent, "currentEvent");
            Scribe_Values.Look(ref this.stageIndex, "stageIndex");
            Scribe_Values.Look(ref this.ticksUntilNext, "ticksUntilNext", 0);
        }
    }
}
