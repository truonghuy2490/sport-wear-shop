ReactJS thuần
Vite
Bootstrap
React Router
Axios
React Redux
---
localStorage/cookie:
- accessToken
- refreshToken

Redux:
- currentUser
- isAuthenticated
- role
- loading
- errorMessage
---
## Kiến trúc nên dùng

```txt
sport-wear-shop/
│
├── backend/
│   ├── SportWearShop.APIs
│   ├── SportWearShop.BusinessLogics
│   ├── SportWearShop.Infrastructure
│   ├── SportWearShop.Repositories
│   ├── SportWearShop.Shared
│   ├── sportwearshop.customer/        # Razor Pages
│   └── sportwearshop.admin/           # ReactJS Admin
│       ├── src/
│       │   ├── api/
│       │   │   ├── axiosClient.js
│       │   │   ├── authApi.js
│       │   │   ├── productApi.js
│       │   │   ├── accountApi.js
│       │   │   └── inventoryApi.js
│       │   │
│       │   ├── pages/
│       │   │   ├── auth/
│       │   │   │   └── LoginPage.jsx
│       │   │   ├── dashboard/
│       │   │   │   └── DashboardPage.jsx
│       │   │   ├── accounts/
│       │   │   ├── products/
│       │   │   ├── categories/
│       │   │   ├── brands/
│       │   │   ├── inventory/
│       │   │   └── orders/
│       │   │
│       │   ├── layouts/
│       │   │   ├── AdminLayout.jsx
│       │   │   ├── Sidebar.jsx
│       │   │   └── Header.jsx
│       │   │
│       │   ├── components/
│       │   │   ├── common/
│       │   │   ├── table/
│       │   │   ├── form/
│       │   │   └── modal/
│       │   │
│       │   ├── routes/
│       │   │   ├── AppRouter.jsx
│       │   │   └── ProtectedRoute.jsx
│       │   │
│       │   ├── hooks/
│       │   │   └── useAuth.js
│       │   │
│       │   ├── utils/
│       │   │   ├── tokenStorage.js
│       │   │   └── constants.js
│       │   │
│       │   ├── App.jsx
│       │   ├── main.jsx
│       │   └── index.css
│       │
│       ├── .env
│       ├── package.json
│       └── vite.config.js
│
├── docker-compose.yml
└── README.md
```

## Cài project cơ bản

```bash
cd frontend
npm create vite@latest sportwearshop.admin -- --template react
cd sportwearshop.admin
npm install
```

Vite hiện yêu cầu Node.js bản khá mới, ví dụ Node `20.19+` hoặc `22.12+`. ([vitejs][1])

Cài package cần thiết:

```bash
npm install react-router-dom
npm install axios
npm install bootstrap
```

Bootstrap có hướng dẫn chính thức để dùng với Vite, mình có thể import trực tiếp CSS/JS trong `main.jsx`. ([Bootstrap][2])

## Setup Bootstrap

Trong `src/main.jsx`:

```jsx
import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.jsx';

import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import './index.css';

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
```

## Setup React Router

```jsx
// src/App.jsx
import { BrowserRouter } from 'react-router-dom';
import AppRouter from './routes/AppRouter';

function App() {
  return (
    <BrowserRouter>
      <AppRouter />
    </BrowserRouter>
  );
}

export default App;
```

React Router dùng `BrowserRouter` để bọc app và quản lý routing phía client. ([React Router][3])

## Chạy project

```bash
npm run dev
```

Sau đó mở:

```txt
http://localhost:5173
```

## Kết luận

Với giai đoạn này, bạn nên đi theo stack:

```txt
ReactJS thuần
Vite
Bootstrap
React Router
Axios
```

Chưa cần Redux ngay. Khi admin bắt đầu lớn hơn, nhiều state dùng chung hơn như auth, role, sidebar, global loading, notification thì thêm **Redux Toolkit** sau vẫn được.

[1]: https://vite.dev/guide/?utm_source=chatgpt.com "Getting Started"
[2]: https://getbootstrap.com/docs/5.2/getting-started/vite/?utm_source=chatgpt.com "Bootstrap & Vite"
[3]: https://reactrouter.com/start/declarative/installation?utm_source=chatgpt.com "Installation"
