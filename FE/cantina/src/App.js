import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import ProtectedRoute from './Components/PrivateRoute';
import Admin from './Pages/Admin';
import Login from './Pages/Login';
import LogoutPage from './Pages/LogoutPage';
import UnauthorizedPage from './Pages/UnauthorizedPage';
import Store from "./Pages/Store";
import MenuPage from './Pages/MenuPage';
import Header from './Components/Header';
import Orders from './Pages/Orders';
import Cart from './Pages/Cart';
import { AuthProvider } from './Contexts/AuthContext';
import { CartProvider } from './Contexts/CartContext';
import Delivery from './Pages/Delivery';
import { SnackbarProvider } from './Contexts/SnackbarContext';
import { SignalRProvider } from './Contexts/SignalRContext';

const App = () => {
  return (
    <SnackbarProvider>
      <AuthProvider>
        <SignalRProvider>
          <CartProvider>
            <Router>
              <Header />
              <Routes>
                <Route path="*" element={<Navigate to="/order" />} />
                <Route path="/unauthorized" element={<UnauthorizedPage />} />
                <Route path="/login" element={<Login />} />
                <Route path="/logout" element={<LogoutPage />} />
                <Route path="/admin" element={<ProtectedRoute allowedRoles={['Admin', 'User']} component={Admin} />} />
                <Route path="/order" element={<ProtectedRoute allowedRoles={['User']} component={Store} />} />
                <Route path="/delivery" element={<ProtectedRoute allowedRoles={['Delivery']} component={Delivery} />} />
                <Route path="/cart" element={<ProtectedRoute allowedRoles={['User']} component={Cart} />} />
                <Route path="/orders" element={<ProtectedRoute allowedRoles={['User']} component={Orders} />} />
                <Route path="/menu" element={<MenuPage />} />
                <Route path="/owner" element={<ProtectedRoute allowedRoles={['Owner']} component={Admin} />} />
              </Routes>
            </Router>
          </CartProvider>
        </SignalRProvider>
      </AuthProvider>
    </SnackbarProvider>
  );
};

export default App;
