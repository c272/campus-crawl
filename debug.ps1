$externalPath = ".\tileEngine\Player\tileEngine.Player\bin\Debug\external"
$compiledPath = ".\tileEngine\Player\tileEngine.Player\bin\Debug\compiled"
$mainBinPath = ".\tileEngine\Player\tileEngine.Player\bin\Debug\main.bin"

echo "Removing old compiled code from TileEngine Player."
if (Test-Path $externalPath) {
    Remove-Item -Recurse $externalPath
}
if (Test-Path $compiledPath) {
    Remove-Item -Recurse $compiledPath
}
if (Test-Path $mainBinPath) {
    Remove-Item $mainBinPath
}

echo "Adding up-to-date compiled code to TileEngine Player."

Copy-Item .\bin\external .\tileEngine\Player\tileEngine.Player\bin\Debug -Recurse -Force
Copy-Item .\bin\compiled .\tileEngine\Player\tileEngine.Player\bin\Debug -Recurse -Force
Copy-Item .\bin\main.bin .\tileEngine\Player\tileEngine.Player\bin\Debug -Force

echo "Done; you can now run TileEngine.Player"