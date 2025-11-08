# Kiistore
# 🎮 GameTopupStore

**GameTopupStore** adalah aplikasi web e-commerce berbasis ASP.NET Core MVC untuk jual beli top-up game online. Aplikasi ini memungkinkan pengguna untuk membeli berbagai jenis top-up game seperti Diamond, UC, Robux, Gem, dan Voucher dengan sistem pembayaran QRIS.

---

## 📋 Daftar Isi

- [Tujuan dan Fokus Proyek](#tujuan-dan-fokus-proyek)
- [Fitur Utama](#fitur-utama)
- [Teknologi yang Digunakan](#teknologi-yang-digunakan)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [Cara Menjalankan Aplikasi](#cara-menjalankan-aplikasi)
- [Struktur Proyek](#struktur-proyek)
- [Screenshots](#screenshots)

---

## 🎯 Tujuan dan Fokus Proyek

### Tujuan Utama
1. **Platform E-Commerce Top-Up Game**
   - Menyediakan platform yang mudah digunakan untuk membeli top-up berbagai game populer
   - Memudahkan transaksi top-up game dengan sistem yang aman dan terpercaya

2. **Manajemen Katalog Game**
   - Mendukung berbagai jenis game populer: MLBB, HOK, AOV, PUBG, Free Fire, Roblox, COC
   - Sistem manajemen produk yang fleksibel untuk admin
   - Auto-seed data sample untuk memudahkan testing

3. **Sistem Pembayaran QRIS**
   - Implementasi pembayaran QRIS sebagai metode pembayaran utama
   - Proses checkout yang sederhana dan cepat

4. **Manajemen Order & Inventory**
   - Sistem keranjang belanja (shopping cart)
   - Tracking order dan status pembayaran
   - Manajemen stok produk otomatis

### Fokus Proyek
- **User Experience**: Interface yang user-friendly dengan tema gaming yang menarik
- **Security**: Sistem autentikasi dan otorisasi untuk admin dan customer
- **Scalability**: Arsitektur yang dapat dikembangkan untuk menambah game dan fitur baru
- **Performance**: Optimasi query database dan caching untuk performa yang baik

---

## ✨ Fitur Utama

### 👤 Customer Features
- ✅ **Katalog Game Top-Up** dengan filter berdasarkan game type dan currency
- ✅ **Detail Produk** dengan informasi lengkap
- ✅ **Shopping Cart** untuk mengelola item sebelum checkout
- ✅ **Checkout System** dengan input game account dan server
- ✅ **Pembayaran QRIS** sebagai metode pembayaran
- ✅ **Order Management** untuk tracking pesanan
- ✅ **Search & Filter** untuk mencari produk dengan mudah

### 🔐 Admin Features
- ✅ **Dashboard** dengan statistik lengkap
- ✅ **Game Top-Up Management** (CRUD operations)
- ✅ **Order Management** untuk melihat dan mengelola pesanan
- ✅ **User Management** untuk mengelola customer
- ✅ **Auto-seed Data** untuk data sample

### 🎮 Game yang Didukung
- 🎮 **Mobile Legends: Bang Bang (MLBB)** - Diamond
- 👑 **Honor of Kings (HOK)** - Diamond
- ⚔️ **Arena of Valor (AOV)** - Diamond/Voucher
- 🔫 **PUBG Mobile** - UC (Unknown Cash)
- 💥 **Free Fire** - Diamond
- 🎲 **Roblox** - Robux
- 🏰 **Clash of Clans (COC)** - Gem

---

## 🛠 Teknologi yang Digunakan

- **Framework**: ASP.NET Core MVC (.NET 9.0)
- **Database**: MongoDB
- **Authentication**: Session-based Authentication
- **Frontend**: 
  - HTML5, CSS3, JavaScript
  - Bootstrap 5
  - Font Awesome Icons
- **Backend**:
  - C# (.NET 9.0)
  - MongoDB.Driver (v3.5.0)
  - BCrypt.Net-Next (v4.0.3) untuk password hashing

---

## 📦 Prerequisites

Sebelum memulai, pastikan Anda telah menginstall:

1. **.NET 9.0 SDK**
   - Download dari: https://dotnet.microsoft.com/download/dotnet/9.0
   - Verifikasi instalasi: `dotnet --version`

2. **MongoDB**
   - Download dari: https://www.mongodb.com/try/download/community
   - Atau gunakan MongoDB Atlas (cloud): https://www.mongodb.com/cloud/atlas
   - Verifikasi instalasi: `mongod --version`

3. **IDE/Editor** (Opsional)
   - Visual Studio 2022
   - Visual Studio Code
   - Rider

4. **Git** (untuk clone repository)
   - Download dari: https://git-scm.com/downloads

---

## 🚀 Installation & Setup

### 1. Clone Repository

```bash
git clone https://github.com/yourusername/GameTopupStore.git
cd GameTopupStore
```

### 2. Install Dependencies

```bash
dotnet restore
```

### 3. Setup MongoDB

#### Opsi A: MongoDB Local
1. Install MongoDB Community Server
2. Jalankan MongoDB service:
   ```bash
   # Windows
   net start MongoDB
   
   # Linux/Mac
   sudo systemctl start mongod
   ```
3. MongoDB akan berjalan di `mongodb://localhost:27017`

#### Opsi B: MongoDB Atlas (Cloud)
1. Buat akun di [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
2. Buat cluster baru (gratis)
3. Dapatkan connection string
4. Update `appsettings.json` dengan connection string Atlas

### 4. Configuration

Edit file `appsettings.json`:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "GameTopupStoreDB"
  }
}
```

**Untuk MongoDB Atlas**, gunakan format:
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority",
    "DatabaseName": "GameTopupStoreDB"
  }
}
```

### 5. Build Project

```bash
dotnet build
```

### 6. Run Application

```bash
dotnet run
```

Aplikasi akan berjalan di:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`

---

## ⚙️ Configuration

### Web Service Configuration

#### 1. **appsettings.json**

File konfigurasi utama untuk aplikasi:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "GameTopupStoreDB"
  }
}
```

**Penjelasan:**
- `ConnectionString`: URL koneksi ke MongoDB server
- `DatabaseName`: Nama database yang akan digunakan
- `AllowedHosts`: Host yang diizinkan mengakses aplikasi (`*` = semua host)

#### 2. **Program.cs**

Konfigurasi middleware dan services:

```csharp
// Add services
builder.Services.AddControllersWithViews();

// Register MongoDB Service
builder.Services.AddSingleton<MongoDBService>();

// Add Session untuk login
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

**Penjelasan:**
- `AddControllersWithViews()`: Menambahkan MVC services
- `AddSingleton<MongoDBService>()`: Register MongoDB service sebagai singleton
- `AddSession()`: Konfigurasi session untuk autentikasi (timeout 30 menit)

#### 3. **Environment Variables** (Opsional)

Untuk production, gunakan environment variables:

```bash
# Windows PowerShell
$env:MongoDB__ConnectionString="mongodb://localhost:27017"
$env:MongoDB__DatabaseName="GameTopupStoreDB"

# Linux/Mac
export MongoDB__ConnectionString="mongodb://localhost:27017"
export MongoDB__DatabaseName="GameTopupStoreDB"
```

### Database Configuration

#### MongoDB Collections

Aplikasi menggunakan 4 collections utama:

1. **Users** - Data pengguna (Admin & Customer)
2. **GameTopups** - Data produk game top-up
3. **Orders** - Data pesanan
4. **Carts** - Data keranjang belanja

#### Auto-Seed Data

Aplikasi akan otomatis menambahkan data sample saat pertama kali diakses jika database kosong:
- 34 item game top-up dari berbagai game
- Data akan ter-insert saat membuka halaman katalog

---

## 🏃 Cara Menjalankan Aplikasi

### Development Mode

```bash
# 1. Pastikan MongoDB berjalan
# Windows
net start MongoDB

# 2. Jalankan aplikasi
dotnet run

# 3. Buka browser
# http://localhost:5000 atau https://localhost:5001
```

### Production Mode

```bash
# 1. Publish aplikasi
dotnet publish -c Release -o ./publish

# 2. Jalankan dari folder publish
cd publish
dotnet GameTopupStore.dll
```

### Menggunakan Visual Studio

1. Buka `GameTopupStore.sln` di Visual Studio
2. Pastikan MongoDB service berjalan
3. Tekan `F5` atau klik "Run"
4. Aplikasi akan otomatis build dan run

---

## 📁 Struktur Proyek

```
GameTopupStore/
│
├── Controllers/              # Controller classes
│   ├── AccountController.cs      # Authentication & Registration
│   ├── AdminController.cs         # Admin panel management
│   ├── CustomerController.cs     # Customer features
│   ├── HomeController.cs         # Home page
│   └── TopupsController.cs       # Game topup management
│
├── Models/                   # Data models
│   ├── Cart.cs                   # Shopping cart model
│   ├── GameTopup.cs             # Game topup product model
│   ├── Order.cs                  # Order model
│   └── User.cs                   # User model
│
├── Services/                 # Business logic services
│   └── MongoDBService.cs         # MongoDB connection & operations
│
├── Views/                    # Razor views
│   ├── Admin/                    # Admin panel views
│   ├── Customer/                 # Customer views
│   ├── Account/                  # Authentication views
│   ├── Home/                     # Home page views
│   └── Shared/                   # Shared layouts & partials
│
├── wwwroot/                  # Static files
│   ├── css/                      # Stylesheets
│   ├── js/                       # JavaScript files
│   └── lib/                      # Third-party libraries
│
├── appsettings.json          # Application configuration
├── Program.cs                # Application entry point
└── GameTopupStore.csproj    # Project file
```

---

## 🔐 Default Accounts

Setelah pertama kali menjalankan aplikasi, Anda perlu membuat akun:

### Membuat Admin Account
1. Register akun baru melalui `/Account/Register`
2. Edit database MongoDB secara manual untuk mengubah role menjadi "Admin"
3. Atau gunakan script untuk membuat admin default

### Membuat Customer Account
1. Register akun baru melalui `/Account/Register`
2. Role otomatis menjadi "Customer"

---


## Mockup
### Dashboard Utama
![WhatsApp Image 2025-11-09 at 00 49 14_3b07dbb8](https://github.com/user-attachments/assets/205685e1-637b-4ad1-9af7-fe13dac12c2f)

### Register
![WhatsApp Image 2025-11-09 at 00 49 30_1a02ff4e](https://github.com/user-attachments/assets/dcbe0e00-4b90-491f-ae71-b9fbd663717a)

### Login
![WhatsApp Image 2025-11-09 at 00 53 41_687dcf31](https://github.com/user-attachments/assets/4e1509f8-fef1-45bd-bb52-9c399f2353cf)

### Katalog Game
![WhatsApp Image 2025-11-09 at 00 53 41_5ef4ab54](https://github.com/user-attachments/assets/12ced484-ea90-499c-9c25-c71117b93eb7)

### Keranjang Item Belanja
![WhatsApp Image 2025-11-09 at 00 53 41_ef8de6b0](https://github.com/user-attachments/assets/96e986d8-5208-4f8d-b349-b18e65177066)

### Checkout Item
![WhatsApp Image 2025-11-09 at 00 53 41_9d753f3b](https://github.com/user-attachments/assets/b26349ac-dc48-484c-a18d-a8bfefcae54f)

### Payment
![WhatsApp Image 2025-11-09 at 00 53 41_83a06bd7](https://github.com/user-attachments/assets/a6e538bf-13ad-4e5e-9192-07e1a91c295e)
![WhatsApp Image 2025-11-09 at 00 53 42_af3bebf7](https://github.com/user-attachments/assets/9940d6dc-f7c4-403d-b3d4-f6dffef5fafe)

### History Pesanan
![WhatsApp Image 2025-11-09 at 00 53 42_a8ba8a9a](https://github.com/user-attachments/assets/94d795d9-4279-4df1-8c13-66f45b8f51de)

### Informasi Akun
![WhatsApp Image 2025-11-09 at 01 06 36_0d076358](https://github.com/user-attachments/assets/6178bdb7-2ebc-4da8-900e-dfa3f1cbd4ad)


## 🎨 Features Detail

### Katalog Game
- Filter berdasarkan game type (MLBB, HOK, AOV, dll)
- Filter berdasarkan currency (Diamond, UC, Robux, dll)
- Search functionality
- Auto-load gambar default jika tidak ada image URL

### Shopping Cart
- Add to cart
- Update quantity
- Remove item
- Calculate total automatically

### Checkout & Payment
- Input game account ID/Username
- Input server game
- QRIS payment method
- Order confirmation

### Admin Panel
- Dashboard dengan statistik
- CRUD game top-up
- View orders
- Manage users

---

## 🐛 Troubleshooting

### MongoDB Connection Error
```
Error: Unable to connect to MongoDB
```
**Solusi:**
1. Pastikan MongoDB service berjalan
2. Cek connection string di `appsettings.json`
3. Untuk MongoDB Atlas, pastikan IP whitelist sudah benar

### Port Already in Use
```
Error: Address already in use
```
**Solusi:**
```bash
# Windows - Cek port yang digunakan
netstat -ano | findstr :5000

# Kill process
taskkill /PID <PID> /F

# Atau ubah port di launchSettings.json
```

### Build Errors
```
Error: Package restore failed
```
**Solusi:**
```bash
dotnet clean
dotnet restore
dotnet build
```

---

## 📝 License

This project is licensed under the MIT License.

---

## 👥 Contributors

- **Muhammad Fiqri Firmansyah** - Initial work

---

## 🙏 Acknowledgments

- MongoDB for the excellent database
- ASP.NET Core team for the amazing framework
- Bootstrap for the UI components
- Font Awesome for the icons

---

## 📞 Support

Jika ada pertanyaan atau masalah, silakan buat issue di GitHub repository ini.

---

**Happy Coding! 🚀**

