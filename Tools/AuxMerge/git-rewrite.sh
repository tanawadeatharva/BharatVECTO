#!/bin/bash
# Merge cb11c451816@Current branch from Ricardo's repo with ab66a68@RICARDO_FORK tag from VECTO-2.0 repo (central). 
#
# NOTE: this script has run in small pieces, not all at once.

set -o  errexit


function mark_original_branches {
	git branch auxmerge/ricardo_original   cb11c451816


	## Mark some important commits.
	#	See rewrite_aux_history-tree_filter.sh
	#
	git tag auxmerge/ric-VectoAuxiliariesTB_begin  5d6fafd2e553 -f \
        -m "Renaming of 'VectoAuxiliaries' --> 'VectoAuxiliariesTB'. "
	git tag auxmerge/ric-VectoAuxiliaries_end  80fd13a4b739 -f \
        -m "Last 'valid' modifications of precursor to 'VectoAuxiliariesTB' folder (without 'TB' suffix)."
	git tag auxmerge/ric-VectoAuxiliaries_begin  538e211c6566 -f \
        -m "Initial commit of precursor to 'VectoAuxiliariesTB' folder (without 'TB' suffix)."
	git tag auxmerge/ric-RICARDO_ROOT  57393fe -f
        -m "First commit delivered by Ricardo."

    git tag auxmerge/ric-import_vecto_core 414ea89aac15312 -m 'The position where Ricardo first imported VECTO-core code (M2.0.41-beta2@ab66a68b7333cd86).'
    git tag auxmerge/jrc_fixup-import_vecto_core 87627329528baf0770 -m 'The position where Ricardo first imported VECTO-core code (M2.0.41-beta2@ab66a68b7333cd86).'
    
    git tag  auxmerge/ric-import_vecto_core_invalid1  273e72bd626b1 -m "Copy v2.0.4-beta2 as '/vecto-sim-ricardoaea' but never touched!"
    git tag  auxmerge/ric-add_vecto-invalid2-TestFolder  ac778435f73a2f431 -m "Ricardo add original VECTO-core in TestFolder"
    
    git tag auxmerge/ric-del_12t_mistake 76310dabe2274 -m "Nik's commit containing Manual-updates AND removing the 12t demo-truck mistake."
 
    git tag auxmerge/ric-added_ReleaseFiles dc67578df1608645500d029  -m "orig-msg: Schematics V11, vsum output fix - Zarb, Glenn

Glen Imported 'VECTO/Release Files/', duplicating User-manual and Generic-vehicles.
NOTE: VS fails, with 27 missing pdfstamper libs, etc
"
	git tag auxmerge/tug-ric_fork_byDana  ab66a68b7333cd8 -f  -m "
This is the fork for unused '/vecto-sim-ricardoaea' folder'.

According to Dana@Ricardo email on 30/Apr/2015, he FORKed 2a006e6dacf5e7
  but JRC(ankostis) assumes he meant the next-one, non-merge commit: 
  tag:milestone/2.0.4-beta2@ab66a68b7333cd8

Nevertheless, by searching, discovered that:
  Dana was referring to folder '/vecto-sim-ricardoaea/',
  imported originally on 16-Sep-2014, and left there almost untouched hereafter
  (now tagged as 'auxmerge/ric-import_vecto_core_invalid1@273e72bd626b1653d97').
    "
	git tag auxmerge/tug-RICARDO_FORK-byDana  f4e6fc911db3fdefda95 -f  \
        -m "This is 'milestones/2.0.4-beta'.
        
According to Dana@Ricardo email on 30/Apr/2015, he FORKed 2a006e6dacf5e7
  but JRC(ankostis) assumes he meant the next-one, non-merge commit: 
  tag:milestone/2.0.4-beta2@ab66a68b7333cd8

Nevertheless, by searching, discovered that:
  Dana was referring to folder '/vecto-sim-ricardoaea/',
  imported originally on 16-Sep-2014, and left there almost untouched hereafter
  (now tagged as 'auxmerge/ric-import_vecto_core_invalid1@273e72bd626b1653d97').
  
  BUT Ricardo's code was eventually based on '/vecto-sim-ricardoaeaTB' folder,
  imported for the 1st time on 30-Sept-2015, 
  (now tag as 'auxmerge/ric-import_vecto_core@414ea89aac15312',
  and in JRC's fixup-branch as 'auxmerge/ric-import_vecto_core@87627329528baf0770'.
  
  EVEN MORE, the cases was not just like this....
  
  By searching tom see which was the actual fork-commit for 'TB' folder
  I got these results:

    $ git diff tag auxmerge/jrc_fixup-import_vecto_core  milestones/2.0.4-beta2|wc
    1104528 1594313 37141033

    $git diff tag auxmerge/jrc_fixup-import_vecto_core   milestones/2.0.4-beta1|wc
    1104502 1594153 37139993

    $ git diff tag auxmerge/jrc_fixup-import_vecto_core  milestones/2.0.4-beta|wc
    1104461 1593946 37138503

  
  The less DIFFS are with '2.0.4-beta'!
  So clearly this is the FORK COMMIT
 "
}

function remove_git_backup {
	rm -rf  .git/refs/original # Clear git-backup to continue re-writting.
}

function fixup_ricardo_sources 
	#git checkout auxmerge/ricardo_original
	#git checkout -b auxmerge/aux-fixup
	git filter-branch ${TMP_DIR:+-d $TMP_DIR} \
			--prune-empty \
			--tree-filter  "$PWD/fixup_aux_history.tree_filter.sh"
}

## Eliminate duplicate folders (mostly safe stuff).
# 
# The TMP_DIR may point to some RAMDisk (i.e. for windows see ImDisk).
#
function cleanup_ricardo_sources {
	#git checkout auxmerge/ricardo_original
	#git checkout -b auxmerge/aux1.3-cleanup
	git filter-branch ${TMP_DIR:+-d $TMP_DIR} \
			--prune-empty \
			--tree-filter  "$PWD/cleanup_aux_history.tree_filter.sh"
}


## Move VECTO sources to root & vecto-aux-->AUX.
#
function reroot_vecto_sources {
	#git checkout auxmerge/aux1.3-cleanup
	#git checkout -b auxmerge/aux2.4-reroot_vecto
	git filter-branch ${TMP_DIR:+-d $TMP_DIR} \
			--prune-empty \
			--tree-filter  "$PWD/root_vecto-mv_aux.tree_filter.sh" 
}


#git checkout Current
#mark_original_branches

## EITHER ALL IN ONE...

remove_git_backup
fixup_ricardo_sources 

## OR IN STAGES...

#remove_git_backup
#cleanup_ricardo_sources 

#remove_git_backup
#reroot_vecto_sources

##  JOIN the 2 unrelated branches and
#   MANUALLY RESOLVE CONFLICTS
#git branch auxmerge/aux3-rebased_on_fork
#git checkout auxmerge/aux3-rebased_on_fork
#git rebase auxmerge/RICARDO_FORK