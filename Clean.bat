@echo off

echo.
echo METACORE
cd ..\MetaCore\src
dotnet clean -clp:ErrorsOnly --configuration Debug
dotnet clean -clp:ErrorsOnly --configuration Release

echo.
echo METAART
cd ..\..\MetaArt\MetaArt
dotnet clean -clp:ErrorsOnly --configuration Debug
dotnet clean -clp:ErrorsOnly --configuration Release

echo.
echo META3D
cd ..\Meta3D
dotnet clean -clp:ErrorsOnly --configuration Debug
dotnet clean -clp:ErrorsOnly --configuration Release

echo.
echo METABUTTON
cd ..\..\MetaButton\src
dotnet clean -clp:ErrorsOnly --configuration Debug
dotnet clean -clp:ErrorsOnly --configuration Release

echo.
echo METAART_VIEWERS
cd ..\..\MetaArt\MetaArt.Viewers
dotnet clean -clp:ErrorsOnly --configuration Debug MetaArt.Viewers.sln
dotnet clean -clp:ErrorsOnly --configuration Release MetaArt.Viewers.sln

cd ..\..\MetaArt

rmdir %userprofile%\.nuget\packages\metacore /q /s
rmdir %userprofile%\.nuget\packages\metaart /q /s
rmdir %userprofile%\.nuget\packages\metaart /q /s