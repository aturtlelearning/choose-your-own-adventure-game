# Ruined watchtower planning

## Notes

- CIDM 4390 AI Agentic Coding Part 4, Category A.
- Requirements interview, approved PRD, and both implementation tickets are complete.
- During planning, the user made final design decisions through a one-question-at-a-time interview.

## Decisions-so-far

- Add a ruined watchtower accessible by traveling west (`w`).
- Keep the encounter short, with an interesting description and exactly two player choices with distinct consequences.
- Return the player to the existing travel menu after the encounter.
- Preserve the north dragon encounter, south merchant encounter, and combat mechanics.
- Support English, Spanish, French, and Italian throughout the new branch.
- Consequences are story-only: no changes to health, gold, equipment, or combat abilities.
- Choice 1: climb the stairs, discover claw marks, and glimpse the dragon's mountain through the broken roof.
- Choice 2: search the ground floor and discover an old guard's journal describing the tower's mysterious abandonment.
- Each choice displays its own narrative and returns directly to the travel menu.
- Allow unlimited repeat visits, offering the same two choices and narratives each time.
- Do not track previous visits or introduce additional player state.
- Use `1` to climb the stairs and `2` to search the ground floor.
- Invalid or blank input displays a translated error and repeats the watchtower menu until a valid selection.
- A valid selection displays its corresponding outcome and returns to the travel menu.
- Console EOF exits gracefully without an infinite loop.
- The English arrival and outcome text below is approved verbatim and is the baseline for accurate Spanish, French, and Italian translations.
- Preserve the mystery of the watchtower's abandonment and its connection to the existing dragon encounter.
- Accept both `w` and `west`, ignoring capitalization and leading or trailing spaces, consistently with existing north and south commands.
- Explain the west option in English, Spanish, French, and Italian travel prompts.
- Preserve north, south, and `e`/`exit` behavior and existing translated invalid-travel-command handling.
- Preserve existing test coverage. The user explicitly authorized changing the existing assertion rejecting west to reflect west as a supported direction, before gameplay implementation.
- After planning, use to-spec, to-tickets, and implementation with test-driven development.
- The user approved the formal PRD and all three testing boundaries: the existing console game flow, public path validator, and Messages localization interface.
- Preserve the intentionally failing west-acceptance test as evidence of the TDD red phase until gameplay implementation is authorized.
- Ticket preparation is authorized; gameplay implementation remains unauthorized.
- Subsequent authorization: the user approved the two-ticket breakdown and authorized implementation of Ticket 01 only, without commits or pushes.
- Ticket 01 is resolved: west travel, both narratives, all four languages, and automated regression coverage are complete. See [validation and TDD evidence](ticket-01-validation.md).
- The user subsequently authorized Ticket 02 with red-green-refactor TDD, no commit, and no push. Ticket 02 is resolved: translated input recovery, graceful watchtower EOF, repeat visits, and state preservation are verified. See [validation and TDD evidence](ticket-02-validation.md).
- Finalization authorization: the user confirmed manual testing of west travel, both story choices, repeat visits, and return to the travel menu, and requested a Category A-only commit and non-force push to main after final validation.

## Fog

- No unresolved product requirements. The user confirmed the completed plan.
- Both tickets are resolved. No unresolved requirements remain for the approved ruined watchtower PRD.

## Approved English story text

### Arrival

A ruined watchtower rises beside the western trail. Ivy grips its cracked stone walls, and wind whistles through the broken roof. Inside, worn stairs climb above a ground floor strewn with fallen timber.

### Choice 1 — Climb the stairs

You carefully climb the worn stairs. Deep claw marks scar the stone near the top. Through the broken roof, you glimpse the dragon’s mountain against the sky. You return to the trail, wondering what once landed here.

### Choice 2 — Search the ground floor

Beneath a fallen beam, you discover an old guard’s journal. Its final entry reads: ‘The wingbeats returned after sunset. We leave before dawn.’ The remaining pages are blank. You replace the journal and return to the trail.
