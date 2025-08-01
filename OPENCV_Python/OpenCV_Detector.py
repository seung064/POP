## OpenCV를 사용하여 QR코드 인식 및 불량 검출 ##

import cv2
import pyzbar.pyzbar as pyzbar ## pyzbar 모듈을 사용하여 QR 코드 인식 및 디코딩
import socket
import pickle
import struct
import datetime
import mysql.connector
from ultralytics import YOLO
import time
#qr_check_time = time.time()
inspection_first_model = YOLO('./inspection_first_best.pt')
inspection_second_model = YOLO('./inspection_second_best.pt')

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
    
    qr_info = {}
    scanned_qr_codes = set()
    scanned_defects = set()
    
    
    while True:
        ret, frame = videocapture.read()
        if not ret:
            break

        for code in pyzbar.decode(frame):
            try:
                
                #qr_code = pyzbar.decode(frame)
                qr_code = code.data.decode('utf-8') ## 미리 저장 해둔 데이터 디코딩
                if qr_code in scanned_qr_codes:
                    continue
                scanned_qr_codes.add(qr_code)

                print("인식 성공 :", qr_code)
                #time.sleep(0)
                
                '''
                qr_info = json.loads(qrcode) ## QR 코드 데이터를 Dict으로 변환
                pk = qr_info["qrcode"] ## PK 값 추출
                print("PK:",pk)
                '''

                #cursor.execute("SELECT qr_code, name, location, time_defect, status FROM defect WHERE qr_code = %s", (qr_code,))
                cursor.execute("SELECT qr_code, name, status, production_time, defective_or_not FROM product WHERE qr_code = %s", (qr_code,))
                qr_info = cursor.fetchone() ## DB에서 QR 코드 정보 조회
                
                #if (qr_info['location'] is None or qr_info['location'] == '') and qr_info['status'] == '반제품':
                if qr_info['status'] == '반제품':

                    # 예외처리 또는 if문으로 qr이 찍혔을때+딜레이 줄것
                    print("1차 검사 시작")
                    
                    inspection_first_results = inspection_first_model.predict(source=frame[:, :, ::-1], save=False, conf=0.5)
                    for inspection_first_result in inspection_first_results: 
                        boxes = inspection_first_result.boxes  # Boxes object
                        if len(boxes) > 0:  
                            for box in boxes:
                                x1, y1, x2, y2 = map(int, box.xyxy[0])  # 좌표
                                conf = float(box.conf[0])               # 신뢰도
                                cls_id = int(box.cls[0])                # 클래스 ID
                                cls_name = inspection_first_model.names[cls_id]          # 클래스 이름

                                # 박스 그리기
                                cv2.rectangle(frame, (x1, y1), (x2, y2), (0, 255, 0), 2)
                                cv2.putText(frame, f"{cls_name} {conf:.2f}", (x1, y1 - 10),
                                            cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
                                
                                print(f"Detected {cls_name} with confidence {conf:.2f} at [{x1}, {y1}, {x2}, {y2}]")
                        
                            #update_query = f"UPDATE product SET location = '1', time_first= '{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")}' WHERE qr_code = %s"
                            update_product_query = f"UPDATE product SET defective_or_not = '1', production_time = '{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")}' WHERE qr_code = %s"
                            update_defect_query = f"UPDATE defect SET location = '1', time_defect = '{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")}', class_defect = '{cls_name}' WHERE qr_code = %s"
                            cursor.execute(update_product_query, (qr_code,))
                            cursor.execute(update_defect_query, (qr_code,))
                            db_connection.commit() # 변경사항 최종
                            #time.sleep(0)
                        
                        else:
                            pass
                            update_product_query = f"UPDATE product SET defective_or_not = '0', production_time = '{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")}' WHERE qr_code = %s"
                            cursor.execute(update_product_query, (qr_code,))
                            db_connection.commit()
                            #time.sleep(0)
                        

                    ## dict에 위치, 불량검출, 현재시간 추가
                    # inspection_info_first={
                    #     "location" : '1',
                    #     "time_first" : datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                    #     "status" : '반제품'
                    #     #"결과" :
                    # }
                    
                    #update_query = f"UPDATE product SET location = '1', time_first= '{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")}' WHERE qr_code = %s"
                    #cursor.execute(update_query, (qr_code,))
                    #db_connection.commit() # 변경사항 최종 
                    #time.sleep(5)
                
                #elif qr_info[4] is None or qr_info[4] == '':
                elif qr_info['location'] == 1 and qr_info['status'] == '완제품':
                #elif qr_info['location'] == '1':
                #elif qr_info[qr_code].get('location')=='1': # 2차 검사
                    print("2차 검사 시작")

                    #inspection_second_results = inspection_second_model(frame, verbose=False) # 프레임 예측
                    #second_annotated_frame = inspection_second_results[0].plot()  # 결과 프레임
                    #frame = inspection_second_results[0].plot()  # 결과 프레임
                    inspection_second_results = inspection_second_model.predict(source=frame[:, :, ::-1], save=False, conf=0.5)
                    for inspection_second_result in inspection_second_results:
                        boxes = inspection_second_result.boxes  # Boxes object
                        if len(boxes) > 0:
                            for box in boxes:
                                x1, y1, x2, y2 = map(int, box.xyxy[0])  # 좌표
                                conf = float(box.conf[0])               # 신뢰도
                                cls_id = int(box.cls[0])                # 클래스 ID
                                cls_name = inspection_second_model.names[cls_id]          # 클래스 이름

                                # 박스 그리기
                                cv2.rectangle(frame, (x1, y1), (x2, y2), (0, 255, 0), 2)
                                cv2.putText(frame, f"{cls_name} {conf:.2f}", (x1, y1 - 10),
                                            cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
                                
                                print(f"Detected {cls_name} with confidence {conf:.2f} at [{x1}, {y1}, {x2}, {y2}]")
                    
                            update_product_query = f"UPDATE product SET defective_or_not = '1' WHERE qr_code = %s"
                            update_defect_query = f"UPDATE defect SET location = '2', time_defect = '{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")}', class_defect = '{cls_name}' WHERE qr_code = %s"
                            cursor.execute(update_product_query, (qr_code,))
                            cursor.execute(update_defect_query, (qr_code,))
                            db_connection.commit() # 변경사항 최종 
                            #time.sleep(0)

                        else:
                            update_product_query = f"UPDATE product SET defective_or_not = '0', production_time = '{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")}' WHERE qr_code = %s"
                            cursor.execute(update_product_query, (qr_code,))
                            db_connection.commit()
                            #time.sleep(0)
                            
                    # inspection_info_second = {
                    #     "location": '2', 
                    #     "time_second": datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                    #     "status": '완제품'
                    #     #"결과" :
                    # }
                    
                    # 기존 데이터에 2차 검수 정보를 업데이트
                    #qr_info[qr_code].update(inspection_info_second) 
                    
                    #update_query = f"UPDATE defect SET location = '2', time_second='{datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")}' WHERE qr_code = %s"
                    

            except Exception as e:
                print(f"에러 타입: {type(e)}")
                print(f"에러 메시지: {e}")
                #qr_info = {**json.loads(qrcode_data), **inspection_info} ## QR코드 데이터와 검사 정보를 합침
                print("QR 코드 정보:", qr_info) # 디버깅용 출력

            # QR 코드 데이터 표시
            #cv2.putText(frame, qrcode_data, (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
        
        #cv2.imshow('IVCam Feed', frame) ## 'IVCam Feed' 창에 비디오 스트림 표시 ## imshow 대신 프레임을 전송
        
        # opencv프레임 전송
        #data = pickle.dumps(annotated_frame)
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

