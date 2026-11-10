# Approved Category B inventory display ticket breakdown

Status: resolved
Category: enhancement

This document preserves the approved planning scopes and acceptance criteria.
Both numbered implementation tickets were published under the inventory-display
issues directory and are now resolved. Their individual validation records
contain TDD evidence, test results, and code-review findings.

## 01: View current equipment from the travel menu in all four languages

**What to build:** Players enter `i` or `inventory`, see an equipment-only display
of their actual weapon and purchased magical upgrade, and return directly to
travel. The full journey works in English, Spanish, French, and Italian, with
no changes to player state or existing gameplay.

**Blocked by:** None. No prerequisite refactor or other implementation ticket
is required. Implementation still requires separate user authorization.

### Acceptance criteria

- [ ] Add the approved public Player.GetInventoryDisplay(Messages) method returning localized inventory text. It reads no console input and changes no player or weapon state.
- [ ] Show the approved translated heading, current weapon name, and existing base maximum damage. Exercise multiple existing weapon types and damage values; do not hardcode sample data.
- [ ] With no weapon, show translated Weapon: None and omit the base maximum damage line; do not create a default item or damage value.
- [ ] Show Magical upgrade: None if none was purchased. Otherwise name the purchased armor or enchantment and display its existing effect: damage reduction or a separately signed magic damage bonus.
- [ ] Determine upgrade status independently of weapon presence, including no-weapon cases with an existing purchased upgrade. Preserve weapon base damage and existing combat calculations.
- [ ] Include no gold, health, strength, agility, other character statistics, identity information, dragon statistics, or weapon artwork.
- [ ] Actual travel command parsing and existing public validation consistently recognize `i` and `inventory`, ignoring capitalization and surrounding spaces. Print inventory and return immediately to travel without an acknowledgement input.
- [ ] Translate all inventory labels and updated travel prompts into English, Spanish, French, and Italian. Preserve the exact approved English labels and existing weapon-name translations.
- [ ] Preserve invalid-travel error-and-retry behavior. In all four languages, update error guidance to list n, s, w, i, and e; preserve all existing short and full-word commands.
- [ ] Direct display tests cover no upgrade, armor, enchantment, and no weapon; verify unchanged health, gold, attributes, equipment references and properties, upgrade ownership, and derived effects.
- [ ] Script both inventory tokens, capitalization, surrounding spaces, repeated viewing, invalid/blank input followed by valid input, and onward navigation. Verify no input is consumed for an inventory acknowledgement and no invalid-input state changes occur.
- [ ] Script inventory before and after merchant purchases and alongside watchtower, north, and exit navigation. Preserve purchase exclusivity and all existing encounter behavior.
- [ ] Localization tests verify complete nonempty labels in all four languages, matching format arguments, no missing required keys or placeholders, and unchanged existing unknown-key fallback. Review translation meaning.
- [ ] Use the approved direct display, console game flow, and Messages test seams; do not test private methods or introduce testing-only production interfaces.
- [ ] Run relevant tests during implementation and the full existing regression suite before completion, including Category A watchtower coverage. Record TDD evidence and results.

**Verification:** Demonstrate `inventory`, return to travel, then another valid
command in each language. Demonstrate the same command before and after a
merchant purchase. Exercise absent equipment directly through the approved
display method. Follow existing merchant display/localization tests and
watchtower console-flow/state-preservation tests as prior art.

**Scope note:** Graceful travel-menu EOF is delivered by ticket 02; this ticket's
console scripts terminate with explicit exit or another existing terminating
encounter. Do not treat completion of ticket 01 as completion of the full PRD.

## 02: Exit gracefully when travel input ends, including after inventory

**What to build:** When console input ends at the travel menu, players see the
existing localized adventure-ending message once and the game terminates. This
also works immediately after inventory without errors, fabricated actions, or
retries on exhausted input.

**Blocked by:** 01 — View current equipment from the travel menu in all four
languages. That ticket supplies the inventory command required to verify EOF
immediately after inventory. No additional prerequisite ticket is needed.

### Acceptance criteria

- [ ] Distinguish EOF from invalid and blank travel input. EOF terminates the game without entering an error-and-retry loop or inventing a command.
- [ ] Reuse the existing translated adventure-ending message exactly once in English, Spanish, French, and Italian.
- [ ] Handle EOF at the first travel prompt and immediately after inventory, including both inventory command tokens.
- [ ] Handle travel EOF after invalid/blank input and after repeat inventory viewing without breaking existing retry behavior for actual input.
- [ ] EOF changes no health, gold, equipment, upgrade ownership, attributes, or combat statistics; preserve existing merchant and watchtower EOF behavior and dragon combat rules.
- [ ] Add bounded scripted-console tests that observe real EOF and fail promptly if the game retries exhausted input. Assert correct output order, one ending message, no error for EOF itself, and no fabricated action.
- [ ] Cover both required EOF locations in every supported language and retain the display, command, localization, and state-preservation coverage from ticket 01.
- [ ] Use the approved console game flow and Messages test seams; introduce no inventory classes, persistence, player state, or testing-only production interfaces.
- [ ] Run relevant tests during implementation and the full existing regression suite before completion, preserving Category A and existing gameplay tests. Record TDD evidence and results.

**Verification:** Demonstrate finite redirected input ending at travel, then
finite input ending immediately after the inventory command. In both cases the
game prints the selected language's existing ending message and returns normally
without requesting more input. Follow existing watchtower EOF tests as prior art.

## Required workflow for both tickets

- Use red-green-refactor TDD: first observe a meaningful failing test for each missing behavior, then add the minimum implementation to pass, then refactor while keeping tests green. Do not write all tests in a batch.
- Retain initially passing characterization tests for existing correct behavior; do not manufacture failures or add unnecessary production changes.
- Preserve the merchant, ruined watchtower, dragon encounter, character state, existing translations, and all four supported languages.
- Reuse existing player/weapon data and Messages. Add no inventory management system or changes to combat calculations.
- Ticket publication is not authorization to implement, commit, or push.

## Proposed order and rationale

1. Complete the localized inventory journey, all supported equipment cases,
   command guidance, and its tests together.
2. Complete the travel-menu EOF correction and its tests against that journey.

Both tickets are independently verifiable when their blockers are satisfied.
Localization and automated tests belong to each player-facing slice rather than
separate technical-layer tickets. The feature is small enough that the normal
journey fits in one context window; splitting weapon output, upgrade output,
translations, and command wiring would leave partial inventory behavior.

## Approval and publication

The user approved this two-ticket breakdown exactly as proposed, including the
dependency of ticket 02 on ticket 01. Both tickets were published and implemented
under separate explicit authorizations. Completion is recorded in the resolved
numbered tickets; the original acceptance-criteria checklists above remain the
planning record.
