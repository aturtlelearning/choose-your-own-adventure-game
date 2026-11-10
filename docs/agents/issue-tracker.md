# Issue tracker: Local Markdown

Issues and specs live as Markdown files in `.scratch/`.

## Conventions

- One feature per directory: `.scratch/<feature-slug>/`.
- Specs: `.scratch/<feature-slug>/spec.md`.
- Implementation tickets: `.scratch/<feature-slug>/issues/<NN>-<slug>.md`,
  numbered from `01`, with one file per ticket.
- Record triage state in a `Status:` line near the top of each issue.
  Use the role strings in `triage-labels.md`.
- Record the category as `Category: bug` or `Category: enhancement`.
- Append conversation history under `## Comments`.

## Publishing and fetching

When a skill says "publish to the issue tracker", create the appropriate
spec or issue file under `.scratch/<feature-slug>/`.

When a skill says "fetch the relevant ticket", read the referenced file.
Resolve issue numbers within the relevant feature directory.

## Wayfinding operations

- Map: `.scratch/<effort>/map.md`, containing Notes, Decisions-so-far,
  and Fog.
- Child tickets: `.scratch/<effort>/issues/NN-<slug>.md`.
- Record ticket type using `Type: research`, `prototype`, `grilling`,
  or `task`.
- Record dependencies using `Blocked by: NN, NN`.
- The frontier consists of open, unblocked, unclaimed tickets;
  choose the lowest number first.
- Claim a ticket by setting `Status: claimed` before working.
- Resolve it by appending `## Answer`, setting `Status: resolved`,
  and adding a summary and link to the map's Decisions-so-far.
- A ticket is unblocked when all its listed dependencies are resolved.

Wayfinding statuses describe execution progress; triage statuses use
the vocabulary in `triage-labels.md`.
