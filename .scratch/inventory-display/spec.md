# PRD: Read-Only Inventory Display

Status: ready-for-agent
Category: enhancement
Plan: approved; all ten design decisions finalized
Phase: complete; both tickets implemented, automatically verified, and manually tested

## Problem Statement

For CIDM 4390 AI Agentic Coding Part 4, Category B, players need a convenient
way to inspect their equipment during an adventure. Current character and combat
summaries contain unrelated statistics and do not provide an equipment-only
view from the travel menu. Players need to see the current weapon and purchased
magical upgrade without changing their character or starting an inventory
management system.

The assignment also requires practicing handoff during planning. A separate
read-only investigation was completed, returned, and used to finalize the plan.

## Solution

Add an inventory display accessible by `i` or `inventory` at the existing travel
menu. Ignore capitalization and leading or trailing spaces. Display the current
weapon's translated name, existing base maximum damage when a weapon is present,
and the name and effect of any purchased magical upgrade. Return directly to the
travel menu after printing, without an additional input prompt.

Keep the display equipment-only. Include a translated inventory heading and
explicit None status for absent equipment. When there is no weapon, display
Weapon: None and omit base maximum damage. Determine upgrade status independently
of weapon availability. When no upgrade was purchased, display Magical upgrade:
None. Purchased armor displays its damage reduction; enchantment displays its
magic damage bonus separately from base weapon damage.

Use existing player and weapon values, existing weapon translations, and the
current Messages localization system for English, Spanish, French, and Italian.
Viewing inventory must not change health, gold, equipment, upgrade ownership,
or combat statistics. Do not display gold, health, attributes, other character
statistics, or weapon artwork.

Preserve north, south, west, exit, and their full-word commands. Explain inventory
in all four travel prompts, and update translated invalid-command guidance to
list n, s, w, i, and e. Preserve the error-and-retry behavior for invalid commands
without changing player state. Treat EOF at the travel menu as graceful exit,
including EOF immediately after inventory: print the existing translated
adventure-ending message and terminate without retrying or changing state.

## User Stories

1. As a player, I want the travel menu to explain inventory, so that I can discover the command.
2. As a player, I want both `i` and `inventory` to work, so that I can use a short or descriptive command.
3. As a player, I want commands to ignore capitalization and surrounding spaces, so that normal typing variations work.
4. As a player, I want a clear inventory heading, so that I can recognize the equipment display.
5. As a player, I want my current weapon's translated name, so that I can identify my equipment.
6. As a player, I want the existing base maximum damage, so that I can understand my weapon's base capability.
7. As a magician, I want my existing lightning bolt spell displayed as my current weapon, so that the display respects existing occupation equipment.
8. As a player without a weapon, I want Weapon: None, so that absence is explicit.
9. As a player without a weapon, I want the base-damage line omitted, so that the display does not invent a weapon or value.
10. As a player without a magical upgrade, I want Magical upgrade: None, so that my upgrade status is clear.
11. As a player with magical armor, I want its name and existing damage reduction shown, so that I can recognize its protection.
12. As a player with weapon enchantment, I want its name and existing magic damage bonus shown, so that I can recognize its effect.
13. As a player with weapon enchantment, I want the bonus separate from base damage, so that I do not confuse the two values.
14. As a player without a weapon, I want upgrade status displayed independently, so that the view faithfully represents tracked upgrade ownership.
15. As a player, I want equipment values taken from my current character, so that examples cannot replace my actual equipment.
16. As a player, I want an equipment-only view, so that unrelated character statistics and artwork do not obscure the information.
17. As a player, I want viewing inventory to leave my character unchanged, so that inspection has no gameplay cost or effect.
18. As a player, I want inventory to return directly to travel, so that I can continue without another confirmation prompt.
19. As a player, I want repeated inventory viewing to work, so that I can check equipment whenever needed.
20. As a player visiting the merchant, I want inventory to reflect my purchase immediately, so that I can verify the existing upgrade.
21. As a player, I want north, south, west, exit, and their full-word commands preserved, so that existing navigation remains available.
22. As a player entering an invalid command, I want translated guidance listing n, s, w, i, and e, so that I can correct the input.
23. As a player, I want invalid commands to leave my state unchanged, so that typing mistakes have no gameplay effect.
24. As a player using redirected console input, I want travel-menu EOF to exit gracefully, so that missing input cannot cause an infinite loop.
25. As a player using redirected console input, I want EOF immediately after inventory to exit gracefully, so that inspecting equipment does not leave the program retrying exhausted input.
26. As a player, I want the existing translated ending message at travel-menu EOF, so that termination remains clear and consistent.
27. As an English-speaking player, I want the approved labels preserved, so that the display matches the agreed wording.
28. As a Spanish-speaking player, I want inventory and updated travel guidance in Spanish, so that the feature works in my selected language.
29. As a French-speaking player, I want inventory and updated travel guidance in French, so that the feature works in my selected language.
30. As an Italian-speaking player, I want inventory and updated travel guidance in Italian, so that the feature works in my selected language.
31. As a player, I want the merchant, ruined watchtower, and dragon combat preserved, so that this display adds convenience without changing the adventure.

## Implementation Decisions

- Add the approved public Player.GetInventoryDisplay(Messages) method returning the complete localized inventory text. It reads existing equipment and upgrade data, reads no console input, and does not mutate player state or weapon properties.
- Game handles inventory commands, prints the returned text, and continues the current player's travel loop. No extra acknowledgement or equipment-selection prompt is introduced.
- Keep actual command parsing and existing public validation consistent for inventory while preserving existing route tokens and normalization behavior.
- Use the existing Weapon, HasMagicalArmor, and HasEnchantedWeapon state. Read base damage from the current weapon and effects from DamageReduction or MagicDamageBonus. Do not hardcode example weapon names or numeric values.
- Preserve the existing mutually exclusive purchase rules. Enchantment remains a player upgrade applied separately from weapon base damage; do not replace weapons or alter combat calculations.
- Handle a missing weapon without a default item or fabricated damage. Compute upgrade display independently, including a purchased upgrade whose weapon has subsequently been removed from the model.
- Reuse Messages.GetMessage and TranslateWeaponForDisplay, adding appropriate inventory labels to the existing JSON dictionaries. Keep all four languages' keys and formatting placeholders synchronized according to the accepted Italian-language ADR.
- Existing complete character and combat displays include unrelated content. Keep them intact; do not instantiate combat or require a dragon to obtain inventory text.
- Treat null travel input as EOF, separately from invalid or blank input. Propagate termination to the game, display the existing adventure-ending message once, and avoid retrying exhausted input. Preserve invalid-command retries.
- Add no inventory classes, item collections, equipment slots, persistence, ownership flags, or additional player state.

## Testing Decisions

The user approved the following testing seams during planning. Test externally
observable results through these public interfaces rather than private methods,
internal call sequences, or newly invented testing interfaces.

| Approved seam | Coverage | Limit |
| --- | --- | --- |
| Player.GetInventoryDisplay(Messages) | Exact equipment output, absent weapon, each upgrade status, separate base damage and bonus, omitted unrelated fields, and unchanged player/weapon state | Does not verify interactive command routing |
| Game.StartGame with scripted console input/output | Command variants, travel guidance, repeated viewing, navigation, invalid-input retry, purchase visibility, and graceful travel EOF | The internally created player is observed through console behavior |
| Messages | Required labels in all four languages, weapon translations, missing-key detection, and compatible formatting | Automated checks do not fully judge translation quality |

- Follow existing merchant tests for upgrade setup, weapon identity and property comparisons, and localized displays. Follow watchtower tests for console setup/cleanup, bounded input consumption, navigation ordering, and character-state comparisons.
- Direct tests cover no upgrade, magical armor, weapon enchantment, and no weapon. Include no-weapon cases with independently tracked upgrade status; enchantment can be purchased with a weapon before clearing the weapon for that test case.
- Verify unchanged health, gold, strength, agility, equipment references/properties, upgrade ownership, and derived effects before and after display. Assert the method does not read console input. Exercise differing existing weapon types and damage values to detect hardcoded examples.
- Use approved English labels as independent expected values. Assert that no weapon omits the base-damage line, no upgrade explicitly displays None, and enchantment never replaces base damage with a combined value.
- Assert that inventory text excludes character identity, gold, health, attributes, other character statistics, weapon artwork, and dragon statistics.
- Script both command tokens, uppercase and mixed-case input, surrounding whitespace, repeated viewing, and onward navigation. Verify the inventory is followed directly by the travel menu and does not consume an acknowledgement input.
- Exercise inventory before and after merchant purchases, preserving existing purchase limits, watchtower navigation, dragon encounter behavior, and exit commands.
- Exercise invalid travel input followed by inventory or another valid command. Confirm translated guidance lists all five short commands and that invalid input does not change player state.
- Test EOF at the travel menu and immediately after inventory in all four languages. Check one translated ending message, no fabricated action or inventory, and no error-and-retry loop. Bound scripted reads so regressions fail instead of hanging.
- Check every required inventory label and updated guidance for presence and nonempty text in all four languages, matching formatting arguments, and no missing-key placeholders. Preserve the existing Messages behavior for genuinely unknown keys. Review translation meaning separately.
- During authorized implementation, use red-green-refactor TDD: observe a meaningful failing test before implementing each missing behavior, make the smallest passing change, then refactor while keeping tests green. Do not batch all tests before implementation. Existing correct behavior may be characterized by initially passing tests without fabricating failures.
- Run relevant direct-display, console-flow, and localization tests during implementation and the full existing regression suite before completion. Record TDD evidence and results; preserve the Category A coverage and existing gameplay tests.

## Out of Scope

- Gameplay implementation, new tests, and implementation tickets during formal PRD creation.
- Inventory management, new inventory classes, item collection, equipping, unequipping, selling, dropping, or swapping equipment.
- New items, rewards, equipment inferred only from narrative text, or new player state.
- Gold, character statistics, or weapon artwork in the inventory display.
- Changes to combat calculations, weapon replacement behavior, merchant prices, upgrade exclusivity, or the ruined watchtower story.
- Changes to other menus' input handling unrelated to the approved travel-menu EOF correction.
- Additional supported languages, persistence, or a graphical inventory interface.

## Further Notes

The requirements interview and all ten design decisions are approved. The
required handoff investigation was completed read-only against Category A commit
fe7fb72 and returned before equipment and interface decisions were finalized.
Its findings and the full decision history remain in [the planning map](map.md).
The handoff document remains in the operating system's temporary directory.

The approved breakdown was published as two implementation tickets, now both
resolved with TDD evidence and successful code reviews. The user confirmed manual
testing of inventory and travel EOF and authorized a Category B-only commit and
non-force push to main after final Release build and regression validation.

The English labels below are approved exactly. The sample weapon and numbers
illustrate formatting; implementation must use the current player's data and
the existing translation system.

### Weapon and no upgrade

```text
Inventory
Weapon: long sword
Base maximum damage: 12
Magical upgrade: None
```

### Magical armor status and effect

```text
Magical upgrade: Magical armor
Damage reduction: 2
```

### Weapon enchantment status and effect

```text
Magical upgrade: Weapon enchantment
Magic damage bonus: +2
```

Without a weapon, display Weapon: None and omit base maximum damage. Continue
displaying upgrade status independently of weapon availability.
