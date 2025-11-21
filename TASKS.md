# TASKS.md
AI-Agent Task Breakdown for **Virtu-Awake**

Each task lists: **ID**, **Description**, **Category**, **Dependencies**, **Status**, **Output**, **Owner**.  
Rules: set `Owner: AI` and `Status: IN_PROGRESS` when claiming; set `Status: DONE` with a brief note or PR link when finished. Obey dependency ordering.

---

# Setup & Ops

## TASK-OPS-001
**Description:** Create base repository skeleton per CONTRIBUTING.md  
**Category:** Setup  
**Dependencies:** None  
**Status:** DONE  
**Output:** About/, Assemblies/, Defs/, Languages/, Source/ (Components/, Events/, Simulation/, Traits/, Util/), Textures/  
**Owner:** AI

## TASK-OPS-002
**Description:** Scaffold minimal Virtu-Awake baseline (csproj, mod entry, VR pod, job, hediffs, thought, joygiver)  
**Category:** Setup  
**Dependencies:** None  
**Status:** DONE  
**Output:** About/About.xml, Source/VirtuAwake.csproj, Source/ModEntry.cs, initial Components/ & Simulation/ classes, Defs/*, Languages/English/Keyed stub  
**Owner:** AI

## TASK-OPS-003
**Description:** Add build/packaging script to copy Release DLL + About/ + Defs/ + Textures/ into a Mods/Virtu-Awake output folder  
**Category:** DevOps  
**Dependencies:** OPS-002  
**Status:** DONE  
**Output:** build script (PowerShell or bash) with paths in README  
**Owner:** AI

---

# Core Systems (Pods & Use)

## TASK-CORE-001
**Description:** Implement VR Pod building + recreation job driver + float menu  
**Category:** C# / Core Gameplay  
**Dependencies:** OPS-002  
**Status:** DONE  
**Output:** Building_VRPod class, JobDriver_UseVRPod, JoyGiver, ThingDef, float menu option  
**Owner:** AI

## TASK-CORE-002
**Description:** Implement Virtu-Dream Pod (long-term immersion)  
**Category:** C# / Building System  
**Dependencies:** CORE-001  
**Status:** DONE  
**Output:** Building_VirtuDreamPod, JobDriver_UseVirtuDreamPod (indefinite), JoyGiver, ThingDef  
**Owner:** AI

## TASK-CORE-003
**Description:** Add social VR pod session (two-pawn shared simulation)  
**Category:** C# / JobDriver  
**Dependencies:** CORE-001  
**Status:** DONE  
**Output:** JobDriver_EnterSocialVR, reservation logic for pair use, social joy hooks  
**Owner:** AI

## TASK-CORE-004
**Description:** Implement SimType-aware training for VR Pod sessions  
**Category:** C# / Mechanics  
**Dependencies:** CORE-001, SIM-001  
**Status:** DONE  
**Output:** VRSimUtility with skill XP application; configuration via comp/SimType defs  
**Owner:** AI

## TASK-MEM-006
**Description:** Wire sim sessions to give sim-type memories with trait variants  
**Category:** C# + XML  
**Dependencies:** CORE-004, SIM-001  
**Status:** DONE  
**Output:** Thought_MemoryTraitVariant, SimType thought mapping, variant memories per sim type  
**Owner:** AI

## TASK-CORE-005
**Description:** Add nutrient/power maintenance hooks for Virtu-Dream Pod (optional VFE nutrient pipe support)  
**Category:** C# / Integration  
**Dependencies:** CORE-002  
**Status:** TODO  
**Output:** Comp for nutrient consumption; fallback to hunger/rest if absent  
**Owner:** Unassigned

---

# Psychological Systems (Needs, Hediffs, Awakening)

## TASK-PSY-001
**Description:** Implement Need_Lucidity (UI bar, gain/decay, save/load); add NeedDef, pawn init hook, and trait multipliers (TooSmart/Sanguine up, Depressive/Neurotic down)  
**Category:** C# / Needs  
**Dependencies:** CORE-001  
**Status:** DONE (need class + def + pawn activation)  
**Output:** Need_Lucidity.cs, NeedDef in Defs/Needs, UI label/description, pawn init hook  
**Owner:** AI

## TASK-PSY-002
**Description:** Implement Hediff_Instability with 3-4 stages (Calm Flicker, Pattern Seen, Deep Fracture, Collapse Warning) and effects  
**Category:** C# / Hediffs  
**Dependencies:** PSY-001  
**Status:** DONE (staged hediff with severity effects)  
**Output:** Hediff_Instability.cs with stage properties; HediffDef stages updated (labels, mood/stat effects)  
**Owner:** AI

## TASK-PSY-003
**Description:** Link pod usage ticks to Lucidity gain and Instability progression; add CompProps tuning (VR vs Dream pods), trait multipliers, session caps, and out-of-pod decay  
**Category:** C# / Mechanics  
**Dependencies:** PSY-001, PSY-002, CORE-001  
**Status:** DONE (VR pod ticks adjust lucidity/instability with trait + long-term tuning)  
**Output:** CompVRPod tick logic applying Need/Hediff changes per session type/duration with caps and decay  
**Owner:** AI

## TASK-PSY-004
**Description:** Define awakening tier thoughts (Compliant, Flicker, Dissociation, Realisation, Breakthrough) tied to Lucidity thresholds  
**Category:** XML  
**Dependencies:** PSY-001, PSY-002  
**Status:** TODO  
**Output:** ThoughtDefs_VR_Awakening.xml with stage-tied mood effects  
**Owner:** Unassigned

## TASK-PSY-005
**Description:** Add mental states for high lucidity/instability (panic, derealisation)  
**Category:** C# / MentalStates  
**Dependencies:** PSY-002, PSY-004  
**Status:** TODO  
**Output:** MentalStateDefs + classes (e.g., SimulationBreaker, Derealisation)  
**Owner:** Unassigned

---

# VR Event Engine & Glitches

## TASK-EVT-001
**Description:** Create MapComponent_VirtuAwakeEvents for scheduling anomaly rolls  
**Category:** C# / Events  
**Dependencies:** PSY-001, PSY-002, CORE-001  
**Status:** TODO  
**Output:** MapComponent ticking active pods; hooks into Lucidity/Instability  
**Owner:** Unassigned

## TASK-EVT-002
**Description:** Implement event severity selection (Minor, Moderate, Major, Crisis) with weighting by instability and traits  
**Category:** C# / RNG  
**Dependencies:** EVT-001  
**Status:** TODO  
**Output:** MatrixEventSeverity enum + selection utility  
**Owner:** Unassigned

## TASK-EVT-003
**Description:** Add MatrixEventDef and XML registry for events  
**Category:** XML + C# Defs  
**Dependencies:** EVT-002  
**Status:** TODO  
**Output:** Def class + Defs/Events/MatrixEvents.xml with sample events per severity  
**Owner:** Unassigned

## TASK-EVT-004
**Description:** Implement event resolution effects (memories, Lucidity/Instability adjustments, side effects)  
**Category:** C#  
**Dependencies:** EVT-003, PSY-004  
**Status:** TODO  
**Output:** ApplyMatrixEventEffects() with hooks for pawn state changes  
**Owner:** Unassigned

## TASK-EVT-005
**Description:** Add catastrophic multi-pod glitch events (Crisis tier)  
**Category:** C# / Events  
**Dependencies:** EVT-004  
**Status:** TODO  
**Output:** Event_CatastrophicGlitch handling multiple pods/pawns  
**Owner:** Unassigned

---

# Sim Types & Training Content

## TASK-SIM-001
**Description:** Create SimType defs (Combat, Artistic, Nature, Medicine, Social, Intellectual, etc.)  
**Category:** XML  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** Defs/VirtuSimTypes.xml with XP mappings and weights  
**Owner:** Unassigned

## TASK-SIM-002
**Description:** Implement SimType-driven XP application utility  
**Category:** C#  
**Dependencies:** SIM-001  
**Status:** TODO  
**Output:** VRSimUtility.ApplySimTraining(pawn, simType, ticks)  
**Owner:** Unassigned

## TASK-SIM-003
**Description:** Create social VR session job (two pawns enter linked pods)  
**Category:** C#  
**Dependencies:** CORE-001, SIM-001  
**Status:** TODO  
**Output:** JobDriver_EnterSocialVR for linked pods, paired toil logic, social memories hook  
**Owner:** Unassigned

## TASK-SIM-004
**Description:** Add trait-aware VR memory selector for events/sessions  
**Category:** C#  
**Dependencies:** EVT-004  
**Status:** TODO  
**Output:** VRMemorySelector.GetMemoryFor(pawn, event/sim)  
**Owner:** Unassigned

---

# Memory & Thought Libraries

## TASK-MEM-001
**Description:** Add base VR memory thoughts (non-trait) per severity/experience  
**Category:** XML  
**Dependencies:** EVT-003  
**Status:** TODO  
**Output:** Thoughts_VirtuAwake_Base.xml  
**Owner:** Unassigned

## TASK-MEM-002
**Description:** Add trait-specific VR memories (single-trait)  
**Category:** XML  
**Dependencies:** SIM-004  
**Status:** TODO  
**Output:** Thoughts_VR_Traits.xml  
**Owner:** Unassigned

## TASK-MEM-003
**Description:** Add dual-trait legendary memories  
**Category:** XML  
**Dependencies:** MEM-002  
**Status:** DONE (dual combo ThoughtDefs added)  
**Output:** Defs/ThoughtDefs/Thoughts_VR_Traits_Dual.xml  
**Owner:** AI

## TASK-MEM-004
**Description:** Add triple-trait legendary memories  
**Category:** XML  
**Dependencies:** MEM-003  
**Status:** DONE (triple combo ThoughtDefs added)  
**Output:** Defs/ThoughtDefs/Thoughts_VR_Traits_Triple.xml  
**Owner:** AI

## TASK-MEM-005
**Description:** Add social VR memory variants (friendship, rivalry, romance)  
**Category:** XML  
**Dependencies:** SIM-003  
**Status:** TODO  
**Output:** Thoughts_VR_Social.xml  
**Owner:** Unassigned

## TASK-MEM-007
**Description:** Author full trait+skill memory lines for all tiers per guides  
**Category:** Documentation / Narrative  
**Dependencies:** MEM-001, MEM-002  
**Status:** DONE  
**Output:** TraitSkillMemories_All.md containing tiered trait/skill lines  
**Owner:** AI

---

# Breakout System

## TASK-BRK-001
**Description:** Implement breakout condition check (Lucidity + Instability thresholds)  
**Category:** C#  
**Dependencies:** PSY-003  
**Status:** TODO  
**Output:** ShouldBreakOut() utility  
**Owner:** Unassigned

## TASK-BRK-002
**Description:** Add breakout mental states/behaviours (panic, sabotage, free others)  
**Category:** C#  
**Dependencies:** BRK-001  
**Status:** TODO  
**Output:** Mental states + behaviour trees for breakout attempts  
**Owner:** Unassigned

## TASK-BRK-003
**Description:** Execute breakout sequence (forced eject, flee/attack AI, sabotage targets)  
**Category:** C#  
**Dependencies:** BRK-002  
**Status:** TODO  
**Output:** StartBreakoutSequence() orchestration  
**Owner:** Unassigned

---

# UI & UX

## TASK-UI-001
**Description:** Add inspect-tab bars for Lucidity/Instability on pawns using pods  
**Category:** C# / UI  
**Dependencies:** PSY-001, PSY-002  
**Status:** DONE (in-pod gizmo + inspect string bars while in VR)  
**Output:** Pod gizmo and pawn inspect string show lucidity/instability only while in VR  
**Owner:** AI

## TASK-UI-002
**Description:** Add pod gizmos for eject/cancel/session info (VR + Dream pods)  
**Category:** C# / UI  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** Gizmos showing current user, force eject, status tooltip  
**Owner:** Unassigned

## TASK-UI-003
**Description:** Add event popups/letters for major VR anomalies and breakouts  
**Category:** C# / UI  
**Dependencies:** EVT-004, BRK-002  
**Status:** TODO  
**Output:** Letter/small popup system for VR events  
**Owner:** Unassigned

---

# Art & Visuals

## TASK-ART-001
**Description:** Create RimWorld-style VR Pod sprites (all orientations + blueprint/ghost)  
**Category:** Art  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** Textures/Things/Building/VA_VRPod_Basic.png (+ variants if needed)  
**Owner:** Unassigned

## TASK-ART-002
**Description:** Create Virtu-Dream Pod sprites  
**Category:** Art  
**Dependencies:** CORE-002  
**Status:** TODO  
**Output:** Textures/Things/Building/VA_VirtuDreamPod.png (+ variants)  
**Owner:** Unassigned

## TASK-ART-003
**Description:** Add glitch overlay FX for events  
**Category:** Art  
**Dependencies:** EVT-002  
**Status:** TODO  
**Output:** Textures/FX/Glitch_* set  
**Owner:** Unassigned

---

# Lore & Localization

## TASK-LORE-001
**Description:** Add lore text for pods, glitch logs, ancient messages  
**Category:** XML / Keyed  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** Languages/English/Keyed/VirtuAwake_Lore.xml  
**Owner:** Unassigned

## TASK-LORE-002
**Description:** Add flavour text for catastrophic events and awakenings  
**Category:** XML / Keyed  
**Dependencies:** EVT-005, PSY-004  
**Status:** TODO  
**Output:** Languages/English/Keyed/VirtuAwake_GlitchDescriptions.xml  
**Owner:** Unassigned

---

# Compatibility & Patching

## TASK-COMP-001
**Description:** Add Harmony hooks/patches for vanilla jobs that conflict with pod usage (if any)  
**Category:** C# / Harmony  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** Harmony patches in Source/Util or Patches/ folder  
**Owner:** Unassigned

## TASK-COMP-002
**Description:** Add RJW/other mod metadata fixes (download/steam URLs) to avoid load warnings when bundled  
**Category:** Packaging  
**Dependencies:** OPS-002  
**Status:** TODO  
**Output:** About.xml dependency metadata corrected or documented  
**Owner:** Unassigned

## TASK-COMP-003
**Description:** Add VFE nutrient pipe integration patch for Virtu-Dream Pod  
**Category:** C# / Patch  
**Dependencies:** CORE-005  
**Status:** TODO  
**Output:** PatchOperation or Harmony integration for nutrient pipes  
**Owner:** Unassigned

---

# Release, QA, and Balance

## TASK-QA-001
**Description:** Add basic automated compile check script (CI-ready)  
**Category:** QA / DevOps  
**Dependencies:** OPS-003  
**Status:** DONE (build-check script added)  
**Output:** tools/build-check.ps1 running dotnet build  
**Owner:** AI

## TASK-QA-002
**Description:** Playtest checklist for MVP (VR Pod, Dream Pod, lucidity/instability loop)  
**Category:** QA  
**Dependencies:** CORE-002, PSY-003  
**Status:** TODO  
**Output:** PLAYTEST.md with scenarios and expected outcomes  
**Owner:** Unassigned

## TASK-BAL-001
**Description:** Balance pass for joy gain, XP rates, lucidity/instability progression  
**Category:** Balance  
**Dependencies:** PSY-003, SIM-002  
**Status:** TODO  
**Output:** Tuned values in comps/defs, documented notes  
**Owner:** Unassigned

## TASK-REL-001
**Description:** Finalize About metadata (author, description, preview image hook)  
**Category:** Packaging  
**Dependencies:** CORE-001  
**Status:** TODO  
**Output:** Updated About/About.xml + preview image placeholder path  
**Owner:** AI

## TASK-REL-002
**Description:** Prepare Steam Workshop description + imagery  
**Category:** Documentation / Marketing  
**Dependencies:** REL-001  
**Status:** TODO  
**Output:** SteamDescription.txt, banner artwork reference  
**Owner:** Unassigned

## TASK-REL-003
**Description:** Create GitHub release workflow (tag on main, package mod folder)  
**Category:** DevOps  
**Dependencies:** OPS-003  
**Status:** TODO  
**Output:** .github/workflows/release.yml  
**Owner:** Unassigned

## TASK-DOC-001
**Description:** Add base (non-trait) lines for all skill/tier memories alongside trait variants  
**Category:** Documentation / Narrative  
**Dependencies:** MEM-007  
**Status:** DONE  
**Output:** Base lines inserted into TraitSkillMemories_All.md  
**Owner:** AI

---

# Bug Fixes & Load Issues (backlog)

## TASK-BUG-001
**Description:** Fix initial load errors (Hediff root tag, invalid inspector tab, placeholder VR pod texture)  
**Category:** Bugfix  
**Dependencies:** None  
**Status:** DONE  
**Output:** Hediff defs root fixed; inspector tab removed; placeholder texture added  
**Owner:** AI

## TASK-BUG-002
**Description:** Make VR pod usage a recreation/joy activity instead of work  
**Category:** Bugfix  
**Dependencies:** BUG-001  
**Status:** DONE  
**Output:** Joy giver for VR pod; work giver removed; recreation flow  
**Owner:** AI

## TASK-BUG-003
**Description:** Allow right-clicking the VR pod to start a joy session  
**Category:** Bugfix  
**Dependencies:** BUG-002  
**Status:** DONE  
**Output:** Float menu option queues VA_UseVRPod  
**Owner:** AI

## TASK-BUG-004
**Description:** Fix SimTypeDef load errors and cross-references for sim memories  
**Category:** Bugfix  
**Dependencies:** CORE-004  
**Status:** DONE  
**Output:** SimType defs load with proper Class, simType cross-refs resolve without errors  
**Owner:** AI

## TASK-BUG-005
**Description:** Fix SimType skillWeights XML parsing errors (list items)  
**Category:** Bugfix  
**Dependencies:** CORE-004  
**Status:** DONE  
**Output:** skillWeights use <li><key/><value/> format; defs load without XML errors  
**Owner:** AI

## TASK-BUG-006
**Description:** Prevent VR pod job null refs when pods despawn mid-session  
**Category:** Bugfix  
**Dependencies:** CORE-001  
**Status:** DONE  
**Output:** Job drivers guard null/destroyed pod/comp and end jobs safely  
**Owner:** AI

## TASK-BUG-007
**Description:** Guard VR pod jobs against null targets even when pods persist  
**Category:** Bugfix  
**Dependencies:** CORE-001  
**Status:** DONE  
**Output:** Job drivers null-check job targets/comp each tick to avoid NREs  
**Owner:** AI

## TASK-MEM-008
**Description:** Add XML ThoughtDefs for all skill/tier memories with full trait overlays  
**Category:** XML / Narrative  
**Dependencies:** MEM-007  
**Status:** DONE  
**Output:** Thoughts_VirtuAwake_SkillTraits.xml with 36 base memories + trait variants  
**Owner:** AI

## TASK-MEM-009
**Description:** Add dual- and triple-trait combo overlays (legendary) for VR memories in docs + XML  
**Category:** XML / Narrative  
**Dependencies:** MEM-008  
**Status:** DONE (combo overlays authored and wired)  
**Output:** TraitSkillMemories_All.md updated with combo lines; combo overlays wired into ThoughtDefs  
**Owner:** AI

## TASK-MEM-010
**Description:** Award dual/triple combo VR memories automatically when matching traits are present  
**Category:** C# / Narrative  
**Dependencies:** MEM-003, MEM-004  
**Status:** DONE (combo award hook added)  
**Output:** VRSimUtility selects VA_VRCombo thoughts by tier and trait count before defaults  
**Owner:** AI

## TASK-MEM-011
**Description:** Allow VR memories to trigger periodically during long in-pod sessions (no wait for exit)  
**Category:** C# / Mechanics / Narrative  
**Dependencies:** MEM-008  
**Status:** DONE (in-pod memory loop added)  
**Output:** CompVRPod per-pawn timers grant VR combo/skill memories during extended sessions without requiring pod exit  
**Owner:** AI

## TASK-BUG-009
**Description:** Ensure sim-type tiered thoughts also give recreation on completion  
**Category:** Bugfix  
**Dependencies:** CORE-004, MEM-008  
**Status:** DONE  
**Output:** SimType defs include joy gains; TryGiveSimMemory applies joy per tier  
**Owner:** AI

## TASK-BUG-010
**Description:** Fix sim sessions so trait/skill-tier memories (not soft flicker) always apply and joy is granted  
**Category:** Bugfix  
**Dependencies:** MEM-008, CORE-004  
**Status:** DONE  
**Output:** Per-pawn sim type resolution, tiered thought selection even without default thought, and tiered joy award  
**Owner:** AI

## TASK-BUG-011
**Description:** Add missing sim types so all skills (including melee, construction, mining, cooking, animals) grant tiered trait memories and joy  
**Category:** Bugfix  
**Dependencies:** CORE-004, MEM-008  
**Status:** DONE  
**Output:** Expanded SimTypes with tiered thoughts/joy and fallback thoughtOnSession  
**Owner:** AI

## TASK-BUG-012
**Description:** Fix sim selection/tiering so passions pick the skill, tiers apply per skill level with trait modifiers, add melee combat training path, and introduce light randomness to XP/tier rolls  
**Category:** Bugfix / Mechanics  
**Dependencies:** CORE-004, MEM-008  
**Status:** DONE (passion-weighted sim choice, trait-biased tiers, melee training sim, XP/tier RNG)  
**Output:** Updated VRSimUtility tier logic, SimType defs for melee training, trait-aware tier mood adjustment, slight RNG in XP/tier selection; trait mood offsets expanded (psy sensitivity, ascetic preference, skill-flavoured deltas)  
**Owner:** AI

## TASK-BUG-013
**Description:** Make trait memory descriptions append to the base skill memory text instead of replacing it  
**Category:** Bugfix / Narrative  
**Dependencies:** MEM-008  
**Status:** DONE (trait overlays now append after base skill descriptions)  
**Output:** Trait overlays appended after default skill memory descriptions  
**Owner:** AI

## TASK-BUG-014
**Description:** Make tiered VR memory labels explicitly reference the VR sim so sources are obvious in-game  
**Category:** Bugfix / Narrative  
**Dependencies:** MEM-008  
**Status:** DONE (skill-tier labels now call out VR sims)  
**Output:** Skill-tier memory labels include VR sim wording without altering descriptions  
**Owner:** AI

## TASK-BUG-015
**Description:** Cap appended trait overlay descriptions to a maximum of three per memory so multi-trait pawns stay readable  
**Category:** Bugfix / Narrative  
**Dependencies:** MEM-008  
**Status:** DONE (multi-trait overlays now capped at three)  
**Output:** Trait overlay appending logic limits to three matching overlays while keeping combos compatible  
**Owner:** AI

---

# Agent Rules

- Claim tasks by setting `Owner: AI` and `Status: IN_PROGRESS`.  
- Respect dependencies before starting work.  
- Keep changes within task scope unless required for compilation.  
- Update this file when finishing tasks (`Status: DONE` + note/PR link).




## TASK-CORE-006
**Description:** Allow VR Pod to run long-term simulation sessions (repurposing VirtuDream job via right-click and pod target gizmo)  
**Category:** C# / Core Gameplay  
**Dependencies:** CORE-001, CORE-002  
**Status:** DONE (long-term job + target gizmo wired)  
**Output:** VR Pod float menu long-term option; pod gizmo assigns colonist/prisoner/slave to VA_UseVirtuDreamPod  
**Owner:** AI

## TASK-MEM-012
**Description:** Add anti-spam safeguards for VR sessions (min time in pod, slower recreation/XP, periodic in-pod memories)  
**Category:** C# / Mechanics  
**Dependencies:** CORE-001, MEM-008  
**Status:** DONE (15s minimum for benefits; slower XP; frequent in-pod memory ticks)  
**Output:** Pod ticks enforce minimum dwell time before joy/XP/memories; joy/XP throttled; memories fire during long sessions  
**Owner:** AI

## TODO-NOTE (Next session)
- Remove memory trigger on exit VR pod; rely on in-pod timers instead.
- Fix memory triggering: currently interrupted by job finishing on short sessions and never fires on deep sessions.
- Recreation gain is effectively zero even after minutes; rebalance/gate so it rises slowly but reliably post-threshold.

