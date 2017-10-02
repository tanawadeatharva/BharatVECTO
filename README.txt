
COMPILING VECTO
---------------

Compiling VECTO requires Microsoft Visual Studio 2013 (or later)

Option 1: Compiling from the command-line

you can build VECTO using the following command:

"c:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe" Vecto.sln /t:Build /p:Configuration=Release

or

"c:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe" Vecto.sln /t:Build /p:Configuration=Debug


Option 2: Load the solution file in Visual Studio and compile VECTO.

The executable can be found in 

<Path to VECTO-Source Folder>\VECTO\bin\Release 

or

<Path to VECTO-Source Folder>\VECTO\bin\Debug

depending on the configuration.