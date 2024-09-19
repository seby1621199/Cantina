import React, { createContext, useState, useContext } from 'react';
import { useSnackbar } from './SnackbarContext';
import { completePurchase } from '../Services/APIService';

export const CartContext = createContext();

export const CartProvider = ({ children }) => {
  const [cart, setCart] = useState([]);
  const [selfPickup, setSelfPickup] = useState(false);
  const showSnackbar = useSnackbar();

  const clearCart = async () => {
    console.log('cart golit');
    setCart([]);
  };

  const addToCart = (product) => {
    setCart((prevCart) => {
      const existingProduct = prevCart.find(item => item.productId === product.productId);
      if (existingProduct) {
        return prevCart.map(item =>
          item.productId === product.productId
            ? { ...item, quantity: item.quantity + 1 }
            : item
        );
      } else {
        return [...prevCart, { ...product, quantity: 1 }];
      }
    });
  };

  const handleCompletePurchase = async () => {
    const orderData = cart.map(item => ({
      id: item.productId,
      quantity: item.quantity
    }));

    const token = localStorage.getItem('token');
    try {
      await completePurchase(orderData, token, selfPickup);
      setCart([]);
      showSnackbar('Order completed successfully!', 'success');
    } catch (error) {
      console.error('Error completing order:', error);
      showSnackbar('Error completing the order. Please try again.', 'error');
    }
  };

  const updateQuantity = (productId, quantity) => {
    setCart((prevCart) => {
      if (quantity <= 0) {
        return prevCart.filter(item => item.productId !== productId);
      }
      return prevCart.map(item =>
        item.productId === productId
          ? { ...item, quantity: quantity }
          : item
      );
    });
  };

  const updateCartProduct = (updatedProduct) => {
    setCart((prevCart) => {
      const existingProduct = prevCart.find(item => item.productId === updatedProduct.id);
      if (existingProduct) {
        if (updatedProduct.active) {
          return prevCart.map(item =>
            item.productId === updatedProduct.id
              ? { ...item, ...updatedProduct }
              : item
          );
        } else {
          return prevCart.filter(item => item.productId !== updatedProduct.id);
        }
      }
      return prevCart;
    });
  };

  const setSelfPickupOption = (value) => {
    setSelfPickup(value);
  };

  return (
    <CartContext.Provider value={{ cart, addToCart, updateQuantity, completePurchase: handleCompletePurchase, selfPickup, setSelfPickupOption, updateCartProduct, clearCart }}>
      {children}
    </CartContext.Provider>
  );
};

export const useCart = () => {
  return useContext(CartContext);
};
