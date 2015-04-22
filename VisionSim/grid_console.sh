#!/bin/bash
# Start Vision-Sim grid server only
# Versions 0.9.2+
#
# May 2014
# greythane @ gmail.com

cd ./bin
wait
echo Starting Vision Grid Server...
mono Vision.Server.exe -skipconfig

