# Shared player stats formatting plan

## Notes

- CIDM 4390 AI Agentic Coding Part 4 refactoring requirement.
- Baseline: commit 549622e, containing completed Category A and Category B functionality.
- Architecture analysis compared shared stats formatting, travel-command interpretation, and trivial combat forwarding methods. The user selected shared player stats formatting.
- The user approved the final method contract and all three testing seams, then authorized to-spec followed by to-tickets.
- Both tickets were subsequently approved, published, and implemented under separate user requests. Both are resolved, and the user confirmed successful manual testing and authorized the final refactor-only commit and non-force push to main.
- Both numbered tickets were subsequently approved and published. The user authorized implementing Ticket 01 only, without production edits, commits, or pushes.

## Decisions-so-far

- Consolidate the duplicated stat formatting in Player and Combat into one formatting implementation owned by Player.
- Add public Player.GetStatsDisplay(Messages messages), returning localized shared stat rows as a string.
- Include strength, agility, health, and gold, followed by existing conditional upgrade effects and optional weapon name and maximum damage, in the existing order.
- Separate rows with Environment.NewLine; return no leading or trailing newline.
- Read existing state only. Read no console input, write no console output, and modify no player, weapon, or combat state.
- Player.DisplayStats retains the character heading, name, race, and occupation before the shared rows, including its existing final newline.
- Combat.GetCombatStatsDisplay retains combat headings, the player name, blank separator, and the complete dragon block, including its existing lack of a trailing newline.
- Preserve every existing heading, content, ordering, space, separator, and newline in both complete displays.
- Keep inventory display independent and unchanged, including its equipment-only content and explicit None statuses.
- Preserve character statistics, merchant purchases, watchtower behavior, inventory commands, travel EOF, combat mechanics, and all four supported languages.
- Reuse existing Messages labels, weapon translations, and formatting conventions. No new translations are required.
- Approved testing seams: Player.GetStatsDisplay, existing Player.DisplayStats, and existing Combat.GetCombatStatsDisplay.
- Establish exact complete-output characterization tests against unchanged production code before extraction.
- Use red-green-refactor TDD for the new shared method, retain characterization tests, and run regression checks throughout.
- Expected production changes are limited to Player and Combat. No new classes, display options, or formatting framework are needed.
- This reversible extraction does not require a new domain term or ADR.
- Ticket 01 is resolved: 48 exact-output characterization tests pass against unchanged production, the Release build has no warnings/errors, and all 360 regressions pass. Standards and Spec reviews have no findings. See [characterization baseline](ticket-01-validation.md).
- The user subsequently authorized implementing Ticket 02. It is resolved: Player.GetStatsDisplay supplies both displays, all 77 focused cases and 389 regressions pass, and both reviews have no findings. Frozen complete-output expectations and inventory remain unchanged. See [TDD and migration evidence](ticket-02-validation.md).

## Fog

- No unresolved requirements remain for the approved shared player stats refactor. Both tickets are resolved; manual testing and finalization authorization are complete.

## Artifacts

- [Formal PRD](spec.md)
- [Approved ticket breakdown](ticket-breakdown.md)
- [Resolved Ticket 01](issues/01-characterize-existing-stats-output.md)
- [Resolved Ticket 02](issues/02-consolidate-shared-player-stats.md)
