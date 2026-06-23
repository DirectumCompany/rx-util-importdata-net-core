# Карта проекта: Утилита импорта данных для Directum RX

## Назначение

Утилита для массового импорта исторических данных в систему **Directum RX** через OData API.
Поддерживает два режима работы: **CLI** (консольное приложение) и **Web UI** (Blazor WASM + ASP.NET Core).

---

## Структура репозитория

```
rx-util-importdata-net-core/
├── README.md                   ← Описание проекта
├── PROJECT_MAP.md              ← Этот файл
├── doc/                        ← Документация (PDF, DOCX)
├── Templates/                  ← Excel-шаблоны для импорта (~30 .xlsx файлов)
│   └── Example/
├── Install/                    ← Скрипты установки
└── src/                        ← Исходный код решения
    ├── ImportData.sln          ← Главный Solution
    ├── ImportData/             ← Основной проект (CLI-утилита)
    ├── Tests/                  ← Проект с тестами (xUnit)
    └── Client/                 ← Web UI обвязка
        ├── Client.sln
        ├── ClientApp/          ← Blazor WebAssembly фронтенд
        └── ImportUtilServer/   ← ASP.NET Core хост + API
```

---

## Проекты Solution

| Проект | Тип | .NET | Назначение |
|---|---|---|---|
| `ImportData` | Console App / Class Lib | net6.0 | Ядро — логика импорта через OData |
| `Tests` | xUnit Test Project | net6.0 | Интеграционные тесты |
| `ClientApp` | Blazor WebAssembly | net6.0 | Веб-интерфейс (SPA) |
| `ImportUtilServer` | ASP.NET Core Web | net6.0 | Хост для Blazor + REST API |

---

## Архитектура

```
┌─────────────────────────────────────────────────────────┐
│                      Web UI Mode                         │
│  ┌─────────────┐   HTTP   ┌──────────────────────────┐  │
│  │  ClientApp  │ ◄──────► │   ImportUtilServer       │  │
│  │ Blazor WASM │          │  ASP.NET Core 6          │  │
│  │ (MudBlazor) │          │  ImportController        │  │
│  └─────────────┘          └──────────┬───────────────┘  │
└─────────────────────────────────────┼───────────────────┘
                                      │ uses
┌─────────────────────────────────────▼───────────────────┐
│                    ImportData (Core)                      │
│  ┌─────────────┐  ┌────────────┐  ┌──────────────────┐  │
│  │ExcelProcessor│  │BusinessLogic│  │EntityProcessor   │  │
│  │(OpenXml)    │  │            │  │                  │  │
│  └──────┬──────┘  └─────┬──────┘  └────────┬─────────┘  │
│         └───────────────▼──────────────────┘             │
│                  ┌──────────────┐                        │
│                  │OData Client  │                        │
│                  │(BatchClient) │                        │
│                  └──────┬───────┘                        │
└─────────────────────────┼───────────────────────────────┘
                          │ OData HTTP
                          ▼
                   Directum RX Services
```

---

## Детальная структура `src/ImportData/`

```
ImportData/
├── Program.cs                  ← Точка входа CLI; парсинг аргументов (NDesk.Options)
├── Constants.cs                ← Константы и названия действий импорта
├── Structures.cs               ← Структуры данных
├── nlog.config                 ← Конфигурация логирования (NLog)
├── _ConfigSettings.xml         ← Настройки подключения к RX (не в git)
├── _ConfigSettings.xml.example ← Пример конфига
├── start.bat / start.sh        ← Скрипты запуска (Windows / Linux)
│
├── Entities/                   ← Доменные сущности
│   ├── Entity.cs               ← Базовый абстрактный класс
│   │
│   ├── Databooks/              ← Справочники (reference data)
│   │   ├── AssociatedApplication.cs
│   │   ├── BusinessUnit.cs
│   │   ├── CaseFile.cs
│   │   ├── Company.cs          ← Контрагенты
│   │   ├── ConfigurationItem*.cs ← CMDB / IT-активы
│   │   ├── Contact.cs
│   │   ├── ContractCategory.cs
│   │   ├── Country.cs
│   │   ├── Currency.cs
│   │   ├── Department.cs
│   │   ├── DocumentKind.cs
│   │   ├── DocumentRegister.cs
│   │   ├── Employee.cs
│   │   ├── IEntityWithImage.cs ← Интерфейс для сущностей с фото
│   │   ├── Login.cs
│   │   ├── Person.cs
│   │   ├── Role.cs
│   │   ├── Service.cs          ← ESM: Услуги
│   │   ├── ServiceCategory.cs  ← ESM: Категории услуг
│   │   └── Substitution.cs
│   │
│   └── EDocs/                  ← Электронные документы
│       ├── DocumentEntity.cs   ← Базовый класс документа
│       ├── Addendum.cs
│       ├── CompanyDirective.cs
│       ├── Contract.cs
│       ├── ContractStatement.cs
│       ├── IncomingLetter.cs
│       ├── Order.cs
│       ├── OutgoingLetter.cs
│       ├── OutgoingLetterAddressees.cs
│       ├── SimpleDocument.cs
│       ├── SupAgreement.cs     ← Доп. соглашения
│       ├── UniversalTransferDocument.cs ← УПД
│       └── Waybill.cs
│
├── Logic/                      ← Бизнес-логика
│   ├── BusinessLogic.cs        ← Оркестрация процесса импорта
│   ├── EntityProcessor.cs      ← Обработка сущностей
│   ├── EntityWrapper.cs        ← Обёртка над сущностью
│   └── ExcelProcessor.cs       ← Чтение XLSX (DocumentFormat.OpenXml)
│
└── IntegrationServicesClient/  ← Клиент к OData API Directum RX
    ├── Client.cs               ← Базовый OData-клиент
    ├── BatchClient.cs          ← Пакетные операции
    ├── ConfigSettingsService.cs
    ├── Attr.cs                 ← Кастомные атрибуты (PropertyOptions, EntityName)
    ├── Exceptions/
    │   ├── FoundMatchesException.cs
    │   └── WellKnownKeyNotFoundException.cs
    └── Models/                 ← OData-интерфейсы (~80+ файлов)
        ├── IEntity.cs / IEntityBase.cs
        ├── ICompanies.cs / IContracts.cs / IDepartments.cs ...
        ├── IESMServices.cs / IESMServiceCategories.cs
        └── ICMDBConfigurationItems.cs ...
```

---

## Детальная структура `src/Client/`

```
Client/
├── ClientApp/                  ← Blazor WebAssembly
│   ├── Program.cs              ← Точка входа WASM
│   ├── App.razor
│   ├── _Imports.razor
│   │
│   ├── Pages/                  ← Razor-страницы
│   │   ├── Index.razor
│   │   ├── Import.razor        ← Главная страница импорта
│   │   ├── UIVariablesView.razor
│   │   ├── BlazorMonacoScroll.razor
│   │   └── Controls/           ← Переиспользуемые компоненты
│   │       ├── BooleanControl.razor
│   │       ├── DirectoryControl.razor
│   │       ├── EnumControl.razor
│   │       ├── FileControl.razor
│   │       ├── GroupHeaderControl.razor
│   │       ├── LabelControl.razor
│   │       ├── LeftPanel.razor
│   │       ├── MultipleFileControl.razor
│   │       ├── PasswordControl.razor
│   │       ├── StringControl.razor
│   │       ├── TooltipControl.razor
│   │       └── UIVariableControl.razor
│   │
│   ├── Services/
│   │   ├── RefreshService.cs
│   │   └── PerformanceTimer.cs
│   ├── Store/
│   │   └── InstallStore.cs     ← Управление состоянием
│   ├── Api/
│   │   └── CommonApi.cs        ← HTTP-клиент к ImportUtilServer
│   ├── Models/
│   │   ├── ClientAppMessages.cs
│   │   ├── ControlState.cs
│   │   ├── TooltipType.cs
│   │   └── UIModels.cs
│   ├── Utils/
│   │   ├── EnumUtils.cs
│   │   └── StringUtils.cs
│   └── wwwroot/
│       ├── index.html
│       ├── css/ (app.css, editor.css)
│       └── js/ (polyfill.js)
│
└── ImportUtilServer/           ← ASP.NET Core хост
    ├── Program.cs              ← Конфигурация сервера
    ├── Controllers/
    │   └── ImportController.cs ← REST endpoints для запуска импорта
    ├── Models/
    │   └── Import.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    └── appsettings.Production.json
```

---

## Тесты `src/Tests/`

```
Tests/
├── Common.cs           ← Вспомогательные утилиты
├── TestSettings.cs     ← Настройки тестовой среды
├── _ConfigSettings.xml ← Конфиг подключения для тестов
│
├── Databooks/          ← Тесты импорта справочников
│   ├── 1_Company.cs    ← Контрагент (единственная запись)
│   ├── 2_Companies.cs  ← Контрагенты (массовый)
│   ├── 3_Persons.cs
│   ├── 4_Logins.cs
│   └── 5_Contacts.cs
│
├── EDocs/              ← Тесты импорта документов
│   ├── 0_CaseFile.cs
│   ├── 1_Contracts.cs
│   ├── 2_Addendums.cs
│   ├── 3_SupAgreements.cs
│   ├── 4_IncomingLetters.cs
│   ├── 5_OutgoingLetters.cs
│   ├── 7_Orders.cs
│   ├── 8_CompanyDirectives.cs
│   └── 9_SimpleDocument.cs
│
└── Templates/          ← Тестовые XLSX-файлы (~20 шт.)
    └── TestDocs/
        └── testDoc.txt
```

---

## Поддерживаемые операции импорта

### Справочники (Databooks)

| Сущность | Класс | Описание |
|---|---|---|
| Контрагенты | `Company` | Организации и ИП |
| Физ. лица | `Person` | Контактные лица |
| Сотрудники | `Employee` | Пользователи системы |
| Подразделения | `Department` | Оргструктура |
| Организации | `BusinessUnit` | Наши юр. лица |
| Логины | `Login` | Учётные записи |
| Контакты | `Contact` | Контакты контрагентов |
| Роли | `Role` | Роли пользователей |
| Замещения | `Substitution` | Замещение сотрудников |
| Страны | `Country` | Справочник стран |
| Валюты | `Currency` | Справочник валют |
| Виды договоров | `ContractCategory` | Категории договоров |
| Виды документов | `DocumentKind` | Виды документов |
| Журналы регистрации | `DocumentRegister` | Регистрационные журналы |
| Прикладные модули | `AssociatedApplication` | Обработчики почты |
| Дела | `CaseFile` | Номенклатура дел |
| ЭД. услуги (ESM) | `Service` | Услуги ESM |
| Категории услуг (ESM) | `ServiceCategory` | Категории услуг ESM |
| Конф. единицы (CMDB) | `ConfigurationItem*` | IT-активы CMDB |

### Электронные документы (EDocs)

| Сущность | Класс | Описание |
|---|---|---|
| Договоры | `Contract` | Договоры с контрагентами |
| Доп. соглашения | `SupAgreement` | Доп. соглашения к договорам |
| Приложения | `Addendum` | Приложения к договорам |
| Акты | `ContractStatement` | Акты выполненных работ |
| Входящие письма | `IncomingLetter` | Входящая корреспонденция |
| Исходящие письма | `OutgoingLetter` | Исходящая корреспонденция |
| Приказы | `Order` | Приказы |
| Распоряжения | `CompanyDirective` | Распоряжения |
| Простые документы | `SimpleDocument` | Произвольные документы |
| Накладные | `Waybill` | Товарные накладные |
| УПД | `UniversalTransferDocument` | Универсальные передаточные документы |

---

## Ключевые зависимости

### ImportData (Core)

| Пакет | Версия | Назначение |
|---|---|---|
| `DocumentFormat.OpenXml` | 2.13.0 | Чтение XLSX |
| `Simple.OData.Client` | 5.21.0 | OData-клиент |
| `Microsoft.OData.Core` | 7.10.0 | OData протокол |
| `NLog` + `NLog.Extensions.Logging` | 4.7.10 | Логирование |
| `ConfigSettings` | 1.0.11 | Конфигурация (XML) |
| `NDesk.Options.Core` | 1.2.5 | Парсинг CLI-аргументов |
| `Microsoft.Extensions.DependencyInjection` | 5.0.1 | DI контейнер |

### ClientApp (Blazor WASM)

| Пакет | Версия | Назначение |
|---|---|---|
| `MudBlazor` | 6.11.0 | UI-компоненты |
| `BlazorMonaco` | 3.1.0 | Встроенный редактор кода |
| `NJsonSchema` | 10.9.0 | Работа с JSON Schema |
| `YamlDotNet` | 13.2.0 | Парсинг YAML |

### ImportUtilServer (ASP.NET Core)

| Пакет | Версия | Назначение |
|---|---|---|
| `Swashbuckle.AspNetCore` | 6.5.0 | Swagger / OpenAPI |
| `Microsoft.AspNetCore.Components.WebAssembly.Server` | 6.0.9 | Хостинг WASM |

### Tests (xUnit)

| Пакет | Версия | Назначение |
|---|---|---|
| `xunit` | 2.4.2 | Тестовый фреймворк |
| `Xunit.Extensions.Ordering` | 1.4.5 | Упорядочивание тестов |

---

## Ключевые паттерны проектирования

- **Template Method** — `Entity.SaveToRX()` определяет алгоритм, подклассы (`Databooks`, `EDocs`) переопределяют шаги
- **Inheritance Hierarchy** — `Entity` → `Databooks` / `DocumentEntity` → конкретные типы
- **Reflection + Attributes** — маппинг свойств через кастомные атрибуты `PropertyOptions`, `EntityName`
- **Batch Operations** — `BatchClient` для пакетной работы с OData API
- **Configuration-Driven** — XML-конфиг определяет поведение без перекомпиляции

---

## Точки входа и запуск

### CLI-режим

```bash
# Windows
start.bat [аргументы]

# Linux
./start.sh [аргументы]

# Прямой вызов
dotnet ImportData.dll --action=ImportCompanies --template=companies.xlsx
```

### Web UI режим

```bash
cd src/Client/ImportUtilServer
dotnet run
# → http://localhost:5000
```

---

## Конфигурация (`_ConfigSettings.xml`)

```xml
<!-- Пример конфигурации подключения к Directum RX -->
<ConfigSettings>
  <ServiceUrl>http://rx-server/sungero/odata/</ServiceUrl>
  <Login>admin</Login>
  <Password>password</Password>
  <!-- ... другие параметры ... -->
</ConfigSettings>
```

---

## Документация (`/doc`)

| Файл | Описание |
|---|---|
| `Directum RX Утилита импорта данных...pdf` | Описание шаблона решения |
| `Утилита импорта 4.1. Инструкция по загрузке данных.pdf` | Руководство пользователя |
| `ESM Инструкция по использованию...docx` | Инструкция для ESM-модуля |
| `Утилита импорта 4.1. Описание тестов.docx` | Описание тестов |
