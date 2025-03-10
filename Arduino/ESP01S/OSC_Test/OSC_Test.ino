#include <SoftwareSerial.h>

// 定义ESP01S的RX和TX引脚
#define ESP_RX 2  // 连接到ESP01S的TX
#define ESP_TX 3  // 连接到ESP01S的RX

// 创建软串口对象
SoftwareSerial espSerial(ESP_RX, ESP_TX);

// 网络设置
const char* ssid = "Motion Computation Lab";
const char* password = "AI010101";
const char* host = "192.168.31.109";
const int port = 7003;

// OSC消息变量
int counter = 0;
unsigned long lastMessageTime = 0;
const int messageInterval = 1000; // 每秒发送一次

void setup() {
  // 初始化串口通信
  Serial.begin(9600);
  Serial.println("\nESP01S OSC测试程序");
  
  // 初始化ESP01S通信
  espSerial.begin(115200);
  
  // 等待ESP01S启动
  delay(1000);
  
  // 重置ESP01S
  Serial.println("重置ESP01S...");
  sendATCommand("AT+RST", 2000);
  
  // 设置为Station模式
  Serial.println("设置Station模式...");
  sendATCommand("AT+CWMODE=1", 1000);
  
  // 连接到WiFi
  Serial.print("连接到WiFi: ");
  Serial.println(ssid);
  
  String cmd = "AT+CWJAP=\"";
  cmd += ssid;
  cmd += "\",\"";
  cmd += password;
  cmd += "\"";
  
  sendATCommand(cmd, 10000); // WiFi连接可能需要更长时间
  
  // 查询IP地址
  Serial.println("获取IP地址...");
  sendATCommand("AT+CIFSR", 2000);
  
  Serial.println("ESP01S初始化完成");
  Serial.println("开始发送OSC消息到 " + String(host) + ":" + String(port));
  Serial.println("命令:");
  Serial.println("  send - 立即发送一条测试消息");
  Serial.println("  start - 开始自动发送消息");
  Serial.println("  stop - 停止自动发送消息");
  Serial.println("  value:123 - 发送特定值");
}

void loop() {
  // 处理自动发送
  unsigned long currentTime = millis();
  if (currentTime - lastMessageTime >= messageInterval) {
    lastMessageTime = currentTime;
    // 自动发送消息
    sendOSCMessage("/test", counter++);
  }
  
  // 处理用户输入
  if (Serial.available()) {
    String input = Serial.readStringUntil('\n');
    input.trim();
    
    if (input.equalsIgnoreCase("send")) {
      Serial.println("发送测试消息...");
      sendOSCMessage("/test", counter++);
    } else if (input.equalsIgnoreCase("start")) {
      Serial.println("开始自动发送消息");
      lastMessageTime = currentTime - messageInterval; // 立即触发第一条消息
    } else if (input.equalsIgnoreCase("stop")) {
      Serial.println("停止自动发送消息");
      lastMessageTime = currentTime; // 重置计时器但不触发发送
    } else if (input.indexOf(":") > 0) {
      // 格式: 地址:值
      int colonPos = input.indexOf(":");
      String address = input.substring(0, colonPos);
      String valueStr = input.substring(colonPos + 1);
      int value = valueStr.toInt();
      
      Serial.println("发送自定义消息: /" + address + " 值: " + value);
      sendOSCMessage("/" + address, value);
    } else {
      // 发送自定义消息
      Serial.println("发送自定义消息: " + input);
      sendOSCMessage("/" + input, counter++);
    }
  }
  
  // 显示ESP01S的任何响应
  while (espSerial.available()) {
    Serial.write(espSerial.read());
  }
}

// 发送AT命令并等待响应
void sendATCommand(String command, int timeout) {
  Serial.print("发送命令: ");
  Serial.println(command);
  
  // 清空接收缓冲区
  while(espSerial.available()) {
    espSerial.read();
  }
  
  // 发送命令
  espSerial.println(command);
  
  // 等待并显示响应
  unsigned long startTime = millis();
  
  while (millis() - startTime < timeout) {
    if (espSerial.available()) {
      char c = espSerial.read();
      Serial.write(c);
    }
  }
  
  Serial.println();
}

// 发送OSC消息
void sendOSCMessage(String address, int value) {
  Serial.print("发送OSC消息: ");
  Serial.print(address);
  Serial.print(" ");
  Serial.println(value);
  
  // 创建UDP连接
  String cmd = "AT+CIPSTART=\"UDP\",\"";
  cmd += host;
  cmd += "\",";
  cmd += port;
  
  sendATCommand(cmd, 1000);
  
  // 构建OSC消息 - 使用符合标准的OSC格式
  // 地址 + 类型标签 + 值
  
  // 1. 计算消息长度
  int paddedAddressLength = ((address.length() + 4) / 4) * 4; // 4字节对齐
  int typeTagLength = 4; // ",i" + 填充
  int valueLength = 4; // 整数值是4字节
  
  int totalLength = paddedAddressLength + typeTagLength + valueLength;
  
  // 2. 创建消息缓冲区
  char buffer[128]; // 足够容纳我们的消息
  memset(buffer, 0, sizeof(buffer));
  
  // 3. 设置地址部分
  address.toCharArray(buffer, address.length() + 1);
  
  // 4. 设置类型标签
  buffer[paddedAddressLength] = ',';
  buffer[paddedAddressLength + 1] = 'i';
  
  // 5. 设置整数值 (大端序)
  buffer[paddedAddressLength + typeTagLength] = (value >> 24) & 0xFF;
  buffer[paddedAddressLength + typeTagLength + 1] = (value >> 16) & 0xFF;
  buffer[paddedAddressLength + typeTagLength + 2] = (value >> 8) & 0xFF;
  buffer[paddedAddressLength + typeTagLength + 3] = value & 0xFF;
  
  // 准备发送数据
  cmd = "AT+CIPSEND=";
  cmd += totalLength;
  
  // 发送数据长度命令
  espSerial.println(cmd);
  delay(100);
  
  // 发送数据
  for (int i = 0; i < totalLength; i++) {
    espSerial.write(buffer[i]);
  }
  delay(100);
  
  // 关闭连接
  sendATCommand("AT+CIPCLOSE", 500);
} 