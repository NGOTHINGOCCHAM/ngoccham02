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
