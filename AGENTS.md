# AGENTS.md
AI Agent Guidelines for **Virtu-Awake**

This document provides instructions for AI agents contributing to the Virtu-Awake project.  
Agents should follow these rules whenever writing code, updating documentation, or modifying project files.

---

# 📘 1. Documentation Index

Before performing any task, agents **must consult the following documents** to understand context, naming conventions, systems, and design:

### **README.md**
Overview of the mod, its conceptual goals, gameplay systems, and features.  
Use this to understand *what* Virtu-Awake is and *how it should behave*.

### **TASKS.md**
The authoritative task list for all AI agents.  
Defines:
- Task IDs  
- Descriptions  
- Categories  
- Dependencies  
- Status fields  
- Expected outputs  
- Ownership rules  

Agents must reference TASKS.md before starting work, and must update it after completing a task.

### **CONTRIBUTING.md**
Rules for style, structure, file organization, art guidelines, and general contributor etiquette.  
Agents must follow all conventions described here to maintain consistency.

### **ROADMAP.md**
Short-, medium-, and long-term plans for the mod.  
Use this to understand future direction and to avoid implementing features out of order.

If any file listed above is missing or incomplete, agents should create or update it as needed.

---

# ⚙ 2. How TASKS.md Works

**TASKS.md** contains all actionable work items for this project.  
It is designed to be machine-friendly and follows a consistent structure.

Each task block contains:

- **ID** – unique identifier (e.g., `TASK-CORE-001`)  
- **Description** – clear explanation of the task  
- **Category** – C#, XML, Art, Documentation, etc.  
- **Dependencies** – tasks that must be completed first  
- **Status** – `TODO`, `IN_PROGRESS`, or `DONE`  
- **Output** – expected files or components created by the task  
- **Owner** – `Unassigned`, `Human`, or `AI`

### Agents must follow these rules:

#### ✔ Claiming a Task
Before starting a task, agents must:
- Set `Owner: AI`
- Set `Status: IN_PROGRESS`

#### ✔ Completing a Task
After finishing a task:
- Set `Status: DONE`
- Add a link or commit reference (if applicable)

#### ✔ Dependency Rules
Agents may *not* begin a task if its dependencies are not completed.

#### ✔ Atomic Commits
Each commit must reference the task ID in its description, e.g.:
- `Implement Need_Lucidity (TASK-PSY-001)`
- `Add VR event severity selection (TASK-EVT-002)`

#### ✔ No Scope Creep
Agents must not modify files outside the scope of the claimed task *unless required for compilation*.

---

# 🧠 3. Behavioural Rules for AI Agents

### ✔ Follow all existing documentation  
Agents must always align their work with:
- Code architecture described in ROADMAP.md  
- Style rules in CONTRIBUTING.md  
- Task definitions in TASKS.md  

### ✔ Maintain RimWorld conventions  
Use existing class patterns:
- `ThingDefs`
- `JobDrivers`
- `MapComponents`
- `Hediffs`
- `Needs`
- `IncidentWorkers`

### ✔ Keep code clean and predictable  
- Use descriptive method names  
- Follow C# conventions used in RimWorld’s source  
- Avoid unnecessary abstractions unless described in ROADMAP.md  

### ✔ Avoid irreversible changes  
Never delete systems, files, or mechanics unless explicitly defined by a task.

### ✔ Never hallucinate file names  
Only use file paths or class names that:
- Already exist, or  
- Are explicitly defined in TASKS.md  

### ✔ Ask for clarification if needed  
If any ambiguity exists, agents should request instructions or add an Issue.

---

# 🤝 4. Coordination Between Agents

If multiple agents are operating:
- Each should only claim tasks they intend to complete
- Agents must not overwrite work from other agents
- Agents should avoid editing the same file simultaneously unless necessary

If conflicts arise, prefer *the simpler implementation* unless ROADMAP.md specifies otherwise.

---

# 🔄 5. When Creating New Systems

Agents introducing new components must:
- Add new tasks to TASKS.md (with unique IDs)
- Update ROADMAP.md if the change affects long-term plans
- Follow the file structure documented in CONTRIBUTING.md

---

# 🏁 6. Completion Criteria

A task is considered complete when:
- The expected output exists  
- The code compiles successfully  
- Behaviour meets the requirements in README.md  
- Documentation is updated appropriately  
- TASKS.md is updated with `Status: DONE`

---

# 📜 7. License & Contribution Terms

All AI-generated contributions are made under the MIT License, in alignment with the project’s licensing structure.  
By contributing, agents agree to the license terms and to the rules specified in this document.

---

Thank you for contributing to **Virtu-Awake**.  
Together, we build surreal dreams, eerie glitches, and unforgettable stories.
