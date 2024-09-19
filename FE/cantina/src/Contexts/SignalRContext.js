import React, { createContext, useState, useEffect, useContext } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useSnackbar } from './SnackbarContext';

const SignalRContext = createContext();

export const SignalRProvider = ({ children }) => {
  const [signalRConnection, setSignalRConnection] = useState(null);
  const showSnackbar = useSnackbar();

  useEffect(() => {
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

    connection.on("OrderStatusChanged", async (orderNotification) => {
        console.log(orderNotification);
      showSnackbar(`Status order ${orderNotification.orderId} changed: ${orderNotification.status}`, 'info');
    });

    connection.start()
      .then(() => {
        console.log("SignalR Connected");
        setSignalRConnection(connection);
      })
      .catch(err => console.error("SignalR Connection Error: ", err));

    connection.onclose(async (error) => {
      console.log("SignalR Disconnected: ", error);
    });

    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, [showSnackbar]);

  return (
    <SignalRContext.Provider value={{ signalRConnection }}>
      {children}
    </SignalRContext.Provider>
  );
};

export const useSignalR = () => useContext(SignalRContext);
