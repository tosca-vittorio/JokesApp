// ============================================================================
// IMPORTAZIONI PER ENTITY FRAMEWORK CORE
// ============================================================================
// Per leggere le variabili d’ambiente dal file `.env`
using DotNetEnv;
// Importa il nostro DbContext personalizzato
// Permette di usare 'JokesDbContext' invece di 'JokesApp.Server.Data.JokesDbContext'
using JokesApp.Server.Data;
using JokesApp.Server.Data.Errors;
using JokesApp.Server.Domain.Entities;
using Microsoft.AspNetCore.Identity;
// Importa il namespace principale di Entity Framework Core
// Contiene: AddDbContext, UseSqlServer, UseNpgsql, ecc.
using Microsoft.EntityFrameworkCore;
// Importa il namespace per Dependency Injection
// (Già incluso di default in progetti ASP.NET Core, ma esplicito per chiarezza)
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


// ========================= //
// Carica automaticamente le variabili dal file .env
Env.Load(); 

// CONFIGURAZIONE APPLICAZIONE // 
// Crea il builder dell'applicazione web
// Questo oggetto serve per configurare tutti i servizi e middleware
var builder = WebApplication.CreateBuilder(args);

// 'args' → Argomenti da command-line (es: dotnet run --environment Production)
// WebApplicationBuilder legge automaticamente:
// - appsettings.json
// - appsettings.{Environment}.json
// - Variabili d'ambiente
// - User secrets (in sviluppo)
// - Command-line arguments


// ============================================================================
// REGISTRAZIONE DEL DBCONTEXT NEL DI CONTAINER
// ============================================================================
// 'builder.Services' → IServiceCollection
// È il CONTENITORE DI DEPENDENCY INJECTION dove registriamo tutti i servizi
builder.Services.AddDbContext<JokesDbContext>(

    // LAMBDA EXPRESSION per configurare il DbContext
    // -----------------------------------------------
    //
    // 'options =>' → Sintassi lambda (funzione anonima)
    //   Forma completa equivalente:
    //   (DbContextOptionsBuilder<JokesDbContext> options) => 
    //   {
    //       return options.UseNpgsql(...);
    //   }
    //
    // 'options' → Oggetto DbContextOptionsBuilder<JokesDbContext>
    //   Serve per costruire le DbContextOptions che verranno passate al costruttore
    //
    // Questo parametro è FORNITO da AddDbContext automaticamente
    // Non lo creiamo noi, ci viene dato per configurarlo
    options =>

    // UseNpgsql → CONFIGURA IL PROVIDER DATABASE
    // ----------------------------------------------
    //
    // Dice a Entity Framework Core di usare PostgreSQL SERVER come database
    // 
    // ALTRI PROVIDER DISPONIBILI:
    //
    // UseSqlServer:
    // options.UseSqlServer(connectionString);
    //
    // MySQL:
    // options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    // Richiede NuGet: Pomelo.EntityFrameworkCore.MySql
    //
    // SQLite (ottimo per sviluppo/test):
    // options.UseSqlite(connectionString);
    // Richiede NuGet: Microsoft.EntityFrameworkCore.Sqlite
    //
    // In-Memory (solo per test unitari):
    // options.UseInMemoryDatabase("TestDb");
    // Richiede NuGet: Microsoft.EntityFrameworkCore.InMemory
    //
    // IMPORTANTE:
    // Il metodo Use* deve corrispondere al pacchetto NuGet installato!
    // UseSqlServer richiede: Microsoft.EntityFrameworkCore.SqlServer
    // UseNpgsql richiede: Npgsql.EntityFrameworkCore.PostgreSQL
    options.UseNpgsql(

        // PARAMETRO: CONNECTION STRING
        // ----------------------------
        //
        // builder.Configuration → IConfiguration
        // Oggetto che legge configurazioni da varie fonti (ordine di priorità):
        // 1. Command-line arguments (massima priorità)
        // 2. Variabili d'ambiente
        // 3. User secrets (solo in Development)
        // 4. appsettings.{Environment}.json
        // 5. appsettings.json (priorità più bassa)
        //
        // GetConnectionString("DefaultConnection") → LEGGE CONNECTION STRING
        // ---------------------------------------------------------------
        //
        // Cerca una connection string chiamata "DefaultConnection"
        //
        // DOVE CERCA (in appsettings.json):
        // {
        //   "ConnectionStrings": {
        //     "DefaultConnection": "Server=localhost;Database=jokesdb;..."
        //   }
        // }
        //
        // GetConnectionString("Nome") è uno SHORTCUT per:
        // builder.Configuration["ConnectionStrings:Nome"]
        //
        // Il carattere ':' in "ConnectionStrings:Nome" rappresenta
        // la gerarchia JSON: sezione → chiave
        //
        // STRUTTURA CONNECTION STRING SQL SERVER:
        // ---------------------------------------
        // "Server=localhost;Database=jokesdb;User Id=sa;Password=Pass123;TrustServerCertificate=True"
        //
        // Componenti:
        // - Server: indirizzo del server (localhost, IP, dominio)
        // - Database: nome del database specifico
        // - User Id: username per autenticazione
        // - Password: password per autenticazione
        // - TrustServerCertificate: ignora validazione certificato (solo dev!)
        // - MultipleActiveResultSets: permette query concorrenti (opzionale)
        //
        // STRUTTURA CONNECTION STRING POSTGRESQL:
        // ---------------------------------------
        // "Host=localhost;Port=5432;Database=jokesdb;Username=postgres;Password=Pass123"
        //
        // Componenti:
        // - Host: indirizzo del server
        // - Port: porta TCP (5432 è default PostgreSQL)
        // - Database: nome del database
        // - Username: username per autenticazione
        // - Password: password per autenticazione
        //
        // PARAMETRI OPZIONALI COMUNI:
        // ----------------------------
        // Pooling=true;Minimum Pool Size=5;Maximum Pool Size=20;
        // → Connection pooling per performance
        //
        // Timeout=30;
        // → Timeout comandi in secondi
        //
        // SSL Mode=Require; (PostgreSQL)
        // → Forza connessione SSL
        builder.Configuration.GetConnectionString("DefaultConnection")

        // OPERATORE NULL-COALESCING '??'
        // -------------------------------
        //
        // Sintassi: valoreSinistra ?? valoreDestra
        //
        // Significato:
        // "Se valoreSinistra è null, usa valoreDestra"
        //
        // Nel nostro caso:
        // GetConnectionString("DefaultConnection") ?? throw new ...
        //
        // Se GetConnectionString restituisce null (chiave non trovata):
        // → lancia InvalidOperationException
        //
        // PERCHÉ LANCIARE UN'ECCEZIONE:
        // -----------------------------
        //
        // ✅ FAIL FAST PRINCIPLE
        // È meglio che l'applicazione SI RIFIUTI DI AVVIARSI
        // piuttosto che:
        // - Partire con configurazione errata
        // - Crashare dopo con errori criptici tipo:
        //   "A connection was successfully established with the server, 
        //    but then an error occurred..."
        // - Causare comportamenti imprevedibili
        // - Rendere il debugging difficile
        //
        // ✅ MESSAGGIO CHIARO E IMMEDIATO
        // Lo sviluppatore vede immediatamente:
        // - Qual è il problema (connection string mancante)
        // - Quale chiave cercare ("DefaultConnection")
        // - Dove aggiungerla (appsettings.json)
        //
        // Esempio di errore:
        // InvalidOperationException: Connection string 'DefaultConnection' not found.
        //   at Program.<Main>$(String[] args) in Program.cs:line 15
        //
        // Lo sviluppatore sa esattamente cosa fare:
        // 1. Aprire appsettings.json
        // 2. Aggiungere la sezione ConnectionStrings
        // 3. Aggiungere la chiave DefaultConnection con la connection string corretta
        //
        // BEST PRACTICE:
        // Sempre validare configurazioni critiche all'avvio
        // Mai lasciare che l'app parta con configurazione incompleta
        ?? throw new InvalidOperationException(

            // Messaggio di errore chiaro e descrittivo
            // Indica esattamente qual è il problema e cosa cercare
            "Connection string 'DefaultConnection' not found.")));

// Configura Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<JokesDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

var logger = app.Logger;

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<JokesDbContext>();

        // Verifica che il database sia raggiungibile all'avvio (fail-fast)
        if (!context.Database.CanConnect())
        {
            logger.LogError(StartupErrorMessages.DatabaseConnectionErrorOnStartup);
            throw new InvalidOperationException(StartupErrorMessages.DatabaseConnectionErrorOnStartup);
        }

        var jokeCount = context.Jokes.Count();
        logger.LogInformation("Numero di barzellette presenti all'avvio: {JokeCount}", jokeCount);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, StartupErrorMessages.DatabaseConnectionErrorOnStartup);
        throw;
    }
}



app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseHsts();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
