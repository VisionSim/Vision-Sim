@ECHO OFF

ECHO =======================================
ECHO Starting Vision Grid Region . . .
ECHO =======================================

chdir /D  %~dp0
cd .\bin
.\Vision.exe -skipconfig
cd ..
Echo.
Echo Vision stopped . . .

set /p nothing= Enter to continue
exit
