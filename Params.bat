@CD /D "%~dp0"
@SET OUTPUT_DIR=%CD%\Bin\Release
@SET DEPLOY_DIR=%LOCALAPPDATA%\BeerRater
@SET SOLUTION_FILE="%CD%\BeerRater.sln"
@set LOG_FILE=Release.log

@GOTO %PROCESSOR_ARCHITECTURE%

:AMD64
@SET NET_FRAMEWORK="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
@GOTO END

:X86
@SET NET_FRAMEWORK="%ProgramFiles%\MSBuild\14.0\Bin\MSBuild.exe"

:END
