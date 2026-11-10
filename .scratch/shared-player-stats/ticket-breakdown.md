# Approved shared player stats refactoring ticket breakdown

Status: resolved
Category: enhancement

This document preserves the approved planning scopes and acceptance criteria.
Both numbered implementation tickets were published and are now resolved, with
separate characterization and TDD validation records. The checklists below
remain the original planning record; completed criteria are recorded in each
resolved ticket.

## 01: Lock existing character and combat display output

**What to build:** Exact-output characterization coverage proving that the
current character summary and full combat display preserve their present
content and layout across supported languages and equipment states.

**Blocked by:** None. This prerequisite changes tests only, as explicitly
required by the user before any production edit.

### Acceptance criteria

- [ ] Characterize both existing complete public displays against unchanged production code before any shared-method implementation.
- [ ] Cover English, Spanish, French, and Italian; no upgrade, armor, and enchantment; and present/absent weapon, including independent retained upgrades.
- [ ] Compare exact complete output, preserving headings, name, race/occupation placement in the character summary, player/dragon sections in combat, row order, spaces, punctuation, and blank separator.
- [ ] Explicitly protect the character display's final console newline and the combat string's absence of a trailing newline, using Environment.NewLine.
- [ ] Use deterministic fixtures and independent expected output. Do not compute expected display text through a future shared formatter.
- [ ] Preserve existing character, combat, inventory, merchant, watchtower, navigation, and localization tests. Run focused characterization tests, Release build, and the full regression suite.
- [ ] Record the passing pre-refactor baseline. Characterization cases should pass against current correct behavior; do not fabricate failures to claim a red phase.
- [ ] Leave production source, localization data, inventory formatting, and gameplay unchanged.

**Verification:** The new tests pass on the existing displays and are capable
of detecting differences in exact formatting rather than merely confirming row
presence. Record the results so ticket 02 can verify its prerequisite.

## 02: Consolidate shared player stats while preserving both displays

**What to build:** One read-only Player-owned formatting implementation supplies
the common player rows to both existing displays. Players see no changes in
character summaries, combat displays, inventory, or gameplay.

**Blocked by:** 01 — Lock existing character and combat display output.
The approved characterization checkpoint must pass before production changes.

### Acceptance criteria

- [ ] Verify ticket 01's passing baseline before editing production code.
- [ ] Add the approved public Player.GetStatsDisplay(Messages messages) method returning strength, agility, health, gold, existing optional upgrade effects, and optional weapon name/maximum damage in their current order.
- [ ] Separate shared rows with Environment.NewLine, with no leading or trailing newline. Include no headings, identity lines, dragon content, separator, or inventory text.
- [ ] Preserve optional-row rules, existing translations, numeric values, damage calculations, and upgrade visibility independent of weapon availability. Read no input, write no console output, and mutate no state.
- [ ] Route both existing displays through the shared implementation while retaining their current headings, identity composition, row placement, separators, and exact newline contracts.
- [ ] Keep inventory independent and unchanged, including its equipment-only scope and explicit None statuses.
- [ ] Add new-method tests at the approved seam for exact shared text, all languages and equipment states, no console interaction, and unchanged player/weapon state.
- [ ] Use red-green-refactor TDD: observe meaningful new-method test failure before implementation, add the smallest passing change, and then migrate/refactor with all characterizations green. Do not batch imagined behavior tests or weaken expected output.
- [ ] Limit production edits to Player and Combat; add no classes, formatting flags, generalized actor framework, or localization changes.
- [ ] Preserve all existing merchant, watchtower, inventory, navigation/EOF, and combat behavior. Retain all old regression tests.
- [ ] Run focused tests during migration, then Release build, full regression suite, whitespace check, and code review. Record red/green evidence and exact-output preservation results.

**Verification:** Compare both complete public display outputs against ticket
01's unchanged expectations, exercise the new shared method directly, and finish
with a passing full regression suite and review.

## Proposed order and rationale

1. Establish the user-requested characterization checkpoint with zero production edits.
2. Implement the complete refactor across the shared method and both consumers.

The first ticket is deliberately a characterization prerequisite rather than
a gameplay feature; it protects both complete observable displays end to end.
The second is one complete refactoring slice, including implementation,
integration, new-method tests, and regression validation. No separate tickets
for labels, languages, or individual source edits are needed.

## Approval and publication

The user approved this two-ticket breakdown exactly as proposed, including the
dependency of ticket 02 on ticket 01. Both tickets were published and implemented
under separate explicit authorizations. Characterization was established before
production changes; the complete refactor then passed automated and manual checks.
