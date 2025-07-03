##QR리더기
##pip install opencv-python (웹캠 작동)
##pip install pyzbar (QR코드, 바코드 인식)
##pip install playsound (비프음 재생)
##pip install cvzone (QR코드 추적 감지 기능) 
##pip install numpy 연산처리

##import cv2
##import pyzbar.pyzbar as pyzbar
##from playsound import playsound

import cv2
from pyzbar import pyzbar

def decode_qr_code(frame):
    qrcodes = pyzbar.decode(frame)
    for qrcode in qrcodes:
        # QR 코드의 위치와 데이터 추출
        x, y, w, h = qrcode.rect
        data = qrcode.data.decode('utf-8')
        
        # QR 코드의 위치에 사각형 그리기
        cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)
        
        # QR 코드 데이터 표시
        cv2.putText(frame, data, (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
    return frame

##while True: