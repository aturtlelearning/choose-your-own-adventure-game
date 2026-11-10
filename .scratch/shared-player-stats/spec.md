# PRD: Shared Player Stats Formatting

Status: ready-for-agent
Category: enhancement
Design: approved, including method contract and three testing seams
Phase: complete; both tickets resolved, automated validation passed, and manual testing confirmed

## Problem Statement

The Choose Your Own Adventure game's character summary and combat display
independently format the same player statistics, optional magical-upgrade
effects, and weapon details. Equipment changes have already required matching
updates to both displays. Future changes risk inconsistent output or omissions.

For the CIDM 4390 AI Agentic Coding Part 4 refactoring requirement, the user
wants one small, meaningful consolidation that improves locality without
changing any visible output or gameplay. The complete displays differ in their
headings, identity content, separator, dragon content, and newline contracts;
those differences must remain exact.

## Solution

Concentrate only the shared contiguous player-stat rows in a read-only method
owned by Player. Both existing displays consume that shared formatting while
retaining their existing screen composition and output behavior.

The approved public method is Player.GetStatsDisplay(Messages messages), returning
a string whose rows are separated by Environment.NewLine, without leading or
trailing newlines. It formats strength, agility, health, gold, existing purchased
upgrade effects, and optional weapon name and maximum damage. It reads no input,
writes no console output, and changes no state.

Players see precisely the same character and combat displays as before. The
inventory display remains independent and unchanged. Preserve English, Spanish,
French, and Italian, existing calculations, and every current encounter.

## User Stories

1. As a player, I want my character summary to look exactly as before, so that the refactor does not change familiar information.
2. As a player, I want the character heading and identity lines preserved, so that my name, race, and occupation remain in their existing positions.
3. As a player, I want my combat display to look exactly as before, so that the refactor does not change combat information.
4. As a player, I want the combat headings and player name preserved, so that the two combatants remain clearly identified.
5. As a player, I want the dragon's complete stat block preserved, so that its information is unaffected by player formatting changes.
6. As a player, I want the blank separator between combatants preserved, so that the layout remains identical.
7. As a player, I want row order, spaces, punctuation, and newlines preserved, so that the visible presentation does not drift.
8. As a player, I want current strength, agility, health, and gold displayed, so that both screens reflect my existing state.
9. As a player with magical armor, I want its existing damage-reduction row preserved, so that its protection remains visible.
10. As a player with weapon enchantment, I want its existing separate magic-damage bonus preserved, so that the display reflects the existing effect.
11. As a player without upgrades, I want unpurchased-effect rows omitted as before, so that the summaries retain their existing content.
12. As a player with a weapon, I want its translated name and current maximum damage preserved, so that both displays show the same equipment details.
13. As a player without a weapon, I want weapon rows omitted from these summaries as before, so that the refactor does not add inventory-style None labels.
14. As a player with an upgrade and no weapon, I want the existing upgrade-effect display preserved, so that equipment absence does not hide tracked effects.
15. As a player, I want viewing statistics to leave my character unchanged, so that presentation cannot affect the adventure.
16. As an English-speaking player, I want existing English text preserved exactly, so that the refactor adds no wording changes.
17. As a Spanish-speaking player, I want existing Spanish displays preserved exactly, so that localization remains complete.
18. As a French-speaking player, I want existing French displays preserved exactly, so that localization remains complete.
19. As an Italian-speaking player, I want existing Italian displays preserved exactly, so that localization remains complete.
20. As a player, I want inventory to retain its independent equipment-only format, so that this consolidation does not change its content or absence statuses.
21. As a player, I want purchases, watchtower visits, navigation, inventory commands, and graceful travel EOF preserved, so that the completed features remain intact.
22. As a player, I want combat rules and calculations unchanged, so that the refactor affects presentation alone.
23. As a maintainer, I want shared player rows formatted in one place, so that a future correction reaches both displays consistently.
24. As a maintainer, I want exact output characterization before extraction, so that tests can detect accidental presentation changes independently of the new shared method.

## Implementation Decisions

- Add the approved public Player.GetStatsDisplay(Messages messages) method returning localized text. It is the single implementation of the shared contiguous player-stat rows.
- Preserve the current shared row order: strength, agility, health, gold, purchased armor reduction when present, purchased enchantment bonus when present, and weapon name followed by weapon maximum damage when a weapon exists.
- Preserve each current conditional rule. Upgrade effects remain independent of weapon availability; unpurchased effect rows and absent-weapon rows remain omitted. Do not add explicit None statuses to the character or combat summaries.
- Use current values and existing Messages labels, TranslateWeaponForDisplay, and formatting placeholders. Preserve existing separate bonus and weapon maximum-damage semantics; do not substitute inventory wording or combine damage values.
- Join shared rows with Environment.NewLine and return no leading or trailing newline. At least strength, agility, health, and gold are always present.
- The shared method contains no character heading, name, race, occupation, combat heading, blank combat separator, dragon rows, or inventory heading.
- Keep character heading, name, race, and occupation formatting in the existing character display, then print the shared rows. Preserve its final console newline.
- Keep combat headings and player name, followed by shared rows, the existing blank separator, and the unchanged dragon block in the existing combat display. Preserve its returned string without a trailing newline.
- Leave the small name-line duplication in the existing display composition. Avoid formatting flags, optional identity modes, new classes, generalized actor formatting, or a configurable presentation framework.
- Keep Player.GetInventoryDisplay and inventory composition independent and unchanged. Do not change combat, purchase, random-roll, navigation, or EOF behavior.
- Expected production modifications are limited to the Player and Combat modules. Existing Messages implementation, localization data, and accepted localization ADR remain unchanged.

## Testing Decisions

All three testing seams below were explicitly approved by the user. Test the
public results rather than private helpers, internal call counts, or reflected
implementation structure.

| Approved seam | Required verification | Limits |
| --- | --- | --- |
| Player.GetStatsDisplay(Messages) | Exact shared rows, order, existing optional-row rules, four languages, no leading/trailing newline, read-only behavior, and no console interaction | Does not prove whole-screen composition |
| Player.DisplayStats(Messages) | Exact captured complete character output, headings, identity lines, ordering, whitespace, and final newline | Does not verify the complete combat display |
| Combat.GetCombatStatsDisplay() | Exact complete combat string, player and dragon sections, headings, blank separator, and absence of a trailing newline | Does not execute combat rounds |

- Before any production edit, establish passing characterization tests for both existing complete displays. Use deterministic player and dragon fixtures and independently reviewed expected text from the current behavior.
- Cover English, Spanish, French, and Italian; no upgrade, magical armor, and weapon enchantment; and present/absent weapon cases, including an upgrade retained after the weapon is cleared for test setup.
- Compare complete strings, including punctuation, spaces, row ordering, separator lines, and platform newlines. Existing row-presence assertions alone are insufficient.
- Do not derive expected complete output by calling the proposed shared method. The characterizations must remain capable of detecting a common mistake in both consumers.
- Preserve current null-weapon omission behavior in the summaries and the independent purchased-effect rows. Include varied existing weapons and values to prevent hardcoded output.
- New-method tests must observe unchanged player and weapon state and no console reads/writes. Use exact expected shared text with no leading/trailing newline.
- Follow existing merchant stats/localization tests and combat-display tests as prior art. Retain watchtower and inventory console-state comparisons as regression protection. Restore console streams and prevent parallel interference in console-capture fixtures.
- Characterization tests describe already-correct behavior and should pass before extraction. Do not manufacture red failures. After that baseline, use red-green-refactor TDD for the new shared method: write a meaningful failing test for the absent contract before the minimum implementation, then refactor while preserving all characterization output.
- Keep all three public seams covered as each existing display is migrated. A formatting refactor must not introduce changed behavior merely to produce a red phase.
- Run focused display tests throughout; finish with the Release solution build, complete regression suite, and git diff --check. The current baseline suite contains 312 tests; additional characterization and method tests will increase that count.
- Record baseline characterization results, new-method red/green evidence, refactor verification, full-suite results, and code-review findings during authorized implementation.

## Acceptance Criteria

- [ ] Before any production change, passing characterization tests capture the exact existing complete character and combat output using deterministic fixtures and independent expected text.
- [ ] Characterization coverage includes all four languages, no upgrade, armor, enchantment, and present/absent weapon states, including retained upgrades without a weapon.
- [ ] Player exposes the approved public GetStatsDisplay(Messages messages) method returning a string, with one shared implementation of the existing contiguous player-stat rows.
- [ ] Shared rows remain in their existing order: strength, agility, health, gold, conditional armor reduction, conditional enchantment bonus, and optional weapon name and maximum damage.
- [ ] The shared result uses Environment.NewLine between rows and has no leading or trailing newline. It contains no headings, identity lines, separator, dragon statistics, or inventory text.
- [ ] Current values, existing localized labels, weapon translations, formatting placeholders, and optional-row omission rules are preserved. No hardcoded example values, combined damage figures, or new None statuses are introduced.
- [ ] Both existing displays consume the shared formatting while preserving their complete output exactly: headings, content, identity lines, row order, punctuation, spaces, separators, and newline behavior.
- [ ] Player.DisplayStats retains its final console newline. Combat.GetCombatStatsDisplay retains its blank separator before the unchanged dragon block and returns no trailing newline.
- [ ] The shared method reads no console input, writes no console output, and changes no player or weapon state, upgrade ownership, derived effects, or combat calculations.
- [ ] Inventory rendering remains independent and unchanged, including its equipment-only content, explicit None statuses, separate bonus, and read-only behavior.
- [ ] Merchant purchases, watchtower encounters, dragon combat, navigation, inventory commands, and graceful travel EOF remain unchanged.
- [ ] English, Spanish, French, and Italian output and localization data remain unchanged; no message keys, mappings, translations, or placeholders are modified.
- [ ] Tests exercise the three approved public seams with exact-output assertions and state-preservation checks; existing characterization expectations are not rewritten to match an accidental output change.
- [ ] After establishing the characterization baseline, implementation follows red-green-refactor TDD for the new method and preserves all characterization tests while migrating both consumers.
- [ ] Production changes are confined to Player and Combat, without new classes, formatting flags, configurable modes, or unrelated refactors.
- [ ] Focused tests, the Release build, the full existing regression suite, whitespace validation, and code review pass, with characterization and TDD evidence recorded before completion.

## Out of Scope

- Production edits or new tests during this specification and ticket-planning phase.
- Changes to inventory rendering, character properties, purchases, magical effects, combat mechanics, random-roll sequences, or dragon formatting.
- Changes to Game, command interpretation, navigation, error handling, or EOF handling.
- Changes to localization text, message keys, mappings, placeholders, or supported languages.
- A new formatting framework, new domain model, inheritance hierarchy, configurable display modes, or inventory management.
- Fixing other architecture candidates or removing combat forwarding methods.
- Commits, pushes, or implementation without later authorization.

## Further Notes

The user chose the stats candidate after comparison with duplicated travel
interpretation and shallow combat forwarding methods. Shared formatting offers
concrete locality and leverage for two existing consumers while keeping scope
small. No existing ADR is contradicted, and no new domain glossary term is
needed for a presentation-only extraction.

The baseline is commit 549622e. The [planning map](map.md) records the approved
design. The architecture HTML report remains outside the repository in the OS
temporary directory. Both numbered tickets are resolved with recorded
characterization and red-green-refactor evidence. The user confirmed successful
manual testing and authorized committing only this refactor and pushing normally
to main after final validation.
