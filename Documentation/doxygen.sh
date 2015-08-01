#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $DIR/../
mkdir -p VisionSim/WhiteCoreDocs/doxygen
rm -fr VisionSim/WhiteCoreDocs/doxygen/*
doxygen Documentation/doxygen.conf
