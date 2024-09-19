import React, { useState, useContext, useEffect } from 'react';
import { Button, Container, Box, Typography, List, ListItem, ListItemText, CircularProgress } from '@mui/material';
import { AuthContext } from '../Contexts/AuthContext';
import { getStatus, getOrders, startDelivery, finishOrder } from '../Services/APIService';

function Delivery() {
    const { token } = useContext(AuthContext);
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [status, setStatus] = useState('');
    const [ordersLoading, setOrdersLoading] = useState(false);

    const fetchStatus = async () => {
        try {
            const currentStatus = await getStatus(token);
            setStatus(currentStatus);

            if (currentStatus !== 'Ready') {
                await fetchOrders();
            }
        } catch (err) {
            setError('Failed to fetch status.');
            console.error('There was an error!', err);
        }
    };

    const fetchOrders = async () => {
        setOrdersLoading(true);
        setError('');
        try {
            const fetchedOrders = await getOrders(token);
            setOrders(fetchedOrders);
            if (fetchedOrders.length === 0) {
                setError('No orders available at the moment.');
            }
        } catch (err) {
            setError('Failed to fetch orders.');
            console.error('There was an error!', err);
        } finally {
            setOrdersLoading(false);
        }
    };

    const handleStartDelivery = async () => {
        setLoading(true);
        setError('');
        try {
            await startDelivery(token);
            await fetchStatus();
            await fetchOrders();
        } catch (err) {
            setError('Failed to start delivery.');
            console.error('There was an error!', err);
        } finally {
            setLoading(false);
        }
    };

    const handleGetOrders = async () => {
        await fetchOrders();
        await fetchStatus();
    };

    const handleFinishOrder = async (orderId) => {
        setLoading(true);
        setError('');
        try {
            await finishOrder(orderId, token);
            // Doar actualizăm statusul, nu mai apelăm automat fetchOrders
            await fetchStatus();
        } catch (err) {
            setError('Failed to finish order.');
            console.error('There was an error!', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchStatus();
    }, []);

    return (
        <Container>
            <Typography variant="h3" component="h3" gutterBottom sx={{ marginTop: "20px" }}>
                Delivery
            </Typography>

            {status === 'Ready' && (
                <Box>
                    <Typography variant="h6" component="h2" gutterBottom>
                        You are not currently delivering. Get orders!
                    </Typography>
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={handleGetOrders}
                        sx={{ mb: 2 }}
                        disabled={loading}
                    >
                        Get Orders
                    </Button>
                    {error && <Typography color="error">{error}</Typography>}
                </Box>
            )}

            {(status === 'Awaiting Pickup' || status === 'Delivery') && (
                <Box>
                    {ordersLoading && <CircularProgress />}
                    {error && <Typography color="error">{error}</Typography>}
                    {orders.length > 0 ? (
                        <Box>
                            <Typography variant="h6" component="h2" gutterBottom>
                                Orders:
                            </Typography>
                            <List>
                                {orders.map(order => (
                                    <ListItem key={order.id}>
                                        <ListItemText
                                            primary={`Name: ${order.name || 'N/A'}`}
                                            secondary={`Location: ${order.location || 'N/A'}, Phone: ${order.userPhone || 'N/A'}, Verification Code: ${order.verificationCode || 'N/A'}, Status: ${order.status || 'N/A'}`}
                                        />
                                        {status === 'Delivery' && order.status === 'Delivery' && (
                                            <Button
                                                variant="contained"
                                                color="success"
                                                onClick={() => handleFinishOrder(order.id)}
                                            >
                                                Finish Order
                                            </Button>
                                        )}
                                    </ListItem>
                                ))}
                            </List>

                            {status === 'Awaiting Pickup' && (
                                <>
                                    <Typography variant="h6" component="h2" gutterBottom>
                                        Pick up orders from the canteen.
                                    </Typography>
                                    <Button
                                        variant="contained"
                                        color="primary"
                                        onClick={handleStartDelivery}
                                        sx={{ mt: 2 }}
                                        disabled={loading}
                                    >
                                        Start Delivery
                                    </Button>
                                </>
                            )}
                        </Box>
                    ) : (
                        <></>
                    )}
                </Box>
            )}

            {status !== 'Ready' && status !== 'Awaiting Pickup' && status !== 'Delivery' && (
                <Typography variant="h6" component="h2" gutterBottom>
                    No delivery actions available at the moment.
                </Typography>
            )}
        </Container>
    );
}

export default Delivery;
