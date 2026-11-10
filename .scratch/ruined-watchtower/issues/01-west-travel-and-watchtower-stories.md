# 01: Travel west and discover either watchtower story

Status: resolved
Category: enhancement
Type: task

**What to build:** Players travel west, see the arrival description and two
numbered choices, discover the selected story, and return to the travel menu.
This complete normal-input journey works in all four languages.

**Blocked by:** None (implementation still requires authorization).

- [x] Actual interactive travel and the public validator accept `w` and `west`, ignoring capitalization and surrounding spaces.
- [x] Travel guidance explains west in English, Spanish, French, and Italian; existing north, south, and `e`/`exit` behavior and invalid-travel errors remain intact.
- [x] Arrival displays the approved description and exactly two choices: `1` to climb the stairs and `2` to search the ground floor.
- [x] Each choice displays only its approved outcome and returns to the existing travel menu.
- [x] English arrival and outcomes match the approved PRD verbatim. Accurate Spanish, French, and Italian translations preserve the mystery and dragon connection.
- [x] Prompts, choices, and narratives use existing Messages localization with matching keys and formatting placeholders in all four languages.
- [x] No visit tracking or additional player state is introduced; health, gold, equipment, and combat abilities are unchanged.
- [x] Preserve the existing failing west-acceptance test until this ticket is implemented. Demonstrate red before making it green; never delete, skip, or weaken it.
- [x] Add tests one red-to-green cycle at a time at the approved console-flow, path-validator, and Messages boundaries. Cover route variants, both outcomes, return to travel, and all four languages.
- [x] Restore invalid-travel rejection coverage with an unsupported command rather than west.
- [x] Run existing merchant, dragon, combat, player, and localization regressions and finish with a passing full suite.

**Verification:** Demonstrate each choice followed by another valid travel command
in each supported language. Follow existing merchant console tests, public game
validation tests, and localization tests. Verify external behavior, not private
methods. Review translation accuracy as well as automated message coverage.

## Required implementation workflow

- [x] Use red-green-refactor TDD for each behavior: first observe a meaningful failing test, then implement only enough to pass, then refactor while keeping the tests green. Do not batch all tests before implementation.
- [x] Refactoring must preserve the approved behavior and public testing boundaries; rerun affected tests after each refactor and the full regression suite before completion.
- [x] Preserve the merchant encounter, dragon combat mechanics, and English, Spanish, French, and Italian support throughout implementation.

Implementation is not authorized by publication of this ticket. Do not implement gameplay code, commit, or push during ticket preparation.

## Answer

Ticket 01 implemented and verified: Release build passed with zero warnings or errors; all 195 Release tests passed; Standards and Spec reviews found no actionable issues. See [validation and TDD evidence](../ticket-01-validation.md). Ticket 02 remains unimplemented. No commits or pushes performed.
