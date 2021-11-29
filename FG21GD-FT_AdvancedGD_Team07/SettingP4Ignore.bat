@echo off
set /p ignoreFile="Enter name of ignore file (e.g p4ignore.txt): "
p4 set P4IGNORE="%cd%\%ignoreFile%"
set /p Exit="Done. Press Enter to Quit."