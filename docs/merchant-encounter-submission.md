# CIDM 4390 AI Agentic Coding Part 3: Merchant Encounter

## Completed feature

Traveling south opens a merchant encounter. Each new character starts with
50 gold and can purchase one magical upgrade for 40 gold, leaving 10 gold:

- **1 - Magical armor:** reduces damage from successful, unblocked dragon hits
  by 2, to a minimum of 0.
- **2 - Magical weapon enchantment:** preserves the occupation-specific weapon
  and adds 2 damage to successful, unblocked attacks.
- **3 - Decline:** returns to the path menu and allows a later purchase.

Purchasing either upgrade prevents further purchases, switching, or stacking.
Later visits acknowledge the prior purchase. Invalid or blank input retries
without changing player state; EOF at the merchant exits gracefully without
purchasing. Character and combat stats show gold and only the purchased bonus.

All new dialogue, options, confirmations, errors, and stats support English,
Spanish, French, and Italian using the existing localization system. Existing
combat rules and weapon damage ranges remain intact. No inventory framework,
replacement weapon, gold-earning system, or persistent shop was introduced.

## Run and demonstrate

Use the .NET 10 SDK from the repository root:

```sh
dotnet run --configuration Release
```

Choose a language, create a character, complete the rolls, proceed on the
adventure, and enter `s` to meet the merchant. Purchase an upgrade or decline.
After purchasing, revisit south to confirm that another purchase is unavailable,
then travel north to see the bonus in combat stats and combat behavior.
Use a new game session to demonstrate the other upgrade.

## Planning and configuration artifacts

- [Approved PRD](../.scratch/merchant-encounter/spec.md).
- [Ticket 01: merchant and armor](../.scratch/merchant-encounter/issues/01-merchant-armor-purchase.md).
- [Ticket 02: weapon enchantment](../.scratch/merchant-encounter/issues/02-magical-weapon-enchantment.md), dependent on Ticket 01.
- [Codex instructions](../AGENTS.md) and [local issue tracker conventions](agents/issue-tracker.md).
- [Triage labels](agents/triage-labels.md) and [domain documentation rules](agents/domain.md).
- Installed skills in `.agents/skills/` with provenance metadata in `skills-lock.json`.
- [Existing Italian-language ADR](0001-add-italian-language-support.md), preserved unchanged.

The PRD and ticket comments retain the decisions and authorization boundaries
from their original planning and implementation phases. Both tickets are now
implemented; the user separately authorized the final submission commit and push.

## Validation

```sh
dotnet build --configuration Release
dotnet test --configuration Release --no-build
git diff --check
```

The completed implementation passes 170 automated test cases: 118 original
regression cases and 52 merchant cases. Coverage includes both purchases,
declining and returning, both cross-purchase attempts, repeat visits, deterministic
damage, misses and defenses, original weapons, localized stats, all four
languages, invalid/blank input, and bounded EOF completion checks.

The user reported successful manual acceptance testing before requesting final
submission. Standards and specification code reviews were completed; the one
identified EOF test coverage gap was fixed. The remaining nonblocking observation
is duplicated upgrade-stat formatting in the existing character and combat
display methods.

Build outputs, test output directories, temporary review snapshots, credentials,
and local IDE settings are excluded from the submission.
