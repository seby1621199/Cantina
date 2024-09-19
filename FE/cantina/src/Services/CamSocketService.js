import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

export function initializeSignalR(onCountsReceived) {
  const getToken = () => localStorage.getItem('token');

  const connection = new HubConnectionBuilder()
    .withUrl("http://localhost:80/camHub", {
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

  connection.on("ReceiveCounts", (cntUp, cntDown) => {
    onCountsReceived(cntUp, cntDown);
  });

  connection.start()
    .then(() => console.log("SignalR Connected"))
    .catch(err => console.error("SignalR Connection Error: ", err));

  connection.onclose(async (error) => {
    console.log("SignalR Disconnected: ", error);
  });

  return connection;
}
