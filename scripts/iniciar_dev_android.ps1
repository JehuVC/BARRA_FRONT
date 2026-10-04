# Conecta el emulador (o el telefono por USB) con la API local que corre en IIS Express.
#
# El front en Android llama a http://localhost:54346/. En Android "localhost" es el propio
# telefono, asi que este script reenvia ese puerto a la PC con "adb reverse".
#
# Cuando correrlo: cada vez que se arranca el emulador (el reenvio se pierde al reiniciarlo)
# y cada vez que se conecta el telefono por USB. No necesita permisos de administrador.
#
# Uso (desde la carpeta BARRA_FRONT):   .\scripts\iniciar_dev_android.ps1
#   Otro puerto:                        .\scripts\iniciar_dev_android.ps1 -Puerto 54346

param([int]$Puerto = 54346)

$adb = $null
$raices = @($env:ANDROID_HOME, $env:ANDROID_SDK_ROOT, "${env:ProgramFiles(x86)}\Android\android-sdk", "$env:LOCALAPPDATA\Android\Sdk")
foreach ($raiz in $raices) {
    if ($raiz -and (Test-Path "$raiz\platform-tools\adb.exe")) { $adb = "$raiz\platform-tools\adb.exe"; break }
}
if (-not $adb) {
    $cmd = Get-Command adb -ErrorAction SilentlyContinue
    if ($cmd) { $adb = $cmd.Source }
}
if (-not $adb) {
    Write-Host "No encontre adb.exe. Instala el Android SDK desde Visual Studio (Tools > Android > Android SDK Manager)" -ForegroundColor Red
    Write-Host "o define la variable de entorno ANDROID_HOME con la carpeta del SDK." -ForegroundColor Red
    exit 1
}

$dispositivos = @(& $adb devices | Select-Object -Skip 1 | Where-Object { $_ -match "\tdevice$" } | ForEach-Object { ($_ -split "\t")[0] })

if ($dispositivos.Count -eq 0) {
    Write-Host "No hay ningun emulador o telefono listo." -ForegroundColor Yellow
    Write-Host "  - Arranca el emulador y espera a ver la pantalla de inicio de Android (el primer arranque tarda varios minutos)."
    Write-Host "  - Si usas telefono: activa la depuracion USB y acepta el aviso en el telefono."
    Write-Host "  - Si aparece como 'offline' en 'adb devices', esperalo un momento y vuelve a correr este script."
    exit 1
}

foreach ($d in $dispositivos) {
    & $adb -s $d reverse "tcp:$Puerto" "tcp:$Puerto" | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "OK: $d  ->  localhost:$Puerto del dispositivo llega a localhost:$Puerto de esta PC" -ForegroundColor Green
    }
    else {
        Write-Host "FALLO el reenvio en $d" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "Recuerda: la API (API_4) debe estar corriendo con F5 en Visual Studio y SQL Server encendido."
