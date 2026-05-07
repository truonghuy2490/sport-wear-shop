```
SportWearShop.Web/
│
├── Pages/
│   ├── Index.cshtml
│   ├── Index.cshtml.cs
│   │
│   ├── Auth/
│   │   ├── Login.cshtml
│   │   ├── Login.cshtml.cs
│   │   ├── Register.cshtml
│   │   └── Register.cshtml.cs
│   │
│   ├── Products/
│   │   ├── Index.cshtml
│   │   ├── Index.cshtml.cs
│   │   ├── Details.cshtml
│   │   └── Details.cshtml.cs
│   │
│   ├── Cart/
│   │   ├── Index.cshtml
│   │   └── Index.cshtml.cs
│   │
│   ├── Checkout/
│   │   ├── Index.cshtml
│   │   └── Index.cshtml.cs
│   │
│   ├── Orders/
│   │   ├── Index.cshtml
│   │   ├── Index.cshtml.cs
│   │   ├── Details.cshtml
│   │   └── Details.cshtml.cs
│   │
│   ├── Profile/
│   │   ├── Index.cshtml
│   │   ├── Index.cshtml.cs
│   │   ├── Addresses.cshtml
│   │   └── Addresses.cshtml.cs
│   │
│   ├── Errors/
│   │   ├── Error.cshtml
│   │   ├── Error.cshtml.cs
│   │   ├── Unauthorized.cshtml
│   │   ├── Unauthorized.cshtml.cs
│   │   ├── Forbidden.cshtml
│   │   ├── Forbidden.cshtml.cs
│   │   ├── NotFound.cshtml
│   │   └── NotFound.cshtml.cs
│   │
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _Header.cshtml
│   │   ├── _Footer.cshtml
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Components/
│   │       ├── _ProductCard.cshtml
│   │       ├── _Pagination.cshtml
│   │       └── _Toast.cshtml
│   │
│   ├── _ViewStart.cshtml
│   └── _ViewImports.cshtml
│
├── Services/
│   ├── Interfaces/
│   │   ├── IProductApiService.cs
│   │   ├── IAuthApiService.cs
│   │   ├── ICartApiService.cs
│   │   ├── IOrderApiService.cs
│   │   └── IUserApiService.cs
│   │
│   └── Implementations/
│       ├── ProductApiService.cs
│       ├── AuthApiService.cs
│       ├── CartApiService.cs
│       ├── OrderApiService.cs
│       └── UserApiService.cs
│
├── Infrastructure/
│   ├── Api/
│   │   ├── ApiClient.cs
│   │   ├── ApiException.cs
│   │   ├── ApiSettings.cs
│   │   └── ApiEndpoints.cs
│   │
│   ├── Authentication/
│   │   ├── JwtCookieManager.cs
│   │   └── CurrentUserService.cs
│   │
│   └── HttpHandlers/
│       └── AuthHeaderHandler.cs
│
├── DIs/
│   └── DependencyInjection.cs
│
├── Helpers/
│   ├── MoneyHelper.cs
│   └── PaginationHelper.cs
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
│
├── appsettings.json
└── Program.cs
```