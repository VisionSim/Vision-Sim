DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd ${DIR}/../
mkdir -p VisionSim/VisionDocs/doxygen
rm -fr VisionSim/VisionDocs/doxygen/*
doxygen Documentation/doxygen.conf