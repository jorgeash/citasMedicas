# Construye el documento HTML de evidencias (portada + analisis + capturas)
# y lo convierte a PDF con Microsoft Edge headless.
$ErrorActionPreference = "Stop"
$root = "C:\Tareas NET\citasMedicas-Main\docs\evidencias"

function Get-ImgDataUri([string]$path) {
    $bytes = [System.IO.File]::ReadAllBytes($path)
    "data:image/png;base64," + [System.Convert]::ToBase64String($bytes)
}

$img1 = Get-ImgDataUri "$root\capturas\01-servidores-running.png"
$img2 = Get-ImgDataUri "$root\capturas\02-restclient-clientes.png"
$img3 = Get-ImgDataUri "$root\capturas\03-migraciones.png"
$img4 = Get-ImgDataUri "$root\capturas\04-datos-tablas.png"
$img5 = Get-ImgDataUri "$root\capturas\05-clientes-locationid.png"

$html = @"
<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="utf-8">
<title>Evidencias - Citas Medicas (Customers / Addresses)</title>
<style>
  @page { size: A4; margin: 14mm; }
  * { box-sizing: border-box; }
  body { font-family: 'Segoe UI', Arial, sans-serif; color: #222; font-size: 11pt; margin: 0; }
  .page { page-break-after: always; }
  .page:last-child { page-break-after: auto; }

  /* Portada */
  .cover { text-align: center; padding-top: 120px; }
  .cover h1 { font-size: 26pt; color: #0b5394; margin-bottom: 4px; }
  .cover h2 { font-size: 15pt; color: #555; font-weight: normal; }
  .cover .box { border: 2px solid #0b5394; border-radius: 10px; padding: 22px; margin: 40px 60px 0; text-align: left; }
  .cover .box p { margin: 8px 0; }
  .cover .tag { color: #0b5394; font-weight: bold; }

  /* Secciones de analisis */
  h2.sec { color: #0b5394; border-bottom: 2px solid #0b5394; padding-bottom: 4px; font-size: 15pt; margin-top: 4px; }
  h3 { color: #134f5c; font-size: 12pt; margin-bottom: 4px; }
  ul { margin: 6px 0 10px 18px; }
  li { margin-bottom: 4px; }
  .code { font-family: Consolas, monospace; background: #eee; padding: 1px 5px; border-radius: 3px; font-size: 9.5pt; }
  .warn { background: #fff3cd; border-left: 4px solid #f0ad4e; padding: 8px 10px; margin: 8px 0; border-radius: 4px; }

  /* Evidencias */
  .evi { text-align: center; }
  .evi .cap { font-weight: bold; color: #0b5394; margin: 6px 0 2px; }
  .evi .desc { color: #555; font-size: 9.5pt; margin-bottom: 8px; }
  .evi img { border: 1px solid #bbb; border-radius: 6px; box-shadow: 2px 2px 6px rgba(0,0,0,.15); }
  .wide  { width: 100%; }
  .narrow { width: 72%; }
  .mid   { width: 88%; }
</style>
</head>
<body>
<!-- PORTADA -->
<div class="page cover">
  <h1>Levantamiento, Mantenimiento y Continuidad de Proyectos</h1>
  <h2>Microservicios Customers y Addresses - Integracion con la biblioteca de clases REST</h2>
  <div class="box">
    <p><span class="tag">Actividad:</span> Analisis, correccion, migracion y verificacion de dos microservicios.</p>
    <p><span class="tag">Microservicios:</span> Customers (Business.Api, puerto 5320) y Addresses (Addresses.Api, puerto 5310).</p>
    <p><span class="tag">Biblioteca REST:</span> NugetPackage_Rest (IRest / RestBuilder) - creacion de clientes estrictamente con sus metodos.</p>
    <p><span class="tag">Base de datos:</span> SQL Server LocalDB - ClinicaDB (migraciones EF Core aplicadas + datos de prueba).</p>
    <p><span class="tag">Resultado:</span> Clientes CLI-0002 y CLI-0003 creados via REST con referencia a su LocationId.</p>
  </div>
  <p style="margin-top:60px; color:#888;">Octubre 2026</p>
</div>

<!-- ANALISIS 1: POR QUE NO COMPILABA -->
<div class="page">
  <h2 class="sec">1. Determinacion: el microservicio original no compilaba ni se ejecutaba</h2>
  <p>Se clono la solucion completa y se ejecuto <span class="code">dotnet build</span> sobre los proyectos originales (<span class="code">Downloads/Customers</span> y <span class="code">Downloads/Addresses</span>). Ambos fallaron por las mismas causas raiz:</p>

  <h3>1.1 Referencias de proyecto inexistentes (la principal)</h3>
  <p>Los proyectos apuntaban a una carpeta <span class="code">Utils</span> externa que no fue entregada con la solucion:</p>
  <ul>
    <li><span class="code">Customers\Business.Infraestructure</span> referencia <span class="code">..\..\..\Utils\T-Rest\T-Rest.csproj</span></li>
    <li><span class="code">Addresses.Core</span> y <span class="code">Customers.Business.Core</span> referencian <span class="code">..\..\..\Utils\Common\Common.csproj</span></li>
  </ul>
  <div class="warn"><b>Warning MSB9008</b>: "<i>el proyecto al que se hace referencia no existe</i>" y en consecuencia cientos de errores <b>CS0246</b> (<span class="code">Common</span>, <span class="code">FluentValidation</span>, <span class="code">MediatR</span> no encontrados) y <b>CS0234</b> (<span class="code">Microsoft.AspNetCore.JsonPatch</span>, <span class="code">System.Linq.Dynamic</span>).</div>

  <h3>1.2 Paquetes NuGet no declarados</h3>
  <p>El codigo usaba <span class="code">FluentValidation</span>, <span class="code">MediatR</span>, <span class="code">JsonPatch</span> y <span class="code">System.Linq.Dynamic</span> sin declararlos en el <span class="code">.csproj</span>, por lo que las clases no se resolvian.</p>

  <h3>1.3 Framework desactualizado e incompatible</h3>
  <p>Los proyectos originales apuntaban a <span class="code">net6.0</span>, mientras el repositorio (slnx, <span class="code">Shared.Kernel</span>, <span class="code">NugetPackage_Rest</span>) usa <span class="code">net9.0</span>. Esto impedia reutilizar las bibliotecas comunes del workspace.</p>

  <h3>1.4 Correcciones aplicadas</h3>
  <ul>
    <li>Se eliminaron las dependencias rotas <span class="code">Utils\Common</span> y <span class="code">Utils\T-Rest</span>.</li>
    <li>Se reemplazaron por la biblioteca <span class="code">NugetPackage_Rest</span> del workspace y por <span class="code">Shared.Kernel</span>.</li>
    <li>Se migraron todos los proyectos a <span class="code">net9.0</span> y se declararon los paquetes NuGet necesarios.</li>
    <li>Se regeneraron las migraciones EF Core y se reescribieron los <span class="code">Program.cs</span>/<span class="code">Startup.cs</span> obsoletos.</li>
  </ul>
  <p><b>Verificacion:</b> ambos proyectos compilan sin errores y se ejecutan en los puertos 5310 y 5320 (ver Evidencia 1).</p>
</div>

<!-- ANALISIS 2: ESTRUCTURA -->
<div class="page">
  <h2 class="sec">2. Estructura de los microservicios</h2>
  <p>Ambos microservicios siguen una arquitectura en capas (Domain - Core - Infraestructure - Persistence - Api) y comparten <span class="code">Shared.Kernel</span> (entidades base, repositorios) y la biblioteca <span class="code">NugetPackage_Rest</span>.</p>

  <h3>2.1 Addresses - registro de direcciones (Countries, States, Cities, Positions)</h3>
  <ul>
    <li><b>Domain</b>: entidades <span class="code">Country</span>, <span class="code">State</span>, <span class="code">City</span>, <span class="code">Position</span> (hereda de <span class="code">BaseEntity</span>).</li>
    <li><b>Core</b>: DTOs, Features CQRS (Command/Query/Validators) por entidad, interfaces de repositorio y <span class="code">DependencyInjection</span>.</li>
    <li><b>Persistence</b>: <span class="code">AddressesDbContext</span>, configuraciones Fluent API con datos de prueba (Seed), repositorios y la migracion <span class="code">20261006010523_InitialAddresses</span>.</li>
    <li><b>Api</b>: controllers REST (<span class="code">PositionsController</span>, <span class="code">CitiesController</span>, etc.), <span class="code">DomainExceptionMiddleware</span> y Scalar UI en <span class="code">http://localhost:5310</span>.</li>
  </ul>
  <p><b>Funcionalidad:</b> la tabla <span class="code">Addresses_Position</span> actua como catalogo de direcciones (LocationId) para los clientes. Provee GET/POST paginados sobre el catalogo.</p>

  <h3>2.2 Customers - gestion de clientes y su LocationId</h3>
  <ul>
    <li><b>Domain</b>: entidades <span class="code">Customer</span>, <span class="code">CustomerContact</span>, <span class="code">CustomerType</span>, <span class="code">Company</span>.</li>
    <li><b>Core</b>: DTOs (incluye <span class="code">LocationSummary</span> externo), Features CQRS, interfaces y configuracion <span class="code">ApiAddressesOptions</span> (URL base del microservicio Addresses).</li>
    <li><b>Infraestructure</b>: <span class="code">LocationService</span>, que consume el microservicio Addresses mediante la libreria <span class="code">NugetPackage_Rest</span> (<span class="code">IRest.Get</span>).</li>
    <li><b>Persistence</b>: <span class="code">BusinessDbContext</span>, configuraciones con Seed, repositorios y la migracion <span class="code">20261006010620_InitialBusiness</span>.</li>
    <li><b>Api</b>: <span class="code">CustomersController</span> (POST/GET de clientes, validando el LocationId contra Addresses), otros controllers de catalogos y Scalar UI en <span class="code">http://localhost:5320</span>.</li>
    <li><b>RestClient</b>: cliente de la actividad que crea clientes usando solo los metodos de <span class="code">NugetPackage_Rest</span>.</li>
  </ul>

  <h3>2.3 Integracion con la biblioteca de clases REST</h3>
  <p>La biblioteca <span class="code">NugetPackage_Rest</span> expone <span class="code">IRest</span> con builders fluidos (<span class="code">Get</span>/<span class="code">Post</span>, <span class="code">WithoutAuth</span>, <span class="code">WithBody</span>, <span class="code">WithQuery</span>, <span class="code">DeserializeWithAsync&lt;T&gt;</span>). Se registra con <span class="code">AddHttpClient&lt;IRest, RestBuilder&gt;</span> y representa la evolucion del <span class="code">Utils\T-Rest</span> que faltaba en la solucion original.</p>
</div>

<!-- EVIDENCIAS -->
<div class="page evi">
  <div class="cap">Evidencia 1 - Integracion y funcionamiento de los microservicios</div>
  <div class="desc">Addresses.Api (5310) y Business.Api (5320) iniciados y respondiendo HTTP. GET a /api/positions y /api/customers.</div>
  <img class="narrow" src="$img1" />
</div>

<div class="page evi">
  <div class="cap">Evidencia 2 - Creacion de los clientes con la biblioteca REST</div>
  <div class="desc">Business.RestClient usa IRest.Post de NugetPackage_Rest para crear CLI-0002 (LocationId 1) y CLI-0003 (LocationId 3). Se muestran los logs "Sending POST request" de la libreria y las respuestas 201.</div>
  <img class="wide" src="$img2" />
</div>

<div class="page evi">
  <div class="cap">Evidencia 3 - Ejecucion de las migraciones</div>
  <div class="desc">dotnet ef database update sobre Addresses y Customers: la base ClinicaDB ya esta al dia con las migraciones 20261006010523_InitialAddresses y 20261006010620_InitialBusiness.</div>
  <img class="wide" src="$img3" />
</div>

<div class="page evi">
  <div class="cap">Evidencia 4 - Datos de prueba registrados en la base de datos</div>
  <div class="desc">Contenido de las tablas Addresses_Country, Addresses_State, Addresses_City, Addresses_Position, Business_Company, Business_CustomerType y Business_Customer.</div>
  <img class="mid" src="$img4" />
</div>

<div class="page evi">
  <div class="cap">Evidencia 5 - Clientes creados y su referencia LocationId</div>
  <div class="desc">JOIN de Business_Customer con Addresses_Position (c.Id = c.LocationId) confirmando que CLI-0002 -> LocationId 1 y CLI-0003 -> LocationId 3 estan persistidos en ClinicaDB.</div>
  <img class="wide" src="$img5" />
</div>

</body>
</html>
"@ | Out-String

$htmlFile = "$root\evidencias.html"
[System.IO.File]::WriteAllText($htmlFile, $html, [System.Text.Encoding]::UTF8)
Write-Output "HTML generado: $htmlFile"

$edge = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
$pdf = "$root\evidencias_clinica.pdf"
$profile = "$env:TEMP\opencode\edgeprofile"
$arg = "--headless=new --disable-gpu --no-first-run --user-data-dir=`"$profile`" --print-to-pdf=`"$pdf`" file:///$($htmlFile.Replace('\','/'))"
Start-Process -FilePath $edge -ArgumentList $arg -Wait -WindowStyle Hidden
if (Test-Path $pdf) { Write-Output "PDF generado: $pdf ($((Get-Item $pdf).Length) bytes)" } else { Write-Output "ERROR: el PDF no se genero." }