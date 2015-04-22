@ECHO OFF

ECHO =======================================
ECHO Starting Vision Standalone Sim . . .
ECHO =======================================

chdir /D  %~dp0
cd .\bin
.\Vision.exe -skipconfig
cd ..
Echo.
Echo Vision stopped . . .

set /p nothing= Enter to continue
exit
