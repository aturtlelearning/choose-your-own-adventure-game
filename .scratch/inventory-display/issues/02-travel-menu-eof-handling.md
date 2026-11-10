# 02: Exit gracefully when travel input ends, including after inventory

Status: resolved
Category: enhancement
Type: task

**What to build:** When console input ends at the travel menu, players see the
existing localized adventure-ending message once and the game terminates. This
also works immediately after inventory without errors, fabricated actions, or
retries on exhausted input.

**Blocked by:** 01 — View current equipment from the travel menu in all four
languages. That ticket supplies the inventory command required to verify EOF
immediately after inventory. No additional prerequisite ticket is needed.

### Acceptance criteria

- [x] Distinguish EOF from invalid and blank travel input. EOF terminates the game without entering an error-and-retry loop or inventing a command.
- [x] Reuse the existing translated adventure-ending message exactly once in English, Spanish, French, and Italian.
- [x] Handle EOF at the first travel prompt and immediately after inventory, including both inventory command tokens.
- [x] Handle travel EOF after invalid/blank input and after repeat inventory viewing without breaking existing retry behavior for actual input.
- [x] EOF changes no health, gold, equipment, upgrade ownership, attributes, or combat statistics; preserve existing merchant and watchtower EOF behavior and dragon combat rules.
- [x] Add bounded scripted-console tests that observe real EOF and fail promptly if the game retries exhausted input. Assert correct output order, one ending message, no error for EOF itself, and no fabricated action.
- [x] Cover both required EOF locations in every supported language and retain the display, command, localization, and state-preservation coverage from ticket 01.
- [x] Use the approved console game flow and Messages test seams; introduce no inventory classes, persistence, player state, or testing-only production interfaces.
- [x] Run relevant tests during implementation and the full existing regression suite before completion, preserving Category A and existing gameplay tests. Record TDD evidence and results.

**Verification:** Demonstrate finite redirected input ending at travel, then
finite input ending immediately after the inventory command. In both cases the
game prints the selected language's existing ending message and returns normally
without requesting more input. Follow existing watchtower EOF tests as prior art.

## Required implementation workflow

- Use red-green-refactor TDD: first observe a meaningful failing test for each missing behavior, then add the minimum implementation to pass, then refactor while keeping tests green. Do not write all tests in a batch.
- Retain initially passing characterization tests for existing correct behavior; do not manufacture failures or add unnecessary production changes.
- Preserve the merchant, ruined watchtower, dragon encounter, character state, existing translations, and all four supported languages.
- Reuse existing player/weapon data and Messages. Add no inventory management system or changes to combat calculations.
- Ticket publication is not authorization to implement, commit, or push.

## Answer

Verified Ticket 01 resolved before starting. Demonstrated 12 failing EOF cases, applied the minimal travel-menu null-input fix, refactored shared test helpers while green, and added 16 retry/repeat EOF regressions. Release build passed with zero warnings or errors; all 81 inventory tests and all 312 regression tests passed. Standards and Spec reviews found no actionable issues. See [TDD evidence and validation](../ticket-02-validation.md). No commit or push performed.
