## QR코드 생성
## 

import qrcode
import json
import os

output_dir = r"C:\Users\gli05\Desktop\POP\POP\QR_Data" 

qr_dict ={}


for i in range(1, 50):
    serial = f"P{i:03}"  # P001, P002...
    Name = f"PCB{i}"   # 기기 이름
    
    # QR 생성
    qr = qrcode.make(serial)
    qr_filename = os.path.join(output_dir, f"QR_{serial}.png")
    qr.save(qr_filename)
    
    # 사전 정보 dict로 저장
    qr_info = {
        "PK": serial,
        "Name": Name
    }

    # json에 저장 (PK를 키로)
    qr_dict[serial] = {
        "PK": serial,
        "Name": Name}
    

# JSON 파일로 저장 (한 번에 전체 저장)
json_path = os.path.join(output_dir, "QR_Info.json") ## JSON 파일 경로 설정
with open(json_path, "w", encoding="utf-8") as f: ## encoding="utf-8"은 한글이 깨지지 않도록 하기 위함
    json.dump(qr_dict, f, indent=4, ensure_ascii=False) ## ensure_ascii=False는 한글이 깨지지 않도록 하기 위함