# Shared Player Stats Ticket 01: Characterization Baseline

Scope: tests only, before any shared player-stat formatting extraction.
Production baseline: 549622eb112c1d5139eb521428ff275cc1800e80.
No production changes, commits, or pushes performed.

## Coverage

- 24 deterministic fixture cases: English, Spanish, French, and Italian; no upgrade, armor, and enchantment; with and without the player's weapon.
- Each fixture is exercised through both existing public displays, producing 48 exact-output characterization cases.
- Player fixtures have fixed identity, attributes, health, weapon data, and purchase state. Equipment varies among dagger, long sword, and lightning bolt spell, with their corresponding occupations.
- Armor or enchantment is purchased before removing the weapon in absent-weapon cases, preserving independent ownership and effect rows. Expected gold is 50 without purchase and 10 after purchase.
- Dragon identity, attributes, health, and weapon are explicitly fixed; seeded dice keep construction deterministic. No attacks or combat rounds run in these tests.

## Exact-output protection

The expected character and combat strings are frozen literals in
[StatsDisplaySnapshots.json](../../CYOATests/StatsDisplaySnapshots.json). They were
authored from the inspected pre-refactor labels and layout, then checked against
the unchanged implementation. Tests do not generate expectations from live
Messages data, either display method, or the future shared formatter.

Tests compare complete strings without trimming spaces, sorting rows, or
discarding punctuation. The only runtime conversion replaces the fixture's LF
separators with Environment.NewLine.

The character snapshots include the exact heading, name, race, occupation,
ordered stat/equipment rows, and final newline. Combat snapshots include all
headings, the player name and stat rows, exactly one blank separator, the whole
dragon block, and no trailing newline. Unpurchased effects and absent weapon
rows remain omitted as in the existing summaries.

Snapshot SHA-256 at the passing baseline:

```text
69F6A1C00F3209FC2A649B81237DDC8833691114B513DA2492A78CFFB76B4D5D
```

Keep these expectations unchanged during Ticket 02; an extraction must preserve
them rather than update snapshots to match accidental output drift.

## Validation results

- First focused run against unchanged production: 48 passed, 0 failed, 0 skipped.
- Release solution build: succeeded, 0 warnings, 0 errors.
- Full Release regression suite: 360 passed, 0 failed, 0 skipped, including the existing 312 cases.
- git diff --check: passed.
- Explicit Git comparison against HEAD confirmed no changes to any production C# source, root application project, or localization data.

```text
dotnet test CYOATests/CYOATests.csproj --configuration Release --no-restore --filter FullyQualifiedName~StatsDisplayCharacterizationTests
dotnet build choose-your-own-adventure-game.sln --configuration Release --no-restore
dotnet test CYOATests/CYOATests.csproj --configuration Release --no-build --no-restore
git diff --check
```

The test project change only copies the frozen JSON fixture into test output.
Existing tests and their assertions were not modified. Character console output
is restored in finally; the fixture disables parallel execution to protect
global console capture.

## TDD checkpoint

Ticket 01 intentionally establishes passing characterization of existing
behavior. All 48 cases passed on their first test execution; no red failures
were manufactured. This is the required baseline before Ticket 02 can introduce
the new shared method using red-green-refactor TDD.

## Standards

No actionable findings. Tests use existing public displays, deterministic
fixtures, frozen complete expected strings, exact whitespace comparisons, and
safe console restoration. Repeated snapshot text is intentional: factoring
expected rows into production-style formatting would weaken independence.

## Spec

No findings for Ticket 01. All required language/equipment combinations, retained
upgrades without a weapon, exact complete output, separators, and differing
trailing-newline contracts are covered. Production behavior and existing tests
are unchanged. No Ticket 02 implementation was introduced.

Two independent read-only reviewers examined the test-project diff and both new
test artifacts against the committed production baseline. The focused, build,
and full-suite results above were supplied to both reviews before resolution.

Summary: Standards 0 findings; Spec 0 findings; no outstanding issue in either axis.
