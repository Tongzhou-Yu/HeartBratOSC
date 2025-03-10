# 项目文档

此目录包含ESP01S OSC测试项目的文档和接线图。

## 文件说明

- `circuit_diagram.png` - Arduino和ESP01S的接线图
- `unity_setup.png` - Unity设置截图

## 接线说明

### ESP01S接线

- ESP01S VCC -> Arduino 3.3V
- ESP01S GND -> Arduino GND
- ESP01S CH_PD/EN -> Arduino 3.3V（通过10kΩ电阻）
- ESP01S RX -> Arduino D3（通过10kΩ电阻）
- ESP01S TX -> Arduino D2

*注意：ESP01S需要稳定的3.3V电源，并且ESP01S的RX引脚需要进行电平转换。*

## Fritzing文件

Fritzing文件（.fzz）包含了完整的电路设计，您可以使用Fritzing软件打开和编辑。

## 添加您自己的图表

1. 使用Fritzing创建电路图
2. 保存为`circuit_diagram.fzz`
3. 导出为PNG或SVG格式
4. 将图片添加到此目录
5. 更新主README.md文件引用这些图片 