using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios MVC.
builder.Services.AddControllersWithViews();

// Configurar la conexión a SQL Server.
builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConexionEventos")));

// Registrar repositorios.
builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Aplicar migraciones e insertar datos de prueba.
await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<EventosContext>();

    // Crear o actualizar la estructura de la base de datos.
    await context.Database.MigrateAsync();

    // Insertar categorías si la tabla está vacía.
    if (!await context.Categorias.AnyAsync())
    {
        context.Categorias.AddRange(
            new Categoria { Nombre = "Tecnología", Activo = true },
            new Categoria { Nombre = "Desarrollo", Activo = true },
            new Categoria { Nombre = "Emprendimiento", Activo = true }
        );

        await context.SaveChangesAsync();
    }

    // Obtener las categorías para relacionarlas con los eventos.
    var categorias = await context.Categorias
        .AsNoTracking()
        .ToListAsync();

    var tecnologia = categorias.First(c => c.Nombre == "Tecnología");
    var desarrollo = categorias.First(c => c.Nombre == "Desarrollo");
    var emprendimiento = categorias.First(c => c.Nombre == "Emprendimiento");

    // Insertar eventos si la tabla está vacía.
    if (!await context.Eventos.AnyAsync())
    {
        context.Eventos.AddRange(
            new Evento
            {
                Titulo = "Congreso de Tecnología",
                Fecha = DateTime.Now.AddDays(30),
                Lugar = "Lima",
                Aforo = 200,
                Precio = 50.00m,
                CategoriaId = tecnologia.Id,
                Activo = true
            },
            new Evento
            {
                Titulo = "Taller de Entity Framework Core",
                Fecha = DateTime.Now.AddDays(15),
                Lugar = "Laboratorio de Sistemas",
                Aforo = 30,
                Precio = 25.00m,
                CategoriaId = desarrollo.Id,
                Activo = true
            },
            new Evento
            {
                Titulo = "Feria de Emprendimiento",
                Fecha = DateTime.Now.AddDays(45),
                Lugar = "Centro de Convenciones",
                Aforo = 500,
                Precio = 0.00m,
                CategoriaId = emprendimiento.Id,
                Activo = true
            },
            new Evento
            {
                Titulo = "Seminario de Computación en la Nube",
                Fecha = DateTime.Now.AddDays(60),
                Lugar = "Auditorio Principal",
                Aforo = 100,
                Precio = 40.00m,
                CategoriaId = tecnologia.Id,
                Activo = true
            }
        );

        await context.SaveChangesAsync();
    }

    // Insertar asistentes si la tabla está vacía.
    if (!await context.Asistentes.AnyAsync())
    {
        context.Asistentes.AddRange(
            new Asistente
            {
                Nombres = "Carlos Ramírez",
                Dni = "12345678",
                Correo = "carlos@example.com",
                Activo = true
            },
            new Asistente
            {
                Nombres = "María Torres",
                Dni = "23456789",
                Correo = "maria@example.com",
                Activo = true
            },
            new Asistente
            {
                Nombres = "José Flores",
                Dni = "34567890",
                Correo = "jose@example.com",
                Activo = true
            },
            new Asistente
            {
                Nombres = "Ana Castillo",
                Dni = "45678901",
                Correo = "ana@example.com",
                Activo = true
            },
            new Asistente
            {
                Nombres = "Luis Mendoza",
                Dni = "56789012",
                Correo = "luis@example.com",
                Activo = true
            }
        );

        await context.SaveChangesAsync();
    }
}

// Configurar el pipeline HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();