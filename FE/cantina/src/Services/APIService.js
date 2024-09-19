import axios from 'axios';

const BASE_URL = 'https://localhost:7076';

export const loginUser = async (email, password) => {
  try {
    const response = await axios.post(`${BASE_URL}/user/login`, { email, password });
    return response.data;
  } catch (error) {
    throw new Error('Login failed: ' + error.message);
  }
};

export const registerUser = async (userData) => {
  try {
    const response = await axios.post(`${BASE_URL}/user/register`, userData);
    return response.status;
  } catch (error) {
    throw new Error('Registration failed: ' + (error.response?.data || error.message));
  }
};

export const fetchProducts = async () => {
  try {
    const response = await axios.get(`${BASE_URL}/product/getAll?withIds=true`);
    return response.data;
  } catch (error) {
    throw new Error('Failed to fetch products: ' + error.message);
  }
};

export const completePurchase = async (orderData, token, selfPickup) => {
  const url = `${BASE_URL}/order/add${selfPickup ? '?selfPickup=true' : ''}`;
  try {
    const response = await axios.post(url, orderData, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response;
  } catch (error) {
    throw new Error('Error completing order: ' + error.message);
  }
};

export const fetchOrders = async (token) => {
  try {
    const response = await axios.get(`${BASE_URL}/order/getAll`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response.data;
  } catch (error) {
    throw new Error('Error fetching orders: ' + error.message);
  }
};

export const getVerificationCode = async (orderId, token) => {
  try {
    const response = await axios.get(`${BASE_URL}/getVerificationCode?orderId=${orderId}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response.data;
  } catch (error) {
    throw new Error('Error fetching verification code: ' + error.message);
  }
};

export const getStatus = async (token) => {
  const response = await axios.get(`${BASE_URL}/getStatus`, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  return response.data;
};

export const getOrders = async (token) => {
  const response = await axios.get(`${BASE_URL}/getOrders`, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  return response.data;
};

export const startDelivery = async (token) => {
  await axios.post(`${BASE_URL}/start`, null, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
};

export const finishOrder = async (orderId, token) => {
  await axios.post(`${BASE_URL}/finish?orderId=${orderId}`, null, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
};
