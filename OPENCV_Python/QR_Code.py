## QR코드 생성 
import qrcode
import json
import os

output_dir = r"C:\Users\o\Desktop\POP\POP\QR_data" 

qr_dict ={}


for i in range(1, 50):
    qrcode_id = f"{i}" 
    name = f"PCB{i}"   # 기기 이름
    
    # QR 생성
    qr = qrcode.make(qrcode_id)
    qr_filename = os.path.join(output_dir, f"QR_{qrcode_id}.png")
    qr.save(qr_filename)
    
    # 사전 정보 dict로 저장
    qr_info = {
        "qrcode": qrcode_id,
        "name": name
    }

    # json에 저장 (PK를 키로)
    qr_dict[qrcode_id] = {
        "qrcode": qrcode_id,
        "name": name}
    

# JSON 파일로 저장 (한 번에 전체 저장)
json_path = os.path.join(output_dir, "QR_Info.json") ## JSON 파일 경로 설정
with open(json_path, "w", encoding="utf-8") as f: ## encoding="utf-8"은 한글이 깨지지 않도록 하기 위함
    json.dump(qr_dict, f, indent=4, ensure_ascii=False) ## ensure_ascii=False는 한글이 깨지지 않도록 하기 위함