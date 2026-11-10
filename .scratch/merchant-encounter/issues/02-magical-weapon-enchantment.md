# 02: Magical weapon enchantment as the alternative purchase

**What to build:** Complete the merchant encounter with the alternative magical
weapon purchase. Players can choose armor or enchant their existing
occupation-specific weapon, with either choice consuming the same single purchase
opportunity. Deliver the enchantment's combat effect, localized presentation, and
automated tests together as one vertical slice.

**Blocked by:** 01 - Merchant encounter with magical armor purchasing. Ticket 01
establishes the playable encounter, character gold and upgrade state, shared
one-purchase rule, repeat-visit behavior, and input/EOF handling reused here.

**Status:** ready-for-agent

Category: enhancement
Implementation: completed (uncommitted)

## Acceptance criteria

- [x] Complete the approved menu with 1 for magical armor, 2 for magical weapon
  enchantment, and 3 to decline. Show current gold, both 40-gold prices, and exact
  benefits: armor reduces unblocked dragon damage by 2, minimum 0; enchantment
  adds 2 damage to successful, unblocked player attacks.
- [x] Choice 2 enchants the existing occupation-specific weapon, deducts 40 gold
  exactly once, leaves 10 gold, and records the upgrade together with the deduction.
  A translated confirmation names the enchantment and remaining balance before
  returning to the path menu.
- [x] Preserve the original weapon, including its name, type, artwork, base maximum
  damage, and random damage-roll range. Apply a flat +2 to actual attack damage
  only after the existing player hit and dragon defense checks.
- [x] An unblocked base damage roll of 5 becomes 7 with enchantment. Misses and
  defended attacks still deal 0. Damage messages agree with health changes, and
  attacks without enchantment retain existing damage behavior.
- [x] Buying either upgrade prevents purchasing the other or purchasing again.
  Repeated visits acknowledge the prior purchase and return to the path menu with
  unchanged gold and upgrade state, without stacking, switching, or reopening
  purchasing. Armor's behavior from Ticket 01 remains intact.
- [x] Declining still permits a later purchase of either upgrade. Invalid or blank
  input still retries without state changes; merchant EOF still exits the game
  gracefully without purchasing or modifying state.
- [x] Both character and combat stats show current gold and the translated
  equivalent of "Magic damage bonus: +2" only when enchantment was purchased.
  Preserve existing stats and original weapon/base damage presentation. Display
  "Damage reduction: 2" only for armor; never display both bonuses for a character.
- [x] Translate the completed menu, enchantment benefit, purchase confirmation,
  repeat-visit acknowledgment, and new stats label into English, Spanish, French,
  and Italian using the existing localization system. Preserve all Ticket 01
  translations and unrelated existing translations, commands, and language behavior.
- [x] Reuse Merchant, Player, Weapon, and the existing combat/localization systems.
  Do not introduce inventory management, replacement weapons, separate armor
  classes, or a complex shop framework.
- [x] Preserve character creation, race/occupation rules, health, agility, strength,
  hit/defense probabilities, dragon behavior, travel, retreat, endings, and the
  existing Italian-language ADR.

## Automated testing and validation

- [x] Test enchantment purchasing through the encounter: 50 gold becomes 10, only
  enchantment is applied, confirmation is correct, and unrelated state is unchanged.
  Cover all existing occupation-specific weapons without replacing them or
  changing their base damage values.
- [x] Test decline then return and enchant, repeated visits after enchantment, and
  both cross-purchase attempts: armor then enchantment, and enchantment then armor.
  Assert no further charge, stacking, or upgrade switching.
- [x] Test exact damage arithmetic deterministically, including 5 becoming 7,
  ordinary damage without enchantment remaining unchanged, and 0 damage for misses and
  defended attacks. Verify the real attack integration uses the bonus correctly
  and reports the effective damage actually applied.
- [x] Retain Ticket 01's deterministic armor checks, unupgraded combat checks,
  input recovery tests, and bounded EOF tests. Include choice 2 in invalid/blank
  input recovery and EOF-without-purchase coverage where appropriate.
- [x] Script a south-to-merchant enchantment purchase and return to the path menu,
  including a repeat visit. Retain armor and decline game-flow coverage and the
  existing Italian flow assertions.
- [x] Parameterize focused checks across English, Spanish, French, and Italian
  for the completed menu, both benefits, enchantment confirmation, repeat
  acknowledgments, errors, and stats. Verify new keys exist and format placeholders
  match without repeating every business-rule case in every language.
- [x] Verify both character and combat stats for no upgrade, armor, and enchantment:
  correct gold, only the purchased bonus visible, original weapon/base damage
  content preserved, and existing stats intact.
- [x] Follow the existing MSTest conventions: console tests nonparallel, restored
  console streams, observable-behavior assertions, and deterministic exact damage
  checks rather than lucky dice rolls or repeated random trials.
- [x] Build and run the entire regression suite, including Ticket 01's tests and
  all new tests. Confirm the full approved merchant design works without breaking
  existing game functionality or any of the four languages.

## Scope and handoff

Implement after Ticket 01; together the tickets complete the approved South-Path
Merchant Encounter PRD. This ticket adds the alternative purchase rather than a
second purchase opportunity. Gold earning, trading, selling, refunds, persistent
inventory, broad refactoring, and unrelated changes remain out of scope.

Creating this ticket does not authorize coding, committing, or pushing. The
approved PRD remains unchanged.

## Comments

- Ticket 01 was verified complete before implementation; all 32 existing merchant
  cases passed. Ticket 02 adds the alternative enchantment purchase, a shared
  purchase limit, localized dialogue/stats, and deterministic player-attack tests.
- Final Release build: zero warnings and errors. Full suite: 170 passed, 0 failed
  (118 original regression cases and 52 merchant cases, including 20 added here).
- Standards review: no violations; one nonblocking duplication in upgrade-stat
  presentation. Spec review's missing EOF enchantment-state assertions were added;
  targeted EOF tests and the full suite passed afterward.
- Existing translation entries were preserved except the intentionally expanded
  merchant introductions and invalid-choice guidance. The Italian-language ADR
  remains unchanged. No commit or push made.
