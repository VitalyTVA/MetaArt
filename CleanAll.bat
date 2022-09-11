@echo off

echo.
echo METACORE
cd ..\MetaCore\src
dotnet clean --configuration Debug
dotnet clean --configuration Release

echo.
echo METAART
cd ..\..\MetaArt\MetaArt
dotnet clean --configuration Debug
dotnet clean --configuration Release

echo.
echo METABUTTON
cd ..\..\MetaButton\src
dotnet clean --configuration Debug
dotnet clean --configuration Release

echo.
echo METAART_VIEWERS
cd ..\..\MetaArt\MetaArt.Viewers
dotnet clean --configuration Debug MetaArt.Viewers.sln
dotnet clean --configuration Release MetaArt.Viewers.sln

cd ..\..\MetaArt

rmdir %userprofile%\.nuget\packages\metacore /q /s
rmdir %userprofile%\.nuget\packages\metaart /q /s
rmdir %userprofile%\.nuget\packages\metaart /q /s