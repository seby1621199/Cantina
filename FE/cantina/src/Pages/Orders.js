import React, { useEffect, useState } from 'react';
import { Container, Paper, Typography, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Dialog, DialogActions, DialogContent, DialogTitle, Button, CircularProgress } from '@mui/material';
import { fetchOrders, getVerificationCode } from '../Services/APIService';

const Orders = () => {
  const [orders, setOrders] = useState([]);
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [verificationCode, setVerificationCode] = useState(null);
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadOrders = async () => {
      const token = localStorage.getItem('token');
      try {
        const orderData = await fetchOrders(token);
        setOrders(orderData);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    loadOrders();
  }, []);

  const handleClickOpen = async (order) => {
    setSelectedOrder(order);
    const token = localStorage.getItem('token');
    if (order.status === 'SelfPickup') {
      try {
        const code = await getVerificationCode(order.id, token);
        setVerificationCode(code);
      } catch (error) {
        console.error(error);
        setVerificationCode('Error fetching code');
      }
    }
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setSelectedOrder(null);
    setVerificationCode(null);
  };

  return (
    <Container sx={{ marginTop: 4 }}>
      <Typography variant="h4" gutterBottom>
        Orders
      </Typography>
      {loading ? (
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '400px' }}>
          <CircularProgress />
          <Typography variant="h7" sx={{ marginLeft: 2, fontWeight: 'semibold' }}>Loading orders</Typography>
        </div>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead sx={{ backgroundColor: "#1976d2" }}>
              <TableRow>
                <TableCell sx={{ color: "white", fontWeight: "semibold" }}>Order ID</TableCell>
                <TableCell sx={{ color: "white" }}>Order Date</TableCell>
                <TableCell sx={{ color: "white" }}>Products</TableCell>
                <TableCell sx={{ color: "white" }}>Total Price</TableCell>
                <TableCell sx={{ color: "white" }}>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {orders.map((order) => (
                <TableRow key={order.id} onClick={() => handleClickOpen(order)} style={{ cursor: 'pointer' }}>
                  <TableCell>{order.id}</TableCell>
                  <TableCell>{new Date(order.orderDate).toLocaleDateString()}</TableCell>
                  <TableCell>
                    {order.orderItems.length > 2 ? (
                      <>
                        {order.orderItems.slice(0, 2).map((item, index) => (
                          <div key={index}>{item.productName}</div>
                        ))}
                        <div>+{order.orderItems.length - 2} more</div>
                      </>
                    ) : (
                      order.orderItems.map((item, index) => (
                        <div key={index}>{item.productName}</div>
                      ))
                    )}
                  </TableCell>
                  <TableCell>{order.totalPrice}</TableCell>
                  <TableCell>{order.status}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>Order Details</DialogTitle>
        <DialogContent>
          {selectedOrder && (
            <div>
              <Typography variant="h6">Order ID: {selectedOrder.id}</Typography>
              <Typography variant="body1">Order Date: {new Date(selectedOrder.orderDate).toLocaleDateString()}</Typography>
              <Typography variant="body1">Status: {selectedOrder.status}</Typography>
              <Typography variant="body1">Total Price: {selectedOrder.totalPrice} RON</Typography>
              <Typography variant="body1">Products:</Typography>
              {selectedOrder.orderItems.map((item, index) => (
                <div key={index}>
                  {item.productName} - {item.quantity} x {item.unitPrice} RON = {item.totalPrice} RON
                </div>
              ))}
              {selectedOrder.status === 'SelfPickup' && (
                <Typography variant="body1">Code: {verificationCode || 'No code found'}</Typography>
              )}
            </div>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color="primary">
            Close
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default Orders;
