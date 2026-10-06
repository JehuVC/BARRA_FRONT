# Patrón de arquitectura del Frontend (BarraApp.Mobile)

Guía para trabajar en el proyecto MAUI y conectarlo a la API (`Barra_AP-VF/API_4`). Complementa a `PATRON_ARQUITECTURA.md` del backend — ese explica las capas del lado servidor, este explica las capas del lado app.

En una frase: **cada pantalla es una pareja Page + ViewModel**, la Page es "tonta" (solo muestra y captura), el ViewModel piensa y le habla a la API a través de un Service, y todo se arma con inyección de dependencias en `MauiProgram.cs` — nada se instancia con `new` a mano.

## 0. Relación con el backend: Core y DTO son copias, no un link

`Core/` y `DTO/` en este repo son **copias manuales** de los mismos proyectos del backend (mismos namespaces, mismas clases: `Req...`, `Res...`, `DTO...`, `ResBase`, `enumErrores`). No es un `ProjectReference` cruzado entre repos — son proyectos `.csproj` normales, referenciados así en `BarraApp.Mobile.csproj`:

```xml
<ProjectReference Include="..\Core\Core.csproj" />
<ProjectReference Include="..\DTO\DTO.csproj" />
```

**Regla importante**: si alguien en backend agrega o cambia un `Req`, `Res`, `DTO` o un código de `enumErrores` que el front consume, hay que copiar ese archivo (o el cambio) a `Proyecto Front/BARRA_FRONT/Core` o `/DTO` a mano. Si no se sincroniza, la deserialización del JSON falla en silencio o el compilador no avisa hasta que pruebas contra la API real.

| Backend (`Barra_AP-VF`) | Frontend (`BARRA_FRONT`) | Contenido |
|---|---|---|
| `Core/` | `Core/` | Entidades, `Req...`, `Res... : ResBase`, `enumErrores` — **copia exacta** |
| `DTO/` | `DTO/` | Forma del JSON de entrada/salida — **copia exacta** |
| `API_4/Controllers` | *(no existe del lado app)* | El "contrato" lo definen las rutas `[Route("api/...")]`; el front solo necesita conocer el string de la ruta |

## 1. Capas y carpetas

| Carpeta | Rol |
|---|---|
| `Views/<Dominio>/XxxPage.xaml(.cs)` | La pantalla. XAML + code-behind mínimo (solo recibe el ViewModel y hace `BindingContext = viewModel`). No contiene lógica. |
| `ViewModels/<Dominio>/XxxViewModel.cs` | El estado y la lógica de esa pantalla: propiedades bindables (`[ObservableProperty]`), acciones (`[RelayCommand]`), llama a los `Services` y decide qué mostrar. |
| `Services/<Dominio>/IXxxService.cs` + `XxxService.cs` | Un método por endpoint que ese dominio consume. Hereda de `ApiServiceBase`. No sabe nada de UI. |
| `Services/Api/` | Infraestructura transversal: `ApiConfig` (URL base), `ApiServiceBase` (POST + manejo de errores de transporte), `AuthHeaderHandler` (agrega el JWT solo), `AuthTokenStore`/`IAuthTokenStore` (SecureStorage). |
| `AppShell.xaml(.cs)` | El mapa de navegación: qué pantallas son "raíz" (`ShellContent`) y cuáles son de detalle (`Routing.RegisterRoute`). Ver sección 5. |
| `MauiProgram.cs` | Registra en el contenedor de DI cada `Service`, `ViewModel` y `Page` nuevos. Si algo no está aquí, no existe para la app. |
| `Resources/Styles/Colors.xaml` / `Styles.xaml` | Paleta y estilos **implícitos** (`TargetType="Entry"` sin `x:Key`) — un `<Entry>` normal ya sale con el estilo del proyecto, no hace falta ponerle `Style=` a mano. |

Cada dominio (`Usuario`, `Producto`, `Pago`, `Comercio`, `Guild`, `Favorito`, `Historial`, `ListaCompra`, `Categoria`) ya tiene su carpeta creada en `Views/` y `ViewModels/` (algunas todavía con solo un `.gitkeep`). `Services/` se crea por dominio solo cuando hace falta (no todos los dominios necesitan su propio service si reusan uno existente).

## 2. Cómo se arma una pantalla (Page + ViewModel)

Ejemplo real, `Login`:

**ViewModel** (`ViewModels/Usuario/LoginViewModel.cs`):
```csharp
public partial class LoginViewModel(ISesionService sesionService, IAuthTokenStore tokenStore) : ObservableObject
{
    [ObservableProperty]
    private string correo = string.Empty;          // genera la propiedad publica "Correo" + notificacion de cambio

    [RelayCommand]
    private async Task IniciarSesionAsync()          // genera el ICommand "IniciarSesionCommand"
    {
        var res = await sesionService.LoginAsync(Correo, Clave);
        // ... revisa res.resultado, actualiza MensajeError o navega
    }
}
```

**Page** (`Views/Usuario/LoginPage.xaml.cs`):
```csharp
public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)   // <- DI se lo entrega ya armado
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
```

**XAML** (`Views/Usuario/LoginPage.xaml`): usa `x:DataType="viewmodels:LoginViewModel"` (bindings compilados — un typo en el nombre de una propiedad truena en compilación, no en producción) y enlaza controles a propiedades/comandos:
```xml
<Entry Text="{Binding Correo}" />
<Button Text="Ingresar" Command="{Binding IniciarSesionCommand}" />
<Label Text="{Binding MensajeError}" IsVisible="{Binding HasError}" />
```

**Regla**: 1 Page = 1 ViewModel, mismo nombre de dominio en ambas carpetas. El ViewModel nunca toca controles de UI directamente (nada de `FindByName` ni manipular el XAML desde código) — todo pasa por binding.

## 3. Cómo se conecta al backend (Services)

`ApiConfig.BaseUrl` es `http://localhost:54346/` en todas las plataformas (puerto de IIS Express de `API_4`; si cambia en Visual Studio, hay que actualizarlo aquí). **No uses `10.0.2.2`:** el IIS Express de Visual Studio solo atiende el host `localhost` y contesta `HTTP 400 Invalid Hostname` a cualquier otro. En Android, `localhost` es el propio teléfono, por eso hay que reenviar el puerto a la PC con el script de la sección 3.1.

### 3.1 Probar en el emulador Android o en un teléfono (paso a paso)

Cada vez que vayas a probar:

1. **SQL Server** encendido (tu instancia local) y tu `API_4/ConnectionStrings.config` apuntando a ella (`BARRAAPPConnectionString2`).
2. **Backend:** en Visual Studio abre la solución del backend y corre `API_4` con **F5** (IIS Express, puerto 54346).
3. **Emulador:** arráncalo desde el Device Manager y **espera a ver la pantalla de inicio de Android**. El primer arranque tarda varios minutos.
4. **Reenvío del puerto:** abre PowerShell en la carpeta `BARRA_FRONT` y corre:
   ```powershell
   .\scripts\iniciar_dev_android.ps1
   ```
   Debe responder `OK: emulator-5554 -> localhost:54346 ...`. **Repítelo cada vez que reinicies el emulador** (el reenvío se pierde). Con un teléfono por USB funciona igual (activa la depuración USB).
5. **App:** en Visual Studio elige el emulador como destino y corre `BarraApp.Mobile`.

Si algo falla:

| Síntoma | Causa y arreglo |
|---|---|
| "No se pudo conectar con el servidor" | No corriste el script del paso 4, o reiniciaste el emulador. Córrelo de nuevo. |
| El script dice que no hay dispositivo | El emulador aún no termina de arrancar, o aparece como `offline` en `adb devices`: espera y repite. |
| HTTP 400 al hacer login | Tienes una versión vieja de `ApiConfig` con `10.0.2.2`: haz `git pull`. |
| Error de base de datos al hacer login | Tu `ConnectionStrings.config` apunta a otra PC o SQL Server está apagado. |

**Alternativa sin emulador:** corre `BarraApp.Mobile` con el destino **Windows Machine**. No necesita el script ni el emulador (también usa `localhost`).

`ApiServiceBase` centraliza las llamadas HTTP:
```csharp
public abstract class ApiServiceBase(HttpClient http)
{
    protected async Task<TRes> PostAsync<TReq, TRes>(string ruta, TReq body)
    {
        // hace el POST, y si NO hay conexion / hay 500 / el JSON viene mal formado -> throw ApiException
        // si la respuesta llega bien (200 con resultado:true o resultado:false) -> la devuelve tal cual
    }
}
```

Un service nuevo solo declara la ruta y los tipos:
```csharp
public class SesionService(HttpClient http) : ApiServiceBase(http), ISesionService
{
    public Task<DTOResLogin> LoginAsync(string correo, string pass) =>
        PostAsync<DTOLogin, DTOResLogin>("api/usuario/login", new DTOLogin { correo = correo, pass = pass });
}
```

**El patrón `ResBase` es el mismo del backend**: `{ bool resultado; List<Error> error; }`. `ApiServiceBase` **no** revisa `resultado` — solo distingue fallas de transporte (`ApiException`, algo se rompió antes de llegar a una respuesta de negocio) de una respuesta HTTP 200 normal. **El ViewModel es quien decide** qué hacer si `resultado == false`, leyendo `error` (igual que haría cualquier cliente HTTP del backend):
```csharp
var res = await sesionService.LoginAsync(correo, clave);
if (!res.resultado) { MensajeError = TraducirError(res.error?.FirstOrDefault()?.mensaje); return; }
```

`AuthHeaderHandler` es un `DelegatingHandler` que se cuelga en la cadena del `HttpClient` y le agrega `Authorization: Bearer <token>` a **cada** request saliente si hay un token guardado en `IAuthTokenStore` (SecureStorage) — ningún service tiene que acordarse de mandarlo a mano. En rutas públicas (login, registro) simplemente no hay token todavía y no se manda header; el backend ya sabe cuáles rutas no lo exigen (`JwtAuthHandler.RutasPublicas` del lado servidor).

## 4. Cómo se registra todo (`MauiProgram.cs`)

```csharp
static void RegistrarApi(IServiceCollection services)
{
    services.AddSingleton<IAuthTokenStore, AuthTokenStore>();
    services.AddTransient<AuthHeaderHandler>();

    services.AddHttpClient<ISesionService, SesionService>(client =>
    {
        client.BaseAddress = new Uri(ApiConfig.BaseUrl);
    })
    .AddHttpMessageHandler<AuthHeaderHandler>();
}

static void RegistrarPantallas(IServiceCollection services)
{
    services.AddTransient<LoginViewModel>();
    services.AddTransient<LoginPage>();
    // cada Page/ViewModel nuevo se agrega aqui igual
}
```

**Regla**: toda `Page`, `ViewModel` y `Service` nuevo se registra aquí. Si falta el registro, la app truena al navegar a esa pantalla (DI no puede armar el constructor). `Services/Api` usa `AddSingleton` para `IAuthTokenStore` (una sola instancia, el token es global a la app) y `AddTransient` para `Page`/`ViewModel` (una instancia nueva cada vez que se navega ahí).

## 5. Navegación (`AppShell.xaml`)

Hay **dos formas** de que una pantalla exista para el Shell, y usar la que no toca revienta en runtime (nos pasó armando el login):

- **`ShellContent` en `AppShell.xaml`**: para pantallas que pueden ser la "raíz" visible — se navega con `GoToAsync("//NombrePagina")` (doble slash = navegación **absoluta**, resetea todo el stack). Úsalo para: login, dashboard, cualquier pantalla principal.
- **`Routing.RegisterRoute(...)` en `AppShell.xaml.cs`**: para pantallas de **detalle**, a las que solo se llega empujando desde otra pantalla ya visible — se navega con `GoToAsync("NombrePagina")` (sin `//`, apila sobre la actual). Úsalo para: detalle de un producto, edición de un item de lista, etc. **Una ruta registrada así NO puede ser la única página del stack** — si intentas `GoToAsync("//RutaRegistrada")` sin que tenga también un `ShellContent`, truena con `"Global routes currently cannot be the only page on the stack"`.

Ejemplo real, `AppShell.xaml`:
```xml
<Shell ... FlyoutBehavior="Disabled" Shell.TabBarIsVisible="False">
    <ShellContent Title="Ingresar" ContentTemplate="{DataTemplate usuario:LoginPage}" Route="LoginPage" />
    <ShellContent Title="Inicio" ContentTemplate="{DataTemplate dashboard:DashboardPage}" Route="DashboardPage" />
</Shell>
```
`Shell.TabBarIsVisible="False"` oculta la barra de pestañas que MAUI pondría automáticamente al tener dos `ShellContent` — la navegación entre ellas es 100% por código (`GoToAsync`), nunca tocando una pestaña.

## 6. Flujo completo de ejemplo (Login)

```
Usuario toca "Ingresar" en LoginPage
  → LoginViewModel.IniciarSesionCommand
       arma DTOLogin { correo, pass }
  → ISesionService.LoginAsync(correo, pass)
       POST api/usuario/login  (ruta publica, sin JWT)
  → ApiServiceBase.PostAsync deserializa la respuesta a DTOResLogin
  ← DTOResLogin { resultado, error[], usuario, sesion }
  → LoginViewModel revisa "resultado":
        false → MensajeError = MensajesError.Traducir(res.error)  // se muestra en la Page via binding
        true  → tokenStore.GuardarTokenAsync(sesion.tokenJwt)     // SecureStorage
              → Shell.Current.GoToAsync("//DashboardPage")        // navegacion absoluta, resetea el stack
```

## 7. Checklist para agregar una pantalla nueva que consuma un endpoint

1. Confirmar que el endpoint ya existe en el backend (`api/<recurso>/<accion>`) — si no existe, coordinar con quien lleve esa parte del backend.
2. **Sincronizar el contrato**: copiar/actualizar el `Req`/`Res`/`DTO` correspondiente de `Barra_AP-VF/Core` o `/DTO` hacia `BARRA_FRONT/Core` o `/DTO` si no existe todavía o si cambió.
3. Si el dominio no tiene `Service` propio: crear `Services/<Dominio>/I<Dominio>Service.cs` + `<Dominio>Service.cs` (hereda `ApiServiceBase`), un método por acción que consuma.
4. Registrar el service en `MauiProgram.RegistrarApi()` con `AddHttpClient<I..., ...>(...).AddHttpMessageHandler<AuthHeaderHandler>()`.
5. Crear `ViewModels/<Dominio>/XxxViewModel.cs`: hereda `ObservableObject`, un `[ObservableProperty]` por cada campo que la UI necesite leer/escribir, un `[RelayCommand]` por cada acción del usuario. Recibe el/los `Service` por constructor.
6. Crear `Views/<Dominio>/XxxPage.xaml` + `.xaml.cs`: `x:DataType` apuntando al ViewModel, controles con `Style` implícito (no hace falta declararlo), constructor que recibe el ViewModel y hace `BindingContext = viewModel`.
7. Registrar `Page` + `ViewModel` en `MauiProgram.RegistrarPantallas()`.
8. Decidir cómo se llega a la pantalla (ver sección 5): ¿es una pantalla raíz? → `ShellContent`. ¿Es un detalle al que se llega empujando desde otra? → `Routing.RegisterRoute`.
9. Compilar al menos un target antes de dar por hecho que sirve: `dotnet build BarraApp.Mobile\BarraApp.Mobile.csproj -f net10.0-android`.
10. Probar contra la API real corriendo localmente (`API_4` con IIS Express) — un `build` exitoso no garantiza que el binding, la ruta o el contrato JSON estén bien.

## 8. Convenciones compartidas (ya implementadas, úsenlas)

**Mensajes de error — `MensajesError.Traducir(res.error)`** (`Services/Api/MensajesError.cs`). El backend manda el *nombre del enum* en `mensaje` (ej. `"correoYaRegistrado"`), que no es para el usuario. Nunca muestres `res.error[0].mensaje` directo: pásalo por `MensajesError.Traducir(res.error, "texto por defecto")`. Traduce por `codigo` (el `enumErrores`). **Si el backend agrega un código nuevo a `enumErrores`, agrégale su texto en ese `switch`** (si falta, cae al texto por defecto).

**Validaciones antes de enviar — `ValidacionCliente`** (`Helpers/ValidacionCliente.cs`): `EsEmailValido`, `EsPasswordValida` (8+, mayúscula, minúscula, número y carácter especial; el backend exige menos, a propósito). El backend siempre vuelve a validar: la del front es solo para no gastar una llamada.

**Sesión.** El JWT vive en `SecureStorage` (`IAuthTokenStore`).
- Al abrir la app, `LoginPage` salta directo al Dashboard si ya hay token guardado (`LoginViewModel.InicializarAsync`).
- Si cualquier llamada protegida responde **401** (token vencido o sesión cerrada), `AuthHeaderHandler` borra el token y manda a `//LoginPage` solo; el ViewModel recibe un `ApiException("Tu sesión expiró...")`. No hay que manejar el 401 pantalla por pantalla.
- Para cerrar sesión: llamar `ISesionService.LogoutAsync()` (revoca el JWT en el servidor) **y** `IAuthTokenStore.LimpiarTokenAsync()`. Solo navegar al login deja la sesión viva.

**Pasar datos entre pantallas.** Por query string y `IQueryAttributable` en el ViewModel destino. Escapa los textos:
```csharp
await Shell.Current.GoToAsync($"{nameof(ListaDetallePage)}?guidLista={lista.GuidLista}&nombre={Uri.EscapeDataString(lista.Nombre)}");
// en el ViewModel destino
public void ApplyQueryAttributes(IDictionary<string, object> query) { ... query["guidLista"] ... }
```

**Pedir un texto corto** (renombrar, crear lista): `Shell.Current.DisplayPromptAsync(...)` en vez de armar una pantalla. **Alertas:** `DisplayAlertAsync` (en .NET 10 `DisplayAlert` está obsoleto).

**Choque de nombres.** Hay entidades de `Core` que se llaman igual que una carpeta/namespace del front (`ListaCompra`, `Guild`, `Producto`). Dentro de `BarraApp.Mobile.ViewModels.ListaCompra`, escribir `ListaCompra` resuelve al namespace y no compila. Usa un alias: `using ListaCompraEntidad = Core.Entidades.Enti_Clases.ListaCompra;`.

**Pantallas ya hechas (para no duplicar trabajo):** Login, Registro, Recuperar contraseña (2 pasos), Perfil, Dashboard, Gremio (crear / unirse / miembros / salir), Mis Listas (crear / renombrar / eliminar) Detalle de lista (agregar producto por búsqueda, cantidad, marcar comprado, quitar) Búsqueda de productos (por nombre o marca, filtro por categoría, favorito y agregar a una lista; se abre desde la barra de búsqueda y las búsquedas recientes del Dashboard) y Detalle de producto (ficha, comparación de precios entre comercios, favorito, agregar a una lista y reportar un precio; se abre tocando un resultado de la búsqueda) y Favoritos (lista, quitar con «Deshacer», agregar a una lista y abrir el detalle; se abre desde «Mis Favoritos» del menú del Dashboard) e Historial (búsquedas y productos vistos, agrupados por día y sin repetidas; tocar una repite la búsqueda; se abre desde «Historial Completo» del menú). El backend devuelve como máximo las últimas 20 entradas y no tiene endpoint para borrar el historial. Y Comercios cercanos (radio de 1 a 25 km, «Cómo llegar» abre el mapa, llamar, registrar y editar comercio con aviso si ya existe uno parecido; se abre desde «Comercios cercanos» del menú del Dashboard o desde el aviso «Sin comercios cerca» del detalle de producto). Y Registrar/editar producto (código de barras, nombre, marca, descripción, categoría y unidad —que se eligen de las existentes o se crean al momento—; avisa si ya hay uno parecido o si el código ya existe; se abre desde «Registrar producto» del menú, desde el botón de la búsqueda sin resultados, o con el ✏️ del detalle de producto). Y Suscripción / Pagos (se elige un plan, se crea la orden, el checkout de Tilopay se abre en un WebView y la app pregunta cada 4 s por el resultado hasta que sea aprobado o rechazado; si el usuario sale a medias, la orden queda guardada para verificarla después; se abre desde «Suscripción / Pagos» del menú del Dashboard). Los planes y precios están en `ViewModels/Pago/PlanPago.cs` porque el backend no tiene catálogo de planes. Y Escáner de código de barras (cámara con la librería `BarcodeScanning.Native.Maui`; si el producto existe abre su ficha, si no ofrece registrarlo con el código ya puesto; también permite escribir el código a mano; se abre con «Escanear Código» del Dashboard o el 📷 de la búsqueda). Esa librería exige Android 6.0 (API 23) como mínimo, por eso `SupportedOSPlatformVersion` de Android es 23.

**Pasar un objeto entre pantallas.** Si ya tienes la entidad completa (y el backend no tiene «obtener por id»), no la vuelvas a pedir: `Shell.Current.GoToAsync("ComercioFormPage", new ShellNavigationQueryParameters { { "comercio", entidad } })` y en la otra pantalla léela en `ApplyQueryAttributes` (`query["comercio"] as ...`). Sirve solo para objetos; para ids simples usa el query string.

**Servicios reutilizables que ya existen** (úsenlos en vez de repetir lógica): `IAgregarAListaFlujo` (elegir una lista y agregar un producto, con sus alertas), `IUbicacionService` (ubicación del usuario; con `pedirPermiso: false` no muestra el aviso de permisos), `IComercioService` (cercanos / crear / actualizar), `IComparadorService`, `IFavoritoService`, `ICategoriaService` (obtener / crear), `IUnidadService` (obtener / crear), `IPagoService` (crear / consultar), `IProductoService` (buscar / obtener / crear / actualizar / registrar precio), y los helpers `Formato.Colones/Distancia/Hace/TryParsePrecio` para mostrar y leer precios.

## 9. Reglas rápidas (resumen)

- 1 Page = 1 ViewModel, misma carpeta de dominio, mismo nombre base.
- El ViewModel no conoce controles de UI ni code-behind — todo por binding/`Command`.
- Fallas de transporte (sin conexión, 500, JSON raro) → `ApiException` (se atrapan con `try/catch` en el ViewModel). Fallas de negocio (credenciales inválidas, etc.) → vienen en `resultado`/`error` del propio DTO, no lanzan excepción.
- Nada se instancia con `new` en una Page/ViewModel de producción — si necesitas algo, se pide por constructor y se registra en `MauiProgram.cs`.
- `ShellContent` = pantalla raíz (navegación `//`). `Routing.RegisterRoute` = pantalla de detalle (navegación sin `//`, nunca puede ser la única del stack).
- Antes de commitear un cambio que toque un contrato compartido con el backend (`Core`/`DTO`), sincronizarlo a mano en ambos repos y avisar al equipo.
