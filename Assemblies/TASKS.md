# TASKS.md
AI-Agent Compatible Task Breakdown for **Virtu-Awake**

This document defines actionable tasks for agents and contributors.  
Each task includes: **ID**, **Description**, **Category**, **Dependencies**, **Status**, **Output**, and **Owner**.  
Agents may claim tasks by setting `Owner: AI` and updating `Status`.

---

# Setup & Ops

## TASK-OPS-001
**Description:** Create base repository directory skeleton per CONTRIBUTING.md  
**Category:** Setup  
**Dependencies:** None  
**Status:** DONE  
**Output:** `About/`, `Assemblies/`, `Defs/`, `Languages/`, `Source/` (`Components/`, `Events/`, `Simulation/`, `Traits/`, `Util/`), `Textures/`  
**Owner:** AI

## TASK-OPS-002
**Description:** Scaffold minimal Virtu-Awake baseline (csproj, mod entry, VR pod, job, hediffs, thought, workgiver)  
**Category:** Setup  
**Dependencies:** None  
**Status:** DONE  
**Output:** `About/About.xml`, `Source/VirtuAwake.csproj`, `Source/ModEntry.cs`, `Source/Components/*`, `Source/Simulation/*`, `Defs/*`, `Languages/English/Keyed/VirtuAwake_Strings.xml`  
**Owner:** AI

---

# Bug Fixes & Load Issues

## TASK-BUG-001
**Description:** Fix initial load errors (Hediff root tag, invalid inspector tab, placeholder VR pod texture)  
**Category:** Bugfix  
**Dependencies:** None  
**Status:** DONE  
**Output:** Validated Hediff defs root, VR pod inspector tab corrected, placeholder texture at `Textures/Things/Building/VA_VRPod_Basic.png`  
**Owner:** AI

## TASK-BUG-002
**Description:** Make VR pod usage a recreation/joy activity instead of work  
**Category:** Bugfix  
**Dependencies:** BUG-001  
**Status:** DONE  
**Output:** Joy giver def for VR pod; work giver removed/disabled; pawns use pods via recreation schedule  
**Owner:** AI

## TASK-BUG-003
**Description:** Allow right-clicking the VR pod to start a joy session  
**Category:** Bugfix  
**Dependencies:** BUG-002  
**Status:** DONE  
**Output:** VR pod float-menu option that queues VA_UseVRPod job  
**Owner:** AI

---

# �sT Core Systems

## TASK-CORE-001
**Description:** Implement VR Pod building + basic recreation job driver  
**Category:** C# / Core Gameplay  
**Dependencies:** None  
**Status:** DONE  
**Output:** `Building_VRPod`, `JobDriver_EnterVRPod`, XML Defs  
**Owner:** AI

## TASK-CORE-002
**Description:** Implement Virtu-Dream Pod (long-term immersion)  
**Category:** C# / Building System  
**Dependencies:** CORE-001  
**Status:** DONE  
**Output:** `Building_VirtuDreamPod`  
**Owner:** AI

## TASK-CORE-003
**Description:** Create XML defs for all VR buildings  
**Category:** XML / Defs  
**Dependencies:** CORE-001, CORE-002  
**Status:** TODO  
**Output:** `ThingDefs_Buildings_VR.xml`  
**Owner:** Unassigned

---

# �Y�� Psychological Systems

## TASK-PSY-001
**Description:** Implement Need_Lucidity  
**Category:** C# / Needs  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** `Need_Lucidity.cs`  
**Owner:** Unassigned

## TASK-PSY-002
**Description:** Implement Hediff_Instability (3�?"4 stages)  
**Category:** C# / Hediffs  
**Dependencies:** PSY-001  
**Status:** TODO  
**Output:** `Hediff_Instability.cs`  
**Owner:** Unassigned

## TASK-PSY-003
**Description:** Link pod usage ��' Lucidity + Instability progression  
**Category:** C#  
**Dependencies:** PSY-001, PSY-002  
**Status:** TODO  
**Output:** VRTick() behaviour  
**Owner:** Unassigned

## TASK-PSY-004
**Description:** Create XML ThoughtDefs for awakening tiers  
**Category:** XML  
**Dependencies:** PSY-001, PSY-002  
**Status:** TODO  
**Output:** `ThoughtDefs_VR_Awakening.xml`  
**Owner:** Unassigned

---

# �YO? VR Event System

## TASK-EVT-001
**Description:** Create MapComponent_MatrixEvents (VirtuAwakeEvents)  
**Category:** C# / Events  
**Dependencies:** PSY-001, PSY-002  
**Status:** TODO  
**Output:** `MapComponent_VirtuAwakeEvents.cs`  
**Owner:** Unassigned

## TASK-EVT-002
**Description:** Implement event severity selection (Minor, Moderate, Major, Crisis)  
**Category:** C# / RNG  
**Dependencies:** EVT-001  
**Status:** TODO  
**Output:** `MatrixEventSeverity` enum, selection logic  
**Owner:** AI

## TASK-EVT-003
**Description:** Implement event definitions (MatrixEventDef)  
**Category:** XML + C# Def  
**Dependencies:** EVT-002  
**Status:** TODO  
**Output:** `MatrixEventDef`, XML registry  
**Owner:** Unassigned

## TASK-EVT-004
**Description:** Implement event resolution handler (apply memories, adjust Instability/Lucidity)  
**Category:** C#  
**Dependencies:** EVT-003  
**Status:** TODO  
**Output:** `ApplyMatrixEventEffects()`  
**Owner:** Unassigned

## TASK-EVT-005
**Description:** Add catastrophic multi-pod glitch events  
**Category:** C#  
**Dependencies:** EVT-004  
**Status:** TODO  
**Output:** `Event_CatastrophicGlitch()`  
**Owner:** Unassigned

---

# �Y�� VR Sim Types & Content

## TASK-SIM-001
**Description:** Create SimType definitions (Combat, Artistic, Nature, Meditation, etc.)  
**Category:** XML  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** `VirtuSimTypes.xml`  
**Owner:** Unassigned

## TASK-SIM-002
**Description:** Implement C# handler for SimType-driven XP  
**Category:** C#  
**Dependencies:** SIM-001  
**Status:** TODO  
**Output:** `VRSimUtility.ApplySimTraining()`  
**Owner:** AI

## TASK-SIM-003
**Description:** Add Social VR (two-pawn shared simulation)  
**Category:** C#  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** `JobDriver_EnterSocialVR`  
**Owner:** Unassigned

## TASK-SIM-004
**Description:** Embed social VR memories  
**Category:** XML  
**Dependencies:** SIM-003  
**Status:** TODO  
**Output:** `ThoughtDefs_VR_Social.xml`  
**Owner:** Unassigned

---

# �Y�� Trait-Based Memories

## TASK-TRT-001
**Description:** Implement trait-aware VR memory chooser  
**Category:** C#  
**Dependencies:** EVT-004  
**Status:** TODO  
**Output:** `VRMemorySelector.GetMemoryFor(pawn, event)`  
**Owner:** AI

## TASK-TRT-002
**Description:** Add XML memories for each trait  
**Category:** XML  
**Dependencies:** TRT-001  
**Status:** TODO  
**Output:** `ThoughtDefs_VR_Traits.xml`  
**Owner:** Unassigned

## TASK-TRT-003
**Description:** Add dual-trait legendary memories  
**Category:** XML  
**Dependencies:** TRT-002  
**Status:** TODO  
**Output:** `ThoughtDefs_VR_Traits_Dual.xml`  
**Owner:** Unassigned

## TASK-TRT-004
**Description:** Add triple-trait legendary memories  
**Category:** XML  
**Dependencies:** TRT-003  
**Status:** TODO  
**Output:** `ThoughtDefs_VR_Traits_Triple.xml`  
**Owner:** Unassigned

---

# �Y'� Breakout System

## TASK-BRK-001
**Description:** Implement Breakout conditions (Lucidity + Instability thresholds)  
**Category:** C#  
**Dependencies:** PSY-001, PSY-002, EVT-004  
**Status:** TODO  
**Output:** `ShouldBreakOut()`  
**Owner:** Unassigned

## TASK-BRK-002
**Description:** Implement mental states for escape attempts  
**Category:** C#  
**Dependencies:** BRK-001  
**Status:** TODO  
**Output:** `MentalState_SimulationBreaker`, `MentalState_Derealisation`  
**Owner:** Unassigned

## TASK-BRK-003
**Description:** Implement breakout sequence (forced eject + behaviour tree)  
**Category:** C#  
**Dependencies:** BRK-002  
**Status:** TODO  
**Output:** `StartBreakoutSequence()`  
**Owner:** AI

---

# �Y�� Lore & Polish

## TASK-LORE-001
**Description:** Add lore text for pods, glitch logs, ancient messages  
**Category:** XML  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** `Keyed/VirtuAwake_Lore.xml`  
**Owner:** Unassigned

## TASK-LORE-002
**Description:** Add flavour text for catastrophic events  
**Category:** XML  
**Dependencies:** EVT-005  
**Status:** TODO  
**Output:** `Keyed/VirtuAwake_GlitchDescriptions.xml`  
**Owner:** Unassigned

---

# �YZ� Art & Visuals

## TASK-ART-001
**Description:** Create RimWorld-style VR Pod sprites  
**Category:** Art  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** `/Textures/Buildings/VRPod.png`  
**Owner:** Unassigned

## TASK-ART-002
**Description:** Create Virtu-Dream Pod sprites  
**Category:** Art  
**Dependencies:** CORE-002  
**Status:** TODO  
**Output:** `/Textures/Buildings/VirtuDreamPod.png`  
**Owner:** Unassigned

## TASK-ART-003
**Description:** Add glitch overlay FX (optional)  
**Category:** Art  
**Dependencies:** EVT-002  
**Status:** TODO  
**Output:** `/Textures/FX/Glitch_*`  
**Owner:** Unassigned

---

# �Y"� Release & Packaging

## TASK-REL-001
**Description:** Create About folder metadata  
**Category:** Packaging  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** `/About/About.xml`  
**Owner:** AI

## TASK-REL-002
**Description:** Publish GitHub release automation  
**Category:** DevOps  
**Dependencies:** REL-001  
**Status:** TODO  
**Output:** GitHub Actions workflow  
**Owner:** Unassigned

## TASK-REL-003
**Description:** Prepare Steam Workshop description + imagery  
**Category:** Documentation / Marketing  
**Dependencies:** REL-001  
**Status:** TODO  
**Output:** `SteamDescription.txt`, banner artwork  
**Owner:** Unassigned

---

# �o" Rules for AI Agents

- Agents may claim tasks by updating:
  - `Owner: AI`
  - `Status: IN_PROGRESS`
- All commits must be atomic and reference task IDs (e.g., `TASK-CORE-001`).
- Avoid rewriting unrelated files unless refactoring is required.
- When completing tasks, set:
  - `Status: DONE`
  - Add PR link under task.

---

# �YOY End of Task Spec
This TASKS.md is living documentation.  
Update it with new features, fixes, and discoveries as Virtu-Awake evolves.
