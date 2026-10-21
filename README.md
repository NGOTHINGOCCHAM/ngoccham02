# DemoMVC – Thực hành ASP.NET Core MVC

Dự án thực hành ASP.NET Core MVC trên .NET 10. Mỗi buổi học tương ứng với một commit.

## Chạy dự án

```bash
dotnet run
```

Sau đó mở http://localhost:5294. Trong VS Code: mở thư mục dự án và nhấn `F5`.

## Buổi 1 – Tìm hiểu môi trường lập trình

| Công cụ | Ghi chú |
|---|---|
| [.NET SDK 10](https://dotnet.microsoft.com/download) | Kiểm tra bằng `dotnet --version` |
| [Visual Studio Code](https://code.visualstudio.com) | Trình soạn thảo code |
| C# Dev Kit (`ms-dotnettools.csdevkit`) | Extension cho C#, tự cài kèm C# (`ms-dotnettools.csharp`) và .NET Install Tool |

File [.vscode/extensions.json](.vscode/extensions.json) khai báo sẵn các extension, VS Code sẽ gợi ý cài khi mở dự án.

Tóm tắt trên website: menu **Buổi 1** (`/Home/Environment`).

Các lệnh thường dùng:

| Lệnh | Ý nghĩa |
|---|---|
| `dotnet new mvc -n DemoMVC` | Tạo dự án MVC mới |
| `dotnet restore` | Tải các package |
| `dotnet build` | Biên dịch |
| `dotnet run` | Chạy ứng dụng |
| `dotnet watch` | Chạy và tự tải lại khi sửa code |
| `dotnet publish -c Release -o ./publish` | Đóng gói để triển khai |

## Buổi 2 – Tìm hiểu về CI/CD

- **CI** (Continuous Integration): mỗi lần push code, hệ thống tự build để phát hiện lỗi sớm.
- **CD** (Continuous Deployment): build thành công thì tự triển khai lên hosting.

Website: https://ngoccham.runasp.net (hosting MonsterASP.NET).

Tóm tắt trên website: menu **Buổi 2** (`/Home/CiCd`).

Dự án dùng **GitHub Actions**, cấu hình ở [.github/workflows/deploy.yml](.github/workflows/deploy.yml). Mỗi lần push lên `main`:

1. Cài .NET 10 → `restore` → `build` → `publish`.
2. Upload bản publish lên MonsterASP.NET bằng **WebDeploy** (action `rasmusbuchholdt/simply-web-deploy`). Action tự dừng ứng dụng trước khi ghi đè file rồi chạy lại.

Với pull request, workflow chỉ build để kiểm tra, không deploy.

Thư mục `bin/`, `obj/` là kết quả build nên được loại khỏi git bằng [.gitignore](.gitignore).

### Đăng ký hosting và domain

1. Đăng ký tài khoản [MonsterASP.NET](https://www.monsterasp.net) (có gói miễn phí) và tạo website. Hosting cấp sẵn tên miền phụ miễn phí dạng `ten-website.runasp.net`.
2. Chọn phiên bản **.NET 10** cho website trong Control Panel.
3. Bật **HTTPS** (chứng chỉ SSL miễn phí) cho tên miền trong Control Panel.
4. Muốn dùng tên miền riêng (.com, .vn…): mua tên miền, trỏ DNS về hosting rồi khai báo tên miền trong Control Panel.

### Kết nối GitHub với hosting

1. Control Panel → **Websites** → chọn website → **Deploy** → thẻ **Visual Studio access**: bật **WebDeploy**, ghi lại các giá trị Server, Site, Login, Password.
2. Repo GitHub → **Settings → Secrets and variables → Actions → New repository secret**, thêm 4 secret:

| Secret | Giá trị |
|---|---|
| `WEBSITE_NAME` | Site, dạng `siteXXXX` |
| `SERVER_COMPUTER_NAME` | `https://<Server>:8172`, ví dụ `https://siteXXXX.siteasp.net:8172` |
| `SERVER_USERNAME` | Login, dạng `siteXXXX` |
| `SERVER_PASSWORD` | Password của WebDeploy |

Từ đó mỗi lần `git push` lên `main`, website tự được cập nhật. Theo dõi tiến trình ở tab **Actions** của repo.

## Buổi 3 – Tìm hiểu kiến trúc .NET MVC

Chạy ứng dụng và mở menu **Buổi 3** (`/Home/Architecture`, code ở [Views/Home/Architecture.cshtml](Views/Home/Architecture.cshtml)): vai trò của Model, View, Controller, luồng xử lý một request, các quy ước đặt tên và cấu trúc thư mục dự án.

## Buổi 4 – Tìm hiểu về định tuyến trong .NET MVC

| Nội dung | Code | URL demo |
|---|---|---|
| Cấu trúc URL `{controller}/{action}/{id?}` | [RoutingController](Controllers/RoutingController.cs) | `/Routing`, `/Routing/Detail/5`, `/Routing/Search?keyword=mvc` |
| Lỗi khi truy cập URL (404, 405, lỗi model binding) | `UseStatusCodePagesWithReExecute` trong [Program.cs](Program.cs), action `StatusCodePage` trong [HomeController](Controllers/HomeController.cs) | Mục 2 của trang `/Routing` |
| Định tuyến với `[Route]` | [CatalogController](Controllers/CatalogController.cs) | `/san-pham`, `/san-pham/2`, `/san-pham/danh-muc/laptop`, `/khuyen-mai` |

## Buổi 5 – Tìm hiểu về gửi nhận dữ liệu giữa M-V-C

| Nội dung | Code | URL demo |
|---|---|---|
| Controller, View, Model | [Controllers/](Controllers/), [Views/](Views/), [Models/](Models/) | |
| Gửi nhận dữ liệu giữa Controller và View: `ViewBag`, tham số action, form POST, query string | [DataController](Controllers/DataController.cs), [Views/Data/Index.cshtml](Views/Data/Index.cshtml) | `/Data`, `/Data/Hello?name=An` |
| Gửi nhận dữ liệu qua Model: strongly-typed view, model binding, kiểm tra dữ liệu bằng Data Annotations | [Student](Models/Student.cs), [StudentController](Controllers/StudentController.cs), [Views/Student/](Views/Student/) | `/Student`, `/Student/Create` |

## Buổi 6 – Tìm hiểu về quản lý trạng thái

Code ở [StateController](Controllers/StateController.cs) và [Views/State/](Views/State/), demo tại `/State`.

| | ViewData | ViewBag | TempData |
|---|---|---|---|
| Kiểu | `ViewDataDictionary` (key-value) | `dynamic` | `ITempDataDictionary` (key-value) |
| Ép kiểu khi đọc | Cần | Không cần | Cần |
| Phạm vi | Một request | Một request | Đến khi được đọc, giữ được qua redirect |

ViewBag và ViewData dùng chung một kho dữ liệu. TempData bị xoá sau khi đọc, trừ khi gọi `Keep()` hoặc đọc bằng `Peek()`.

## Buổi 7 – Tìm hiểu về Layout

Tóm tắt trên website: menu **Buổi 7** (`/Home/LayoutDemo`).

- Layout dùng chung: [Views/Shared/_Layout.cshtml](Views/Shared/_Layout.cshtml), được chọn trong [Views/_ViewStart.cshtml](Views/_ViewStart.cshtml). Menu chia theo từng buổi học, dùng navbar và dropdown của Bootstrap.
- **Tích hợp Bootstrap 5** (5.3.3, nằm trong [wwwroot/lib/bootstrap](wwwroot/lib/bootstrap)): nhúng `bootstrap.min.css` trong `<head>` và `bootstrap.bundle.min.js` cuối `<body>` của Layout.
- **Tách file CSS và tích hợp vào Layout**: CSS của Layout (header, menu, footer) nằm ở [wwwroot/css/layout.css](wwwroot/css/layout.css), được nhúng vào Layout bằng thẻ `<link>`. Trang chủ có CSS riêng [wwwroot/css/home.css](wwwroot/css/home.css), nhúng qua `@section Styles` (Layout gọi `RenderSectionAsync("Styles")`).

## Buổi 8 (16/9) – Kết nối cơ sở dữ liệu

Tóm tắt trên website: menu **Buổi 8** (`/Home/Database`), có hiển thị trạng thái kết nối CSDL.

1. Package cùng phiên bản với .NET: `Microsoft.EntityFrameworkCore.SqlServer` và `Microsoft.EntityFrameworkCore.Tools` (10.0.13). Công cụ dòng lệnh: `dotnet tool install -g dotnet-ef`.
2. Model [Product](Models/Product.cs) gồm `Id`, `Name`, `Price`, `CreatedAt`. [ApplicationDbContext](Data/ApplicationDbContext.cs) có `DbSet<Product>`, `Price` đặt `HasPrecision(18, 2)`.
3. Chuỗi kết nối `DefaultConnection` trong [appsettings.json](appsettings.json), đăng ký `AddDbContext` + `UseSqlServer` trong [Program.cs](Program.cs).
4. Tạo bảng: `dotnet ef migrations add InitialCreate` rồi `dotnet ef database update` (code migration nằm trong [Migrations/](Migrations/)).

Chuỗi kết nối mặc định dùng LocalDB, chỉ chạy trên Windows. Trên macOS/Linux, trỏ tới SQL Server khác bằng user-secrets (không ghi mật khẩu vào code):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True"
dotnet ef database update
```

Demo định tuyến `[Route]` của buổi 4 được đổi tên thành `CatalogController` (URL `/san-pham` giữ nguyên) để dành tên `Product` cho bài này.
