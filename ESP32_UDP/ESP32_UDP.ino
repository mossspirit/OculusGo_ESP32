#include <WiFi.h>
#include <WiFiUdp.h>

#define LED 4

const char ssid[] = "ESP32_wifi"; // SSID
const char pass[] = "esp32pass";  // password
const int localPort = 10000;      // ポート番号

const IPAddress ip(192, 168, 4, 1);       // IPアドレス(ゲートウェイも兼ねる)
const IPAddress subnet(255, 255, 255, 0); // サブネットマスク

WiFiUDP udp;
char packetBuffer[255];

void setup() {
  Serial.begin(115200);

  pinMode(LED, OUTPUT);
  WiFi.softAP(ssid, pass);           // SSIDとパスの設定
  delay(100);                        // 追記：このdelayを入れないと失敗する場合がある
  WiFi.softAPConfig(ip, ip, subnet); // IPアドレス、ゲートウェイ、サブネットマスクの設定

  Serial.print("AP IP address: ");
  IPAddress myIP = WiFi.softAPIP();
  Serial.println(myIP);

  Serial.println("Starting UDP");
  udp.begin(localPort);  // UDP通信の開始(引数はポート番号)

  Serial.print("Local port: ");
  Serial.println(localPort);
}

void loop() {
   int packetSize = udp.parsePacket();
 
  if (packetSize) {
 
    int len = udp.read(packetBuffer, packetSize);
    //終端文字設定
    if (len > 0) packetBuffer[len] = '\0';
    Serial.println(packetBuffer);

    if(strcmp(packetBuffer, "a") == 0){
      Serial.println("LED_ON");
      digitalWrite(LED, HIGH);
    }
    
    if(strcmp(packetBuffer, "b") == 0){
      Serial.println("LED_OFF");
      digitalWrite(LED, LOW);
    }
  } 
}
