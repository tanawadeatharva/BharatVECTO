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

## VETO.vbproj & MORE(!) conflicts on  4b779d2323ece680 "Schematics V11, vsum output fix":
#  Glenn added 'VECTO/Release Files/*'
#  ABORT REBASE to rewrite rest commits properly distributing 'VECTO/Release Files/'
git tag auxmerge/aux_fixup-added_ReleaseFiles 4b779d2323ece6806470c8e -m "orig-msg: Schematics V11, vsum output fix - Zarb, Glenn

Glen Imported 'VECTO/Release Files/', replicating manual and generic Vehicles
NOTE: VS fails, with 27 missing pdfstamper libs, etc
"
git cherry-pick auxmerge/aux_fixup-added_ReleaseFiles
function move_to_root_ReleaseFiles() {
	rsync -ma VECTO/Release\ Files/ .
	git rm -rf VECTO/Release\ Files
	rm -f './User Manual/pics/Thumbs.db'
	git add 'User Manual' 'Generic Vehicles' 'Declaration'
    ## Convert
    # <None Include="Release Files\User Manual\Release Notes.pdf" />
    ###
    # <None Include="..\User Manual\Release Notes.pdf">
      # <Link>Release Files\Release Notes.pdf</Link>
    # </None>
	sed -i 's_<\(\w\+\)\s\+Include="\(Release Files\\\([^"]\+\)\)"\s*/>_<\1 Include="..\\\3" >\n      <Link>\2</Link>\n    </\1>_' VECTO/VECTO.vbproj

	patch VECTO/VECTO.vbproj << EOF ## FAILS!!! Do it manually
	--- a/VECTO/VECTO.vbproj
	+++ b/VECTO/VECTO.vbproj
	@@ -765,9 +765,5 @@
	   </ItemGroup>
	   <Import Project="$(MSBuildBinPath)\Microsoft.VisualBasic.targets" />
	   <PropertyGroup>
	-    <PostBuildEvent>XCOPY "$(ProjectDir)Release Files" "$(TargetDir)" /Y /E</PostBuildEvent>
	+    <PostBuildEvent>XCOPY "$(SolutionDir)User Manual" "$(SolutionDir)Generic Vehicles" "$(TargetDir)" /Y /E</PostBuildEvent>
	   </PropertyGroup>
EOF
	git add VECTO/VECTO.vbproj
}
move_to_root_ReleaseFiles
git commit -m "JRC: Re-root Ric's 'Release Files', update VECTO.vbproj."

git cherry-pick 4b779d2323ece6806470..auxmerge/aux-fixup


## CONFLICT DECLARATION FILES on 4f494692f4cac435b "git-tfs-id: [http://tfs00.element.root.com:8080/tfs/TFSCollection]$/VECTO;C2019"
#
move_to_root_ReleaseFiles
git commit -m "JRC: Add RICARDO Declaration files.

- sed-ed conflicting VECTO.vbproj.
- Orig-commit-message:

 git-tfs-id: [http://tfs00.element.root.com:8080/tfs/TFSCollection]$/VECTO;C2019
"


## EMPTY COMMIT on  bf441b7 "Add referenced file"
#  SKIP IT, `itextsharp.dll` already there!
git reset && git cherry-pick --continue

## SKIP almost empty e7d7b7dd5dbddc4d"Remove TFS bindings"
#  Bindings have been removed during "fixup" rewrite - this commits contains
#  just a space after `global` in VECTO.sln.

## MISSING FILE IN a6ec62ecdc98f9 "Remove 12t Delivery Truck from \Generic Vehicles\Engineering Mode as per Nik's email."
#
move_to_root_ReleaseFiles ## AND manually merge .gitignore.
git add
git commit -m "JRC-rewrite: Remove 12t Delivery Truck from \Generic Vehicles\Engineering Mode as per Nik's email.

Move all 'VECTO/Release Files' to root, and resolve conflict in 'VECTO/VECTO.vbproj'.

REST MSG: Remove 12t Delivery Truck from \Generic Vehicles\Engineering Mode as per Nik's email.
Replace User Manual folder with one the from Q driver as per Nik's email.
Added files to Developer Guide as per Nik's email.
"

## finished!

## COMPARE WITH LATER MILESTONES
#  to see which one looks close to rebased AUX-sources.
#
# Run output of next cmd manually.
git tag -l | grep '/2' | xargs -n1 -I XXX echo git diff auxmerge/jrc-merge_tug3-ok_BeforeReleaseFiles XXX \| wc

## auxmerge/jrc-rebase_on_tug-2.0.4-beta@(6a1d148a2aef020d)

$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.0.1-beta0 | wc
1035928 3216401 42980069
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.0.1-beta1 | wc
1035925 3216460 42980898
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.0.3-beta0 | wc
1035403 3215881 42955933
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.0.4-beta | wc
1035453 3216044 42957529
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.0.4-beta1 | wc
1035540 3216370 42959917
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.0.4-beta2 | wc
1035566 3216530 42960957
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.0.4-beta3 | wc
1035726 3217224 42967144
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.0.4-beta4_Test | wc
1035741 3217349 42967891
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.1 | wc
1130261 3313717 45526197
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.1.1 | wc
1130595 3314676 45532558
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.1.2 | wc
1130635 3314320 45525013
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.1.3 | wc
1131140 3316060 45527823
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.1.4 | wc
1131160 3316152 45528358
$ git diff auxmerge/jrc-rebase_on_tug-2.0.4-beta milestones/2.2 | wc
1131865 3318693 45527057
(winpy34)
