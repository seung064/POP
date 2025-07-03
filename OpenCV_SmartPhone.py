import cv2
import datetime

videocapture = cv2.VideoCapture(1, cv2.CAP_DSHOW)

while True:
    ret, frame = videocapture.read()
    if not ret:
        break
    
    cv2.imshow('IVCam Feed', frame)
    
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

videocapture.release()
cv2.destroyAllWindows()