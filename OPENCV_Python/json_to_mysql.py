import json
import mysql.connector

# JSON 파일 경로 (raw string)
JSON_FILE_PATH = r'C:\Users\o\Desktop\POP\POP\QR_data\QR_Info.json'

DB_CONFIG = {
    'host': "127.0.0.1",
    'user': "root",
    'password': "1111",
    'database': "pop_project"
}

TABLE_NAME = 'product'

def migrate_json_to_mysql():
    """
    JSON 파일 데이터를 읽어 MySQL defect 테이블에 삽입 또는 업데이트 수행
    """
    db_connection = None
    cursor = None
    try:
        # 1. JSON 읽기
        with open(JSON_FILE_PATH, 'r', encoding='utf-8') as f:
            data_from_json = json.load(f)
        print(f"'{JSON_FILE_PATH}' 파일에서 {len(data_from_json)}개의 데이터를 성공적으로 읽었습니다.")

        # 2. DB 연결
        db_connection = mysql.connector.connect(**DB_CONFIG)
        cursor = db_connection.cursor()
        print("MySQL 데이터베이스에 성공적으로 연결되었습니다.")

        insert_count = 0
        update_count = 0

        for qrcode, product_info in data_from_json.items():
            name = product_info.get("name")
            status = product_info.get("status")

            if not name:
                print(f"경고: QR코드 '{qrcode}'에 'name' 정보가 없습니다. 건너뜁니다.")
                continue
            if not status:
                print(f"경고: QR코드 '{qrcode}'에 'status' 정보가 없습니다. 건너뜁니다.")
                continue

            # SQL 쿼리: INSERT 또는 중복시 UPDATE (name, status 갱신)
            query = f"""
                INSERT INTO {TABLE_NAME} (qr_code, name, status)
                VALUES (%s, %s, %s)
                ON DUPLICATE KEY UPDATE
                    name = VALUES(name),
                    status = VALUES(status)
            """
            cursor.execute(query, (qrcode, name, status))

            if cursor.rowcount == 1:
                insert_count += 1
            elif cursor.rowcount == 2:
                update_count += 1

        # 4. 커밋
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
        if cursor:
            cursor.close()
        if db_connection and db_connection.is_connected():
            db_connection.close()
            print("MySQL 연결이 해제되었습니다.")

if __name__ == "__main__":
    migrate_json_to_mysql()