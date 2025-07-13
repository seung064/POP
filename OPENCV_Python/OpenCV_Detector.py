## OpenCV를 사용하여 QR코드 인식 및 불량 검출 ##

import cv2
import pyzbar.pyzbar as pyzbar ## pyzbar 모듈을 사용하여 QR 코드 인식 및 디코딩
import socket
import pickle
import struct
import datetime
import mysql.connector


## 스마트폰 카메라를 사용하여 QR 코드 인식 및 디코딩
def run_camera1():

   
    #videocapture = cv2.VideoCapture(0, cv2.CAP_DSHOW)
    #videocapture = cv2.VideoCapture(0) ## 카메라 장치 열기 (0은 기본 카메라)
    
    # 소켓 서버 설정
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM) ## 소켓 객체 생성
    server_socket.bind(('localhost', 9999)) ## 로컬 호스트와 포트 9999에 바인딩
    server_socket.listen(1) ## 클라이언트 연결 대기
    print("viewer.py 연결 대기 중...")
    conn, addr = server_socket.accept() ## 클라이언트 연결 수락
    print("연결됨:", addr)
    
    
    videocapture = cv2.VideoCapture(0, cv2.CAP_DSHOW)
    try:
        db_connection = mysql.connector.connect(
            host="127.0.0.1", # DB 서버 주소
            user="root",  # DB 사용자 이름
            password="1111",# DB 비밀번호
            database="pop_project" # 사용할 데이터베이스 이름
        )
        cursor = db_connection.cursor(dictionary=True) # 결과를 딕셔너리 형태로 받기 위해 dictionary=True
        print("MySQL 데이터베이스에 성공적으로 연결되었습니다.")
        
    except mysql.connector.Error as e:
        print(f"DB 연결 오류: {e}")
        return
    #1 processed_qr_codes = set()
    
    qr_info = {}
    

    while True:
        ret, frame = videocapture.read()
        if not ret:
            break

        
        #1 if not detected_qrcode: ## QR 코드가 인식되지 않은 경우에만 실행
        for code in pyzbar.decode(frame):
            try:
                
                #qr_code = pyzbar.decode(frame)
                qr_code = code.data.decode('utf-8') ## 미리 저장 해둔 데이터 디코딩
                print("인식 성공 :", qr_code)
                '''
                qr_info = json.loads(qrcode) ## QR 코드 데이터를 Dict으로 변환
                pk = qr_info["qrcode"] ## PK 값 추출
                print("PK:",pk)
                '''
                cursor.execute("SELECT qr_code, name, location, time_first, time_second, status FROM defect WHERE qr_code = %s", (qr_code,))
                qr_info = cursor.fetchone() ## DB에서 QR 코드 정보 조회
                
                if qr_info['location'] is None or qr_info['location'] == '':
                #if qr_info is not None:
                    #if qr_info[2] is None or qr_info[2] == '':
                    #if "location" not in qr_info: # 1차 검사
                    print("1차 검사 시작")
                    ## dict에 위치, 불량검출, 현재시간 추가
                    inspection_info_first={
                        "location" : '1',
                        "time_first" : datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                        "status" : '반제품'
                        #"결과" :
                    }

                    ## QR코드 정보와 검사 정보 합침 

                    qr_info[qr_code] = {**qr_info, **inspection_info_first} # QR 코드 정보와 검사 정보를 합쳐서 새로운 dict에 저장/**는 dict 병합 연산자

                    update_query = f"UPDATE defect SET location = '1', status = '반제품', time_first= '{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")}' WHERE qr_code = %s"
                    cursor.execute(update_query, (qr_code,))
                    db_connection.commit() # 변경사항 최종 
                
                #elif qr_info[4] is None or qr_info[4] == '':
                elif qr_info['location'] == 1:
                #elif qr_info['location'] == '1':
                #elif qr_info[qr_code].get('location')=='1': # 2차 검사
                    print("2차 검사 시작")
                    
                    inspection_info_second = {
                        "location": '2', 
                        "time_second": datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                        "status": '완제품'
                        #"결과" :
                    }
                    # 기존 데이터에 2차 검수 정보를 업데이트
                    #qr_info[qr_code].update(inspection_info_second) 
                    
                    update_query = f"UPDATE defect SET location = '2', status = '완제품', time_second='{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")}' WHERE qr_code = %s"
                    cursor.execute(update_query, (qr_code,))
                    db_connection.commit() # 변경사항 최종 

            except Exception as e:
                print(f"에러 타입: {type(e)}")
                print(f"에러 메시지: {e}")
                #qr_info = {**json.loads(qrcode_data), **inspection_info} ## QR코드 데이터와 검사 정보를 합침
                print("QR 코드 정보:", qr_info) # 디버깅용 출력

            # QR 코드 데이터 표시
            #cv2.putText(frame, qrcode_data, (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
        
        #cv2.imshow('IVCam Feed', frame) ## 'IVCam Feed' 창에 비디오 스트림 표시 ## imshow 대신 프레임을 전송
        
        # opencv프레임 전송
        data = pickle.dumps(frame)
        size = struct.pack(">L", len(data))
        conn.sendall(size + data)
        
        # 종료조건
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
    
    # 카메라와 소켓 종료
    videocapture.release() # 카메라 해제
    conn.close() ## 소켓 연결 종료
    cv2.destroyAllWindows()
    
    # 데이터베이스 연결 종료
    if db_connection.is_connected():
            cursor.close()
            db_connection.close()
  
  
if __name__ == "__main__":
    run_camera1()



##pip install opencv-python (웹캠 작동)
##pip install pyzbar (QR코드, 바코드 인식)
##pip install playsound (비프음 재생)
##pip install cvzone (QR코드 추적 감지 기능) 
##pip install numpy 연산처리

