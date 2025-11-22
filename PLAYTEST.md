# PLAYTEST: Virtu-Awake MVP (VR Pod)

VR-only checks after the dream pod removal. Focus: VR pod UX, lucidity/instability loop, recreation gain, and sim training.

## Setup
- Enable Dev Mode for quick spawn; start a test colony with at least one VR pod and power.
- Ensure pawns have the lucidity need (enters once a pawn uses a pod) and that instability hediff can appear during VR.
- Keep two humanlikes on the map for cancel/eject checks; add a second pod for social checks if desired.

## Scenarios
- **Solo session flow**
  - Order a pawn into a pod (float menu or gizmo). Benefits unlock after ~15s; a memory roll should fire once around the 20–25s mark.
  - Expected: status gizmo shows the user + timers; lucidity rises by roughly 0.08–0.1 per full session, instability by ~0.02; total joy ~0.18–0.22 (base ticks + one memory).
  - Lucidity should persist after exit and decay slowly (~0.08/day); instability should decay over a few in-game minutes when out of VR.

- **Cancel and force eject controls**
  - Start a session, use **Cancel session** before the pawn lies down: job ends, reservation clears, status gizmo returns to idle.
  - Start again, wait until the pawn is inside, then use **Force eject**: pawn stands and leaves, user list clears, message fires, no lingering jobs or reservations.
  - Requeue another pawn immediately to confirm the pod is free.

- **Training + XP balance**
  - Use a pawn with a major Shooting passion in a combat sim session. Track XP gain (expect ~70–85 XP per full session after tuning).
  - Verify the memory trigger grants the appropriate tiered thought/joy and that passion weighting biases the chosen sim type/skill tier.
  - Repeat with a non-passion pawn to ensure XP still applies but at lower effective weighting.

- **Lucidity/instability progression loop**
  - Run 3–4 back-to-back sessions with short breaks; lucidity should climb toward the 0.55+ threshold, triggering the higher instability gain bonus once crossed.
  - Instability should linger between sessions (decaying ~0.05/day) and climb toward stage 2 with sustained use; note any stage transitions or side effects.

- **Edge checks**
  - Social attempt: queue two pawns into linked pods and ensure both use their own pods without blocking.
  - Destroy/forbid a pod mid-use to confirm the job aborts safely and users are cleared.
  - Downed/mental pawns should be rejected by float menu/JoyGiver; no NREs or stuck jobs.
