# ADLAS Academy Guest Manager / نظام إدارة ضيوف أكاديمية أدلس

A bilingual ASP.NET Core MVC web application branded for ADLAS Academy to manage academy guests and exhibition visitors.

تطبيق ويب ثنائي اللغة باستخدام ASP.NET Core MVC بهوية أكاديمية أدلس لإدارة ضيوف الأكاديمية وزوار المعارض.

## English

### Overview
ADLAS Academy Guest Manager is the ADLAS Academy branded version of the Exhibition Guest Manager project. It provides a modern bilingual interface for managing guests with Arabic/English localization and RTL/LTR support.

### Main Features
- Secure login using ASP.NET Core Identity
- Customers/guests CRUD
- Search and filtering
- Pagination
- Excel export
- Arabic/English localization
- RTL/LTR layout support
- Modern Bootstrap 5 UI
- Clean layered architecture

### Tech Stack
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server
- Bootstrap 5
- ClosedXML

### Solution Structure
- `ExhibitionGuestManager.Domain`
- `ExhibitionGuestManager.Application`
- `ExhibitionGuestManager.Infrastructure`
- `ExhibitionGuestManager.UI`

### Login
Credentials are seeded for authorized internal use. Do not publish shared credentials in client-facing materials.

### How to Run
1. Install the .NET 8 SDK.
2. Restore packages:

```bash
dotnet restore
```

3. Configure the SQL Server connection string in `appsettings.json` or user secrets.
4. Before running the app, make sure the SQL Server database configured in `appsettings.json` exists and is accessible by the current Windows/user account. This version does not include EF Core migrations.
5. Build the solution:

```bash
dotnet build ExhibitionGuestManager.sln
```

6. Run the MVC UI project:

```bash
dotnet run --project src/ExhibitionGuestManager.UI/ExhibitionGuestManager.UI.csproj
```

### Launch URLs
- [https://localhost:5001](https://localhost:5001)
- [http://localhost:5000](http://localhost:5000)

### Language Support
- Arabic and English are available from the navbar.
- Arabic uses RTL.
- English uses LTR.

### Screenshots
- Login page
- Customers list
- Create/Edit customer

### Architecture Notes
- MVC web app only
- No API controllers
- No Repository Pattern
- No CQRS
- No MediatR

### Company Version Note
This is the ADLAS Academy branded company version.

## العربية

### نبذة
نظام إدارة ضيوف أكاديمية أدلس هو النسخة المخصصة بهوية أكاديمية أدلس من مشروع إدارة ضيوف المعارض. يوفر واجهة حديثة ثنائية اللغة لإدارة الضيوف مع دعم العربية والإنجليزية واتجاهي RTL/LTR.

### المزايا الرئيسية
- تسجيل دخول آمن باستخدام ASP.NET Core Identity
- إدارة كاملة للضيوف / العملاء
- البحث والتصفية
- ترقيم الصفحات
- تصدير إلى Excel
- دعم اللغتين العربية والإنجليزية
- دعم اتجاه RTL و LTR
- واجهة حديثة باستخدام Bootstrap 5
- بنية طبقية نظيفة

### التقنيات المستخدمة
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server
- Bootstrap 5
- ClosedXML

### هيكل الحل
- `ExhibitionGuestManager.Domain`
- `ExhibitionGuestManager.Application`
- `ExhibitionGuestManager.Infrastructure`
- `ExhibitionGuestManager.UI`

### بيانات الدخول
بيانات الدخول مخصصة للاستخدام الداخلي المصرح به. لا تنشر بيانات دخول مشتركة في مواد تسليم العميل.

### طريقة التشغيل
1. تثبيت .NET 8 SDK.
2. استعادة الحزم:

```bash
dotnet restore
```

3. ضبط سلسلة الاتصال الخاصة بـ SQL Server في `appsettings.json` أو عبر user secrets.
4. قبل تشغيل التطبيق، تأكد من أن قاعدة بيانات SQL Server المحددة في `appsettings.json` موجودة ويمكن للمستخدم الحالي الوصول إليها. هذه النسخة لا تحتوي على EF Core migrations.
5. بناء الحل:

```bash
dotnet build ExhibitionGuestManager.sln
```

6. تشغيل مشروع الواجهة:

```bash
dotnet run --project src/ExhibitionGuestManager.UI/ExhibitionGuestManager.UI.csproj
```

### روابط التشغيل
- [https://localhost:5001](https://localhost:5001)
- [http://localhost:5000](http://localhost:5000)

### دعم اللغات
- يمكن التبديل بين العربية والإنجليزية من الشريط العلوي.
- العربية تستخدم RTL.
- الإنجليزية تستخدم LTR.

### أماكن الصور التوضيحية
- صفحة تسجيل الدخول
- قائمة الضيوف
- صفحة إضافة / تعديل ضيف

### ملاحظات معمارية
- تطبيق MVC فقط
- لا توجد API Controllers
- لا يوجد Repository Pattern
- لا يوجد CQRS
- لا يوجد MediatR

### ملاحظة نسخة الشركة
هذه هي النسخة المخصصة بهوية أكاديمية أدلس.
