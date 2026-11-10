# PRD: South-Path Merchant Encounter

Status: ready-for-agent
Category: enhancement
Design approved: 2026-11-10
Phase: 2 - formal specification; implementation not authorized

## Problem Statement

The Choose Your Own Adventure game needs a small merchant encounter for a
CIDM 4390 assignment. Traveling south currently displays a narrative and returns
to the path menu. Players need to be able to purchase magical armor or a magical
weapon upgrade that affects later dragon combat, without losing existing gameplay
or support for English, Spanish, French, and Italian.

## Solution

Traveling south introduces a merchant selling two mutually exclusive upgrades.
Each new character starts with 50 gold. Either upgrade costs 40 gold, and each
character may purchase only one item total. The merchant offers numeric choices:
1 for magical armor, 2 for magical weapon enchantment, and 3 to decline.

Armor reduces damage from each successful, unblocked dragon hit by 2, with a
minimum of 0 damage. Weapon enchantment adds 2 damage to each successful,
unblocked player attack while preserving the player's occupation-specific weapon.
Misses and defended attacks continue to deal 0 damage.

The menu shows current gold, both prices, and the exact benefits. Successful
purchases identify the upgrade and confirm the remaining 10 gold. Declining
allows a later purchase. After a purchase, future merchant visits acknowledge it
and return to the path menu without charging or stacking bonuses. All merchant
text and new stat labels use the current localization system in all four languages.

## User Stories

1. As a player, I want to encounter the merchant when traveling south, so that
   I can prepare for dragon combat.
2. As a player, I want each new character to start with 50 gold and no purchased
   upgrade, so that the purchase opportunity is independent of earlier characters.
3. As a player, I want to see my current gold before choosing, so that I know
   what I can spend.
4. As a player, I want to see both items priced at 40 gold, so that I can compare
   their benefits at the same cost.
5. As a player, I want the menu to explain armor's 2-point damage reduction and
   the weapon's 2-point damage bonus, so that I can make an informed choice.
6. As a player, I want choice 1 to purchase magical armor, so that I can improve
   protection without changing health or agility.
7. As a player, I want choice 2 to enchant my existing weapon, so that I can deal
   more damage while keeping my occupation-specific equipment.
8. As a player, I want a purchase to deduct exactly 40 gold once, so that I retain
   10 gold and receive exactly one upgrade.
9. As a player, I want confirmation naming my purchase and remaining gold,
   so that I know the transaction succeeded.
10. As a player, I want choice 3 to decline without changing gold or equipment,
    so that I can continue without committing.
11. As a player, I want to return after declining and still be able to purchase,
    so that I can defer my decision.
12. As a player, I want purchasing either upgrade to prevent any further purchase
    with that character, so that the one-item limit is consistent.
13. As a player, I want later visits to acknowledge my previous purchase and
    return to the path menu, so that visits cannot charge me or stack bonuses.
14. As a player, I want invalid or blank input to show a translated error and
    repeat the menu without changing my state, so that I can correct mistakes.
15. As a player using redirected input, I want EOF at the merchant menu to exit
    the game gracefully without a purchase or state changes, so that input
    exhaustion cannot cause an infinite loop.
16. As an armored player, I want an unblocked dragon hit dealing 5 damage to deal
    3 instead, so that my purchase provides the promised protection.
17. As an armored player, I want incoming damage of 1 or 2 to become 0 rather
    than negative damage, so that armor cannot heal me.
18. As an unarmored player, I want to take normal damage, so that existing combat
    remains unchanged when I do not purchase armor.
19. As a player with an enchanted weapon, I want an unblocked damage roll of 5
    to deal 7 damage, so that the upgrade guarantees its stated bonus.
20. As a player, I want missed and defended attacks to deal 0 regardless of
    upgrades, so that existing hit and defense mechanics remain intact.
21. As a player, I want character and combat stats to show current gold and only
    my purchased upgrade's bonus, so that my current benefits are visible.
22. As a player, I want my original weapon name, base damage range, and existing
    stats preserved, so that magical upgrades do not obscure existing equipment.
23. As an English-, Spanish-, French-, or Italian-speaking player, I want merchant
    dialogue, options, confirmations, errors, and new stat labels in my selected
    language, so that the encounter matches the rest of the game.
24. As an existing player, I want character creation, other travel options,
    dragon encounters, retreat, and game endings preserved, so that this addition
    does not break established functionality.
25. As the student maintaining this game, I want focused automated tests of the
    encounter and damage rules, so that I can demonstrate the assignment works
    without building a complex shop or inventory framework.

## Implementation Decisions

- Add a small Merchant class responsible for encounter dialogue, purchasing
  choices, input handling, and confirmations. Integrate it into south travel.
- Store gold and purchased magical upgrade state on Player. Each new character
  starts with 50 gold and no upgrade. State belongs to the character, not to a
  Merchant instance or a global shop.
- Enforce one purchase total: armor and weapon enchantment are mutually exclusive.
  Apply the 40-gold deduction and selected upgrade together once. Repeat visits,
  declining, invalid input, blank input, and EOF must not alter that state.
- After a purchase or decline, return to the path menu. On a later visit after a
  purchase, acknowledge the purchase and return without reopening purchasing.
- Distinguish EOF from blank input. Merchant EOF must propagate an exit outcome
  to the game rather than returning to another input loop. Limit this change to
  handling EOF encountered at the merchant; unrelated input loops are out of scope.
- Reuse Weapon and the existing attack and defense sequences. Do not replace the
  occupation-specific weapon, alter its base maximum damage or random roll range,
  or change its display name or artwork for the enchantment.
- Apply armor only after a dragon hit passes the existing player defense check:
  effective damage is the larger of 0 and rolled damage minus 2. Without armor,
  use the existing rolled damage. Preserve the existing health floor behavior.
- Apply weapon enchantment only after a player hit passes the existing dragon
  defense check: effective damage is rolled weapon damage plus 2. Otherwise the
  attack still deals 0. Preserve existing hit probabilities and defense rules.
- Damage messages and health changes must reflect effective damage. Do not
  change starting health, strength, agility, race or occupation rules, or dragon
  generation to accommodate upgrades.
- Extend both existing character and combat stats displays with current gold.
  Show "Damage reduction: 2" only for purchased armor and
  "Magic damage bonus: +2" only for purchased enchantment, translated for the
  selected language. Preserve existing stats and base weapon damage presentation.
- Use the existing Messages service and JSON dictionaries for all new dialogue,
  menus, labels, purchase acknowledgments, confirmations, and errors. Keep numeric
  merchant choices identical across languages; do not add localized command maps.
- Update south-path narrative as needed to introduce the merchant coherently.
  Preserve existing translations outside the intentional south-encounter change,
  canonical race and occupation values, language selection, and fallback behavior.
- Respect the accepted Italian-language support decision and preserve its ADR.
- Use existing test seams where possible. Keep any additional deterministic damage
  seam small and at the existing attack/damage boundary; exact method signatures
  and the representation of upgrade state are implementation details, not new
  architectural frameworks.

## Testing Decisions

- Test observable behavior: gold balances, upgrade effects, health changes,
  encounter outcomes, displayed text, and completion. Avoid tests tied to private
  fields, helper structure, or a particular internal upgrade representation.
- Use the existing MSTest project. Existing Player, Dragon, and Combat tests
  provide prior art for state and combat assertions; ItalianLanguageTests provide
  prior art for localization keys, format arguments, console capture, and full
  startup/character/south-path flows.
- Keep console tests nonparallel and restore Console input/output after each test,
  following existing console-test conventions.
- Verify a fresh character has 50 gold and neither bonus. For each purchase, assert
  10 gold, exactly the selected upgrade, confirmation text, and unchanged original
  weapon and unrelated stats.
- Verify decline preserves state and permits a later purchase. Revisit after each
  purchase and assert an acknowledgment, return to the path menu, unchanged gold,
  no second purchase, and no stacked or switched bonus.
- Verify invalid and blank input display localized errors and re-prompt without
  changing state; then supply valid input to demonstrate recovery.
- Verify EOF directly at the menu and after invalid or blank input terminates
  without any purchase or state changes. Include an integrated game-flow check
  with bounded completion so an accidental retry loop fails safely.
- Test armor arithmetic deterministically, including 5 becoming 3, 2 becoming 0,
  and 1 becoming 0. Test ordinary unarmored damage remains unchanged.
- Test enchantment arithmetic deterministically, including 5 becoming 7, and
  preserve ordinary weapon damage without enchantment. Check occupation-specific
  weapon identity and base damage values remain intact.
- Verify both attack directions still deal 0 on misses or successful defense,
  including when upgrades are present. Verify the deterministic calculations are
  used by actual attacks and that displayed damage agrees with health changes.
  Do not rely on repeated random trials or lucky dice rolls for exact assertions.
- Cover south-to-merchant-to-path flow after purchasing and declining, plus the
  merchant EOF exit. Update the existing Italian south-path test input to include
  a merchant choice while retaining its existing localization assertions.
- Use parameterized checks across English, Spanish, French, and Italian for
  merchant choices, benefits, confirmations, errors, repeat acknowledgments, and
  stat labels. Verify new keys exist and format placeholders match, without
  multiplying every business-rule scenario across every language.
- Verify character and combat stats show gold before and after purchase, hide
  unpurchased bonuses, show the purchased bonus, and preserve existing stat content.
- During implementation validation, build and run the entire existing regression
  suite together with the new tests. Existing character creation, language support,
  combat, retreat, and exit behavior must continue to pass. This specification
  does not claim that feature tests or regression validation have run yet.

## Out of Scope

- Implementing this feature during Phase 2 or creating implementation tickets
  as part of this PRD-writing task.
- Inventory management, separate armor classes, equipment slots, item catalogs,
  complex shop frameworks, or replacing occupation-specific weapons.
- Additional purchases, stacking, selling, refunds, trading, gold earning, variable
  prices, or a broader economy.
- New languages, a localization-system rewrite, unrelated translation changes,
  or changes to existing travel/combat commands.
- Changes to base character stats, hit/defense probabilities, random dice behavior,
  dragon balance, or unrelated game-loop behavior.
- Saving purchases across game sessions, new diagrams, or broad architectural
  refactoring.
- Committing or pushing changes.

## Further Notes

- The user approved this design and completed Phase 1 planning on 2026-11-10.
  This PRD records those decisions for Phase 2 without authorizing implementation.
- The ready-for-agent status uses the repository's configured triage vocabulary;
  it indicates specification readiness, not permission to begin coding.
- The user already approved the test boundaries: direct deterministic purchase
  and damage checks, scripted console-flow tests, four-language coverage, and the
  existing regression suite. No additional interview is needed for this PRD.
- No unresolved product decisions remain. A future phase may split this PRD into
  small implementation tickets before coding is authorized.
