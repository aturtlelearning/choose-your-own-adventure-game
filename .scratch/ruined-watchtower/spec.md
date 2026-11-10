# PRD: West-Path Ruined Watchtower

Status: ready-for-agent
Category: enhancement
Design approved: 2026-11-10
Phase: complete; both tickets implemented, automatically tested, and manually verified

## Problem Statement

For CIDM 4390 AI Agentic Coding Part 4, Category A, the existing Choose Your
Own Adventure game needs a small new location with an interesting description
and meaningful player choices. Players currently travel north to the dragon or
south to the merchant. They need another short story branch without changing
combat, purchases, player statistics, or the four supported languages.

## Solution

Add a ruined watchtower on the west path. Accept `w` and `west`, ignoring
capitalization and leading or trailing spaces. Explain the west option in the
English, Spanish, French, and Italian travel prompts. Preserve the existing
north, south, and `e`/`exit` behavior and translated invalid-travel handling.

On every visit, display the approved arrival narrative and exactly two choices:
`1` to climb the stairs and `2` to search the ground floor. Each valid choice
displays its own approved narrative and returns directly to the existing travel
menu. The consequences are discoveries in the story, with no changes to health,
gold, equipment, or combat abilities.

Allow unlimited visits with identical options and outcomes. Track no visits and
introduce no additional player state. Invalid or blank watchtower input displays
a translated error and repeats the watchtower menu. EOF at that menu exits the
game gracefully without a retry loop or a fabricated selection.

## User Stories

1. As a player, I want the travel menu to explain the west option, so that I can discover the ruined watchtower.
2. As a player, I want both `w` and `west` to work, so that travel commands remain consistent with north and south.
3. As a player, I want capitalization and surrounding spaces ignored, so that ordinary typing variations work.
4. As a player, I want invalid travel commands handled as before, so that navigation remains familiar.
5. As a player, I want a short description on arrival, so that I can picture the watchtower.
6. As a player, I want exactly two numbered actions, so that the encounter stays clear and manageable.
7. As a player, I want choice `1` to climb the stairs, so that I can discover claw marks and glimpse the dragon's mountain.
8. As a player, I want choice `2` to search the ground floor, so that I can discover the guard's journal and the mystery of the abandonment.
9. As a player, I want only the selected outcome displayed, so that my choice determines the discovery.
10. As a player, I want both outcomes to return to the travel menu, so that I can continue my adventure.
11. As a player, I want unlimited repeat visits, so that I can explore both stories in either order.
12. As a player, I want the same options and narratives on every visit, so that the encounter remains predictable.
13. As a player, I want exploration to leave my health unchanged, so that the story does not alter my survival chances.
14. As a player, I want exploration to leave my gold and equipment unchanged, so that it does not affect merchant purchases.
15. As a player, I want exploration to leave combat abilities unchanged, so that the dragon encounter retains its existing rules.
16. As a player, I want invalid or blank watchtower input to show a translated error and repeat the choices, so that I can correct mistakes.
17. As a player using redirected console input, I want EOF to end the game gracefully, so that missing input cannot cause an infinite loop.
18. As an English-speaking player, I want the approved story text reproduced exactly, so that the agreed atmosphere and mystery are preserved.
19. As a Spanish-speaking player, I want all new story text, choices, prompts, and errors in Spanish, so that I can play the full branch.
20. As a French-speaking player, I want all new story text, choices, prompts, and errors in French, so that I can play the full branch.
21. As an Italian-speaking player, I want all new story text, choices, prompts, and errors in Italian, so that I can play the full branch.
22. As a player, I want the north dragon encounter and south merchant encounter preserved, so that the new location adds to the existing adventure.
23. As a player, I want `e`/`exit` to retain its behavior, so that I can still end the adventure as before.

## Implementation Decisions

- Extend existing travel validation and actual route selection consistently; validation alone must not claim support for a route the game cannot enter.
- Integrate the watchtower into the existing adventure loop so either completed outcome returns to the current character's travel menu.
- Keep watchtower behavior separate from merchant purchases and dragon combat. Do not change their rules or add rewards, damage, random rolls, inventory items, or visit flags.
- Use the existing JSON-based Messages localization approach. Keep message keys and formatting placeholders synchronized across English, Spanish, French, and Italian, consistent with the accepted Italian-language ADR.
- Use language-independent numeric choice tokens `1` and `2`; explain them in the selected language. The new travel tokens remain `w` and `west` in all languages.
- Handle a null console read as EOF, separately from blank or invalid input. End the game gracefully rather than returning to a menu that retries exhausted input.
- No new persistence, player properties, or changes to combat interfaces are required.
- No further module interfaces are prescribed by this PRD; prefer existing public behavior over new testing-only interfaces.

## Testing Decisions

Test externally observable behavior through public interfaces. Avoid private
method tests, internal call assertions, random combat dependencies in watchtower
tests, or assertions that merely duplicate the implementation.

Approved testing boundaries:

| Public boundary | What it verifies | Limits |
| --- | --- | --- |
| Game startup with redirected console input/output | Actual west routing, both choices, selected narratives, return to travel, repeat visits, retry behavior, EOF, and navigation to existing encounters | Console output alone cannot directly prove every player property is unchanged |
| Existing public path validator | Supported commands, capitalization, whitespace, and invalid commands | Does not prove the interactive route parser enters the watchtower |
| Existing public Messages interface | Four-language message availability, translations, key coverage, and formatting compatibility | Automated checks cannot fully judge translation quality |

- Follow existing merchant end-to-end console tests, game path-validation tests, and Italian localization coverage as prior art.
- The explicitly authorized change to the existing test rejecting `west` expects acceptance. It was confirmed red before gameplay implementation and is preserved and passing after implementation.
- Retain invalid-command coverage using an unsupported command during implementation, alongside acceptance coverage for `w`, `west`, uppercase, and padded input.
- Exercise both narratives and verify the other narrative is absent for that selection. Use the approved English text as an independent expected value.
- Exercise repeated visits with both choices, followed by another travel command, to verify return behavior and unchanged choices.
- Exercise invalid input, blank input, then a valid choice; separately exercise EOF at the watchtower menu and after invalid input.
- Check arrival, choices, both outcomes, error messages, and west travel guidance in all four languages. Review translations for accurate meaning and preserved mystery.
- Preserve existing merchant, player, dragon, combat, and localization regression coverage. Confirm the feature leaves player state unchanged at an appropriate public boundary; if a new boundary is needed to observe this directly, agree it before adding tests.
- During authorized implementation, use one behavior per red-to-green cycle, then run the relevant regressions and full test suite. Do not implement gameplay merely to make the authorized red test pass in this specification phase.

## Out of Scope

- Gameplay implementation during PRD preparation.
- Creating implementation tickets before the to-tickets step.
- New combat, enemies, rewards, health changes, gold changes, equipment changes, or combat bonuses.
- Tracking visits, collecting the journal, unlocking branches, alternate endings, or branching based on player statistics.
- New supported languages or translated travel-command aliases.
- Changes to the merchant encounter, dragon encounter, combat mechanics, or existing exit behavior.
- Resolving the watchtower's mystery beyond the approved text.

## Further Notes

The user approved the complete Category A requirements interview, PRD, testing
boundaries, and two implementation tickets. Both tickets are resolved; their
validation records preserve the red-green-refactor evidence and review results.
The original planning map remains the record of decisions and approved text.
The user confirmed successful manual testing of west travel, both story choices,
repeat visits, and return to the travel menu, and authorized committing and
pushing Category A to main.

The following English text is approved verbatim. Use it as the baseline for
accurate Spanish, French, and Italian translations.

### Arrival

A ruined watchtower rises beside the western trail. Ivy grips its cracked stone walls, and wind whistles through the broken roof. Inside, worn stairs climb above a ground floor strewn with fallen timber.

### Choice 1 — Climb the stairs

You carefully climb the worn stairs. Deep claw marks scar the stone near the top. Through the broken roof, you glimpse the dragon’s mountain against the sky. You return to the trail, wondering what once landed here.

### Choice 2 — Search the ground floor

Beneath a fallen beam, you discover an old guard’s journal. Its final entry reads: ‘The wingbeats returned after sunset. We leave before dawn.’ The remaining pages are blank. You replace the journal and return to the trail.
