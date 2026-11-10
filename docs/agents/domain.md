# Domain docs

## Before exploring

Read the root `GLOSSARY.md`, if present, and ADRs relevant to the work
under `docs/adr/`.

Also read the existing ADR at
`docs/0001-add-italian-language-support.md` when working on localization.

If `GLOSSARY-MAP.md` is introduced later, follow its links to the
glossaries relevant to the topic.

If glossary files or ADR directories do not exist, proceed silently.
Do not suggest creating them upfront. The domain-modeling skill
creates them when terms or decisions are resolved.

## Layout

This repository uses a single-context layout:

- `GLOSSARY.md`: shared domain vocabulary.
- `docs/adr/`: architecture decision records.

## Use the glossary's vocabulary

Use glossary terms in issue titles, proposals, hypotheses, and test names.
Avoid synonyms the glossary explicitly excludes.

If a needed concept is missing, reconsider the terminology or note the
gap for domain-modeling.

## Flag ADR conflicts

Explicitly identify proposals that contradict an existing ADR.
Reference the ADR and explain why its decision may need revisiting.
