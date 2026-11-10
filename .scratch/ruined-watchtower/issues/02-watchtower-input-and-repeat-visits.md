# 02: Recover from invalid input and revisit the ruined watchtower

Status: resolved
Category: enhancement
Type: task

**What to build:** Players correct invalid choices, explore repeatedly in either
order without changing player state, and exit gracefully when console input ends.
These complete scenarios work in all four languages.

**Blocked by:** 01 — Travel west and discover either watchtower story.
The interactive encounter and localized menus must exist before these scenarios
can be implemented and verified.

- [x] Invalid or blank watchtower input displays a translated error and repeats the two choices without displaying an outcome.
- [x] Valid input after any number of invalid or blank inputs displays only the selected outcome and returns to travel.
- [x] EOF at the watchtower menu exits the game gracefully, without looping, inventing a selection, or retrying an exhausted travel menu. This also holds after invalid or blank input.
- [x] Unlimited repeat visits offer the same arrival, choices, and outcomes, including both choice orders and repeating the same choice.
- [x] Retries and repeat visits change no health, gold, equipment, or combat abilities and introduce no visit flags or additional player state.
- [x] Errors and repeated menus work in English, Spanish, French, and Italian with matching keys and formatting placeholders.
- [x] Add console-flow tests incrementally for invalid-then-valid, blank-then-valid, immediate EOF, EOF after invalid input, and repeated visits followed by onward travel. Verify localized messages through Messages.
- [x] Exercise watchtower visits interleaved with the merchant, including preservation of purchases; confirm onward north and exit behavior without changing combat rules.
- [x] Use only the three approved public testing boundaries. Obtain agreement before adding any new boundary needed to directly observe player state.
- [x] Retain ticket 01 coverage, run relevant regressions, finish with a passing full suite, and record results and translation review.

**Verification:** Demonstrate recovery, both stories across repeat visits, and
termination with exhausted redirected input. Use a bounded timeout where needed
to catch EOF loops without hanging the test runner. Follow one behavior test and
minimal implementation per TDD cycle.

## Required implementation workflow

- [x] Use red-green-refactor TDD for each behavior: first observe a meaningful failing test, then implement only enough to pass, then refactor while keeping the tests green. Do not batch all tests before implementation.
- [x] Refactoring must preserve the approved behavior and public testing boundaries; rerun affected tests after each refactor and the full regression suite before completion.
- [x] Preserve the merchant encounter, dragon combat mechanics, and English, Spanish, French, and Italian support throughout implementation.

Implementation is not authorized by publication of this ticket. Do not implement gameplay code, commit, or push during ticket preparation.

## Answer

Ticket 01 was verified complete before starting. Ticket 02 is implemented and verified: Release build passed with zero warnings or errors; all 55 watchtower tests and all 231 regression tests passed. Independent Standards and Spec reviews found no actionable issues. See [TDD and validation evidence](../ticket-02-validation.md). No commit or push performed.
