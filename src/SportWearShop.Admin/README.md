# SportWearShop Admin Manager

Admin management system for SportWearShop, built with ReactJS and integrated with ASP.NET Core Web API backend.

---

## Overview

SportWearShop Admin is an internal dashboard used by administrators and staff to manage:

- Authentication / Authorization
- Product Management
- Product Variant Management
- Category Management
- Brand Management
- Inventory Management
- Customer Account Management
- Order Management
- Dashboard Monitoring

This frontend communicates with the SportWearShop backend APIs using JWT authentication with refresh token support.

---

## Tech Stack

### Frontend
- ReactJS
- Vite
- Bootstrap 5
- React Router DOM
- Axios
- React Redux

### Authentication
- JWT Access Token
- Refresh Token
- Role-based Authorization

### Storage
- localStorage / cookie
    - accessToken
    - refreshToken

---

## Redux Global State

```js
{
  currentUser,
  isAuthenticated,
  role,
  loading,
  errorMessage
}
```

---

## Project Structure

```txt
sportwearshop.admin/
│
├── src/
│   ├── api/
│   ├── pages/
│   ├── layouts/
│   ├── components/
│   ├── routes/
│   ├── hooks/
│   ├── utils/
│   ├── redux/
│   ├── App.jsx
│   └── main.jsx
```

---

## Core Features

### Authentication
- Login
- Logout
- Auto refresh token
- Protected routes
- Role-based access control

### Dashboard
- Overview statistics
- Quick navigation shortcuts
- Business summary

### Product Management
- Product list
- Product detail
- Create product
- Update product
- Soft delete / deactivate
- Publish product

### Product Variant Management
- Add single variant
- Batch create variants
- Update variant
- Publish variant
- Activate / deactivate variant

### Category Management
- Category CRUD
- Parent / child category
- Category hierarchy

### Brand Management
- Brand CRUD
- Brand active / inactive
- Pagination
- Search

### Inventory Management
- Stock overview
- Inventory movements
- Stock adjustment
- Product stock tracking

### Account Management
- Admin / Staff account management
- Role assignment
- User profile management

### Order Management
- Order listing
- Order detail
- Status management

---

## Installation

### Prerequisites

- Node.js >= 20
- npm
- backend API running

### Install

```bash
npm install
```

### Run

```bash
npm run dev
```

Default:

```txt
http://localhost:5173
```

---

## Environment Variables

Example `.env`

```env
VITE_API_BASE_URL=http://localhost:5068/api
```

---

## Authentication Flow

1. User login
2. Backend returns:
   - accessToken
   - refreshToken
3. Store tokens
4. Axios interceptor attaches access token
5. If 401:
   - call refresh token API
   - retry original request
6. If refresh fails:
   - logout
   - redirect login

---

## Route Protection

Protected routes use:

- authentication check
- role validation

Example:

- Admin
- Staff

Unauthorized users are redirected.

---

## Error Handling

Handled scenarios:

- 401 Unauthorized
- Invalid refresh token
- API validation errors
- Conflict errors (409)
- Not found routes
- network errors

---

## Future Improvements

- Toast notification system
- Better dashboard analytics
- Implement with orders
- Responsive admin improvements
