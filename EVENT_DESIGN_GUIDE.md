# EVENT_DESIGN_GUIDE.md
### Guidelines for Creating VR Events, Glitches, Anomalies & Awakenings in **Virtu-Awake**

This document explains how to design VR events for Virtu-Awake in a way that preserves the mod’s tone, narrative depth, and psychological subtlety.

Use this guide alongside:
- `INTENT.md` (creative philosophy)
- `STYLE_GUIDE.md` (writing/coding style)
- `LORE_GUIDE.md` (worldbuilding rules)
- `TASKS.md` (task tracking)

Events are the heart of Virtu-Awake.  
They shape memories, narrative arcs, awakening sequences, instability progression, and breakouts.

---

# 🧠 1. What is a “VR Event”?

A VR event is any anomaly, experience, or psychological moment that happens to a pawn while using:
- VR Pods (short-term)
- Virtu-Dream Pods (long-term)
- Shared simulations (social VR)

Events may:
- Generate memory thoughts  
- Modify Lucidity  
- Modify Instability  
- Trigger emotional reactions  
- Advance Awakening tiers  
- Lead to breakouts  

They are **subtle, surreal, and psychologically-driven** — not cinematic or explosive.

---

# 🧩 2. Event Severity Levels

Virtu-Awake uses four canonical tiers:

### **1. Minor Anomaly**
- Gentle oddities  
- Small Lucidity/Instability changes  
- Mostly flavour  
- Pawns shrug them off  

**Examples:**  
- Flicker, repeated frame, NPC blip, colour shift  

---

### **2. Moderate Glitch**
- Noticeable disturbance  
- Small–medium emotional effect  
- Identity friction or mild dissociation  
- Starts to feel “wrong”  

**Examples:**  
- Avatar distortion  
- Incorrect shadows  
- Spatial loops  
- Time stutters  

---

### **3. Major Glitch**
- Deeply unsettling  
- Identity instability  
- World behaving in impossible ways  
- Serious psychological impact  

**Examples:**  
- Wireframe world  
- Digital doubles  
- Voices without sources  
- Physical laws breaking  

---

### **4. Crisis Event**
- Complete rupture  
- Identity collapse  
- Boundary between self/reality/simulation blurs  
- May trigger breakout  

**Examples:**  
- Simulation collapse  
- Room “peeling back”  
- Pawn sees pod from inside the VR  
- Direct existential query  

---

# 🎭 3. Event Structure Template

Use this standard structure when designing events:

### **1. Event Name**  
Short, descriptive, evocative.  
Example: *“Delayed Double”*, *“Unloaded Horizon”*

### **2. Severity**  
Minor / Moderate / Major / Crisis

### **3. Memory Text (1–3 sentences)**  
Follow STYLE_GUIDE.md rules:
- First-person past tense  
- Subtle, sensory-driven  
- No exposition  
- No tech jargon  

### **4. Effects**
Include:
- Lucidity offset  
- Instability offset  
- Mood offset (if any)  
- Awakening stage triggers  
- Trait variants (optional)  
- Chance to escalate  

### **5. Conditions / Weighting**
Define:
- Required Instability level  
- Required Lucidity range  
- Trait influences  
- Pod type (VR vs Dream)  
- Chance weight  

### **6. Optional Narrative Flags**
Examples:
- `HasSeenWireframe`
- `HasBeenQuestioned`
- `DigitalEchoImprinted`

These unlock chained events.

---

# 🧬 4. Trait Interaction Rules

Traits should **change how events feel**, not how they function.

### Recommended variants:
- **Psychopath** → finds glitches interesting  
- **BodyPurist** → fears avatar distortion  
- **TooSmart** → notices simulation logic  
- **Neurotic** → panics early  
- **Sanguine** → reframes horror positively  
- **Depressive** → interprets events hopelessly  
- **PsySensitive** → experiences metaphysical bleed  

Each event *may* have:
- Alternative text  
- Different emotional impact  
- Higher chance of triggering escalation

Avoid comedic or meta variants.

---

# 🌀 5. Awakening Events (Tiered Design)

Awakening is not a single event — it is a **progression**.

Use this template for tiered awakening events:

### Tier 1 — **Compliant**
- Comforting sim behaviour  
- Soft flickers  
- Minor anomalies  
- Purpose: introduce subtle uncertainty

### Tier 2 — **Flicker**
- Noticeable distortions  
- Avatar/scene mismatches  
- Purpose: raise self-awareness

### Tier 3 — **Dissociation**
- Detachment from body/world  
- Harder glitches  
- Purpose: challenge identity stability

### Tier 4 — **Realisation**
- Pawn recognises simulation as false  
- Glimpses outside or “behind”  
- Purpose: prepare for breakthrough

### Tier 5 — **Breakthrough**
- Pawn fully “wakes” in the sim  
- Direct rupture  
- Foundational crisis event  
- May trigger breakout

Each tier should:
- Have 3–10 possible events  
- Include multiple trait-reactive variants  
- Never give direct explanations  
- Preserve subtlety  
- Deepen psychological stakes

---

# 💥 6. Breakout Event Design

A breakout should feel like a **psychological eruption**, not a combat encounter.

### Design principles:
- Trigger only when Lucidity high + Instability severe  
- Trigger after at least one Major/Crisis event  
- Memory text should reflect identity rupture  
- Post-breakout mental state must feel:
  - Confused  
  - Desperate  
  - Overwhelmed  
  - Emotionally coherent with the awakening chain  

### A breakout is NOT:
- An action-movie sequence  
- A combat buff  
- A punishment  

It is a **story moment**.

---

# 🌌 7. Atmosphere & Sensory Palette

When writing event text, use:

### Light
- Flicker  
- Soft glow  
- Unnatural shadows  
- Overexposure  

### Sound
- Static  
- Echoes  
- Wrong pitch  
- Silence  

### Space
- Loops  
- Collapsing boundaries  
- Scale uncertainty  

### Identity
- Out-of-sync movements  
- Faulty avatars  
- Fragmented memory  

Avoid concrete physical threats or gore.

---

# 🔍 8. What NOT To Include

### ❌ Hard explanations  
Events must not reveal:
- How VR works  
- Who built it  
- What is “really happening”  

### ❌ Traditional monsters or horror tropes  
No demons, robots, zombies.

### ❌ Action-heavy scenes  
No fights, explosions, dramatic chase sequences.

### ❌ Earth references  
No references to famous brands, technologies, or media.

### ❌ Meta commentary  
No breaking the fourth wall.

---

# 🧩 9. Good Event vs Bad Event Examples

### ✔ Good
**Memory:**  
> *“The horizon repeated itself. Walking forward felt like walking in place, but the sky kept shifting.”*  
**Severity:** Moderate  
**Effects:** +Lucidity, +Instability  
**Tone:** Subtle, unsettling, unexplained

### ✗ Bad
**Memory:**  
> *“A demon burst out of the ground and screamed at me until I fainted.”*  
**Reason:** Explicit, loud, lore-breaking, horror trope

---

# 🛠 10. Event Writing Checklist

Before committing an event, confirm:

- [ ] Does it follow the tone rules?  
- [ ] Is it subtle, psychological, and sensory-first?  
- [ ] Does it NOT explain the system?  
- [ ] Are trait variants consistent with trait personality?  
- [ ] Does the event escalate appropriately from previous tiers?  
- [ ] Does the text fit RimWorld’s concise style?  
- [ ] Does it enrich emergent narrative?  

If *no* — revise.

---

End of EVENT_DESIGN_GUIDE.md
