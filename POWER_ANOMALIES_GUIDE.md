# POWER_ANOMALIES_GUIDE.md
### Power Behaviour, Energy Instability & Archeotech Effects in **Virtu-Awake**

This document explains how **power anomalies** function across all Virtu-Awake pods, event systems, and awakening mechanics.  
It covers:
- Power fluctuations  
- Brownouts and overloads  
- Archeotech energy behaviour  
- Simulation degradation  
- Psychological responses  
- Event-writing rules  
- Forbidden content  

Power anomalies create tension, unpredictability, and narrative flavour without resorting to catastrophic electrical failures or action-movie drama.

Use alongside:
- `LORE_GUIDE.md`
- `EVENT_DESIGN_GUIDE.md`
- `WORLDSPACE_NOTES.md`
- `AWAKENING_TIERS_GUIDE.md`

---

# ⚡ 1. Power Philosophy

Power in Virtu-Awake is **not** treated as:
- Electrical engineering  
- Earth-style circuitry  
- “The Matrix” energy farms  

Instead, the pods use **archeotech energy substrates** — semi-organic, semi-psychic power matrices that influence perception, stability, and simulation depth.

Key principles:
- Power anomalies affect *worldspace*, not hardware.  
- Fluctuations create *psychological distortions*, not sparks and explosions.  
- Power does **not** cause physical harm — only surreal experience.  
- Higher power → stronger immersion  
- Lower power → instability and thinning  

The system behaves like a *fragile dream engine*, not a generator.

---

# 🔋 2. Power States in Virtu-Awake

### **1. Full Power**  
- Stable worldspace  
- Strong sensory coherence  
- Minimal glitches  
- Pawns feel comforted, anchored  

### **2. High Load (Near Peak)**  
- Simulation “overthinks”  
- Extra NPCs or patterns appear  
- Minor uncanny behaviours  
- Increased Lucidity  

### **3. Brownout (Low Power)**  
- Colours fade  
- Movement feels heavy or delayed  
- Sound desynchronises  
- Instability rises sharply  

### **4. Critical Low**  
- Geometry collapses  
- Lighting becomes nonlinear  
- Sudden spatial distortions  
- Major glitch events unlock  

### **5. Undercurrent / Archeotech Resonance**  
Occurs when:
- Using ancient batteries  
- Using Zetrin capacitors (Royalty tech levels)  
- Connecting multiple pods in a chain  

Effects:
- Shared hallucinations  
- Echoing memories  
- Social VR anomalies  
- “Group dreaming”  
- Trait-reactive disturbances  

This state is rare and narratively potent.

---

# ⚙ 3. Mechanical Effects of Power Irregularities

### Power *should not* cause:  
- Fire  
- Sparks  
- Burns  
- Explosions  
- Electrical injury  

Virtu-Awake is a psychological mod, not a hazard mod.

### Power *should* affect:
- Instability (Hediff)  
- Lucidity (Need)  
- Memory types generated  
- VR event severity  
- Awakening progression  
- Social VR failures  
- Pod malfunction thoughts  

### Example logic:
- **Brownout** → +Instability, +Lucidity, unlock Moderate Glitches  
- **High Load** → +Lucidity but stable worldspace  
- **Critical Low** → Enable Crisis events and Tier-4/5 Awakening sequences  

---

# 🔌 4. Pod-Specific Anomaly Rules

### **VR Pod (Standard)**  
- Brownouts cause flickers, loops, delayed avatars  
- Critical low power exposes wireframes  
- Overload causes duplicate NPCs or voice echoes  

### **Virtu-Dream Pod (Suspension)**  
Dream pods are extremely sensitive to:
- Micro-outages  
- Energy pulses  
- Grid fluctuations  

Effects:
- Long-form hallucinations  
- Memory bleed between sessions  
- Shared dream events (if multiple pods)  
- Premature Awakening attempts  
- Rare catastrophic “dream inversion” moments  
  *(no gore — just complete surreal detachment)*

### **Humanoid Power Converter (“The Battery Arc”)**  
If your mod setup includes prisoner-powered VR systems:

Thematically:
- Energy is drawn from neural resonance, not “biological power”  
- Power flow affects the prisoner’s dream layer  
- Prisoners are more prone to Breakthrough events  
- Lucidity increases even when they resist  

Mechanically:
- Unstable power sources → high Instability growth  
- Sudden surges → trait-reactive hallucinations  
- Moral consequences reflected through memories  

---

# 🌀 5. Power-Driven Event Types

### **1. Flicker Events (Low Power)**
- Frames repeat  
- Colours wash out  
- Sim freezes momentarily  
- NPCs reset position  

### **2. Overload Events (High Power)**
- Too many background NPCs spawn  
- Ambient noise layers stack incorrectly  
- Brightness “steps” up and down  
- World feels overly crisp or sharp  

### **3. Brownout Distortions**
- Geometry bends  
- Movement stutters  
- Sound drops out  
- Avatar limbs desync  

### **4. Grid Interference**
Occurs when:
- Batteries fail  
- Solar/wind fluctuations spike  
- Multiple pods share lines  
- Colonies have erratic power networks  

Results:
- Cross-pawn hallucination echoes  
- Memory fragment sharing  
- Social VR mishaps  

### **5. Archeotech Resonance**
Rare, powerful, and ambiguous:
- Shared visions  
- Meaningful symbolic imagery  
- Echoes of past users  
- Subtle psychic overtones  
- Lore fragments  

Never reveal their origin.

---

# 🧬 6. Trait Effects on Power Anomaly Perception

### **Psychopath**  
Calm, analytical — interprets power anomalies as “interesting data.”

### **Neurotic**  
High stress reactions to brownouts; panic risk increases.

### **Sanguine**  
Amused or fascinated by flickers or colour shifts.

### **PsySensitive**  
Feels resonance, presence, or “messages” during power events.

### **TooSmart**  
Deduces pattern failures and anticipates brownouts.

### **BodyPurist**  
Disturbed by avatar desync induced during low power.

Trait interactions should be:
- Subtle  
- Psychological  
- Avoiding comedy  

---

# ⚠ Forbidden Power Behaviours (Must Never Appear)

❌ Sparks, wires, electrocution, electrical gore  
❌ “The system is alive and draining life-force”  
❌ Literal machinery or sci-fi exposition  
❌ Over-explaining the power source  
❌ Earth electronics metaphors  
❌ Obvious Matrix-style “battery farm” implications  
❌ Loud, action-movie dramatics  

Virtu-Awake is quiet, internal, ambiguous sci-fi.

---

# 🧠 7. Memory Tone Guidelines for Power Anomalies

### Good Memory Style:
> *“The lights dimmed inside the sim, and the colours faded with them, as if someone turned down a thought.”*

### Good Surreal Style:
> *“When the power dipped, the world seemed to hold its breath. Even my avatar did.”*

### Forbidden Style:
✗ *“I got electrocuted.”*  
✗ *“The hardware failed and exposed the cables.”*  
✗ *“A loud explosion shut down the pod.”*  

---

# 🔧 8. Implementation Notes for Developers

### Use the following hooks:
- PowerTraderComp  
- CompPowerBattery  
- Map power net watchers  
- Per-tick pod state checks (250–500 tick intervals)

### Power anomalies influence:
- Need_Lucidity  
- Hediff_Instability  
- VR event weighting  
- Awakening tier progression  

### Avoid:
- Per-frame power checks  
- Tick-heavy loops  
- Complex physics simulation  

Use lightweight interval triggers to stay performant.

---

# ✔ Summary

Power anomalies in Virtu-Awake must feel:

- Quiet  
- Surreal  
- Psychological  
- Archeotech-adjacent  
- Never fully explained  
- Never violent  
- Always narratively relevant  

They are atmospheric disruptions that ripple through the **mind**, not the **hardware**, pushing pawns toward deeper instability, higher lucidity, and the fragile edge of awakening.
