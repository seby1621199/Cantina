import React, { useContext, useEffect, useState } from 'react';
import { Navigate } from 'react-router-dom';
import { AuthContext } from '../Contexts/AuthContext';
import { CartContext } from '../Contexts/CartContext';
import { useSnackbar } from '../Contexts/SnackbarContext';

const LogoutPage = () => {
  const { logout } = useContext(AuthContext);
  const { clearCart } = useContext(CartContext);
  const showSnackbar = useSnackbar();

  useEffect(() => {
    clearCart();
    logout();
    showSnackbar('You have logged out!', 'success');

  }, [logout, clearCart]);

  return <Navigate to="/login" />;
};

export default LogoutPage;
