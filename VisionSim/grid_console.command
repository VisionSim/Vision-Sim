#!/bin/bash
# Start the Vision-Sim Grid server
# Version 0.9.2+
#
# May 2014
# greythane @ gmail.com

WOASDIR="${0%/*}"
cd $WOASDIR
echo $WOASDIR

cd ./bin
echo Starting Vision Grid server...
mono Vision.Server.exe -skipconfig
wait

exit

