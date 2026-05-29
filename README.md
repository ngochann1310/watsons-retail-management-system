# Watsons Retail Management System (WatsonsRMS)
**Xây Dựng Hệ Thống Thông Tin Quản Lý Bán Hàng Cho Chuỗi Cửa Hàng Bán Lẻ Mỹ Phẩm Watsons** 

## 1. Introduction (Giới thiệu)
This project is a Management Information System (MIS) designed for the Watsons cosmetic retail chain. It aims to synchronize inventory across branches, automate sales reporting, and optimize internal management processes through a centralized database.

### Key Features (Tính năng chính):
*   **Sales Management:** Order processing, invoicing, and multi-method payment handling.
*   **Inventory Management:** Real-time stock tracking with low-stock and expiration alerts.
*   **CRM & HRM:** Management of customer profiles (loyalty points, membership tiers) and employee records.
*   **Analytics & Reporting:** Interactive dashboards for visualizing revenue, top-selling products, and stock status.
*   **System Utilities:** Integrated **AI Chatbot** for quick data lookup and **Data Backup & Restore** tools.

## 2. Technical Stack (Công nghệ sử dụng)
*   **Language:** C# (Object-Oriented Programming).
*   **Framework:** .NET Framework (WinForms).
*   **Database:** Microsoft SQL Server (T-SQL).
*   **Reporting:** Microsoft RDLC (Report Definition Language Client).

### NuGet Packages (Thư viện hỗ trợ):
*   `Guna.UI2.WinForms`: For modern and responsive UI design.
*   `Guna.Charts.WinForms`: For interactive data visualization on Dashboards.
*   `Microsoft.ReportingServices.ReportViewerControl.Winforms`: For displaying and exporting reports.
*   `Newtonsoft.Json`: For data handling and AI Chatbot processing.

## 3. Installation & Requirements (Cài đặt và Yêu cầu)
### Prerequisites (Yêu cầu hệ thống):
To run this project, please ensure the following are installed:
1.  **Microsoft Visual Studio** (with WinForms support).
2.  **SQL Server & SQL Server Management Studio (SSMS)**.
3.  **Microsoft RDLC Report Designer Extension** (for VS).

### Steps to Setup (Các bước thiết lập):
1.  **Database:** Open SSMS and run the SQL script found in the `/db` folder to initialize the database schema and sample data.
2.  **Configuration:** Open the `.sln` file in Visual Studio. Update the `App.config` file's connection string to match your SQL Server instance.
3.  **Build:** NuGet packages will be automatically restored upon building. Press `F5` to start the application.

## 4. Test Credentials (Thông tin đăng nhập)
The system uses role-based access control. You can use the following accounts for testing:

| Role (Phân quyền) | Username (Tên đăng nhập) | Password (Mật khẩu) |
| :--- | :--- | :--- |
| **Administrator (Quản trị viên)** | `le_tran` | `Qtv1234@` |
| **Sales Staff (NV Bán hàng)** | `an_nguyen` | `Nvbh123@` |
| **Warehouse Staff (NV Kho)** | `thanh_ngo` | `Nvk1234@` |
| **Store Manager (Quản lý)** | `tuan_bui` | `Qlch123@` |
| **Warehouse Manager (QL Kho)** | `huong_hoang` | `Qlk1234@` |

> **Note:** Username format is `firstname_lastname` (lowercase, no accents).

## 5. Usage (Hướng dẫn sử dụng)
*   **Dashboard:** View real-time KPIs and access the AI Chatbot for quick queries.
*   **Sales:** Go to "Orders" -> "Create New" -> Add Products -> "Payment" -> "Invoice".
*   **Inventory:** Manage receipts and stock adjustments in the "Inventory Management" module [24].
*   **Data Safety:** Access the "Backup & Restore" menu to safeguard system data.

## 6. Project Structure (Cấu trúc dự án)
*   `/src`: C# source code and Assets (Giao diện và xử lý nghiệp vụ).
*   `/db`: SQL scripts and `.bak` files for database setup (Cơ sở dữ liệu).

## 7. Contributors (Người thực hiện)
*   **Student:** Quách Ngọc Hân.
*   **Supervisor:** Th.S. Trương Xuân Hương.
