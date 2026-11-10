# Inventory display planning

## Notes

- CIDM 4390 AI Agentic Coding Part 4, Category B.
- Requirements interview, all ten design decisions, formal PRD, and two-ticket breakdown are approved. Both tickets were implemented, reviewed, automatically verified, and manually tested. The user authorized the final Category B commit and non-force push to main.
- Planning used grill-with-docs and one question at a time, with final decisions made by the user.
- The assignment requires practicing handoff with a separate Codex session. The user explicitly requested pausing this interview before deciding which equipment fields to display.

## Decisions-so-far

- Make the inventory display available from the existing travel menu.
- Return to the travel menu after showing equipment.
- Show the current weapon and purchased magical armor or weapon enchantment. The completed handoff investigation found no additional tracked player equipment.
- Viewing inventory must not change health, gold, equipment, or combat stats.
- Support English, Spanish, French, and Italian.
- Keep the feature small and reuse existing systems; do not introduce an inventory management system.
- Preserve the merchant, ruined watchtower, dragon encounter, and existing functionality.
- Category A is complete and committed at fe7fb72. Its existing artifacts remain unchanged.
- Display the current weapon name and its existing base maximum damage.
- Display the effect of the purchased magical upgrade: weapon enchantment as +2 magic damage separately from base weapon damage, or magical armor as damage reduction: 2.
- If no upgrade was purchased, explicitly indicate that no magical upgrade is equipped.
- Use only equipment and values already tracked by the game; preserve existing combat calculations.
- Translate all inventory labels into English, Spanish, French, and Italian.
- Accept `i` and `inventory` at the existing travel menu, ignoring capitalization and leading or trailing spaces.
- After displaying inventory, return directly to the travel menu without changing player state.
- Explain the inventory command in all four translated travel prompts.
- Preserve north, south, west, and exit commands.
- If the player has no current weapon, display a translated "Weapon: None" message and omit the base maximum damage line.
- Show magical-upgrade status independently, even when no weapon is equipped. Do not invent a default weapon or damage value.
- Include an automated test for the no-weapon case; preserve existing combat functionality and all four languages.
- Keep the inventory equipment-only: show a translated inventory heading, current weapon name, base maximum damage when present, and the purchased magical upgrade's name and effect.
- If no magical upgrade has been purchased, display a translated "None" status.
- Do not include gold, health, strength, agility, other character statistics, or weapon artwork.
- Include both direct inventory display tests and scripted console-flow tests.
- Direct tests cover no magical upgrade, magical armor, weapon enchantment, and no weapon, and verify that displaying inventory never changes player state.
- Console-flow tests cover `i` and `inventory`, capitalization, surrounding spaces, repeat viewing, and return to the travel menu.
- Localization tests cover English, Spanish, French, and Italian, including missing message keys.
- Run the full existing regression suite.
- Use red-green-refactor TDD, writing a failing test before implementing each missing behavior.
- Preserve translated error-and-retry behavior for invalid travel commands, and update the error guidance in English, Spanish, French, and Italian to list `n`, `s`, `w`, `i`, and `e`.
- Continue accepting full-word commands `north`, `south`, `west`, `inventory`, and `exit`.
- Invalid input must not change player state; preserve all existing gameplay.
- EOF at the travel menu exits gracefully, including immediately after viewing inventory. Display the existing translated adventure-ending message and terminate without an error-and-retry loop.
- EOF must not change health, gold, equipment, or combat statistics. Reuse the existing localization system in all four languages.
- Include automated tests for EOF at the travel menu and immediately after inventory, using red-green-refactor TDD.
- The English inventory labels below are approved exactly as written as the baseline for Spanish, French, and Italian translations.
- Weapon names and numeric values come from existing player and weapon data. Example weapon names and values must not be hardcoded into gameplay.
- Add a read-only Player.GetInventoryDisplay(Messages) method returning localized inventory text from existing equipment and upgrade data. The method must not read console input or modify player state.
- Game.cs handles `i`/`inventory`, prints the returned text, and returns to the travel menu.
- Use Messages for all four supported languages. Add no new inventory classes and make no changes to combat mechanics.
- The direct-display test seam is Player.GetInventoryDisplay(Messages); scripted console-flow tests verify commands and navigation, with localization tests through Messages.
- The user approved the complete Category B plan and confirmed that all ten design decisions and the required handoff investigation are finalized.
- Both approved numbered tickets were published. The user subsequently authorized implementing Ticket 01 only, using red-green-refactor TDD, with no commit or push.
- Ticket 01 is resolved: read-only inventory text, inventory commands, all four languages, and required tests are complete. See [TDD evidence and validation](ticket-01-validation.md). Ticket 02 was left for subsequent authorization.
- The user subsequently authorized Ticket 02 only, retaining the no-commit/no-push constraint. Verified Ticket 01 resolved and its 53 inventory tests passing before starting.
- Ticket 02 is resolved: graceful travel-menu EOF, including after either inventory command, passes all four languages and retry/repeat scenarios. All 81 inventory tests and all 312 regressions pass. See [TDD evidence and validation](ticket-02-validation.md).
- The user confirmed successful manual testing of the completed Category B inventory display and EOF handling and authorized committing Category B only, then pushing normally to main after final validation.

## Fog

- No unresolved requirements remain for the approved Category B PRD; both tickets are resolved.

## Approved English inventory text

Example with a weapon and no upgrade:

```text
Inventory
Weapon: long sword
Base maximum damage: 12
Magical upgrade: None
```

For purchased magical armor, replace the upgrade status with:

```text
Magical upgrade: Magical armor
Damage reduction: 2
```

For purchased weapon enchantment, replace the upgrade status with:

```text
Magical upgrade: Weapon enchantment
Magic damage bonus: +2
```

Without a weapon, show `Weapon: None` and omit the base maximum damage line.
Always show upgrade status independently. These examples establish the exact
English labels; equipment names and numeric values must use the current player's
existing data and translations, not hardcoded example values.

## Returned handoff findings

The user returned the read-only investigation report against Category A commit
fe7fb72. No product decisions were made during that investigation.

- Player tracks one nullable Weapon plus HasMagicalArmor and HasEnchantedWeapon ownership flags. No additional player equipment storage was found. See Player.cs:18 and Weapon.cs:3.
- Purchases are mutually exclusive. Enchantment preserves the existing weapon; its bonus is separate from base weapon damage. See Player.cs:29 and Player.cs:254.
- DamageReduction and MagicDamageBonus are derived effects, not additional items. Gold is currency; weapon ASCII art is metadata. Narrative mentions of a magic ring do not establish ownership.
- Player.DisplayStats includes identity and character statistics beyond equipment. Combat.GetCombatStatsDisplay includes the dragon and is unsuitable as a direct inventory display. See Player.cs:173 and Combat.cs:50.
- Reuse Messages.GetMessage, TranslateWeaponForDisplay, and existing localized weapon/damage/effect labels. Dedicated inventory heading and ownership/absence labels are not present. Keep new text synchronized across all four languages. See Messages.cs:62, Messages.cs:99, and the localization ADR.
- The existing travel loop retains the current player. Actual parsing and public validation are separate and must agree on a new command. Existing tokens are n/north, s/south, w/west, and e/exit. See Game.cs:45, Game.cs:111, and Game.cs:129.
- Existing travel EOF loops and incomplete path_invalid guidance were identified by the handoff; the user subsequently approved graceful travel EOF and complete translated command guidance as part of Category B.
- Existing merchant display tests and watchtower console-flow/state-preservation tests provide prior art. The user subsequently approved direct inventory display, console-flow, and Messages localization testing. See CYOATests/MerchantTests.cs:250 and CYOATests/WatchtowerTests.cs:244.

## Handoff checkpoint

The requested read-only handoff report was returned and used to complete the
original requirements interview. The user confirmed the handoff exercise is
complete. The temporary handoff document remains outside the workspace.
The approved PRD and both tickets were subsequently published. Both tickets were
implemented under separate explicit user authorizations, with recorded TDD
evidence and successful regression checks. No commits or pushes were performed
during implementation; finalization was authorized after manual testing.
