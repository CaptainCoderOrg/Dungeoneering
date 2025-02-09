set mypath=%cd%
set editor="D:\Unity  Editors\6000.0.34f1\Editor\Unity.exe"
@echo %mypath%
"D:\Unity  Editors\6000.0.34f1\Editor\Unity.exe" -quit -batchmode -logFile stdout.log -projectPath "Dungeoneering/" -executeMethod WebGLBuilder.Build