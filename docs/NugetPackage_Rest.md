# NugetPackage_Rest

Cliente HTTP fluido para la comunicación entre microservicios .NET. Construido con la guía
*NugetPackage_Rest — Construcción de un cliente HTTP fluido para microservicios .NET*
(paquete `NugetPackageRest`, versión **2.2.0**, .NET 9, C#) e integrado en el microservicio
**Billing.Service** del sistema de gestión clínica.

- **Guía completa:** `guia_rest.pdf` (raíz del repositorio)
- **Feed local del paquete:** `NugetPackage_Rest/local-feed/`
- **`nuget.config`:** en la raíz del repositorio (registra el feed `LocalNuget`)

---

## Estructura

```
NugetPackage_Rest/                        ← solución independiente de citasMedicas.slnx
├── NugetPackage_Rest.sln
├── local-feed/
│   └── NugetPackageRest.2.2.0.nupkg      ← feed local (Opción A de la guía)
├── NugetPackage_Rest/                    ← la librería (se empaqueta)
│   ├── NugetPackage_Rest.csproj
│   ├── Builders/
│   │   ├── HttpRequestExecutor.cs        (internal: envío + traducción de errores)
│   │   ├── NotContentRequestBuilder.cs   (internal: cadena GET)
│   │   ├── WithContentRequestBuilder.cs  (internal: cadena POST/PUT/PATCH/DELETE)
│   │   └── RestBuilder.cs                (pública: implementa IRest)
│   ├── Configs/RequestSettings.cs
│   ├── Exceptions/
│   │   ├── ApiException.cs
│   │   └── ApiFailureReason.cs
│   ├── Extensions/
│   │   ├── RequestExtensions.cs
│   │   └── ServiceCollectionExtension.cs (AddRequestLogging)
│   └── Interfaces/
│       ├── IFluents/   (IFluentContent, IFluentFormat, IFluentAuth<TNext>, IFluentResponse)
│       ├── IRequests/  (INotContentRequest, IWithContentRequest)
│       └── IServices/  (IRest)
└── NugetPackage_Rest.Tests/              ← pruebas xUnit (17 pruebas, no se empaqueta)
```

## La cadena fluida

Cada etapa devuelve una interfaz distinta que solo expone los métodos válidos a continuación;
el orden correcto lo garantiza el compilador.

| Etapa | Interfaz | Métodos | Lleva a |
|---|---|---|---|
| 0. Verbo | `IRest` | `Get`, `Post`, `Put`, `Patch`, `Delete` | `INotContentRequest` / `IWithContentRequest` |
| 1. Autenticación | `INotContentRequest` / `IWithContentRequest` | `WithoutAuth()`, `WithBearer(token)`, `WithBasic(user, pwd)` | `IFluentAuth<TNext>` |
| 2. Headers y URI | `IFluentAuth<TNext>` | `WithHeaders(...)` (repetible), `WithUri(base, endpoint)` | GET: `IFluentContent`; resto: `IFluentFormat` |
| 3. Body | `IFluentFormat` | `WithBody`, `WithFormUrlEncoded`, `WithFormData`, `WithoutBody` | `IFluentContent` |
| 4. Ejecutar | `IFluentContent` | `DeserializeWithAsync<T>()`, `GetContentAsStringAsync()`, `GetContentAsByteArrayAsync()`, `await` directo | Resultado o `ApiException` |

```csharp
var response = await _rest.Post
    .WithoutAuth()
    .WithUri(_options.BaseUrl, "/api/Pagos/procesar")
    .WithBody(new { facturaId, monto, moneda })
    .DeserializeWithAsync<HttpResponse<string>>();
```

## Manejo de errores

Toda falla se convierte en **una única** `NugetPackage_Rest.Exceptions.ApiException`:

| `Reason` | Cuándo |
|---|---|
| `Network` | `HttpRequestException`: conexión rechazada, DNS, servicio caído |
| `Timeout` | `TaskCanceledException`: se agotó `HttpClient.Timeout` |
| `HttpError` | Respuesta con código no 2xx (lleva `StatusCode` y `ResponseBody`) |
| `Deserialization` | 2xx pero el JSON no se pudo convertir a `T` |
| `Unknown` | Cualquier otro fallo al enviar o leer |

`ApiException` expone además `RequestMethod`, `RequestUri` e `InnerException`.

> **Ojo con el `await` directo** sobre la cadena: devuelve el `HttpResponseMessage` crudo
> (aunque sea 404/500) y se salta el manejo de errores y el log. Usa siempre un método terminal.

---

## Build, pruebas y empaquetado

```bash
# Compilar (GeneratePackageOnBuild genera también el .nupkg)
dotnet build NugetPackage_Rest/NugetPackage_Rest.csproj

# Pruebas (17)
dotnet test NugetPackage_Rest/NugetPackage_Rest.sln

# Empaquetar en Release hacia el feed local
dotnet pack NugetPackage_Rest/NugetPackage_Rest.csproj -c Release \
    -o NugetPackage_Rest/local-feed -p:GeneratePackageOnBuild=false
```

> `p:GeneratePackageOnBuild=false` es obligatorio: sin él, `dotnet pack` falla con **NU5026**
> porque el `.dll` ya fue empaquetado por `GeneratePackageOnBuild`.

### Después de cambiar la librería (paso 28 de la guía)

1. Actualizar `<Version>` en `NugetPackage_Rest.csproj` siguiendo SemVer
   (2.2.1 correcciones · 2.3.0 funciones nuevas compatibles · 3.0.0 breaking changes).
2. `dotnet test` → debe quedar en verde.
3. Repetir `dotnet pack` y **limpiar la caché** si no cambió la versión:
   `dotnet nuget locals all --clear` (o borra `~/.nuget/packages/nugetpackagerest`).
4. En cada consumidor: `dotnet add package NugetPackageRest --version <nueva>`.

---

## Feed local (cómo consume Billing el paquete)

`nuget.config` (raíz del repositorio):

```xml
<packageSources>
  <clear />
  <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  <add key="LocalNuget" value="NugetPackage_Rest/local-feed" />
</packageSources>
```

La ruta es **relativa al `nuget.config`**, así funciona igual en Windows y en WSL/Linux.
Los `.nupkg` están en `.gitignore`: cada máquina genera su feed con `dotnet pack`.

---

## Integración en Billing.Service (Parte II de la guía)

La regla de la arquitectura en capas se respeta: **solo `Billing.Infrastructure` conoce la
librería**; `Billing.Core` define contratos en términos de negocio y `Billing.Api` solo
configura.

```
PagosController (API)
   └─► MediatR: ProcesarPagoCommandHandler (Core)
          └─► IPagoService.ProcesarPagoAsync(...)      ← interfaz en Core
                 └─► PagoService (Infrastructure)      ← implementación
                        └─► IRest.Post...WithBody(...) ← NugetPackageRest
                               └─► HTTP POST → Pasarela /api/Pagos/procesar
```

### Archivos nuevos

| Proyecto | Archivo | Contenido |
|---|---|---|
| Core | `Configs/PasarelaOptions.cs` | `BaseUrl`, `ClientId`, `ClientSecret`, `MaxReintentos` |
| Core | `Services/IPagoService.cs` | Contrato `ProcesarPagoAsync(...)` |
| Core | `Exceptions/DomainException.cs` | `Errores { PAGO_FALLIDO = 601, ... }` |
| Core | `Features/Billing/Pagos/Commands/ProcesarPago/ProcesarPagoCommand.cs` | Comando MediatR + handler |
| Infrastructure | `Services/PagoService.cs` | Implementación `internal` que inyecta `IRest` |
| Infrastructure | `Extensions/Extension.cs` | `AddInfrastructure(IConfiguration)` |
| Api | `Controllers/PagosController.cs` | `POST api/Pagos/procesar` |
| Tests | `Fakes/FakeHttpMessageHandler.cs`, `Tests/*.cs` | 6 pruebas |

### Archivos modificados

- `Billing.Core/DependencyInjection.cs` → sobrecarga `AddApplication(IConfiguration)` que
  enlaza `PasarelaOptions` (Patrón Options).
- `Billing.Core/Billing.Core.csproj` → paquete `Microsoft.Extensions.Options.ConfigurationExtensions`.
- `Billing.Api/appsettings.json` → secciones `Pasarela`, `RestSettings` y `Serilog`.
- `Billing.Api/Program.cs` → Serilog + `AddApplication(configuration)` + `AddInfrastructure(configuration)`.
- `Billing.Api/Billing.Api.csproj` → referencias a `Billing.Infrastructure` y `Serilog.AspNetCore`.
- `citasMedicas.slnx` → proyectos `Billing.Infrastructure` y `Billing.Tests`.

### Registro en DI (`Billing.Infrastructure/Extensions/Extension.cs`)

```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    services.AddRequestLogging(configuration);                 // RestSettings → IOptions<RequestSettings>
    services.AddHttpClient<IRest, RestBuilder>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);             // vencer → ApiException(Timeout)
    });
    services.AddScoped<IPagoService, PagoService>();
    return services;
}
```

`AddHttpClient<IRest, RestBuilder>()` registra la librería como **typed client** de
`IHttpClientFactory` (pool de handlers, rotación de DNS, sin agotamiento de sockets).
`Microsoft.Extensions.Http` es obligatorio en `Billing.Infrastructure`: la librería no lo trae.

### Configuración (`Billing.Api/appsettings.json`)

```json
"Pasarela": {
  "BaseUrl": "https://api.pasarela.example",
  "ClientId": "billing-demo",
  "ClientSecret": "",
  "MaxReintentos": 2
},
"RestSettings": {
  "EnableRequestLogs": false
}
```

> `EnableRequestLogs: true` escribe el **body completo** de cada request en Serilog
> (contraseñas incluidas si van en el body): déjalo en `false` en producción.
> El `ClientSecret` debe venir de *user-secrets* o variables de entorno, nunca del repositorio.

### Traducción de errores de técnico a negocio

El handler de Core **no** referencia la librería: captura `Exception` y la convierte en
`DomainException` con código propio, para que la API responda 400 con código de negocio
en lugar de un 500 genérico.

---

## Pruebas

| Proyecto | Pruebas | Resultado |
|---|---|---|
| `NugetPackage_Rest.Tests` | 17 (ApiException 2 · Executor 5 · GET 5 · POST 5) | ✅ 17/17 |
| `Billing.Tests` | 6 (PagoService 3 · Handler 3) | ✅ 6/6 |

Las pruebas no usan red: `FakeHttpMessageHandler` sustituye el `HttpMessageHandler` y
devuelve respuestas fabricadas o lanza excepciones.

```bash
dotnet test NugetPackage_Rest/NugetPackage_Rest.sln    # 17/17
dotnet test src/Billing.Service/Billing.Tests/Billing.Tests.csproj   # 6/6
```

---

## Desviaciones respecto a la guía

Decisiones tomadas para encajar la guía en este repositorio (todas verificadas):

1. **Firma fuerte de ensamblajes.** `Directory.Build.props` de la raíz aplica
   `SignAssembly=true` con `key.snk` a todos los proyectos. Por eso el
   `InternalsVisibleTo` de la librería y de `Billing.Infrastructure` lleva la clave pública
   completa (`PublicKey=00240000...`), exigida por **CS1726**. Sin esta línea el build no
   compila, y sin firma el consumer firmado avisa con **CS8002**.
2. **`HttpResponse<T>` propio de Billing.** La guía usa un sobre `Succeeded/Result`; aquí se
   reutiliza el `HttpResponse<T>` existente de `Billing.Core.DTOs` (propiedad `Data`) para no
   duplicar tipos en el sistema.
3. **Sin `AddNewtonsoftJson`.** La guía lo recomienda para que la API respete `[JsonProperty]`;
   no se añadió para no alterar la serialización de los endpoints existentes de Billing.
   Si añades DTOs con `[JsonProperty]`, añade también el paquete
   `Microsoft.AspNetCore.Mvc.NewtonsoftJson` y `AddNewtonsoftJson(...)` en `Program.cs`.
4. **`DomainException` → 400.** La guía lo hace en un middleware "no mostrado"; aquí el
   `PagosController` hace `catch (DomainException)` y devuelve `400 { errorCode, errorMessage }`.
5. **Ubicación.** La solución vive en `NugetPackage_Rest/` dentro de este repositorio
   (la guía usa `C:\Proyectos\NugetPackage_Rest`), y Billing consume el paquete por feed
   local (Opción A) en lugar de `ProjectReference`.

---

## Errores comunes (de la guía 3.15)

| Síntoma | Causa / solución |
|---|---|
| `Unable to resolve service for type IRest` | Falta `AddInfrastructure(...)` en `Program.cs` o `Microsoft.Extensions.Http`. |
| 401 con token válido | `WithBearer` ya agrega `Bearer `: quita el prefijo del proveedor. |
| Propiedades nulas al deserializar | DTO con `[JsonPropertyName]` (System.Text.Json) en vez de `[JsonProperty]` (Newtonsoft). |
| `The request message was already sent` | Se reutilizó una cadena guardada en una variable: inicia una cadena nueva por llamada. |
| `dotnet pack` → NU5026 | Añade `-p:GeneratePackageOnBuild=false`. |
| La llamada tarda 100 s en fallar | Timeout por defecto de `HttpClient`: configura `client.Timeout` en `AddHttpClient`. |
| No aparecen logs de la librería | Falta `Log.Logger` / `builder.Host.UseSerilog()` en la API. |
