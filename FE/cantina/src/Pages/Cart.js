import React, { useContext } from 'react';
import { CartContext } from '../Contexts/CartContext';
import { Card, CardContent, CardActions, Typography, IconButton, TextField, Button, Box, FormControlLabel, Checkbox } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import SentimentVeryDissatisfiedIcon from '@mui/icons-material/SentimentVeryDissatisfied';
import RemoveIcon from '@mui/icons-material/Remove';
import DeleteIcon from '@mui/icons-material/Delete';
import { Margin } from '@mui/icons-material';

function Cart() {
    const { cart, updateQuantity, completePurchase, selfPickup, setSelfPickupOption } = useContext(CartContext);

    const handleIncrement = (productId, quantity, stock) => {
        if (quantity < stock && quantity) {
            updateQuantity(productId, quantity + 1);
        }
    };


    const handleDecrement = (productId, quantity) => {
        if (quantity > 1) {
            updateQuantity(productId, quantity - 1);
        } else {
            updateQuantity(productId, 0);
        }
    };

    const handleQuantityChange = (productId, stock, event) => {
        const newQuantity = parseInt(event.target.value, 10);
        if (newQuantity >= 1 && newQuantity <= stock) {
            updateQuantity(productId, newQuantity);
        }
    };

    const handleRemove = (productId) => {
        updateQuantity(productId, 0);
    };

    const handleSelfPickupChange = (event) => {
        setSelfPickupOption(event.target.checked);
    };

    const handleCompletePurchase = () => {
        completePurchase(selfPickup);
    };

    return (
        <div style={{ margin: 10 }}>
            <Typography variant="h4" gutterBottom>Cart</Typography>
            {cart.length > 0 ? (
                cart.map(product => (
                    <Card key={product.productId} sx={{ marginBottom: 2, background: "#f9f9f92b" }}>
                        <CardContent>
                            <Typography variant="h6">{product.name}</Typography>
                            <Typography variant="body2" color="text.secondary">
                                Price: {product.price} RON
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                Stock: {product.stock}
                            </Typography>
                        </CardContent>
                        <CardActions>
                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                <IconButton size="small" color="primary" onClick={() => handleDecrement(product.productId, product.quantity)}>
                                    <RemoveIcon />
                                </IconButton>
                                <TextField
                                    size="small"
                                    type="number"
                                    value={product.quantity}
                                    onChange={(event) => handleQuantityChange(product.productId, product.stock, event)}
                                    sx={{ width: 70, mx: 1 }}
                                    inputProps={{ min: 1, max: product.stock }}
                                />
                                {product.quantity && (
                                    <IconButton size="small" color="primary" onClick={() => handleIncrement(product.productId, product.quantity, product.stock)}>
                                        <AddIcon />
                                    </IconButton>
                                )}
                                <IconButton size="small" color="secondary" onClick={() => handleRemove(product.productId)}>
                                    <DeleteIcon />
                                </IconButton>
                            </Box>
                        </CardActions>
                    </Card>
                ))
            ) : (
                < >
                    <SentimentVeryDissatisfiedIcon />
                    <Typography variant="body1">Your cart is empty.</Typography>
                </>
            )}
            {cart.length > 0 && (
                <Box sx={{ marginTop: 2 }}>
                    <FormControlLabel
                        control={<Checkbox checked={selfPickup} onChange={handleSelfPickupChange} />}
                        label="Self Pickup"
                    />
                    <Button variant="contained" color="primary" onClick={handleCompletePurchase}>
                        Complete Purchase
                    </Button>
                </Box>
            )}

        </div>
    );
}

export default Cart;
