using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RandevuPanel.Data;
using RandevuPanel.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddScoped<IAppointmentService, AppointmentService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    var conn = db.Database.GetDbConnection();
    await conn.OpenAsync();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = """
        IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ServiceNotes')
        BEGIN
            CREATE TABLE ServiceNotes (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                VehicleBrand NVARCHAR(100) NULL,
                VehicleModel NVARCHAR(100) NULL,
                VehicleYear INT NULL,
                PhotoPath NVARCHAR(500) NULL,
                PossibleOperations NVARCHAR(MAX) NULL,
                TotalCost DECIMAL(18,2) NULL,
                OperationNote NVARCHAR(MAX) NULL,
                OperationNotPossible BIT NOT NULL DEFAULT 0,
                CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
                UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
                CreatedByUserId INT NOT NULL,
                CONSTRAINT FK_ServiceNotes_AppUser FOREIGN KEY (CreatedByUserId)
                    REFERENCES Users(Id) ON DELETE CASCADE
            )
        END
        ELSE IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ServiceNotes' AND COLUMN_NAME = 'VehicleYear')
        BEGIN
            ALTER TABLE ServiceNotes ADD VehicleYear INT NULL
        END
        """;
    await cmd.ExecuteNonQueryAsync();
    await conn.CloseAsync();
}

app.Run();
