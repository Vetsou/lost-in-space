# Contributing guidelines

## Branches
- The `main` branch is the production-ready branch.
- All development work should be done on feature branches and then merged into `main` once the work is done.
- Use short, descriptive branch names that clearly relate to the issue or feature being worked on.
- Branches should be deleted after merging (this is done automatically).

## Pull Requests
- A pull request should address a single issue but may address multiple issues if they are small enough and closely related.
- Pull requests should be merged by the person who created them.
- Each pull request must be reviewed by at least one CODEOWNER before merging.
- Pull request titles should follow the following pattern: `<pr-type>(#<issue-nums>) <pr-full-title>`
  Allowed PR types: `feat`, `chore`, `refactor`, `fix`. Issue numbers should correspond to one or more closed issues.
  Example: `fix(#11,#8) Fix audio settings volume and fullscreen toggle`