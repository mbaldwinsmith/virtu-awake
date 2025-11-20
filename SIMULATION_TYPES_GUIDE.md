# SIMULATION_TYPES_GUIDE.md
### How to Design, Name, and Implement VR Simulation Types in **Virtu-Awake**

This guide explains how to create new VR simulation types (“SimTypes”) that feel consistent with Virtu-Awake's psychological, narrative, and RimWorld-friendly style.  
SimTypes determine **what experience the pawn has**, **what memories they form**, **what mood effects they generate**, and **what skills they train.**

Use this guide when writing new SimTypes, adding XML definitions, or implementing new training logic.

---

# 🌐 1. What is a SimType?

A **SimType** represents a category of virtual experience:  
- Combat arena  
- Dream garden  
- Artistic creation space  
- Social gathering rooms  
- Meditative void realms  
- Nature exploration  
- Skill training environments  
- Abstract mental landscapes

Each SimType should:
- Provide XP in one or more skills  
- Create thematic memory thoughts  
- Include trait-aware variants  
- Fit the psychological + surreal atmosphere  

SimTypes are *not* literal “games” — they are experiential mental spaces.

---

# ⭐ 2. Design Philosophy

SimTypes must:
- Feel immersive and strange  
- Enhance pawn psychology  
- Reveal personality differences  
- Allow emergent storytelling  
- Echo RimWorld’s understated tone  
- Avoid hard sci-fi explanations  
- Avoid sensory overload or flashy aesthetics  

VR in Virtu-Awake is calm, surreal, dreamlike.

---

# 🎨 3. Allowed SimType Categories

These categories form the official “palette.”  
All new SimTypes must fall into (or near) one of these themes.

### 🟢 Recreation / Joy Sims
Simple, peaceful experiences:
- **Serene Meadow**
- **Calm Ocean**
- **Memory Garden**
- **Starlight Field**

Purpose: mood recovery, emotional wellness.

---

### 🟦 Training Sims (Skill XP)
Structured environments that train skills through narrative tasks:

**Combat Training**
- Silent Arena  
- Echo Duel Chamber  
- Motion-Loop Range  

**Crafting/Artistic Training**
- Sculptor’s Hall  
- Pattern Loom Studio  
- Virtual Workshop  

**Medicine/Bio Training**
- Healing Labyrinth  
- Patient Holograph Bay  

**Intellectual/Research**
- Thought Maze  
- Logic Chamber  
- Quiet Library  

**Nature/Plants**
- Orchard Simulator  
- Forest Echo Dome  

---

### 🟪 Psychological Sims (Lucidity/Instability-sensitive)
Dreamlike spaces influenced by pawn traits:
- Infinite Corridor  
- Mirror Rooms  
- Soft Void  
- Snowfield of Memory  
- The Listening Shelves  

These often produce deeper memories.

---

### 🟣 Social Sims
Shared environments for two or more pawns:
- Campfire Hall  
- Floating Garden  
- Echo Lounge  
- Memory Café  

Should reinforce:
- Friendships  
- Romance  
- Rivalries  
- Relationship changes  

---

### 🔴 Advanced & Late-Game Sims
Unlocked by research or progression:
- Collective Dreamspace  
- Archeotech Archive  
- Neural Deep Dive  
- Prime Pattern Hall  

These may unlock legendary VR events.

---

# 🧬 4. How to Define a New SimType (Format)

Every new SimType must include:

### **1. Name**
Short, thematic, non-Earth reference.

### **2. Type**
One of:  
`Recreation`, `Training`, `Psychological`, `Social`, or `Advanced`.

### **3. Description**
1–2 sentences describing the experience *without over-explaining*.

### **4. Skill XP (optional)**
If training, assign:
- Primary skill  
- XP multiplier  

### **5. Memory Templates**
At least 3–6 memory descriptions, e.g:
- Minor / Moderate / Major events  
- Trait-specific variants  

### **6. Instability Weight**
How likely this SimType is to provoke glitches:
- Low / Medium / High

Psychological sims always have **Medium–High**.

### **7. Lucidity Tendency**
Does this SimType gently awaken pawns or keep them calm?
- Sedative  
- Neutral  
- Awakening  

Meditation-based sims tend to be Awakening-oriented.

---

# 🧩 5. Writing Guidelines for SimTypes

### **Do:**
✔ Keep descriptions sensory, soft, and surreal  
✔ Match tone from INTENT.md & STYLE_GUIDE.md  
✔ Use nature metaphors or dreamlike spaces  
✔ Let traits change how pawns experience the simulacrum  
✔ Reinforce psychological depth  

### **Do NOT:**
✗ Reference real-world games or franchises  
✗ Use loud cyberpunk aesthetics  
✗ Describe gore, violence, or horror tropes  
✗ Break the fourth wall  
✗ Over-explain the “technology”  

---

# 🧠 6. Trait Variation Rules

When designing SimType memories:
- **Psychopath** → finds oddities interesting or amusing  
- **Neurotic** → becomes uneasy earlier  
- **BodyPurist** → dislikes avatar/body distortions  
- **TooSmart** → notices simulation logic errors  
- **Sanguine** → reframes even strange things positively  
- **Depressive** → interprets surreal moments tragically  
- **PsySensitive** → hears “echoes” or feels presences  

Trait variants must be:
- Subtle  
- Character-driven  
- Non-comedic  
- Non-explanatory  

---

# 🌀 7. Instability & SimType Interaction

Each SimType interacts with Instability differently:

### Low-Instability Sims
- Recreational meadows  
- Nature loops  
- Basic training simulators  

These produce:
- Minor anomalies  
- Occasional flickers  

---

### Medium-Instability Sims
- Social VR rooms  
- Abstract intellectual spaces  
- Complex training environments  

These may produce:
- Moderate glitches  
- Strange loops  
- Mild dissociation  

---

### High-Instability Sims
- Psychological dream-chambers  
- Mirror halls  
- The Soft Void  
- Archeotech sims  

These produce:
- Major glitches  
- Identity disruption  
- Awakening-tier events  
- Rare Crisis triggers  

High-risk, high-storytelling consequences.

---

# 🔮 8. Legendary SimTypes (Ultra-Rare)

Some SimTypes may unlock:
- Legendary memories  
- Trait-combo exclusive events  
- Archeotech story hints  

Examples:
- **The Pale Library** — books that shift when stared at  
- **The Blank Shore** — the tide never moves  
- **The Chamber of Echoed Steps** — footsteps that aren’t yours  

These should be:
- Minimalist  
- Haunting  
- Completely unexplained  

---

# 🛠 9. SimType Implementation Checklist

### Before adding a new SimType:
- [ ] Does it fit one of the allowed categories?  
- [ ] Does it adhere to tone rules?  
- [ ] Are memory templates included?  
- [ ] Are trait variants logical?  
- [ ] Are instability weights reasonable?  
- [ ] Does the description feel dreamlike and subtle?  
- [ ] Is it compatible with Awakening tiers?  
- [ ] Does it avoid banned content?  
- [ ] Is it emergent and story-friendly?  

---

# ✔ 10. Summary

SimTypes are the *foundation of VR flavour* in Virtu-Awake.  
They must always be:

- Surreal  
- Psychological  
- Softly mysterious  
- RimWorld-authentic  
- Trait-reactive  
- Emergent-friendly  

If a SimType enriches inner worlds, emotional depth, and dreamlike storytelling — it belongs in Virtu-Awake.

End of SIMULATION_TYPES_GUIDE.md
