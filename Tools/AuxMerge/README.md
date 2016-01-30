## Scripts for Merging Ricardo's sources to VECTO-core.
#
# by ankostis

The merging of VECTO-AUX <--> VECTO-Core has happenned in these stages:

1. Identify fork-point in VECTO-code's history
   (see `git-rewrite.sh`):

        milestone/2.0.4-beta

2. FIXUP VECT-AUX sources to be similar to the above VECTO-core sources
   (see `git-rewrite.sh`).


3. Merge the rewritten VECTO-AUX-sources into identified VECTO_Core version
   (see `rebase_aux.sh`).

4. Rebase re-written branch in step 2 onto merged-commit in step-3 (with many manual actions)
   (see `rebase_aux.sh`).

5. Append these tools into sources.

6. NEXT steps:
   - Await Ricardo's fixes (TCs failing, aux-model discrepancy).
   - Merge with later milestones, till latest 2.2.


- The `del*.sh` files are utilities for other remotes,
  to delete refs now abandoned (not complete).

That's it.