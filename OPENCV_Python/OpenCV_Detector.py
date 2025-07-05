## OpenCV를 사용하여 QR코드 인식 및 불량 검출 ##

import cv2
import pyzbar.pyzbar as pyzbar ## pyzbar 모듈을 사용하여 QR 코드 인식 및 디코딩
import socket
import pickle
import struct
import datetime
import json


## 스마트폰 카메라를 사용하여 QR 코드 인식 및 디코딩
def run_camera1():
    videocapture = cv2.VideoCapture(0, cv2.CAP_DSHOW)
    
    # 소켓 서버 설정
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM) ## 소켓 객체 생성
    server_socket.bind(('localhost', 9999)) ## 로컬 호스트와 포트 9999에 바인딩
    server_socket.listen(1) ## 클라이언트 연결 대기
    print("viewer.py 연결 대기 중...")
    conn, addr = server_socket.accept() ## 클라이언트 연결 수락
    print("연결됨:", addr)
    
    #1 processed_qr_codes = set()
    inspection_QR = {}

    while True:
        ret, frame = videocapture.read()
        if not ret:
            break

        
        #1 if not detected_qrcode: ## QR 코드가 인식되지 않은 경우에만 실행
        for code in pyzbar.decode(frame):
            
            qrcode_data = code.data.decode('utf-8') ## 미리 저장 해둔 데이터 디코딩
            print("인식 성공 :", qrcode_data)

            qr_info = json.loads(qrcode_data) ## QR 코드 데이터를 Dict으로 변환
            pk = qr_info["PK"] ## PK 값 추출
            print("PK:",pk)

            #1 if pk not in processed_qr_codes: ## 이미 처리된 QR 코드인지 확인
            if pk not in inspection_QR: # 1차 검사
            ## dict에 위치, 불량검출, 현재시간 추가
                inspection_info_first={
                    "location" : "1",
                    "datetime(1차)" : datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                    #"status" : # 나중에 추가 예정
                }

                ## QR코드 정보와 검사 정보 합침
                #1 qr_info1 = { pk : {**qr_info, **inspection_info}} 

                inspection_QR[pk] = {**qr_info, **inspection_info_first} # QR 코드 정보와 검사 정보를 합쳐서 새로운 dict에 저장/**는 dict 병합 연산자
                #1 inspection_QR.add(pk) ## QR 코드가 처리되었음을 기록


            elif inspection_QR[pk].get("location")=="1": # 2차 검사

                print(f"PK: {pk}) 2차 검사 시작")
                
                inspection_info_second = {
                    "location": "2", 
                    "datetime(2차)": datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                    #"status": # 나중에 추가 예정
                }
                # 기존 데이터에 2차 검수 정보를 업데이트
                inspection_QR[pk].update(inspection_info_second) 
                #1 inspection_QR.add(pk) ## QR 코드가 처리되었음을 기록


            #qr_info = {**json.loads(qrcode_data), **inspection_info} ## QR코드 데이터와 검사 정보를 합침
            print("QR 코드 정보:", inspection_QR) # 디버깅용 출력

            # QR 코드 데이터 표시
            #cv2.putText(frame, qrcode_data, (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
        
        ##cv2.imshow('IVCam Feed', frame) ## 'IVCam Feed' 창에 비디오 스트림 표시 ## imshow 대신 프레임을 전송
        
        # opencv프레임 전송
        data = pickle.dumps(frame)
        size = struct.pack(">L", len(data))
        conn.sendall(size + data)
        
        # 종료조건
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    videocapture.release() # 카메라 해제
    conn.close() ## 소켓 연결 종료
    
    cv2.destroyAllWindows()
    
if __name__ == "__main__":
    run_camera1()



##pip install opencv-python (웹캠 작동)
##pip install pyzbar (QR코드, 바코드 인식)
##pip install playsound (비프음 재생)
##pip install cvzone (QR코드 추적 감지 기능) 
##pip install numpy 연산처리

