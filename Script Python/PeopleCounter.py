import numpy as np
import cv2 as cv
import Person
import time
import asyncio
from signalrcore.hub_connection_builder import HubConnectionBuilder

## NOTA:
# Acest program a fost modificat pentru a trimite spre Serverul Web numarul de persoane din cladire
# placand de la Scriptul initial "https://github.com/Gupu25/PeopleCounter", codul din acest github repository nu imi apartine.
# Hub SignalR
hub_url = "http://localhost:80/camHub"
# Variabile pentru SignalR
hub_connection = None
message_interval = 5  # Interval
cnt_up = 0
cnt_down = 0
# Initializare SignalR
async def setup_signalr_connection():
    global hub_connection
    # Crearea conexiunii la hub-ul SignalR
    hub_connection = HubConnectionBuilder() \
        .with_url(hub_url) \
        .build()
    hub_connection.start()
    await asyncio.sleep(2)

# Functie pentru trimiterea mesajelor
async def send_message_to_server(cnt_up, cnt_down):
    if hub_connection:
        try:
            hub_connection.send("UpdateCounts", [cnt_up, cnt_down])
        except Exception as e:
            print(f"Eroare la trimiterea mesajului: {e}")

async def main():
    global cnt_up, cnt_down
    
    # Deschide fișierul log
    try:
        log = open('log.txt', "w")
    except Exception as e:
        print("Nu se poate deschide fișierul log.", e)
        return

    # Sursa video
    #cap = cv.VideoCapture('rtsp://192.168.0.106:8080/h264_pcm.sdp')
    cap = cv.VideoCapture('t2.mp4')
    if not cap.isOpened():
        print("Nu s-a putut deschide fișierul video.")
        log.close()
        return

    # Proprietăți video
    h = 480
    w = 640
    frameArea = h * w
    areaTH = frameArea / 250
    print('Area Threshold', areaTH)

    # Linii de intrare/ieșire
    line_up = int(2 * (h / 5))
    line_down = int(3 * (h / 5))

    up_limit = int(1 * (h / 5))
    down_limit = int(4 * (h / 5))

    print("Red line y:", str(line_down))
    print("Blue line y:", str(line_up))

    line_down_color = (255, 0, 0)
    line_up_color = (0, 0, 255)

    pt1 = [0, line_down]
    pt2 = [w, line_down]
    pts_L1 = np.array([pt1, pt2], np.int32)
    pts_L1 = pts_L1.reshape((-1, 1, 2))

    pt3 = [0, line_up]
    pt4 = [w, line_up]
    pts_L2 = np.array([pt3, pt4], np.int32)
    pts_L2 = pts_L2.reshape((-1, 1, 2))

    pt5 = [0, up_limit]
    pt6 = [w, up_limit]
    pts_L3 = np.array([pt5, pt6], np.int32)
    pts_L3 = pts_L3.reshape((-1, 1, 2))

    pt7 = [0, down_limit]
    pt8 = [w, down_limit]
    pts_L4 = np.array([pt7, pt8], np.int32)
    pts_L4 = pts_L4.reshape((-1, 1, 2))

    # Substractor de fundal
    fgbg = cv.createBackgroundSubtractorMOG2(detectShadows=True)

    # Elementele structurante pentru filtre morfologice
    kernelOp = np.ones((3, 3), np.uint8)
    kernelOp2 = np.ones((5, 5), np.uint8)
    kernelCl = np.ones((11, 11), np.uint8)

    # Variabile
    font = cv.FONT_HERSHEY_SIMPLEX
    persons = []
    max_p_age = 5
    pid = 1

    # Inițializează conexiunea SignalR
    await setup_signalr_connection()

    # Inițializează variabilele pentru cronometrare
    last_message_time = time.time()

    try:
        while cap.isOpened():
            # Citește un frame din sursa video
            ret, frame = cap.read()
            if not ret:
                print("Eroare la citirea frame-ului.")
                break

            for i in persons:
                i.age_one() 

            
            # Aplica substracția de fundal
            fgmask = fgbg.apply(frame)
            fgmask2 = fgbg.apply(frame)

            # Binarizare pentru a elimina umbrele (culoare gri)
            try:
                ret, imBin = cv.threshold(fgmask, 200, 255, cv.THRESH_BINARY)
                ret, imBin2 = cv.threshold(fgmask2, 200, 255, cv.THRESH_BINARY)
                mask = cv.morphologyEx(imBin, cv.MORPH_OPEN, kernelOp)
                mask2 = cv.morphologyEx(imBin2, cv.MORPH_OPEN, kernelOp)
                mask = cv.morphologyEx(mask, cv.MORPH_CLOSE, kernelCl)
                mask2 = cv.morphologyEx(mask2, cv.MORPH_CLOSE, kernelCl)
            except Exception as e:
                print("Eroare la procesarea imaginii:", e)
                print('UP:', cnt_up)
                print('DOWN:', cnt_down)
                break

            contours0, _ = cv.findContours(mask2, cv.RETR_EXTERNAL, cv.CHAIN_APPROX_SIMPLE)
            for cnt in contours0:
                area = cv.contourArea(cnt)
                if area > areaTH:
                
                    M = cv.moments(cnt)
                    cx = int(M['m10'] / M['m00'])
                    cy = int(M['m01'] / M['m00'])
                    x, y, w, h = cv.boundingRect(cnt)

                    new = True
                    if cy in range(up_limit, down_limit):
                        for i in persons:
                            if abs(x - i.getX()) <= w and abs(y - i.getY()) <= h:
                                # Obiectul este aproape de unul care a fost detectat înainte
                                new = False
                                i.updateCoords(cx, cy)  # Actualizează coordonatele obiectului și resetează vârsta
                                if i.going_UP(line_down, line_up):
                                    cnt_up += 1
                                    print("ID:", i.getId(), 'a trecut sus la', time.strftime("%c"))
                                    log.write("ID: " + str(i.getId()) + ' a trecut sus la ' + time.strftime("%c") + '\n')
                                elif i.going_DOWN(line_down, line_up):
                                    cnt_down += 1
                                    print("ID:", i.getId(), 'a trecut jos la', time.strftime("%c"))
                                    log.write("ID: " + str(i.getId()) + ' a trecut jos la ' + time.strftime("%c") + '\n')
                                break
                            if i.getState() == '1':
                                if i.getDir() == 'down' and i.getY() > down_limit:
                                    i.setDone()
                                elif i.getDir() == 'up' and i.getY() < up_limit:
                                    i.setDone()
                            if i.timedOut():
                                # Elimină i din lista persons
                                index = persons.index(i)
                                persons.pop(index)
                                del i  # Eliberează memoria ocupată de i
                        if new:
                            p = Person.MyPerson(pid, cx, cy, max_p_age)
                            persons.append(p)
                            pid += 1

                    cv.circle(frame, (cx, cy), 5, (0, 0, 255), -1)
                    cv.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)

            for i in persons:
                cv.putText(frame, str(i.getId()), (i.getX(), i.getY()), font, 0.3, i.getRGB(), 1, cv.LINE_AA)

            str_up = 'UP: ' + str(cnt_up)
            str_down = 'DOWN: ' + str(cnt_down)
            frame = cv.polylines(frame, [pts_L1], False, line_down_color, thickness=2)
            frame = cv.polylines(frame, [pts_L2], False, line_up_color, thickness=2)
            frame = cv.polylines(frame, [pts_L3], False, (255, 255, 255), thickness=1)
            frame = cv.polylines(frame, [pts_L4], False, (255, 255, 255), thickness=1)
            cv.putText(frame, str_up, (10, 40), font, 0.5, (255, 255, 255), 2, cv.LINE_AA)
            cv.putText(frame, str_up, (10, 40), font, 0.5, (0, 0, 255), 1, cv.LINE_AA)
            cv.putText(frame, str_down, (10, 90), font, 0.5, (255, 255, 255), 2, cv.LINE_AA)
            cv.putText(frame, str_down, (10, 90), font, 0.5, (255, 0, 0), 1, cv.LINE_AA)

            cv.imshow('Frame', frame)
            cv.imshow('Mask', mask)
            
            # Trimite mesaje la server la fiecare interval de 5 secunde
            current_time = time.time()
            if current_time - last_message_time >= message_interval:
                await send_message_to_server(cnt_up, cnt_down)
                last_message_time = current_time

            # Apasă ESC pentru a ieși
            k = cv.waitKey(30) & 0xff
            if k == 27:
                break
    except KeyboardInterrupt:
        print("Închiderea aplicației...")

    # Curățare
    log.flush()
    log.close()
    cap.release()
    cv.destroyAllWindows()

if __name__ == "__main__":
    asyncio.run(main())
