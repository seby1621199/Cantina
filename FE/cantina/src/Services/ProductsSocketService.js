import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import axios from 'axios';

export function initializeSignalR(onProductUpdated) {
  const getToken = () => localStorage.getItem('token');

  const connection = new HubConnectionBuilder()
    .withUrl("https://localhost:7076/orderHub", {
      accessTokenFactory: getToken
    })
    .configureLogging(LogLevel.Information)
    .withAutomaticReconnect({
      nextRetryDelayInMilliseconds: (retryContext) => {
        if (retryContext.previousRetryCount === 0) {
          return 0;
        }
        return Math.min(10000, retryContext.previousRetryCount * 1000);
      }
    })
    .build();

  connection.on("ReceiveNotificationProductUpdate", async (productId) => {
    try {
      const token = getToken();
      const response = await axios.get(`https://localhost:7076/product/getById?id=${productId}`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      const updatedProduct = response.data;
      onProductUpdated(updatedProduct);
    } catch (error) {
      console.error("Error fetching updated product:", error);
    }
  });

  connection.start()
    .then(() => console.log("SignalR Connected"))
    .catch(err => console.error("SignalR Connection Error: ", err));

  connection.onclose(async (error) => {
    console.log("SignalR Disconnected: ", error);
  });

  return connection;
}
