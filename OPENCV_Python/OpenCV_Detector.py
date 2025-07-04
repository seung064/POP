## OpenCV를 사용하여 QR코드 인식 및 불량 검출 ##

import cv2
##import datetime
import pyzbar.pyzbar as pyzbar ## pyzbar 모듈을 사용하여 QR 코드 인식 및 디코딩
import socket
import pickle
import struct



## 스마트폰 카메라를 사용하여 QR 코드 인식 및 디코딩
def run_camera():
    videocapture = cv2.VideoCapture(0, cv2.CAP_DSHOW)
    
    # 소켓 서버 설정
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM) ## 소켓 객체 생성
    server_socket.bind(('localhost', 9999)) ## 로컬 호스트와 포트 9999에 바인딩
    server_socket.listen(1) ## 클라이언트 연결 대기
    print("viewer.py 연결 대기 중...")
    conn, addr = server_socket.accept() ## 클라이언트 연결 수락
    print("연결됨:", addr)
    
    
    while True:
        ret, frame = videocapture.read()
        if not ret:
            break
        
        ##cv2.imwrite('./QR_Code1', frame) ## qr인식 사진 테스트용
        
        ##decode_qr_code(frame)
        for code in pyzbar.decode(frame):
            qrcode_data = code.data.decode('utf-8')
            print("인식 성공 :", qrcode_data)
            
            print("rect:", code.rect)
            
            # QR 코드의 위치에 사각형 그리기
            x, y, w, h = code.rect
            cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)
            
            # QR 코드 데이터 표시
            cv2.putText(frame, qrcode_data, (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
        
        cv2.imshow('IVCam Feed', frame) ## 'IVCam Feed' 창에 비디오 스트림 표시
        ## imshow 대신 프레임을 전송
        data = pickle.dumps(frame) 
        size = struct.pack(">L", len(data))
        conn.sendall(size + data)
        
        
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    videocapture.release()
    conn.close() ## 소켓 연결 종료
    cv2.destroyAllWindows()
    
if __name__ == "__main__":
    run_camera()

##pip install opencv-python (웹캠 작동)
##pip install pyzbar (QR코드, 바코드 인식)
##pip install playsound (비프음 재생)
##pip install cvzone (QR코드 추적 감지 기능) 
##pip install numpy 연산처리
