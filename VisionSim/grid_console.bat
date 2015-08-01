@ECHO OFF


ECHO =======================================
ECHO Starting Vision Grid servers . . .
ECHO =======================================

chdir /D  %~dp0
cd bin
Vision.Server.exe -skipconfig
cd ..
Echo.
Echo Vision Grid servers stopped . . .

set /p nothing= Enter to continue
exit
