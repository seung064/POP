import json
import mysql.connector

# [수정] 경로 문자열 앞에 r을 붙여 raw string으로 만들어 안정성을 높입니다.
JSON_FILE_PATH = r'C:\Users\o\Desktop\POP\POP\QR_data\QR_Info.json'

DB_CONFIG = {
  'host': "127.0.0.1",
  'user': "root",
  'password': "1111",
  'database': "pop_project"
}

# 데이터를 삽입할 테이블 이름 (이 변수를 SQL 쿼리에서 사용할 것입니다)
TABLE_NAME = 'defect' 

def migrate_json_to_mysql():
    """
    JSON 파일의 데이터를 읽어 MySQL 데이터베이스에 삽입합니다.
    """
    # db_connection 변수를 try 블록 바깥에서 초기화
    db_connection = None
    cursor = None
    try:
        # 1. JSON 파일 읽기
        with open(JSON_FILE_PATH, 'r', encoding='utf-8') as f:
            data_from_json = json.load(f)
        print(f"'{JSON_FILE_PATH}' 파일에서 {len(data_from_json)}개의 데이터를 성공적으로 읽었습니다.")

        # 2. 데이터베이스 연결
        db_connection = mysql.connector.connect(**DB_CONFIG)
        cursor = db_connection.cursor()
        print("MySQL 데이터베이스에 성공적으로 연결되었습니다.")

        # 3. 데이터 삽입/업데이트
        insert_count = 0
        update_count = 0
        
        # JSON 데이터는 { "P001": {"qrcode": "P001", "name": "PCB1"}, ... } 형태입니다.
        # .items()를 사용하면 qrcode 변수에는 "P001", "P002" 등이 할당됩니다.
        for qrcode, product_info in data_from_json.items():
            name = product_info.get("name") # 'name' 키로 이름 가져오기
            
            if not name:
                # [수정] 정의되지 않은 'pk' 대신 올바른 변수인 'qrcode'를 사용합니다.
                print(f"경고: PK '{qrcode}'에 'name' 정보가 없습니다. 건너뜁니다.")
                continue

            # [수정] 
            # 1. f-string 안에서 테이블 이름을 변수로 사용하려면 {TABLE_NAME}으로 감싸야 합니다.
            # 2. MySQL 테이블의 컬럼 이름이 'qrcode', 'name'이 맞는지 확인하세요.
            query = f"""
            INSERT INTO {TABLE_NAME} (qr_code, name)
            VALUES (%s, %s)
            ON DUPLICATE KEY UPDATE name = VALUES(name)
            """
            
            # [수정] 정의되지 않은 'pk' 대신 올바른 변수인 'qrcode'를 튜플에 전달합니다.
            cursor.execute(query, (qrcode, name))
            
            # cursor.rowcount는 쿼리 실행으로 영향을 받은 행의 수를 반환합니다.
            # 1이면 INSERT, 2이면 UPDATE, 0이면 아무 변화 없음.
            if cursor.rowcount == 1:
                insert_count += 1
            elif cursor.rowcount == 2:
                update_count += 1
        
        # 4. 변경사항 최종 커밋
        db_connection.commit()
        
        print("\n데이터베이스 작업 완료!")
        print(f"새로 추가된 데이터: {insert_count}개")
        print(f"업데이트된 데이터: {update_count}개")

    except FileNotFoundError:
        print(f"오류: '{JSON_FILE_PATH}' 파일을 찾을 수 없습니다.")
    except json.JSONDecodeError:
        print(f"오류: '{JSON_FILE_PATH}' 파일의 JSON 형식이 올바르지 않습니다. 비어있거나 깨졌을 수 있습니다.")
    except mysql.connector.Error as err:
        print(f"데이터베이스 오류: {err}")
    finally:
        # 5. 연결 해제
        if cursor:
            cursor.close()
        if db_connection and db_connection.is_connected():
            db_connection.close()
            print("MySQL 연결이 해제되었습니다.")

# --- 스크립트 실행 ---
if __name__ == "__main__":
    migrate_json_to_mysql()