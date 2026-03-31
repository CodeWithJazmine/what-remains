# What Remains — Stream 1 Prep Notes
### Review & Bug Fix Session | Unity 6.3 LTS HDRP

---

## What Is This Game?

**What Remains** is a stealth survival game set in **Bellhaven**, a mid-size Northeast city that's gone quiet in the wrong way. Two survivors — a couple — are holed up in an apartment. Every day one of them has to go out. Every night you count what came back.

The emotional core is the relationship between the two characters. Who goes out. Who stays behind. What you're willing to risk.

**v1 design pillars:**
- Atmosphere-first — the world should feel before it explains
- Resource pressure — every phase costs something
- Detection is the mistake — no combat in v1; being seen means you failed
- Clear failure conditions — die to a zombie or run out of food

---

## What's Been Built So Far

### ✅ Project Setup
- Unity project renamed to **What Remains**
- GitHub repo renamed to **what-remains**
- Concept document written (`WhatRemains_Concept.docx`)
- README written for the repo
- City name locked: **Bellhaven**
- Game title locked: **What Remains**

### 🔲 Still Pending (not today's focus)
- Set Product Name in Project Settings → Player
- Name the two survivors (currently placeholders: Cosmo & Wanda)

### Game Architecture (walk through live)
The core systems that exist in the codebase:

| System | File(s) | Status |
|---|---|---|
| Game loop / phase manager | `GameManager.cs` | Partially stubbed — has **2 bugs** |
| Shelter entry flow | `GameManager.cs` → `EnterShelterFlow()` | Stubbed — has **1 bug** |
| Spawn points | `GameManager.cs` | NullRef bug |
| Zones / scenes | Scene setup | Exists but not wired up properly yet |

---

## Today's Agenda

### 1. Walkthrough (~10–15 min)
Walk through the project hierarchy and existing scripts in the Unity Editor. Show:
- The folder structure
- `GameManager.cs` — the phase system skeleton
- The scene setup in HDRP (lighting, volumes)
- The concept doc to give context on where this is all going

### 2. Bug Fix 1: `spawnPoints` NullReferenceException (~5 min)
**File:** `GameManager.cs`

The `spawnPoints` field is declared but never initialized. Any code that tries to add to or iterate over it throws a `NullReferenceException` at runtime.

**Fix:** Initialize the list — either inline at declaration or in `Awake()`.

See `fix_spawnPoints.cs` for the exact change.

### 3. Bug Fix 2: `EnterShelterFlow()` Scene Load Timing (~10–15 min)
**File:** `GameManager.cs`

`LoadScene()` is called synchronously inside `EnterShelterFlow()`, which means it runs immediately and tears down the current scene before `PhaseIntro` or `PhaseSummary` have a chance to finish. The result: UI disappears mid-animation, or the summary is never shown.

**Fix:** Replace `LoadScene` with a coroutine using `LoadSceneAsync`, and only activate the new scene after the phase UI has finished.

See `fix_EnterShelterFlow.cs` for the full coroutine implementation.

---

## What's Coming After This Stream

For reference when talking about the roadmap:

- **Game loop:** `BeginPhase()` cases, day/night derived from `phaseCount`, food consumption
- **Inventory:** `ItemData` ScriptableObject, shared base inventory with HUD, loot points in Zone1, loot RNG
- **Zombie AI:** State machine (Idle → Detect → Aggro), patrol path, detection cone, Zone1 exit trigger
- **Survivor stations:** Rest Area sleep bonus, visual feedback on assignment

---

## Notes & Talking Points

- This is Unity **6.3 LTS** with **HDRP** — worth mentioning why HDRP (volumetric lighting, atmospheric fog, the quiet-city look)
- The game is being built as a **learning project** with a specific v1 goal: a data-driven resource and loot system using ScriptableObjects
- Survivor names are still placeholders — good moment to open it up to chat
- The `IInteractable` interface is planned for loot points — mention the pattern even if it's not implemented yet
