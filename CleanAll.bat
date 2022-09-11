@echo off

echo.
echo METAART
git clean -fdx
rmdir %userprofile%\.nuget\packages\metaart /q /s
rmdir %userprofile%\.nuget\packages\metaart /q /s

echo.
echo METACORE
cd ../metacore
git clean -fdx
rmdir %userprofile%\.nuget\packages\metacore /q /s

echo.
echo METABUTTON
cd ../metabutton
git clean -fdx
cd ../metaart