#!/bin/bash
# Startup script for the Vision-Sim Region server service
# Versions 1.0.4+
#
# July 2017
# Britanyann Copperfield <britanyann@imperialestatex.biz>
#

cd ./bin
wait
echo Starting Vision-Sim Region Simulator...
screen -S Sim -d -m mono Vision.exe -skipconfig
sleep 3
screen -list
echo "To view the Sim server console,  use the command : screen -r Sim"
echo "To detach fron the console use the command : ctrl+a d  ..ctrl+a > command mode,  d > detach.."
echo
