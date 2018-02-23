@echo off

:CountLines
setlocal
set /a totalNumLines = 0
for /r %1 %%F in (*.cs) do (
  for /f %%N in ('find /v /c "" ^<"%%F"') do set /a totalNumLines+=%%N
)

echo Total lines: %totalNumLines%
pause >nul