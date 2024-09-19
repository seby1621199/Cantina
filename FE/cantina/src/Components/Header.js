import React, { useContext, useState, useEffect } from 'react';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import Badge from '@mui/material/Badge';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import { Link } from 'react-router-dom';
import { AuthContext } from '../Contexts/AuthContext';
import { CartContext } from '../Contexts/CartContext';
import ShoppingBagIcon from '@mui/icons-material/ShoppingBag';
import PersonIcon from '@mui/icons-material/Person';
import MenuBookIcon from '@mui/icons-material/MenuBook';
import RestaurantIcon from '@mui/icons-material/Restaurant';
import DeliveryDiningIcon from '@mui/icons-material/DeliveryDining';
import { initializeSignalR } from '../Services/CamSocketService';

const Header = () => {
    const { userRoles, logout } = useContext(AuthContext);
    const { cart } = useContext(CartContext);
    const [anchorEl, setAnchorEl] = useState(null);
    const [cntUp, setCntUp] = useState(0);
    const [cntDown, setCntDown] = useState(0);

    useEffect(() => {
        const connection = initializeSignalR((up, down) => {
            setCntUp(up);
            setCntDown(down);
        });

        return () => {
            connection.stop();
        };
    }, []);

    const handleMenu = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const exit = () => {
        handleClose();
        logout();
    };

    return (
        <AppBar position="static">
            <Toolbar>
                <Typography
                    variant="h6"
                    sx={{ flexGrow: 1, fontStyle: "none", textDecoration: "none", color: "white" }}
                    component={Link}
                    to="/order"
                >
                    Cantina
                </Typography>
                <Typography
                    component="span"
                    sx={{
                        fontSize: '0.875rem',
                        color: 'white',
                        ml: 1
                    }}
                >
                    {cntDown - cntUp} People Inside
                </Typography>
                {userRoles.includes('Delivery') && (
                    <IconButton color="inherit" component={Link} to="/delivery">
                        <DeliveryDiningIcon />
                    </IconButton>
                )}
                {userRoles.includes('User') && (
                    <IconButton color="inherit" component={Link} to="/order">
                        <RestaurantIcon />
                    </IconButton>
                )}

                <IconButton color="inherit" component={Link} to="/menu">
                    <MenuBookIcon />
                </IconButton>

                {cart.length > 0 && (
                    <IconButton color="inherit" component={Link} to="/cart">
                        <Badge badgeContent={cart.length} color="secondary">
                            <ShoppingBagIcon />
                        </Badge>
                    </IconButton>
                )}
                {userRoles.length > 0 && (
                    <>
                        <IconButton
                            color="inherit"
                            onClick={handleMenu}
                        >
                            <PersonIcon />
                        </IconButton>
                        <Menu
                            anchorEl={anchorEl}
                            open={Boolean(anchorEl)}
                            onClose={handleClose}
                        >
                            <MenuItem component={Link} to="/orders" onClick={handleClose}>Orders</MenuItem>
                            <MenuItem component={Link} to="/logout" onClick={handleClose} >Logout</MenuItem>
                        </Menu>
                    </>
                )}
            </Toolbar>
        </AppBar>
    );
};

export default Header;
