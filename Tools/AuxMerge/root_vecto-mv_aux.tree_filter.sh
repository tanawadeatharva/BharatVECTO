# root_vecto-mv_aux.tree_filter.sh: 
#	A git tree-filter script preparing Ricardo's repo to merge with VECTO-2.0 repo (central).
# 
# It is used by the cmd::
#		
#		git filter-branch ${TMP_DIR:+-d $TMP_DIR} --prune-empty  --tree-filter "$PWD/root_vecto-mv_aux.tree_filter.sh" auxmerge/aux-reroot_vecto
#
# Assumes repo has been cleaned-up with cleanup_aux_history.tree_filter.sh
#
# by ankostis, 4-Dec-2015

set -o  errexit


## /vecto-sim-ricardoaeaTB: VECTO-core modified by Ricardo
# Move its contents to the root of the project.
#
if [ -d vecto-sim-ricardoaeaTB ]; then
	rsync -ma vecto-sim-ricardoaeaTB/  .
    rm -rf vecto-sim-ricardoaeaTB
fi
grep 'vecto-sim-ricardoaeaTB' -rlZ * | xargs -r0I XXX sed 's/vecto-sim-ricardoaeaTB\\//g' -i XXX

## /VectoAuxiliaries-TB: AdvancedAUX UI and code
# Rename it as `VectoAuxiliaries`.
#
if [ -d VectoAuxiliaries-TB ]; then
	mv VectoAuxiliaries-TB/ VectoAuxiliaries
fi
grep 'VectoAuxiliaries-TB' -rlZ * | xargs -r0I XXX sed 's/..\\VectoAuxiliaries-TB/VectoAuxiliaries/g' -i XXX
