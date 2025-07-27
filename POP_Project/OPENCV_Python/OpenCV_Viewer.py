## WPF에서 OpenCV 비디오를 보여주기 위한 뷰어

import cv2
import socket
import pickle
import struct

## TCP소켓 클라이언트 설정
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client_socket.connect(('localhost', 9999))

## 데이터 수신
data = b""  ## 바이트
payload_size = struct.calcsize(">L") ## 프레임 크기를 받기 위한 페이로드 사이즈 설정

while True:
    # 프레임 사이즈 받기
    while len(data) < payload_size: ## 데이터가 페이로드 사이즈보다 작을 때까지
        data += client_socket.recv(4096) ## 4096바이트씩 데이터 수신
    packed_size = data[:payload_size] ## 페이로드 사이즈만큼 데이터 추출/ 앞 4바이트 추출
    data = data[payload_size:] ## 추출한 데이터는 data에서 제거 / 그 뒤로는 실제프레임
    frame_size = struct.unpack(">L", packed_size)[0] ## 페이로드 사이즈를 언팩하여 프레임 사이즈로 변환

    # 프레임 데이터 수신
    while len(data) < frame_size: 
        data += client_socket.recv(4096)  
    frame_data = data[:frame_size] ## 프레임 사이즈만큼 데이터 추출
    data = data[frame_size:] ## 남은 데이터는 다음 프레임 용도

    # 디코드 후 표시
    frame = pickle.loads(frame_data) # 바이트 -> Opencv이미지로 디코드(변환)
    cv2.imshow("Viewer", frame) #iewer 창으로 화면 표시


    if cv2.waitKey(1) == 27:  # ESC 키
        break

client_socket.close()
cv2.destroyAllWindows()

