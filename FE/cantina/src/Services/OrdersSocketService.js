import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useSnackbar } from '../Contexts/SnackbarContext';

const showSnackbar = useSnackbar();

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

  connection.on("ReceiveFinishKitchenNotification", async (orderId) => {
    showSnackbar(`Order ${orderId} get ready to delivery`, 'success');
  });

  connection.start()
    .then(() => console.log("SignalR Connected"))
    .catch(err => console.error("SignalR Connection Error: ", err));

  connection.onclose(async (error) => {
    console.log("SignalR Disconnected: ", error);
  });

  return connection;
}
