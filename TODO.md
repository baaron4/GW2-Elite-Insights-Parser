# TODO

- [ ] Sync this branch (`logOptimiser`) with upstream `master`.
      Branched from `0ef94e3` and not kept up to date automatically on
      purpose - do NOT auto-merge/rebase in CI or on a schedule, since
      that could change parser behavior out from under active
      development on the `EI_PLAYER_FILTER` change without warning.
      Sync manually when ready:
      `git fetch upstream && git rebase upstream/master` (re-resolving
      the `JsonLogBuilder.cs` conflict if upstream touched that file),
      then force-push this branch.
