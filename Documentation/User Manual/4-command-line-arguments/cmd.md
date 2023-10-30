## Command Line Arguments

![](pics/cmd3.png)

The VECTO 4.x commandline tool can be used to start simulations from the command line and  runs without  graphical user interface. If multiple job-files are specified or a job-file contains multiple simulation runs (i.e., multiple cycles and/or loadings) these simulations are executed in parallel.

### General Notes

-   The order in which the arguments are provided is arbitrary.
-   If a file path includes space characters (e.g. "C:\\VECTO Test Files\\Demo.vecto") then double quotes have to be used (as in the picture above).
-   If not the complete file path is defined (e.g. "file1.vecto" instead of "c:\\data\\file1.vecto") then VECTO expects the file in the application directory (where vectocmd.exe is located).
-   `ams` command arguments are not arbitrary and should follow the order specified.
-   To avoid confirmation for running the simulation in `ams` command prepend yes or no to the tool execution as follows: `"yes"|vectocmd.exe [ARGUMENTS]`

### Basic usage 

        vectocmd.exe [-h] [-v] [-q] FILE1.(vecto|xml) [FILE2.(vecto|xml) ...]
        vectocmd.exe [-h] [-v] [-q] -ams PREVIOUS_STEP.xml CURRENT_STEP.xml OUTPUT.xml

### List of command line arguments

- FILE1.vecto [FILE2.vecto ...]: A list of vecto-job files (with the 
       extension: .vecto). At least one file must be given. Delimited by 
       whitespace.
- PREVIOUS_STEP.xml: Previous manufacturing step VIF.
- CURRENT_STEP.xml: Current interim or completed manufacturing step VIF.
- OUTPUT.xml: Output path for the new VIF.
- -ams: Append manufacturing step.
- -t: output information about execution times.
- -mod: write mod-data in addition to sum-data.
- -eng: switch to engineering mode (implies -mod).
- -v: Shows verbose information (errors and warnings will be displayed).
- -vv: Shows more verbose information (infos will be displayed).
- -vvv: Shows debug messages (slow!).
- -vvvv: Shows all verbose information (everything, slow!).
- -V: show version information.
- -h: Displays this help.
