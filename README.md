# What Remains

> *A stealth survival game about two people holding on.*

---

## Concept

You and the person you love are holed up in an apartment in Bellhaven. The streets below are quiet in the wrong way. Every day one of you has to go out. Every night you count what came back.

Bellhaven was the kind of city young couples moved to when the rent was almost right and it felt like theirs. Mixed blocks, mid-rise walkups, corner groceries, a farmers market on Saturdays. Familiar enough to feel like somewhere real. Empty enough now to feel like nowhere at all.

---

## Setting

| | |
|---|---|
| **Location** | Bellhaven. A mid-size Northeast city — mixed-use blocks, mid-rise residential bleeding into shuttered commercial streets. |
| **Time period** | Ambiguous and timeless. No specific year, no cultural markers. |
| **Tone** | Tense and atmospheric. Quiet desperation — not action, not horror. |

---

## Characters

Two survivors. A couple. Every assignment decision carries relational weight — who goes out, who stays behind, what you're willing to risk for the other person.

- **Survivor 1** — Name TBD *(placeholder: Cosmo)*
- **Survivor 2** — Name TBD *(placeholder: Wanda)*

---

## Gameplay loop (v1)

1. **Base phase** — Assign survivors to stations. Manage shared inventory.
2. **Explore phase** — One survivor enters a single location. Sneak past zombies. Loot. Escape or die.
3. **Resolution** — Return to base. Time advances. Consume food. Repeat.

> v1 does not include hiding, combat, or crafting.

---

## Design pillars

- **Atmosphere-first** — The world should feel before it explains.
- **Resource pressure** — Every phase costs something. The loop should feel like a slow drain.
- **Detection is the mistake** — No combat in v1. Being seen means you failed.
- **Readability** — The player should always understand their state and options.
- **Clear failure conditions** — Die to a zombie or run out of food.

---

## v1 learning goal

Implement a data-driven resource and loot system using ScriptableObjects.

---

## Scope protection (v1)

- One survivor explores at a time
- Detection = fail state (run back or die — no hiding)
- One explore location (one layout, one patrol pattern)

---

*What Remains — Concept v1*
