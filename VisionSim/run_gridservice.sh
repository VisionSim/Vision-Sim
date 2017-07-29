#!/bin/bash
# Startup script for Vision-Sim grid server service
# Versions 1.0.4+
#
# July 2017
# Britanyann Copperfield <britanyann@imperialestates.biz>
#

cd ./bin
wait
echo Starting Vision-Sim GridServer...
screen -S Grid -d -m mono Vision.Server.exe -skipconfig
sleep 3
screen -list
echo "To view the Grid server console, use the command : screen -r Grid"
echo "To detach fron the console use the command : ctrl+a d  ..ctrl+a > command mode,  d > detach.."
echo
