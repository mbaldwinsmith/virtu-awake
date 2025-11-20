# Contributing to Virtu-Awake

First off — thank you for your interest in contributing to **Virtu-Awake**!  
This project thrives on creativity, weird storytelling ideas, psychological flavour, and clean modding practices. Whether you want to write new VR memories, add event logic, optimise code, or expand compatibility, your help is welcome.

Please read the guidelines below before submitting issues or pull requests.

---

## 🧭 Project Goals

1. **Immersive VR simulation** (recreation, training, social sims, long-term immersion)  
2. **Psychological depth & emergent storytelling** (memories, traits, glitch events)  
3. **Narrative escalation & horror** (Instability, Lucidity, awakenings, breakouts)

All contributions should respect these core themes.

---

## 🗂 Repository Structure

Virtu-Awake is built around the following directories:
├── About/ # Mod metadata
├── Assemblies/ # Compiled DLLs
├── Defs/ # XML defs (ThoughtDefs, Hediffs, Buildings, etc.)
├── Languages/ # Localisation files
├── Source/ # C# project
│ ├── Components/ # Need_Lucidity, Hediff_Instability
│ ├── Events/ # VR event engine, event workers
│ ├── Simulation/ # VR pod drivers and logic
│ ├── Traits/ # Trait-based memory handlers
│ └── Util/ # Shared helpers
└── Textures/ # Art assets

If adding new systems, please follow existing naming conventions.

---

## 🧩 How to Contribute

### 1. **Reporting Issues**
Please open an Issue if you:
- Found a bug
- Noticed a memory firing incorrectly
- Encountered a compatibility conflict
- Have an idea for new sim types, trait variants, or event chains

Be descriptive and include reproduction steps when possible.

---

### 2. **Submitting Pull Requests**

**Branching:**
- Create feature branches using the format:  
  `feature/my-new-feature`  
  `fix/vr-pod-npe`  
  `content/new-trait-memories`

**Before opening a PR:**
- Ensure the code compiles in Debug mode
- Run RimWorld with the mod enabled to verify stability
- Follow the C# style of the project:
  - PascalCase for classes
  - camelCase for locals/fields
  - `this.` for instance fields
  - XML defs grouped logically

**PRs should include:**
- A clear description of the change
- Screenshots or logs if applicable
- XML and C# updated consistently

Small PRs are easier to merge than giant ones.

---

## 🧠 Content Contributions (Memories, Events, Traits)

Virtu-Awake welcomes:
- New **VR sim types**
- New **memory text**
- **Trait variations**
- **Glitch events**
- **Breakout behaviours**
- **Rare legendary experiences**

Guidelines:
- Keep memories short and RimWorld-in-style
- Avoid Earth-specific references
- Trait interactions must reflect vanilla tone
- Horror elements should be unsettling, not gory
- Social VR scenes must involve consent from both pawns

---

## 🎨 Art Contributions

Art must follow the RimWorld visual style:
- 64×64 or 128×128 as appropriate
- Clean, flat shading
- Soft outlines
- Top-down, isometric perspective
- No AI-generated artefacts unless heavily refined

Submit textures as `.png` with alpha.

---

## 🔌 Compatibility & Patching

When adding compatibility patches:
- Place them in a new folder under `Patches/`
- Use `PatchOperationSequence` or Harmony patches where needed
- Avoid overwriting vanilla methods unless necessary

---

## 🗣 Communication & Collaboration

Feel free to use:
- GitHub Issues (bug reports, discussion)
- GitHub Discussions (ideas, suggestions)
- PR comments (feedback, code review)

Every contributor is treated with respect.  
We aim for a friendly, creative, and supportive environment.

---

## 📜 License

Virtu-Awake uses the **MIT License**.  
Contributions are accepted under the same terms.

---

Thank you for helping shape Virtu-Awake!  
Let’s build surreal memories, unsettling glitches, and unforgettable stories together.


