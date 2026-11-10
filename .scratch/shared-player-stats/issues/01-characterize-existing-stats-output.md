# 01: Lock existing character and combat display output

Status: resolved
Category: enhancement
Type: task

**What to build:** Exact-output characterization coverage proving that the
current character summary and full combat display preserve their present
content and layout across supported languages and equipment states.

**Blocked by:** None. This prerequisite changes tests only, as explicitly
required by the user before any production edit.

### Acceptance criteria

- [x] Characterize both existing complete public displays against unchanged production code before any shared-method implementation.
- [x] Cover English, Spanish, French, and Italian; no upgrade, armor, and enchantment; and present/absent weapon, including independent retained upgrades.
- [x] Compare exact complete output, preserving headings, name, race/occupation placement in the character summary, player/dragon sections in combat, row order, spaces, punctuation, and blank separator.
- [x] Explicitly protect the character display's final console newline and the combat string's absence of a trailing newline, using Environment.NewLine.
- [x] Use deterministic fixtures and independent expected output. Do not compute expected display text through a future shared formatter.
- [x] Preserve existing character, combat, inventory, merchant, watchtower, navigation, and localization tests. Run focused characterization tests, Release build, and the full regression suite.
- [x] Record the passing pre-refactor baseline. Characterization cases should pass against current correct behavior; do not fabricate failures to claim a red phase.
- [x] Leave production source, localization data, inventory formatting, and gameplay unchanged.

**Verification:** The new tests pass on the existing displays and are capable
of detecting differences in exact formatting rather than merely confirming row
presence. Record the results so ticket 02 can verify its prerequisite.

## Answer

Established 48 passing exact-output cases covering both existing displays across four languages and all six upgrade/weapon states. Production source and localization data remain unchanged. Release build passed with zero warnings or errors; all 360 regression tests passed. Independent Standards and Spec reviews found no actionable issues. See [characterization baseline and validation](../ticket-01-validation.md). Ticket 02 remains unimplemented. No commit or push performed.
