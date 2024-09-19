import React, { useState, useEffect } from 'react';
import { fetchProducts } from '../Services/APIService';
import Product from '../Components/Product';
import { initializeSignalR } from '../Services/ProductsSocketService';
import { useCart } from '../Contexts/CartContext';

function Store() {
  const [products, setProducts] = useState([]);
  const [allProducts, setAllProducts] = useState([]);
  const { updateCartProduct } = useCart();

  useEffect(() => {
    fetchProducts()
      .then(data => {
        setProducts(data);
        setAllProducts(data);
      })
      .catch(error => {
        console.error("There was an error!", error);
      });

    const signalRConnection = initializeSignalR((updatedProduct) => {
      setProducts(prevProducts => {
        const existingProduct = prevProducts.find(product => product.id === updatedProduct.id);
        if (existingProduct) {
          return prevProducts.map(product =>
            product.id === updatedProduct.id ? updatedProduct : product
          );
        } else {
          return [...prevProducts, updatedProduct];
        }
      });

      setAllProducts(prevAllProducts => {
        const existingProduct = prevAllProducts.find(product => product.id === updatedProduct.id);
        if (existingProduct) {
          return prevAllProducts.map(product =>
            product.id === updatedProduct.id ? updatedProduct : product
          );
        } else {
          return [...prevAllProducts, updatedProduct];
        }
      });
      updateCartProduct(updatedProduct);
    });

    return () => {
      signalRConnection.stop().catch(err => console.error("SignalR disconnection error: ", err));
    };
  }, [updateCartProduct]);

  const activeProducts = allProducts.filter(product => product.active);

  return (
    <div style={{ display: "flex", flexWrap: "wrap", justifyContent: "center" }}>
      {activeProducts.map(product => (
        <Product
          key={product.id}
          productId={product.id}
          name={product.name}
          price={product.price}
          photo="https://media.istockphoto.com/id/1472680285/photo/healthy-meal-with-grilled-chicken-rice-salad-and-vegetables-served-by-woman.webp?b=1&s=170667a&w=0&k=20&c=D4EsPmVWVJTM3sdZbA141EE53yVVxVmYPOGbiSNIP6M="
          stock={product.stock}
        />
      ))}
    </div>
  );
}

export default Store;
