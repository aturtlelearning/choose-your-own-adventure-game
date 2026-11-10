# 01: Merchant encounter with magical armor purchasing

**What to build:** A playable south-path merchant encounter where a character
can buy magical armor for 40 gold or decline and return later. The armor provides
protection in subsequent dragon combat, and the purchase is visible in localized
confirmations and stats. Deliver gameplay, translations, and automated tests
together as one vertical slice.

**Blocked by:** None (can start immediately once implementation is authorized).

**Status:** ready-for-agent

Category: enhancement
Implementation: completed (uncommitted)

## Acceptance criteria

- [x] Each new character starts with 50 gold and no purchased magical upgrade.
  Gold and upgrade state belong to Player and are independent of other characters
  and Merchant instances.
- [x] Traveling south opens a small Merchant-managed encounter and uses a coherent
  introduction instead of the previous south-path narrative. The encounter handles
  dialogue, choices, purchases, and confirmations without an inventory framework
  or separate armor class.
- [x] This intermediate slice exposes only working choices: 1 for magical armor
  and 3 to decline. The menu shows current gold, the 40-gold armor price, and the
  exact benefit: 2 less damage from successful, unblocked dragon hits, minimum 0.
  Choice 2 and weapon enchantment are introduced by Ticket 02.
- [x] Buying armor deducts exactly 40 gold once, leaves 10 gold, and records the
  purchase together with the deduction. A translated confirmation names magical
  armor and shows the remaining gold. The player returns to the path menu.
- [x] Declining returns to the path menu without changing gold or equipment.
  Returning later still allows a purchase.
- [x] Purchasing armor consumes the character's single purchase opportunity.
  Later visits acknowledge the purchase and return to the path menu without
  reopening purchasing, charging gold, or applying another bonus.
- [x] Invalid or blank input produces a translated error and repeats the menu
  without changing gold or equipment. In this slice, choice 2 is invalid.
- [x] EOF at the merchant menu exits the game gracefully without a purchase or
  state changes. Distinguish EOF from blank input and propagate the exit instead
  of returning to another input loop.
- [x] After the existing dragon hit and player defense checks, purchased armor
  reduces rolled damage by 2 to a minimum of 0. Damage messages and health changes
  agree. Misses and defended attacks still deal 0; unarmored damage is unchanged.
- [x] Preserve existing health, strength, agility, hit probabilities, defense
  rules, health-floor behavior, dragon generation, and original weapons.
- [x] Both character and combat stats show current gold. Only armored characters
  show the translated equivalent of "Damage reduction: 2". Existing stats,
  occupation-specific weapon names, artwork, and base damage presentation remain.
- [x] All new dialogue, menu options, benefit descriptions, errors, confirmations,
  repeat-visit acknowledgments, and stat labels support English, Spanish, French,
  and Italian through the existing localization system. Preserve unrelated
  translations, language selection and fallback, canonical values, and commands.
- [x] Preserve character creation, other travel options, dragon encounters,
  retreat, game endings, and the existing Italian-language ADR.

## Automated testing and validation

- [x] Use the existing MSTest project to test fresh-character state, armor purchase,
  exact gold deduction, confirmation, decline followed by purchase, and repeated
  visits without extra charges or stacked bonuses. Assert original equipment and
  unrelated stats remain unchanged.
- [x] Test damage arithmetic deterministically: 5 becomes 3, 2 becomes 0, and
  1 becomes 0. Verify unarmored damage is unchanged and missed or defended attacks
  deal 0. Exercise the real attack integration so a disconnected helper cannot
  satisfy the tests; verify reported damage agrees with health changes.
- [x] Use a small deterministic seam at the existing attack/damage boundary only
  if needed. Do not depend on repeated random trials or alter production dice
  behavior to obtain exact assertions.
- [x] Script south-to-merchant-to-path flows for purchasing and declining, including
  decline then return and purchase. Adapt the existing Italian south-path test
  with a merchant choice while retaining its existing localization assertions.
- [x] Test invalid input, blank input, and recovery with a valid choice. Verify
  state remains unchanged before that valid choice.
- [x] Test EOF immediately and after invalid or blank input, asserting unchanged
  player state and bounded completion. Include a full game-flow EOF check that
  detects returning to another input loop without leaving a hung test process.
- [x] Parameterize focused localization checks across all four languages for
  displayed choices, price, benefit, confirmations, errors, repeat acknowledgments,
  and gold/armor labels. Verify new keys and matching format placeholders.
- [x] Verify both stats displays before and after purchasing: gold changes from
  50 to 10, armor's label appears only after purchase, and existing content remains.
- [x] Keep console tests nonparallel and restore input/output after each test,
  following existing test conventions. Assert observable behavior rather than
  private fields or a particular internal state representation.
- [x] Build and run the entire existing regression suite plus new tests before
  marking the ticket complete. Keep this slice green independently of Ticket 02.

## Scope and handoff

This is the first of the two approved tickets for the South-Path Merchant
Encounter PRD. Ticket 02 reuses this encounter and shared one-purchase rule to add
the alternative enchantment. Do not add gold earning, selling, refunds, inventory
management, persistence, unrelated EOF changes, or broad architectural refactoring.

The ticket is ready for implementation planning; creating it does not authorize
coding, committing, or pushing. The approved PRD remains unchanged.

## Comments

- Implementation completed for Ticket 01 only. Release build passed with zero
  warnings and errors. All 150 tests passed (118 existing and 32 new merchant
  cases). Standards review found no violations and one nonblocking display-code
  duplication; specification review found no gaps. Existing translation entries
  and the Italian-language ADR were verified preserved. No commit or push made.
