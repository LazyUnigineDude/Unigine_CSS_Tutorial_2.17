@echo off
setlocal EnableDelayedExpansion
set APP="bin\Editor_double_x64.exe"
if exist %APP% (
	start "" %APP% $(COMMAND_LINE)
) else (
	set MESSAGE=%APP% not found"
	echo !MESSAGE!
)