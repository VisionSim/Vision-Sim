#!/bin/bash
# Start the Vision-Sim server only
# Versions 0.9.2+
#
# May 2014
# greythane @ gmail.com

cd ./bin
sleep 1
echo Starting Standalone Region Simulator...
mono Vision.exe -skipconfig
wait
exit

