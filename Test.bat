@echo off

echo.
echo METACORE
cd ..\MetaCore\src
dotnet test

echo.
echo METAART
cd ..\..\MetaArt\MetaArt
dotnet test

echo.
echo METABUTTON
cd ..\..\MetaButton\src
dotnet test

echo.
echo METAART_VIEWERS
cd ..\..\MetaArt\MetaArt.Viewers
dotnet test MetaArt.Viewers.sln

cd ..\..\MetaArt
