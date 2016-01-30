## COMMAND-LOG, NOT MEANT TO RUN AS A SCRIPT.
#
git merge milestones/2.0.4-beta  -s recursive  -X renormalize -Xignore-space-at-eol
git checkout milestones/2.0.4-beta -- VECTO/GUI/{F_VEH_AuxDlog.vb,F_VEH_AuxDlog.resx,F_VEH_AuxDlog.Designer.vb,F_MAINForm.vb,F_VECTO.vb} \
        VECTO/Input\ Files/cVECTO.vb

#### Manual merge .sln & .vbproj. ####

git commit -m 'Merge Ricardo initial-VECTO-core with forked point `milestones/2.0.4-beta`. 

+ All conflicts replaced with `2/0/4-beta`,
  apart from .sln & .vbproj.
+ Synthesize next artifical commit with Ricardo changes on original VECTO-core.
'

## Synthesize next artifical commit with RIcardo's changes on original VECTO-core.
git checkout auxmerge/jrc_fixup-import_vecto_core -- VECTO/GUI/{F_VEH_AuxDlog.vb,F_VEH_AuxDlog.resx,F_VEH_AuxDlog.Designer.vb,F_MAINForm.vb,F_V ECTO.vb} VECTO/Input\ Files/cVECTO.vb
git commit -m 'Synthesize an artifical commit with Ricardo changes on top of original VECTO-core.'

##  REBASE FIXUP ontop of MERGED BASE.
#
#### NOT SURE CORRRECT!!
git checkout -b auxmerge/jrc-merge_tug auxmerge/aux-fixup
git rebase auxmerge/aux-fixup auxmerge/jrc-merge_tug --onto=auxmerge/jrc_fixup-import_vecto_core  --ignore-whitespace
## User-Manual conflicts on  b7a8675fe271c29e "ADDING USER MANUAL - FIRST AS IS CHECKIN":
#  because manual already there.
git checkout b7a8675fe271c29e -- User\ Manual/GUI/{ENG-Editor.html,GBX-Editor.html,VECTO-Editor.html,VECTO-Editor_Aux.html,VEH-Editor.html,mainform.html,settings.html} \
		User\ Manual/fileformat/{VDRI.html,VMOD.html,VSUM.html,index.html}
## VETO.vbproj conflict on  74cc5f17b046992 "WIP - FILE VIEWERS ADDED":
#  MANUALLY Delete vscc parts of ricardo's file.

## Conflicts on 35f1fa6564b1 "DOCS DONE":
#  Just added modified files

## VETO.vbproj & MORE(!) conflicts on   4b779d2323ece680 "Schematics V11, vsum output fix":
#  Glenn added 'VECTO/Release Files/*'
#  ABORT REBASE to rewrite rest commits properly distributing 'VECTO/Release Files/'
git tag tag auxmerge/aux_fixup-added_ReleaseFiles 4b779d2323ece6806470c8e -m "orig-msg: Schematics V11, vsum output fix - Zarb, Glenn

Glen Imported 'VECTO/Release Files/', replicating manual and generic Vehicles
NOTE: VS fails, with 27 missing pdfstamper libs, etc
"
git cherry-pick auxmerge/aux_fixup-added_ReleaseFiles
function move_to_root_ReleaseFiles() {
	rsync -ma VECTO/Release\ Files/ .
	git rm -rf VECTO/Release\ Files
	rm ./User Manual/pics/Thumbs.db
	git add 'User Manual' 'Generic Vehicles' 
	patch VECTO/VECTO.vbproj << EOF ## FAILS!!! Do it manually
	--- a/VECTO/VECTO.vbproj
	+++ b/VECTO/VECTO.vbproj
	@@ -765,9 +765,6 @@
	   </ItemGroup>
	   <Import Project="$(MSBuildBinPath)\Microsoft.VisualBasic.targets" />
	   <PropertyGroup>
	-    <PostBuildEvent>XCOPY "$(ProjectDir)Release Files" "$(TargetDir)" /Y /E</PostBuildEvent>
	+    <PostBuildEvent>XCOPY "$(ProjectDir)\..\Generic Vehicles" "$(TargetDir)" /Y /E</PostBuildEvent>
	+    <PostBuildEvent>XCOPY "$(ProjectDir)\..\User Manual" "$(TargetDir)" /Y /E</PostBuildEvent>
	   </PropertyGroup>
EOF
	sed -i 's/Release Files\\/..\\/' VECTO/VECTO.vbproj
	git add VECTO/VECTO.vbproj
}
git commit -m 'JRC-rewrite: Schematics V11, vsum output fix - Zarb, Glenn

Move all `VECTO/Release Files` to root, and resolve conflict in `VECTO/VECTO.vbproj`.
'
git cherry-pick 4f494692f4cac435b970497b..auxmerge/aux-fixup

## EMPTY COMMIT on  bf441b7 "Add referenced file"
#  SKIP IT, `itextsharp.dll` already there!
git reset && git cherry-pick --continue

## SKIP almost empty e7d7b7dd5dbddc4d"Remove TFS bindings"
#  Bindings havebeen removed during "fixup" rewrite - this commits contains 
#  just a space after `global` in VECTO.sln.

## MISSING FILE IN a6ec62ecdc98f9 "Remove 12t Delivery Truck from \Generic Vehicles\Engineering Mode as per Nik's email."
# 
move_to_root_ReleaseFiles ## AND manually merge .gitignore.
git commit -m 'JRC-reqrite: Remove 12t Delivery Truck from \Generic Vehicles\Engineering Mode as per Nik's email.

Move all `VECTO/Release Files` to root, and resolve conflict in `VECTO/VECTO.vbproj`.

REST MSG: Remove 12t Delivery Truck from \Generic Vehicles\Engineering Mode as per Nik's email.
Replace User Manual folder with one the from Q driver as per Nik's email.
Added files to Developer Guide as per Nik's email.
'

## finished!

## COMPARE WITH LATER MILESONES
#  to see which one looks close to rebased AUX-sources.
#
# Run output of next cmd manually.
git tag -l | grep '/2' | xargs -n1 -I XXX echo git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles XXX \| wc
 git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.0.1-beta0 | wc
1034241 3210452 42918389
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.0.1-beta1 | wc
1034238 3210511 42919218
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.0.3-beta0 | wc
1033716 3209932 42894253
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.0.4-beta | wc
1033766 3210095 42895849
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.0.4-beta1 | wc
1033853 3210421 42898237
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.0.4-beta2 | wc
1033879 3210581 42899277
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.0.4-beta3 | wc
1034039 3211275 42905464
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.0.4-beta4_Test | wc
1034054 3211400 42906211
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.1 | wc
1128574 3307768 45464517
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.1.1 | wc
1128908 3308727 45470878
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.1.2 | wc
1128948 3308371 45463333
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.1.3 | wc
1129453 3310111 45466143
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.1.4 | wc
1129473 3310203 45466678
git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles milestones/2.2 | wc
1130178 3312744 45465377