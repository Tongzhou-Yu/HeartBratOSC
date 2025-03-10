# ESP01S OSC测试工程

这是一个基础的测试工程，用于验证ESP01S模块与Unity之间的OSC通信。

## 硬件需求

- Arduino Uno（或兼容板）
- ESP01S WiFi模块
- 连接线
- 面包板
- 10kΩ电阻（2个，用于ESP01S的电平转换）
- 0.1μF电容（可选，用于稳定ESP01S的电源）

## 软件需求

- Arduino IDE
- Unity 2020.3或更高版本
- [extOSC](https://github.com/Iam1337/extOSC) Unity插件（或使用项目中提供的SimpleOSCReceiver脚本）

## 项目结构

```
ESP01S_OSC_Test/
├── Arduino_Code/
│   └── ESP01S_Test/            # ESP01S测试和OSC发送程序
├── Unity/
│   └── HeartBeatOSC/           # Unity项目
│       └── Assets/
│           ├── Scripts/
│           │   └── SimpleOSCReceiver.cs  # 简单OSC接收脚本
│           └── Scenes/
│               └── SampleScene.unity    # 示例场景
└── Documentation/              # 项目文档和接线图
```

## 接线说明

### ESP01S接线

- ESP01S VCC -> Arduino 3.3V
- ESP01S GND -> Arduino GND
- ESP01S CH_PD/EN -> Arduino 3.3V（通过10kΩ电阻）
- ESP01S RX -> Arduino D3（通过10kΩ电阻）
- ESP01S TX -> Arduino D2

*注意：ESP01S需要稳定的3.3V电源，并且ESP01S的RX引脚需要进行电平转换。*

## 安装和配置

### Arduino部分

1. 下载并安装Arduino IDE
2. 按照接线图连接ESP01S
3. 配置并上传`Arduino_Code/ESP01S_Test/ESP01S_Test.ino`
4. 在串口监视器中测试ESP01S的响应

### Unity部分

1. 打开Unity项目
2. 从Asset Store或GitHub下载并导入extOSC插件，或使用提供的SimpleOSCReceiver.cs脚本
3. 打开SampleScene场景
4. 创建一个空游戏对象，命名为"OSCReceiver"
5. 将SimpleOSCReceiver脚本添加到该对象
6. 设置接收端口为7001（与Arduino代码中一致）
7. 选择要控制的目标游戏对象

## 使用方法

1. 确保ESP01S已正确连接并上传代码
2. 确保Arduino和Unity所在的电脑在同一网络中
3. 修改Arduino代码中的WiFi名称、密码和电脑IP地址
4. 打开串口监视器观察ESP01S的连接状态
5. 运行Unity项目，观察OSC消息的接收情况

## 故障排除

- **ESP01S不响应：** 检查电源和接线，确保CH_PD连接到3.3V
- **无法连接WiFi：** 检查网络名称和密码是否正确
- **Unity未收到数据：** 检查IP地址和端口设置，检查防火墙设置 