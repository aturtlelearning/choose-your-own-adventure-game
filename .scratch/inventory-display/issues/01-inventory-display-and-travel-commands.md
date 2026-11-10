# 01: View current equipment from the travel menu in all four languages

Status: resolved
Category: enhancement
Type: task

**What to build:** Players enter `i` or `inventory`, see an equipment-only display
of their actual weapon and purchased magical upgrade, and return directly to
travel. The full journey works in English, Spanish, French, and Italian, with
no changes to player state or existing gameplay.

**Blocked by:** None. No prerequisite refactor or other implementation ticket
is required. Implementation still requires separate user authorization.

### Acceptance criteria

- [x] Add the approved public Player.GetInventoryDisplay(Messages) method returning localized inventory text. It reads no console input and changes no player or weapon state.
- [x] Show the approved translated heading, current weapon name, and existing base maximum damage. Exercise multiple existing weapon types and damage values; do not hardcode sample data.
- [x] With no weapon, show translated Weapon: None and omit the base maximum damage line; do not create a default item or damage value.
- [x] Show Magical upgrade: None if none was purchased. Otherwise name the purchased armor or enchantment and display its existing effect: damage reduction or a separately signed magic damage bonus.
- [x] Determine upgrade status independently of weapon presence, including no-weapon cases with an existing purchased upgrade. Preserve weapon base damage and existing combat calculations.
- [x] Include no gold, health, strength, agility, other character statistics, identity information, dragon statistics, or weapon artwork.
- [x] Actual travel command parsing and existing public validation consistently recognize `i` and `inventory`, ignoring capitalization and surrounding spaces. Print inventory and return immediately to travel without an acknowledgement input.
- [x] Translate all inventory labels and updated travel prompts into English, Spanish, French, and Italian. Preserve the exact approved English labels and existing weapon-name translations.
- [x] Preserve invalid-travel error-and-retry behavior. In all four languages, update error guidance to list n, s, w, i, and e; preserve all existing short and full-word commands.
- [x] Direct display tests cover no upgrade, armor, enchantment, and no weapon; verify unchanged health, gold, attributes, equipment references and properties, upgrade ownership, and derived effects.
- [x] Script both inventory tokens, capitalization, surrounding spaces, repeated viewing, invalid/blank input followed by valid input, and onward navigation. Verify no input is consumed for an inventory acknowledgement and no invalid-input state changes occur.
- [x] Script inventory before and after merchant purchases and alongside watchtower, north, and exit navigation. Preserve purchase exclusivity and all existing encounter behavior.
- [x] Localization tests verify complete nonempty labels in all four languages, matching format arguments, no missing required keys or placeholders, and unchanged existing unknown-key fallback. Review translation meaning.
- [x] Use the approved direct display, console game flow, and Messages test seams; do not test private methods or introduce testing-only production interfaces.
- [x] Run relevant tests during implementation and the full existing regression suite before completion, including Category A watchtower coverage. Record TDD evidence and results.

**Verification:** Demonstrate `inventory`, return to travel, then another valid
command in each language. Demonstrate the same command before and after a
merchant purchase. Exercise absent equipment directly through the approved
display method. Follow existing merchant display/localization tests and
watchtower console-flow/state-preservation tests as prior art.

**Scope note:** Graceful travel-menu EOF is delivered by ticket 02; this ticket's
console scripts terminate with explicit exit or another existing terminating
encounter. Do not treat completion of ticket 01 as completion of the full PRD.

## Required implementation workflow

- Use red-green-refactor TDD: first observe a meaningful failing test for each missing behavior, then add the minimum implementation to pass, then refactor while keeping tests green. Do not write all tests in a batch.
- Retain initially passing characterization tests for existing correct behavior; do not manufacture failures or add unnecessary production changes.
- Preserve the merchant, ruined watchtower, dragon encounter, character state, existing translations, and all four supported languages.
- Reuse existing player/weapon data and Messages. Add no inventory management system or changes to combat calculations.
- Ticket publication is not authorization to implement, commit, or push.

## Answer

Implemented Ticket 01 only. Release build passed with zero warnings or errors; all 53 inventory tests and all 284 regression tests passed. Standards and Spec code reviews found no actionable issues. See [TDD evidence and validation](../ticket-01-validation.md). Ticket 02 EOF correction remains unimplemented. No commit or push performed.
