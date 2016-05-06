##Command Line Arguments

Both, Vecto 2.2 and Vecto 3.x can be started from the commandline. The usage is quite different and described below.

<div class="vecto2">

![](pics/cmd.png)

It is possible to control basic functions of VECTO via command line arguments (e.g. to automate calculations and results analysis using scripts).

###General Notes

-   The order in which the arguments are provided is arbitrary.
-   If a file path includes space characters (e.g. "C:\\VECTO Test Files\\Demo.vecto") then double quotes have to be used (as in the picture above).
-   If not the complete file path is defined (e.g. "file1.vecto" instead of "c:\\data\\file1.vecto") then VECTO expects the file in the application directory (where VECTO.exe is located).



###Engineering/Declaration Mode

        VECTO.exe -run [-close] [file1.vecto file2.vecto ... fileN.vecto]


Runs calculation(s) either with the provided .vecto file(s) or (if no file names are defined) with the files already loaded on start up\*. If -**close** is used then VECTO closes after calculations are done.


###Batch Mode

		VECTO.exe -run -batch [-close] [file1.vecto file2.vecto ... fileN.vecto] [cycle1.vdri cycle2.vdri ... cycleN.vdri]



 Switches to BATCH mode and runs with the provided .vecto and .vdri files. If no files are defined the pre-loaded files\* are used. If -**close** is used then VECTO closes after calculations are done.


###Opening files

		VECTO.exe file1.xxx


If the file has one of the following extensions it is opened with the associated editor dialog: .vecto, .vgbx, .veng, .vveh. Note: if more than one .vecto file is provided they will be loaded in the file list (replacing the pre-loaded list\*) instead.


 *pre-loaded files: When VECTO starts it loads the file lists (.vecto, .vdri) of the last session, see [Application Files](#application-files). These files can be changes manually if VECTO is not running.

</div>

<div class="vecto3">

![](pics/cmd3.png)

The Vecto 3.x commandline tool runs completely without a graphical user interface. If multiple job-files are specified or a job-file contains multiple simulation runs (i.e., multiple cycles and/or loadings) these simulations are executed in parallel. Vectocmd supports both types of input files, JSON (in engineering and declaration mode) and XML (Declaration mode only).

The basic usage is as follows

        vectocmd.exe [-h] [-v] FILE1.(vecto|xml) [FILE2.(vecto|xml) ...]

###List of command line arguments

- FILE1.vecto [FILE2.vecto ...]: A list of vecto-job files (with the 
       extension: .vecto). At least one file must be given. Delimited by 
       whitespace.
- -t: output information about execution times
- -mod: write mod-data in addition to sum-data
- -eng: switch to engineering mode (implies -mod)
- -v: Shows verbose information (errors and warnings will be displayed)
- -vv: Shows more verbose information (infos will be displayed)
- -vvv: Shows debug messages (slow!)
- -vvvv: Shows all verbose information (everything, slow!)
- -V: show version information
- -h: Displays this help.


</div>