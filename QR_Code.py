## QR코드 생성
import qrcode
db = "qrcode 데이터"
productionQR = qrcode.QRCode()
img = productionQR.make_image(fill_color="black", back_color="white")
img.save("QR_Code.png")