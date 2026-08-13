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
