using BoletasAguaLuzRazor.Data;
using BoletasAguaLuzRazor.Models;
using BoletasAguaLuzRazor.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using QuestPDF.Infrastructure;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages(o=>{
 o.Conventions.AuthorizeFolder("/");
 o.Conventions.AllowAnonymousToFolder("/Cuenta");
 o.Conventions.AllowAnonymousToPage("/Error");
 o.Conventions.AuthorizeFolder("/Admin","Admin");
});
builder.Services.AddAuthorization(o=>o.AddPolicy("Admin",p=>p.RequireRole("Admin")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o=>{
 o.LoginPath="/Cuenta/Login"; o.AccessDeniedPath="/Cuenta/Denegado";
 o.ExpireTimeSpan=TimeSpan.FromHours(8); o.Cookie.HttpOnly=true; o.Cookie.SameSite=SameSiteMode.Lax;
 o.Events.OnValidatePrincipal=async c=>{
 var db=c.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
 var id=int.TryParse(c.Principal?.FindFirstValue(ClaimTypes.NameIdentifier),out var n)?n:0;
 var u=await db.Usuarios.FindAsync(id);
 if(u==null || !u.Activo || u.PasswordHash!=c.Principal?.FindFirstValue("version")){c.RejectPrincipal();await c.HttpContext.SignOutAsync();}
 };
});
builder.Services.AddDbContext<AppDbContext>(o=>o.UseSqlite("Data Source=App_Data/servicios.db"));
builder.Services.AddScoped<IPasswordHasher<Usuario>,PasswordHasher<Usuario>>();
builder.Services.AddScoped<ReciboPdfService>();
QuestPDF.Settings.License=LicenseType.Community;
var app=builder.Build();
Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath,"App_Data"));
using(var scope=app.Services.CreateScope()){
 var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();db.Database.EnsureCreated();
 // Upgrade the previous database without deleting accounts or receipt history.
 var connection=db.Database.GetDbConnection();connection.Open();
 using(var command=connection.CreateCommand()){
 command.CommandText="SELECT COUNT(*) FROM pragma_table_info('Recibos') WHERE name='Servicio'";
 if(Convert.ToInt32(command.ExecuteScalar())==0){
 using var transaction=connection.BeginTransaction();command.Transaction=transaction;
 command.CommandText="ALTER TABLE Recibos ADD COLUMN Servicio TEXT NOT NULL DEFAULT 'General'; DROP INDEX IF EXISTS IX_Recibos_UsuarioId_Anio_Mes; CREATE UNIQUE INDEX IF NOT EXISTS IX_Recibos_UsuarioId_Anio_Mes_Servicio ON Recibos (UsuarioId, Anio, Mes, Servicio);";
 command.ExecuteNonQuery();transaction.Commit();
 }
 }
 using(var command=connection.CreateCommand()){
 command.CommandText="SELECT COUNT(*) FROM pragma_table_info('Recibos') WHERE name='Eliminado'";
 if(Convert.ToInt32(command.ExecuteScalar())==0){
 using var transaction=connection.BeginTransaction();command.Transaction=transaction;
 command.CommandText="ALTER TABLE Recibos ADD COLUMN Eliminado INTEGER NOT NULL DEFAULT 0; ALTER TABLE Recibos ADD COLUMN EliminadoFecha TEXT NULL; ALTER TABLE Recibos ADD COLUMN EliminadoPor INTEGER NULL;";
 command.ExecuteNonQuery();transaction.Commit();
 }
 }
 using(var command=connection.CreateCommand()){
 command.CommandText="DROP INDEX IF EXISTS IX_Recibos_UsuarioId_Anio_Mes_Servicio; CREATE UNIQUE INDEX IF NOT EXISTS IX_Recibos_Activos_Periodo ON Recibos(UsuarioId,Anio,Mes,Servicio) WHERE Eliminado=0;";
 command.ExecuteNonQuery();
 }connection.Close();
 if(!db.Configuraciones.Any()){db.Configuraciones.Add(new Configuracion());db.SaveChanges();}
}
if(!app.Environment.IsDevelopment()){app.UseExceptionHandler("/Error");app.UseHsts();app.UseHttpsRedirection();}
app.UseStaticFiles();app.UseRouting();app.UseAuthentication();app.UseAuthorization();app.MapRazorPages();app.Run();
