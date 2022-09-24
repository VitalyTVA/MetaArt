@echo off

echo.
echo METACORE
cd ..\MetaCore\src
dotnet build -clp:ErrorsOnly --configuration Debug

echo.
echo METAART
cd ..\..\MetaArt\MetaArt
dotnet build -clp:ErrorsOnly --configuration Debug

echo.
echo META3D
cd ..\Meta3D
dotnet build -clp:ErrorsOnly --configuration Debug

echo.
echo METABUTTON
cd ..\..\MetaButton\src
dotnet build -clp:ErrorsOnly --configuration Debug

echo.
echo METAART_VIEWERS
cd ..\..\MetaArt\MetaArt.Viewers
dotnet build -clp:ErrorsOnly --configuration Debug MetaArt.Viewers.sln

cd ..\..\MetaArt
