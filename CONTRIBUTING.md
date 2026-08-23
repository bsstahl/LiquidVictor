---
title: Contributing to LiquidVictor
description: Contribution expectations for the LiquidVictor SpeakerOps repository, including product scope, workflow, and logging conventions.
---

## Contributing to LiquidVictor

This repository is LiquidVictor, a fully functional product in the SpeakerOps family for media tracking, presentation asset management, and presentation assembly. Contributions should keep the product focused on presentation assets, deck assembly, versioned material tracking, and the relationships that help build and regenerate presentations without owning the wider conference submission lifecycle.

## Purpose and scope

LiquidVictor owns:

* presentation asset and media metadata
* slide and deck composition logic
* version tracking and asset lineage
* material reuse, regeneration, and presentation assembly workflows
* local representation of content objects used to build presentations

LiquidVictor does not own:

* conference CFP or submission workflow management
* talk catalog identity or proposal language
* publication and session lifecycle outside the repository boundary
* family-wide governance outside the product boundary

When in doubt, keep the work aligned to the LiquidVictor product domain and the repository's existing design docs, while preserving the product's independence within the broader SpeakerOps family.

## Required repository guidance

Before making changes, read the repository guidance that applies to the work:

* [README.md](./README.md)
* [CONTEXT.md](./CONTEXT.md)
* [docs/VERSION_MANAGEMENT.md](./docs/VERSION_MANAGEMENT.md)
* [docs/QUICK_REFERENCE.md](./docs/QUICK_REFERENCE.md)
* [docs/latex-support.md](./docs/latex-support.md)
* [.github/instructions/logging.instructions.md](./.github/instructions/logging.instructions.md)

## Tooling and environment

This repository is expected to target the latest .NET line with C# conventions consistent with the repo's existing .NET setup.

Use the standard .NET CLI from the repository root:

```bash
dotnet build
dotnet test
```

Keep the project runnable from the repo root with no hidden setup steps. When implementation work begins, follow the repo's local standards and keep all product docs aligned with the current domain model.

## Development workflow

### 1. Start from the design baseline

Use the canonical domain docs as the first source of truth before editing code or schema. Keep asset definitions, deck assembly rules, and media-handling workflows aligned to the repo docs.

### 2. Keep the task narrow and explicit

Prefer small, well-scoped changes. Avoid broad "cleanup" or "improve architecture" tasks without a concrete requirement or failing case.

### 3. Use TDD when behavior changes

For any code change that modifies behavior:

1. Write or update the failing test first
2. Confirm the test fails for the right reason
3. Implement only the minimum fix
4. Refactor carefully while keeping the relevant tests green
5. Re-run the relevant validation before continuing

### 4. Keep docs and implementation in sync

This repo is deliberately documentation-heavy. When a change affects a domain decision, schema, or boundary, update the relevant docs alongside the work.

### 5. Follow the logging convention

When adding logging:

* log activity and boundary transitions at informational levels
* keep payload bodies, record snapshots, and other verbose diagnostics at trace level
* avoid promoting payload detail to information unless the log message itself describes a meaningful product event rather than the data contents

## Branching and review expectations

* Use a short-lived feature branch for work
* Keep branch scope focused on one change or one cohesive task
* Prefer small commits over large batch edits
* Do not push to remote until the work is ready to share
* Use pull requests for merge review and discussion
* Accept pull requests from individual contributors
* Fully automated PRs are not accepted (those submitted by an agent instead of an individual)
* AI assistance is allowed when preparing a pull request, but the human contributor remains responsible for the content, accuracy, and intent of the submission

When the repo reaches its implementation stage, human review should remain required for changes that affect architecture, schema, or business rules.

## Documentation and coding conventions

Follow the repo's design and implementation guidance and keep documentation consistent with the product domain language. Avoid generic framework language or unrelated template wording.

## Boundaries and “do not touch” guidance

The repo has specific domain boundaries. Keep work inside these boundaries unless the user explicitly asks for a broader change.

When a source or file is external to this repo, treat it as authoritative only at its own boundary and keep LiquidVictor responsible for the lightweight relationship model, not for owning that external lifecycle.

## Pull requests and validation

Before opening or completing a pull request:

* ensure the change matches the repo's current design intent
* verify related docs are updated when required
* run the smallest relevant validation locally
* confirm the change is scoped and understandable
* include any assumptions or follow-up work explicitly in the PR description

## Questions and escalation

If a change is ambiguous, crosses an external system boundary, or could affect the LiquidVictor domain boundary, ask for clarification before making a broad implementation change.

The default stance is: stay narrow, stay domain-aligned, and keep the repo focused on the SpeakerOps presentation asset concept.
