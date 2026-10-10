# Genera capturas de pantalla (PNG, estilo terminal) a partir de las salidas
# reales de la consola. Las observaciones se leen de docs/evidencias/console.
Add-Type -AssemblyName System.Drawing

$ErrorActionPreference = "Stop"
$root = "C:\Tareas NET\citasMedicas-Main\docs\evidencias"

function New-TerminalPng {
    param(
        [string]$Text,
        [string]$OutFile,
        [string]$Title
    )

    $lines = $Text -split "`r?`n"
    $font = New-Object System.Drawing.Font("Consolas", 12, [System.Drawing.FontStyle]::Regular)
    $titleFont = New-Object System.Drawing.Font("Consolas", 13, [System.Drawing.FontStyle]::Bold)

    $pad = 16
    $lineH = 20
    $charW = 8
    $maxLen = 0
    foreach ($l in $lines) { if ($l.Length -gt $maxLen) { $maxLen = $l.Length } }

    $w = [Math]::Max(720, $maxLen * $charW + 2 * $pad + 24)
    $h = $lines.Count * $lineH + $pad * 2 + 40

    $bmp = New-Object System.Drawing.Bitmap($w, $h)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
    $g.Clear([System.Drawing.Color]::FromArgb(12, 12, 24))

    function Get-Brush([int]$r, [int]$g, [int]$b) {
        New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb($r, $g, $b))
    }

    # Barra de titulo (look de ventana)
    $barBrush = Get-Brush 30 30 46
    $g.FillRectangle($barBrush, 0, 0, $w, 34)
    $titleBrush = Get-Brush 180 180 200
    $g.DrawString($Title, $titleFont, $titleBrush, $pad, 8)
    $barBrush.Dispose()

    $y = 46
    $green   = Get-Brush 86 214 120
    $gray    = Get-Brush 211 211 226
    $yellow  = Get-Brush 229 192 123
    $cyan    = Get-Brush 86 182 230
    $white   = Get-Brush 255 255 255

    foreach ($l in $lines) {
        $brush = $gray
        if ($l -match '^PS |^PS>|^C:\\') { $brush = $green }
        elseif ($l -match '^\[|^[0-9]{2}:[0-9]{2}.*INF|^Sending ') { $brush = $yellow }
        elseif ($l -match '^--|^====|^###') { $brush = $cyan }
        elseif ($l -match '^RESULTADO|^ERROR|^Cliente|^PASO') { $brush = $white }
        $g.DrawString($l, $font, $brush, $pad, $y)
        $y += $lineH
    }

    $green.Dispose(); $gray.Dispose(); $yellow.Dispose(); $cyan.Dispose(); $white.Dispose()
    $g.Dispose()
    $bmp.Save($OutFile, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Output "PNG generado: $OutFile"
}

$capturas = "$root\capturas"
if (-not (Test-Path $capturas)) { New-Item -ItemType Directory -Path $capturas | Out-Null }

# 01 - Servidores corriendo
$t1 = Get-Content "$root\console\01-servidores-corriendo.txt" -Raw -Encoding UTF8
New-TerminalPng -Text $t1 -OutFile "$capturas\01-servidores-running.png" -Title "Evidencia 1 - Microservicios ejecutandose (Addresses 5310 / Customers 5320)"

# 02 - RestClient creando los clientes (libreria NugetPackage_Rest)
$t2 = Get-Content "$root\console\02-restclient-clientes.txt" -Raw -Encoding UTF8
New-TerminalPng -Text $t2 -OutFile "$capturas\02-restclient-clientes.png" -Title "Evidencia 2 - Creacion de clientes con la libreria REST (IRest.Post)"

# 03 - Migraciones
$t3 = Get-Content "$root\console\03-migraciones.txt" -Raw -Encoding UTF8
New-TerminalPng -Text $t3 -OutFile "$capturas\03-migraciones.png" -Title "Evidencia 3 - Migraciones aplicadas (EF Core database update)"

# 04 - Datos de las tablas
$t4 = Get-Content "$root\console\04-datos-tablas.txt" -Raw -Encoding UTF8
New-TerminalPng -Text $t4 -OutFile "$capturas\04-datos-tablas.png" -Title "Evidencia 4 - Datos de prueba en la base de datos ClinicaDB"

# 05 - Clientes con LocationId (JOIN)
$t5 = Get-Content "$root\console\05-clientes-locationid.txt" -Raw -Encoding UTF8
New-TerminalPng -Text $t5 -OutFile "$capturas\05-clientes-locationid.png" -Title "Evidencia 5 - Clientes creados y su referencia LocationId"

Write-Output "Todas las capturas generadas."