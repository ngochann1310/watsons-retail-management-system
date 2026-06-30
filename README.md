# Sales Management Information System for Watsons Cosmetics Retail Store Chain 

## 1. Introduction
This project is a Management Information System (MIS) designed for the Watsons cosmetic retail chain. It aims to synchronize inventory across branches, automate sales reporting, and optimize internal management processes through a centralized database.

### Key Features:
*   **Sales Management:** Order processing, invoicing, and multi-method payment handling.
*   **Inventory Management:** Real-time stock tracking with low-stock and expiration alerts.
*   **CRM & HRM:** Management of customer profiles (loyalty points, membership tiers) and employee records.
*   **Analytics & Reporting:** Interactive dashboards for visualizing revenue, top-selling products, and stock status.
*   **System Utilities:** Integrated **AI Chatbot** for quick data lookup and **Data Backup & Restore** tools.
### Main Modules:
- Dashboard
- Sales Management
- Inventory Management
- Product Management
- Supplier Management
- Customer Management
- Employee Management
- Promotion Management
- Reporting & Analytics
- Backup & Restore
- AI Chatbot

## 2. Technical Stack
*   **Language:** C# (Object-Oriented Programming).
*   **Framework:** .NET Framework (WinForms).
*   **Database:** Microsoft SQL Server (T-SQL).
*   **Reporting:** Microsoft RDLC (Report Definition Language Client).

### NuGet Packages:
*   `Guna.UI2.WinForms`: For modern and responsive UI design.
*   `Guna.Charts.WinForms`: For interactive data visualization on Dashboards.
*   `Microsoft.ReportingServices.ReportViewerControl.Winforms`: For displaying and exporting reports.
*   `Newtonsoft.Json`: For data handling and AI Chatbot processing.

## 3. Installation & Requirements
### Prerequisites:
To run this project, please ensure the following are installed:
1.  **Microsoft Visual Studio** (with WinForms support).
2.  **SQL Server & SQL Server Management Studio (SSMS)**.
3.  **Microsoft RDLC Report Designer Extension** (for VS).

### Steps to Set up:
1.  **Database:** Initialize the database using one of the following methods:
- Execute the SQL script located in the `/database` folder.
- Restore the provided `.bak` database backup using SQL Server Management Studio (SSMS).
2.  **Configuration:** Open the `.sln` file in Visual Studio. Update the `App.config` file's connection string to match your SQL Server instance.
3.  **Build:** NuGet packages will be automatically restored upon building. Press `F5` to start the application.

## 4. Test Credentials
The system uses role-based access control. You can use the following accounts for testing:

| Role | Username | Password |
| :--- | :--- | :--- |
| **Quản trị viên** | `le_tran` | `Qtv1234@` |
| **Nhân viên bán hàng** | `an_nguyen` | `Nvbh123@` |
| **Nhân viên kho** | `thanh_ngo` | `Nvk1234@` |
| **Quản lý cửa hàng** | `tuan_bui` | `Qlch123@` |
| **Quản lý kho** | `huong_hoang` | `Qlk1234@` |

> **Note:** Username format is `firstname_lastname` (lowercase, no accents).

## 5. Usage 
*   **Dashboard:** View real-time KPIs and access the AI Chatbot for quick queries.
*   **Sales:** Go to "Orders" -> "Create New" -> Add Products -> "Payment" -> "Invoice".
*   **Inventory:** Manage receipts and stock adjustments in the "Inventory Management" module.
*   **Data Safety:** Access the "Backup & Restore" menu to safeguard system data.

## 6. Project Structure
*   `/source`: WinForms source code and application assets.
*   `/database`: SQL scripts and backup (.bak) files.
*   `/document`: Software Requirements Specification (SRS) and project documentation.

## 7. Author
*   Quách Ngọc Hân.

## 8. License
*  This project was developed for educational and portfolio purposes only.
*  Commercial use is not permitted without the author's permission.
