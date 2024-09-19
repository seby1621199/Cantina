import React, { useContext } from 'react';
import { Card, CardMedia, CardContent, CardActions, Typography, Button, Box, IconButton, TextField } from '@mui/material';
import { CartContext } from '../Contexts/CartContext';
import AddIcon from '@mui/icons-material/Add';
import RemoveIcon from '@mui/icons-material/Remove';

function Product({ productId, name, price, photo, stock }) {
  const { cart, addToCart, updateQuantity } = useContext(CartContext);
  const cartItem = cart.find(item => item.productId === productId);

  const handleAddToCart = () => {
    const product = { productId, name, price, photo, stock };
    addToCart(product);
  };

  const handleIncrement = () => {
    if (cartItem.quantity < stock) {
      updateQuantity(productId, cartItem.quantity + 1);
    }
  };

  const handleDecrement = () => {
    if (cartItem.quantity > 1) {
      updateQuantity(productId, cartItem.quantity - 1);
    } else {
      updateQuantity(productId, 0);
    }
  };

  const handleQuantityChange = (event) => {
    const newQuantity = parseInt(event.target.value, 10);
    if (newQuantity >= 1 && newQuantity <= stock) {
      updateQuantity(productId, newQuantity);
    } else if (newQuantity < 1) {
      updateQuantity(productId, 0);
    }
  };

  return (
    <Card sx={{ maxWidth: 345, margin: 2 }}>
      <CardMedia
        component="img"
        height="140"
        image={photo}
        alt={name}
      />
      <CardContent sx={{ justifyContent: 'center', padding: "20px" }}>
        <Typography gutterBottom variant="h5" component="div">
          {name}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Stock: {stock}
        </Typography>
        <Typography sx={{ marginTop: 10 }} variant="h9" color="primary">
          {price} RON
        </Typography>
      </CardContent>
      <CardActions sx={{ display: 'flex', justifyContent: 'center' }}>
        {cartItem ? (
          <Box sx={{ display: 'flex', alignItems: 'center' }}>
            <IconButton size="small" color="primary" onClick={handleDecrement}>
              <RemoveIcon />
            </IconButton>
            <TextField
              size="small"
              type="number"
              value={cartItem.quantity}
              onChange={handleQuantityChange}
              sx={{ width: 53, mx: 1 }}
              inputProps={{ min: 1, max: stock }}
            />
            {cartItem.quantity < stock && (
              <IconButton size="small" color="primary" onClick={handleIncrement}>
                <AddIcon />
              </IconButton>
            )}
          </Box>
        ) : (
          <Button size="small" color="primary" onClick={handleAddToCart}>
            Add to Cart
          </Button>
        )}
      </CardActions>
    </Card>
  );
}

export default Product;
