## QR코드 생성
## 
import qrcode
db = "001"
productionQR = qrcode.QRCode()
img = productionQR.make_image(fill_color="black", back_color="white")
img.save("QR_Code1.png")