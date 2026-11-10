# 02: Consolidate shared player stats while preserving both displays

Status: resolved
Category: enhancement
Type: task

**What to build:** One read-only Player-owned formatting implementation supplies
the common player rows to both existing displays. Players see no changes in
character summaries, combat displays, inventory, or gameplay.

**Blocked by:** 01 — Lock existing character and combat display output.
The approved characterization checkpoint must pass before production changes.

### Acceptance criteria

- [x] Verify ticket 01's passing baseline before editing production code.
- [x] Add the approved public Player.GetStatsDisplay(Messages messages) method returning strength, agility, health, gold, existing optional upgrade effects, and optional weapon name/maximum damage in their current order.
- [x] Separate shared rows with Environment.NewLine, with no leading or trailing newline. Include no headings, identity lines, dragon content, separator, or inventory text.
- [x] Preserve optional-row rules, existing translations, numeric values, damage calculations, and upgrade visibility independent of weapon availability. Read no input, write no console output, and mutate no state.
- [x] Route both existing displays through the shared implementation while retaining their current headings, identity composition, row placement, separators, and exact newline contracts.
- [x] Keep inventory independent and unchanged, including its equipment-only scope and explicit None statuses.
- [x] Add new-method tests at the approved seam for exact shared text, all languages and equipment states, no console interaction, and unchanged player/weapon state.
- [x] Use red-green-refactor TDD: observe meaningful new-method test failure before implementation, add the smallest passing change, and then migrate/refactor with all characterizations green. Do not batch imagined behavior tests or weaken expected output.
- [x] Limit production edits to Player and Combat; add no classes, formatting flags, generalized actor framework, or localization changes.
- [x] Preserve all existing merchant, watchtower, inventory, navigation/EOF, and combat behavior. Retain all old regression tests.
- [x] Run focused tests during migration, then Release build, full regression suite, whitespace check, and code review. Record red/green evidence and exact-output preservation results.

**Verification:** Compare both complete public display outputs against ticket
01's unchanged expectations, exercise the new shared method directly, and finish
with a passing full regression suite and review.

## Answer

Verified Ticket 01's 48 passing characterizations and frozen fixture checksum before production edits. Implemented GetStatsDisplay through red-green tests, then migrated character and combat displays separately while all exact-output expectations stayed green. Release build passed with zero warnings or errors; all 77 focused tests and all 389 regressions passed. Standards and Spec reviews found no actionable issues. See [TDD and migration evidence](../ticket-02-validation.md). Inventory and unrelated production files are unchanged. No commit or push performed.
