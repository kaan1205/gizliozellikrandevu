# RandevuPanel — Kurulum Talimatları

## Gereksinimler

- .NET 8 SDK → https://dotnet.microsoft.com/download/dotnet/8.0
- SQL Server Express → https://www.microsoft.com/tr-tr/sql-server/sql-server-downloads
- Visual Studio 2022 veya VS Code + C# Dev Kit eklentisi

---

## 1. Projeyi İndirin / Klonlayın

```powershell
git clone <repo-url>
cd gizliozellikrandevu
```

---

## 2. Connection String Ayarlayın

`RandevuPanel/RandevuPanel/appsettings.json` dosyasını açın.
SQL Server Express kurulumunuza göre server adını doğrulayın:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=RandevuPanelDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

Named Instance kullanıyorsanız `.\SQLEXPRESS` yerine `.\MSSQLSERVER` veya `(localdb)\MSSQLLocalDB` yazabilirsiniz.

---

## 3. NuGet Paketlerini Yükleyin

```powershell
cd RandevuPanel/RandevuPanel
dotnet restore
```

---

## 4. Veritabanı Migration & Oluşturma

```powershell
# RandevuPanel/RandevuPanel klasöründeyken:
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Başarılı çıktı: `Done. To undo this action, use 'ef migrations remove'`

---

## 5. Uygulamayı Çalıştırın

```powershell
dotnet run
```

Tarayıcıda açın: `https://localhost:5001` veya `http://localhost:5000`

---

## 6. İlk Kullanım

1. `/Auth/Register` sayfasından hesap oluşturun
2. `/Auth/Login` sayfasından giriş yapın
3. Dashboard'a yönlendirilirsiniz

---

## Proje Yapısı

```
RandevuPanel/
├── RandevuPanel.sln
└── RandevuPanel/
    ├── Controllers/       → HTTP isteklerini karşılar
    ├── Data/              → DbContext (EF Core)
    ├── Models/            → Veritabanı modelleri (AppUser, Appointment)
    ├── Services/          → İş mantığı katmanı
    ├── ViewModels/        → Form ve liste modelleri
    ├── Views/             → Razor sayfaları (.cshtml)
    ├── wwwroot/           → CSS, JS statik dosyalar
    ├── Migrations/        → EF Core migration dosyaları (otomatik)
    ├── Program.cs         → Uygulama başlangıç noktası
    └── appsettings.json   → Yapılandırma
```

---

## Sık Karşılaşılan Sorunlar

### "Cannot open database" hatası
SQL Server Express çalışıyor mu kontrol edin:
```powershell
Get-Service -Name "MSSQL*"
Start-Service -Name "MSSQL$SQLEXPRESS"
```

### "dotnet ef not found" hatası
```powershell
dotnet tool install --global dotnet-ef
```

### SSL Sertifika hatası tarayıcıda
```powershell
dotnet dev-certs https --trust
```
