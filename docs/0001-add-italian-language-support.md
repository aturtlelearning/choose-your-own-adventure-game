# ADR 0001: Add Italian Language Support

## Status

Accepted and implemented.

## Context

The game supported English, Spanish, and French. Italian was requested as a
fourth language so Italian-speaking players could use translated prompts,
narratives, and character labels without changing the game rules.

## Decision

Extend the existing JSON-based localization approach:

- Update `Game.cs` to display `4. Italiano` and map selection `4` to `Italian`.
  Preserve the existing selections and English fallback for unrecognized input.
- Add an `Italian` section to `language_data.json` containing all 69 message
  keys, localized race and occupation input mappings, and race, occupation, and
  weapon display mappings. Preserve formatting placeholders and existing languages.
- Leave `Messages.cs` unchanged. It already loads the section named by
  `CurrentLanguage` and converts localized input to the canonical values used by
  gameplay logic.

Add `CYOATests/ItalianLanguageTests.cs` to verify message loading, key coverage,
format argument counts, input validation and normalization, display mappings,
switching back to existing languages, startup selection, and an Italian
character creation and south-path flow. `dotnet build` completed with no warnings
or errors, and `dotnet test` passed all 118 tests locally. Manual play in Italian
was also confirmed successful.

## Consequences

- Players can select Italian while existing languages and gameplay remain intact.
- Internal race and occupation values remain unchanged, so combat and character
  rules need no language-specific branches.
- Commands such as `roll`, `n`, `s`, `e`, `a`, and `r` retain their existing behavior;
  Italian prompts explain which tokens to enter.
- Future message changes must keep Italian translations, mappings, and formatting
  placeholders synchronized with the other languages.
