# VIRTU-AWAKE SKILL × TRAIT MEMORY LIST
### Tiered VR Memories with Mood Effects (Lucidity/Instability Not Yet Applied)

This document defines **VR training/recreation memories** for:

- All **skills** (RimWorld core skills)
- Across **3 tiers** of experience
- With **base mood effects** per skill-tier
- And **trait-based modifiers** to mood & tone

Use this as the content source for:
- ThoughtDefs
- memory generators
- simulation-specific flavour

Lucidity / Instability / Awakening are **not** considered here yet.

---

## 1. Global Structure

### Skills

We use the 12 vanilla skills:

1. Shooting  
2. Melee  
3. Construction  
4. Mining  
5. Cooking  
6. Plants  
7. Animals  
8. Crafting  
9. Artistic  
10. Medical  
11. Social  
12. Intellectual  

---

### Tiers

Every skill has 3 VR experience tiers:

- **Tier 1 – Soft Practice**  
  Short, pleasant, slightly uncanny.  
  Mood: **small positive** (+3)

- **Tier 2 – Deep Session**  
  Focused training, emotionally meaningful.  
  Mood: **moderate positive** (+6)

- **Tier 3 – Intense / “Near-legendary”**  
  Strong emotional or symbolic impact, but not full legendary archeotech.  
  Mood: **large positive** (+10) or slight negative if thematically fitting.

Traits then tilt these mood values up/down and alter flavour text.

---

### Traits Used

From `TRAIT_MEMORY_COMPENDIUM`:

- Psychopath  
- Neurotic  
- Sanguine  
- Depressive  
- Ascetic  
- Gourmand  
- Brawler  
- TooSmart  
- PsySensitive  
- BodyPurist  
- Kind  

---

## 2. Base Mood Effects by Tier

| Tier | Label           | Base Mood Effect |
|------|-----------------|------------------|
| 1    | Soft Practice   | +3               |
| 2    | Deep Session    | +6               |
| 3    | Intense Session | +10              |

You can override base mood effect for specific skills/tier below where appropriate.

---

## 3. Trait Mood Modifiers (Global)

Each trait nudges mood up or down relative to **base**:

| Trait       | Modifier | Notes                                                                 |
|-------------|----------|-----------------------------------------------------------------------|
| Psychopath  | +0       | Same mood, tone more detached                                        |
| Neurotic    | -1 tier  | Tier 1: +2, Tier 2: +5, Tier 3: +8 (more anxious, less relaxing)     |
| Sanguine    | +1 tier  | Tier 1: +4, Tier 2: +7, Tier 3: +11 (finds extra joy)                |
| Depressive  | -2 flat  | Tier 1: +1, Tier 2: +4, Tier 3: +8 (can be bittersweet)              |
| Ascetic     | 0 / -1   | If sim is simple/quiet: base; if flashy: -1                          |
| Gourmand    | +1 on Cooking, 0 on others                                                       |
| Brawler     | +1 on Melee, 0 otherwise                                                         |
| TooSmart    | 0        | Mood unchanged; interpretation more thoughtful                        |
| PsySensitive| 0 or ±1  | +1 if emotional sim; -1 if sim feels “hollow”; decide per memory     |
| BodyPurist  | -1 if sim heavily avatar-based; 0 if low-body-focus                              |
| Kind        | +1 on Social, Animals, Medical, Artistic sims; 0 on others                       |

You can encode this numerically in XML or in the generator logic.

---

## 4. Base Skill Memories (By Tier)

Each entry below:

- **Base label** (for ThoughtDef label)
- **Base description** (neutral tone)
- **Base mood effect** (before trait modifiers)

### 4.1 Shooting – “Silent Range”

**Tier 1 – Soft Practice**  
- Label: `VR: steady shot practice`  
- Description:  
  *“In the sim, the targets drifted through a quiet field. Every shot felt unhurried, each impact soft and distant.”*  
- Base Mood: **+3**

**Tier 2 – Deep Session**  
- Label: `VR: flowing marksmanship`  
- Description:  
  *“The world narrowed to breath and timing. Shots landed where they belonged, as if the range had learned my rhythm.”*  
- Base Mood: **+6**

**Tier 3 – Intense Session**  
- Label: `VR: impossible focus`  
- Description:  
  *“For a while, nothing existed but the stillness before the trigger. The sim felt more precise than real life.”*  
- Base Mood: **+10**

---

### 4.2 Melee – “Silent Arena”

**Tier 1**  
- Label: `VR: light sparring`  
- Description:  
  *“I traded soft blows with quiet echoes. No one could really get hurt, but my body still remembered the movements.”*  
- Base Mood: +3  

**Tier 2**  
- Label: `VR: fluent combat drills`  
- Description:  
  *“Every step and strike flowed together. The arena seemed to know exactly how far I needed to move.”*  
- Base Mood: +6  

**Tier 3**  
- Label: `VR: perfect counter`  
- Description:  
  *“The sim threw a strike I’d never seen. My body answered before I could think, as if it had been waiting.”*  
- Base Mood: +10  

---

### 4.3 Construction – “Ghost Frames”

**Tier 1**  
- Label: `VR: simple framing`  
- Description:  
  *“Walls rose and dissolved around me. Laying down structure felt strangely easy in there.”*  
- Base Mood: +3  

**Tier 2**  
- Label: `VR: clean blueprint`  
- Description:  
  *“The sim unfolded a building piece by piece. Each part seemed to show me where it wanted to go.”*  
- Base Mood: +6  

**Tier 3**  
- Label: `VR: moving architecture`  
- Description:  
  *“The design shifted as I worked, responding to every choice. When it settled, it felt more like a memory than a plan.”*  
- Base Mood: +10  

---

### 4.4 Mining – “Echoing Strata”

**Tier 1**  
- Label: `VR: gentle tunneling`  
- Description:  
  *“Each strike on the rock echoed softly, as if the stone was hollow and patient.”*  
- Base Mood: +3  

**Tier 2**  
- Label: `VR: reading the veins`  
- Description:  
  *“The sim highlighted fractures and seams, guiding me to the best cuts like a quiet suggestion.”*  
- Base Mood: +6  

**Tier 3**  
- Label: `VR: living stone`  
- Description:  
  *“The rock seemed to breathe between blows, opening just enough where it mattered.”*  
- Base Mood: +10  

---

### 4.5 Cooking – “Sim Kitchen”

**Tier 1**  
- Label: `VR: easy recipes`  
- Description:  
  *“The ingredients never spoiled, and the heat was always just right. It felt like cooking on a gentle script.”*  
- Base Mood: +3  

**Tier 2**  
- Label: `VR: flowing prepwork`  
- Description:  
  *“Chopping, seasoning, and stirring all lined up in one smooth motion. The kitchen and I moved together.”*  
- Base Mood: +6  

**Tier 3**  
- Label: `VR: perfect dish`  
- Description:  
  *“The sim plated a meal I could almost taste. For a second, I believed I had made something flawless.”*  
- Base Mood: +10  

---

### 4.6 Plants – “Threaded Garden”

**Tier 1**  
- Label: `VR: gentle sowing`  
- Description:  
  *“Soil parted easily, seeds fell into place, and the rows lined themselves up in calm, straight lines.”*  
- Base Mood: +3  

**Tier 2**  
- Label: `VR: reading growth`  
- Description:  
  *“The sim showed every plant’s health in quiet colours. It felt like knowing the garden’s mood at a glance.”*  
- Base Mood: +6  

**Tier 3**  
- Label: `VR: breathing field`  
- Description:  
  *“For a moment, the crops swayed in perfect unison, as if the whole field was exhaling in relief.”*  
- Base Mood: +10  

---

### 4.7 Animals – “Soft Encounters”

**Tier 1**  
- Label: `VR: calm animal handling`  
- Description:  
  *“The creatures approached without fear, following simple gestures and tones.”*  
- Base Mood: +3  

**Tier 2**  
- Label: `VR: shared trust`  
- Description:  
  *“The sim let me watch a wary animal relax, one slow step at a time.”*  
- Base Mood: +6  

**Tier 3**  
- Label: `VR: unspoken understanding`  
- Description:  
  *“For a few breaths, it felt like the animal and I shared the same thought.”*  
- Base Mood: +10  

---

### 4.8 Crafting – “Sleeping Workshop”

**Tier 1**  
- Label: `VR: simple assembly`  
- Description:  
  *“Pieces slotted together without a fight. The bench felt almost grateful to be used.”*  
- Base Mood: +3  

**Tier 2**  
- Label: `VR: flawless routine`  
- Description:  
  *“The sim laid out every tool in the right order. My hands just followed.”*  
- Base Mood: +6  

**Tier 3**  
- Label: `VR: invisible tolerances`  
- Description:  
  *“I could sense when a piece wasn’t quite right before I touched it.”*  
- Base Mood: +10  

---

### 4.9 Artistic – “Quiet Studio”

**Tier 1**  
- Label: `VR: easy sketches`  
- Description:  
  *“Lines appeared where I meant them to, for once.”*  
- Base Mood: +3  

**Tier 2**  
- Label: `VR: flowing expression`  
- Description:  
  *“The sim gave me space to try shapes and colours I’d never risk on a real canvas.”*  
- Base Mood: +6  

**Tier 3**  
- Label: `VR: image that lingers`  
- Description:  
  *“The piece I made in there keeps resurfacing when I close my eyes.”*  
- Base Mood: +10  

---

### 4.10 Medical – “Calm Clinic”

**Tier 1**  
- Label: `VR: basic treatment drills`  
- Description:  
  *“Bandages wrapped true, wounds stayed clean, and no one cried out.”*  
- Base Mood: +3  

**Tier 2**  
- Label: `VR: steady surgery practice`  
- Description:  
  *“The sim walked me through each step with quiet patience. Nothing rushed, nothing skipped.”*  
- Base Mood: +6  

**Tier 3**  
- Label: `VR: almost losing no one`  
- Description:  
  *“Every cut felt precise. When the patient stabilized, I realized I’d been holding my breath.”*  
- Base Mood: +10  

---

### 4.11 Social – “Echo Lounge”

**Tier 1**  
- Label: `VR: light conversations`  
- Description:  
  *“Talk flowed easily. No awkward pauses, just steady, gentle chatter.”*  
- Base Mood: +3  

**Tier 2**  
- Label: `VR: shared story`  
- Description:  
  *“The sim replayed an evening of talking with people who listened all the way through.”*  
- Base Mood: +6  

**Tier 3**  
- Label: `VR: being understood`  
- Description:  
  *“For a while, every word landed where I meant it to.”*  
- Base Mood: +10  

---

### 4.12 Intellectual – “Quiet Library”

**Tier 1**  
- Label: `VR: easy reading session`  
- Description:  
  *“Pages turned themselves, and the words stayed where I left them.”*  
- Base Mood: +3  

**Tier 2**  
- Label: `VR: clear insight`  
- Description:  
  *“The sim laid out a problem like a diagram I could walk around.”*  
- Base Mood: +6  

**Tier 3**  
- Label: `VR: thought that stays`  
- Description:  
  *“I left with an idea that still feels sharper than most real ones.”*  
- Base Mood: +10  

---

## 5. Trait × Skill Memory Variants (Pattern)

For each **Skill + Tier**, you now have a **Base Memory** (label, description, base mood).  
For each **Trait**, apply:

1. **Mood adjustment** (per table in §3)  
2. **Tone change** matching `TRAIT_MEMORY_COMPENDIUM`  

Below is the *pattern* for every combination, plus a few **fully written examples**.

---

### 5.1 Pattern Template (for every Skill+Tier+Trait)

For any given base memory:

```text
Base:
  Label: VR: [skill-specific label]
  Desc:  [base description]
  Mood:  [base mood by tier]

Trait Variant:
  Label: same or slightly tweaked
  Desc:  rewrite in the trait’s voice, preserving the core event
  Mood:  base + trait modifier (clamped to reasonable range)
```

---

## 6. Some Fully Expanded Examples

### 6.1 Shooting × Tier 2 × All Traits

**Base (Shooting, Tier 2)**  
- Label: `VR: flowing marksmanship`  
- Desc: *“The world narrowed to breath and timing. Shots landed where they belonged, as if the range had learned my rhythm.”*  
- Base Mood: +6  

**Psychopath**  
- Mood: +6  
- Desc: *“The sim reduced everything to timing and impact. Clean, predictable, efficient.”*  

**Neurotic**  
- Mood: +5  
- Desc: *“The shots landed, but I kept waiting for something to go wrong just off the edge of the range.”*  

**Sanguine**  
- Mood: +7  
- Desc: *“Everything lined up—the breathing, the targets, the rhythm. It actually felt good to just hit what I aimed at.”*  

**Depressive**  
- Mood: +4  
- Desc: *“The shots went where they were meant to. It was almost comforting how little it changed anything.”*  

**Ascetic**  
- Mood: +6 (simple, focused sim suits them)  
- Desc: *“Just breathing and distant targets. No noise, no spectacle. That part I appreciated.”*  

**Gourmand**  
- Mood: +6  
- Desc: *“The air tasted dry and metallic after each shot, but at least it was steady.”*  

**Brawler**  
- Mood: +6  
- Desc: *“It wasn’t close-quarters, but the controlled rhythm still settled something in my muscles.”*  

**TooSmart**  
- Mood: +6  
- Desc: *“The pattern of targets and timing wasn’t random. The sim seemed to be testing more than my aim.”*  

**PsySensitive**  
- Mood: +7 (this range feels emotionally “calm”)  
- Desc: *“There was a stillness in the range between shots, like the space itself was holding its breath with me.”*  

**BodyPurist**  
- Mood: +5 (slight discomfort with artificial body feel)  
- Desc: *“My hands moved right, but the recoil didn’t quite feel real. Useful practice, but it sat strangely in my bones.”*  

**Kind**  
- Mood: +6  
- Desc: *“There were no real targets behind the targets. That made it easier to focus.”*  

---

### 6.2 Social × Tier 1 × All Traits

**Base (Social, Tier 1)**  
- Label: `VR: light conversations`  
- Desc: *“Talk flowed easily. No awkward pauses, just steady, gentle chatter.”*  
- Base Mood: +3  

Trait variants:

- **Psychopath** (+3)  
  *“The sim kept the conversation moving without friction. It was interesting how predictable the replies were.”*  

- **Neurotic** (+2)  
  *“The talk was smooth, but I kept wondering what would happen if the sim stopped filling the gaps.”*  

- **Sanguine** (+4)  
  *“It was nice just to talk without snags for once.”*  

- **Depressive** (+1)  
  *“The conversation flowed, but it felt more like watching warmth than having it.”*  

- **Ascetic** (+3)  
  *“Simple talk. No noise, no drama. That was enough.”*  

- **Gourmand** (+3)  
  *“Light conversation, like a mild snack—nothing heavy, nothing sharp.”*  

- **Brawler** (+3)  
  *“No raised voices, no edge, just calm words. Different kind of sparring, I guess.”*  

- **TooSmart** (+3)  
  *“The dialogue followed a neat pattern. It was almost relaxing to watch it run.”*  

- **PsySensitive** (+4)  
  *“The room felt gently warm around the voices, like the sim was trying to be kind.”*  

- **BodyPurist** (+2)  
  *“My face moved along with the words, but something in the expressions felt too even.”*  

- **Kind** (+4)  
  *“Everyone seemed to listen, even if they weren’t real. It still felt good.”*  

---

### 6.3 Medical × Tier 3 × All Traits (Summary Mood Only)

**Base (Medical, Tier 3)**  
- Label: `VR: almost losing no one`  
- Desc: *“Every cut felt precise. When the patient stabilized, I realized I’d been holding my breath.”*  
- Base Mood: +10  

Trait-adjusted moods:

- Psychopath: +10 – *calm clinical success*  
- Neurotic: +8 – *relieved but shaken*  
- Sanguine: +11 – *deeply relieved, grateful*  
- Depressive: +8 – *relief tinged with “it’s only a sim”*  
- Ascetic: +10 – *appreciates the focused, no-frills environment*  
- Gourmand: +10 – *neutral; not food-related*  
- Brawler: +10 – *respect for controlled precision*  
- TooSmart: +10 – *intrigued by clear feedback loop*  
- PsySensitive: +11 – *strong emotional resonance with saving a life, even simulated*  
- BodyPurist: +9 – *slight discomfort at artificial bodies*  
- Kind: +11 – *strong positive, “helped someone” feeling*  

You can write full descriptions by applying each trait’s tone from `TRAIT_MEMORY_COMPENDIUM`.

---

## 7. Complete Combination Matrix (Logical, Not Fully Written Out)

For **every Skill** in §4 and **every Tier**, generate:

- 1 **Base memory** (already listed)
- 11 **Trait variants** using:

  - Base description → rewritten with trait tone  
  - Base mood → adjusted by trait’s modifier from §3  
  - Optional tiny label tweaks per trait if needed  

So the full set is:

- Shooting: 3 tiers × 11 traits  
- Melee: 3 tiers × 11 traits  
- Construction: 3 tiers × 11 traits  
- Mining: 3 tiers × 11 traits  
- Cooking: 3 tiers × 11 traits  
- Plants: 3 tiers × 11 traits  
- Animals: 3 tiers × 11 traits  
- Crafting: 3 tiers × 11 traits  
- Artistic: 3 tiers × 11 traits  
- Medical: 3 tiers × 11 traits  
- Social: 3 tiers × 11 traits  
- Intellectual: 3 tiers × 11 traits  

Total: **132 trait-sensitive memories**, each with a tiered mood effect.

This document gives you:

- All **base entries** per skill & tier  
- A **global trait mood and tone map**  
- Several **fully expanded examples** per skill  

You (or Codex) can now script a generator that:

1. Takes `(Skill, Tier, Trait)`  
2. Pulls base memory  
3. Applies trait tone template  
4. Adjusts mood value per trait rules  

---

End of SKILL × TRAIT MEMORY LIST (First Pass)
