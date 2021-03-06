# Vision Sim

The Vision Development Team has begun its own branch of Aurora-sim/ VisionSim virtual world server code.

The Vision server is an OpenSim/Aurora-Sim derived project with heavy emphasis on supporting all users, 
increased technology focus, heavy emphasis on working with other developers,
whether it be viewer based developers or server based developers, 
and a set of features around stability and simplified usability for users.

It is advised to Linux users to use a Mono version higher then 3.2.8, following a report about GC.Collect() not cleaning up memory correctly. The most current version of Mono is 4.0.1 (Released 28th April 2015)*

## Configuration
*To see how to configure Vision, look at "Setting up VisionSim.txt" in the VisionDocs folder for more information*

Windows:
   Run the 'runprebuild.bat' file.
   This will check you current system configuration, compile the correct Visual Studio 2010 soultion and project files and prompt you to build immediately (if desired)

*nix:      (Also OSX)
   Execute the 'runprebuild.sh' form a terminal or console shell.
   You will be prompted for your desired configuration, the appropriate solution and project files for Mono will be compiled and finally, prompt you to build immediately (if desired)
   
OSX:
   Run the 'runprebuild.command' shell command by 'double clicking' in Finder.
   You will be prompted for your desired configuration, the appropriate solution and project files for Mono will be compiled and finally, prompt you to build immediately (if desired)

## Compiling Vision-Sim

*To compile Vision, look at the Compiling.txt in the VisionDocs folder for more information*

## Vision-Sim Build Status

Windows .Net 4.5 [![Build status](https://ci.appveyor.com/api/projects/status/a90lejf562n9sxwy?svg=true)](https://ci.appveyor.com/project/britanyanncopperfield/vision-sim)

Linux 64 Bit [![Build Status](https://travis-ci.org/VisionSim/Vision-Sim.svg?branch=master)](https://travis-ci.org/VisionSim/Vision-Sim)

Pull requests [![Issue Stats](http://www.issuestats.com/github/VisionSim/Vision-Sim/badge/pr)](http://www.issuestats.com/github/VisionSim/Vision-Sim)

Issues closed [![Issue Stats](http://www.issuestats.com/github/VisionSim/Vision-Sim/badge/issue)](http://www.issuestats.com/github/VisionSim/Vision-Sim)

## Configuration

*To see how to configure Vision, look at "Setting up Vision.txt" in the VisionDocs folder for more information*

## Router issues
If you are having issues logging into your simulator, take a look at http://forums.osgrid.org/viewtopic.php?f=14&t=2082 in the Router Configuration section for more information on ways to resolve this issue.

## Support
Support is available from various sources.

* IRC channel #Vision-support on freenode (http://webchat.freenode.net?channels=%23Vision-support)

* Check out https://github.com/VisionSim/Vision-Sim for the latest developments, downloads and forum