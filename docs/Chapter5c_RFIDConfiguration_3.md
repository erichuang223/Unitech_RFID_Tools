# Unitech RFID Command Set — 第 5 章（下）

- **版本 (Version)**: V1.02
- **說明**: 藍色字體標示為新增/更新功能 (New/Updated functions in blue)
- **適用韌體 (Applied to FW)**: RM300P V0.1.0.14 或更新版本

> 第 5 章 RFID Configuration（RFID 參數設定）內容龐大（5.1~5.62），分為三個子文件：
> [5a（5.1~5.20）](Chapter5a_RFIDConfiguration_1.md)、[5b（5.21~5.42）](Chapter5b_RFIDConfiguration_2.md)、本檔為 **5c（5.43~5.62）**。

---

## 文件章節目錄 (Document Table of Contents)

| 章節 | 標題 | 檔案 |
|---|---|---|
| 1 | Command Format | [Chapter1_CommandFormat.md](Chapter1_CommandFormat.md) |
| 2 | General Commands | [Chapter2_GeneralCommands.md](Chapter2_GeneralCommands.md) |
| 3 | RFID Operation | [Chapter3_RFIDOperation.md](Chapter3_RFIDOperation.md) |
| 4 | RFID Data Reply | [Chapter4_RFIDDataReply.md](Chapter4_RFIDDataReply.md) |
| 5 | RFID Configuration | [Chapter5a](Chapter5a_RFIDConfiguration_1.md)（5.1~5.20）/ [Chapter5b](Chapter5b_RFIDConfiguration_2.md)（5.21~5.42）/ [Chapter5c](Chapter5c_RFIDConfiguration_3.md)（本章，5.43~5.62） |
| 6 | RFID Extended Operation | [Chapter6_RFIDExtendedOperation.md](Chapter6_RFIDExtendedOperation.md) |
| 7 | Event Notification Commands | [Chapter7_EventNotification.md](Chapter7_EventNotification.md) |
| 8 | Firmware Update Commands | [Chapter8_FirmwareUpdate.md](Chapter8_FirmwareUpdate.md) |

---

## 第 5 章（下）：RFID Configuration（RFID 參數設定）5.43 ~ 5.62

### 本章目錄

- [5.43 / 5.44 Get / Set Power Saving Timeout（省電逾時）— 0x132 / 0x133](#543--544-get--set-power-saving-timeout省電逾時--0x132--0x133)
- [5.45 / 5.46 Get / Set UART Default Baud Rate（UART 預設鮑率）— 0x150 / 0x151](#545--546-get--set-uart-default-baud-rateuart-預設鮑率--0x150--0x151)
- [5.47 / 5.48 Get / Set GPIO Pins Configuration（GPIO 腳位設定）— 0x180 / 0x181](#547--548-get--set-gpio-pins-configurationgpio-腳位設定--0x180--0x181)
- [5.49 / 5.50 Get / Set GPIO Pins State（GPIO 腳位狀態）— 0x182 / 0x183](#549--550-get--set-gpio-pins-stategpio-腳位狀態--0x182--0x183)
- [5.51 / 5.52 Get / Set LBT Configure Settings（LBT 參數設定）— 0x190 / 0x191](#551--552-get--set-lbt-configure-settingslbt-參數設定--0x190--0x191)
- [5.53 / 5.54 Get / Set Antenna Switching Mode for Single Antenna（單天線切換模式）— 0x1A0 / 0x1A1](#553--554-get--set-antenna-switching-mode-for-single-antenna單天線切換模式--0x1a0--0x1a1)
- [5.55 / 5.56 Get / Set Frequency Switching Mode for Single Frequency（單頻率切換模式）— 0x1A2 / 0x1A3](#555--556-get--set-frequency-switching-mode-for-single-frequency單頻率切換模式--0x1a2--0x1a3)
- [5.57 / 5.58 Get / Set Antenna Detection Settings（天線偵測設定）— 0x1A4 / 0x1A5](#557--558-get--set-antenna-detection-settings天線偵測設定--0x1a4--0x1a5)
- [5.59 / 5.60 Get / Set Configuration Storage Mode（設定儲存模式）— 0x1F0 / 0x1F1](#559--560-get--set-configuration-storage-mode設定儲存模式--0x1f0--0x1f1)
- [5.61 / 5.62 Get / Set Error Suppression Mode（錯誤抑制模式）— 0x1F2 / 0x1F3](#561--562-get--set-error-suppression-mode錯誤抑制模式--0x1f2--0x1f3)

本章續接 5b，涵蓋省電逾時、UART 鮑率、GPIO、LBT 參數、天線/頻率切換模式、天線偵測、設定儲存模式與錯誤抑制模式。每組 Get/Set 合併說明並標註兩個 Command ID。

---

### 5.43 / 5.44 Get / Set Power Saving Timeout（省電逾時）— 0x132 / 0x133

設定/讀取進入睡眠前的閒置時間。需搭配 [5.41/5.42 省電模式](Chapter5b_RFIDConfiguration_2.md) 使用：在省電模式下，閒置超過此時間即進入睡眠。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><PSTIME><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><PSTIME><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| PSTIME | 2 Byte | 省電逾時（Power Saving Timeout），單位 ms，即進入睡眠前的閒置時間 |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x32><0xFF 0xC9><0x03>
RFID ⇒ <0x02><0x00 0x07><0x01 0x32><0x00><0x0B 0xB8><0xFF 0x03><0x03>   ; 3000 ms

; Set
Host ⇒ <0x02><0x00 0x06><0x01 0x33><0x0B 0xB8><0xFF 0x03><0x03>          ; 3000 ms
RFID ⇒ <0x02><0x00 0x05><0x01 0x33><0x00><0xFF 0xC7><0x03>
```

- `PSTIME = 0x0BB8` = 3000，代表閒置 **3000 ms（3 秒）** 後進入睡眠。

---

### 5.45 / 5.46 Get / Set UART Default Baud Rate（UART 預設鮑率）— 0x150 / 0x151

設定/讀取 UART 通訊鮑率。**注意：新鮑率在裝置下次重新開機後才生效。**

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><BAUDRATE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><BAUDRATE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| BAUDRATE | 4 Byte | UART 鮑率，常用值：115200 / 460800 / 921600 |

> 設定生效時機：新鮑率在裝置下次重新開機（reboot）後才生效（New baud rate will take effect on the next device reboot）。

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x50><0xFF 0xAB><0x03>
RFID ⇒ <0x02><0x00 0x09><0x01 0x50><0x00><0x00 0x01 0xC2 0x00><0xFE 0xE3><0x03>

; Set
Host ⇒ <0x02><0x00 0x08><0x01 0x51><0x00 0x0E 0x10 0x00><0xFF 0x88><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x51><0x00><0xFF 0xA9><0x03>
```

- Get 回覆 `BAUDRATE = 0x0001C200` = 115200 bps。
- Set 帶 `BAUDRATE = 0x000E1000` = 921600 bps（重開機後生效）。

---

### 5.47 / 5.48 Get / Set GPIO Pins Configuration（GPIO 腳位設定）— 0x180 / 0x181

設定/讀取指定 GPIO 腳位的方向（輸入/輸出/替代功能）。

```
Get  Host ⇒ <STX><LENGTH><ID><PINNUM><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><PINNUM><PINDIR><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><PINNUM><PINDIR><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| PINNUM | 1 Byte | GPIO 腳位編號（GPIO Pin Number） |
| PINDIR | 1 Byte | GPIO 腳位方向，見下方對照表 |

#### PINDIR（腳位方向）中英對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0 | 輸入 | input |
| 1 | 輸出 | output |
| 2 | 替代功能（若該腳位支援） | Alt-Func if avail |

#### 範例

```
; Get：讀取 2 號腳位設定
Host ⇒ <0x02><0x00 0x05><0x01 0x80><0x02><0xFF 0x78><0x03>
RFID ⇒ <0x02><0x00 0x07><0x01 0x80><0x00><0x02><0x00><0xFF 0x76><0x03>

; Set：設定 2 號腳位為輸出
Host ⇒ <0x02><0x00 0x06><0x01 0x81><0x02><0x01><0xFF 0x75><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x81><0x00><0xFF 0x79><0x03>
```

- Get 回覆 `PINNUM = 0x02`、`PINDIR = 0x00`（輸入）。
- Set 帶 `PINNUM = 0x02`、`PINDIR = 0x01`（輸出）。

---

### 5.49 / 5.50 Get / Set GPIO Pins State（GPIO 腳位狀態）— 0x182 / 0x183

讀取/設定指定 GPIO 腳位的高低電位狀態（輸入腳位可讀狀態，輸出腳位可設定狀態）。

```
Get  Host ⇒ <STX><LENGTH><ID><PINNUM><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><PINNUM><PINSTATE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><PINNUM><PINSTATE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| PINNUM | 1 Byte | GPIO 腳位編號 |
| PINSTATE | 1 Byte | GPIO 腳位電位狀態，見下方對照表 |

#### PINSTATE（腳位狀態）中英對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0 | 低電位 | low |
| 1 | 高電位 | high |

#### 範例

```
; Get：讀取 2 號腳位狀態
Host ⇒ <0x02><0x00 0x05><0x01 0x82><0x02><0xFF 0x76><0x03>
RFID ⇒ <0x02><0x00 0x07><0x01 0x82><0x00><0x02><0x00><0xFF 0x74><0x03>

; Set：設定 2 號腳位為高電位
Host ⇒ <0x02><0x00 0x06><0x01 0x83><0x02><0x01><0xFF 0x73><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x83><0x00><0xFF 0x77><0x03>
```

- Get 回覆 `PINNUM = 0x02`、`PINSTATE = 0x00`（低電位）。
- Set 帶 `PINNUM = 0x02`、`PINSTATE = 0x01`（高電位）。

---

### 5.51 / 5.52 Get / Set LBT Configure Settings（LBT 參數設定）— 0x190 / 0x191

設定/讀取 LBT（聽後再說）的 RSSI 門檻。需搭配 [5.15/5.16 LBT 啟用](Chapter5a_RFIDConfiguration_1.md)：啟用 LBT 後，發射前偵測通道訊號，若高於此門檻則視為通道忙碌。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><RSSITHSD><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><RSSITHSD><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| RSSITHSD | 2 Byte | LBT RSSI 門檻，單位 cdBm（百分之一 dBm），為有號數。例如 -7400 代表 -74 dBm |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x90><0xFF 0x6B><0x03>
RFID ⇒ <0x02><0x00 0x07><0x01 0x90><0x00><0xE3 0x18><0xFE 0x6D><0x03>

; Set
Host ⇒ <0x02><0x00 0x06><0x01 0x91><0xE3 0x18><0xFE 0x6D><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x91><0x00><0xFF 0x69><0x03>
```

- `RSSITHSD = 0xE318`（有號數 = -7400），代表 LBT 門檻 **-74.00 dBm**。

---

### 5.53 / 5.54 Get / Set Antenna Switching Mode for Single Antenna（單天線切換模式）— 0x1A0 / 0x1A1

設定/讀取切換天線時是否中斷射頻輸出。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><ASMODE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><ASMODE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ASMODE | 1 Byte | 單天線切換模式（Antenna Switching Mode），見下方對照表 |

#### ASMODE（天線切換模式）中英對照表

| 數值 | 模式（中文） | 原文 |
|---|---|---|
| 0x00 | 停止模式：切換天線時中斷射頻輸出 | Stop Mode. Stop RF output during antenna switching |
| 0x01 | 不停止模式：切換天線時維持射頻輸出 | Non-Stop Mode. Keep RF output during antenna switching |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0xA0><0xFF 0x5B><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0xA0><0x00><0x00><0xFF 0x59><0x03>

; Set：設為 Non-Stop Mode
Host ⇒ <0x02><0x00 0x05><0x01 0xA1><0x01><0xFF 0x58><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0xA1><0x00><0xFF 0x59><0x03>
```

- Get 回覆 `ASMODE = 0x00`（停止模式）；Set 帶 `0x01` 改為不停止模式。

---

### 5.55 / 5.56 Get / Set Frequency Switching Mode for Single Frequency（單頻率切換模式）— 0x1A2 / 0x1A3

設定/讀取切換頻率（跳頻）時是否中斷射頻輸出，意義同 5.53/5.54 但針對頻率切換。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><FSMODE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><FSMODE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| FSMODE | 1 Byte | 單頻率切換模式（Frequency Switching Mode），見下方對照表 |

#### FSMODE（頻率切換模式）中英對照表

| 數值 | 模式（中文） | 原文 |
|---|---|---|
| 0x00 | 停止模式：切換頻率時中斷射頻輸出 | Stop Mode. Stop RF output during frequency switching |
| 0x01 | 不停止模式：切換頻率時維持射頻輸出 | Non-Stop Mode. Keep RF output during frequency switching |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0xA2><0xFF 0x59><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0xA2><0x00><0x00><0xFF 0x57><0x03>

; Set：設為 Non-Stop Mode
Host ⇒ <0x02><0x00 0x05><0x01 0xA3><0x01><0xFF 0x56><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0xA3><0x00><0xFF 0x57><0x03>
```

- Get 回覆 `FSMODE = 0x00`（停止模式）；Set 帶 `0x01` 改為不停止模式。

---

### 5.57 / 5.58 Get / Set Antenna Detection Settings（天線偵測設定）— 0x1A4 / 0x1A5

啟用/停用天線偵測。啟用後裝置會偵測天線是否正常連接（未連接時可回報錯誤，見 [Ch4.10 OPERRCODE 0xD2](Chapter4_RFIDDataReply.md)）。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><ANTDET><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><ANTDET><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTDET | 1 Byte | 天線偵測設定（Antenna Detection），見下方對照表 |

#### ANTDET（天線偵測）中英對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0x00 | 停用天線偵測 | Disable Antenna Detection |
| 0x01 | 啟用天線偵測 | Enable Antenna Detection |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0xA4><0xFF 0x57><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0xA4><0x00><0x00><0xFF 0x55><0x03>

; Set：啟用
Host ⇒ <0x02><0x00 0x05><0x01 0xA5><0x01><0xFF 0x54><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0xA5><0x00><0xFF 0x55><0x03>
```

- Get 回覆 `ANTDET = 0x00`（停用）；Set 帶 `0x01` 改為啟用。

---

### 5.59 / 5.60 Get / Set Configuration Storage Mode（設定儲存模式）— 0x1F0 / 0x1F1

設定/讀取參數儲存模式：決定設定變更要同時寫入 Flash（重開機後保留）或只存於 RAM（重開機後失效）。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><STMODE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><STMODE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| STMODE | 1 Byte | 設定儲存模式（Configuration Storage Mode），見下方對照表 |

#### STMODE（儲存模式）中英對照表

| 數值 | 模式（中文） | 原文 |
|---|---|---|
| 0x00 | 同時存入 RAM 與 Flash（設定永久保留） | Save-to RAM/Flash Mode |
| 0x01 | 只存入 RAM（重開機後不保留） | Save-to RAM only Mode |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0xF0><0xFF 0x0B><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0xF0><0x00><0x00><0xFF 0x09><0x03>

; Set：設為只存 RAM
Host ⇒ <0x02><0x00 0x05><0x01 0xF1><0x01><0xFF 0x08><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0xF1><0x00><0xFF 0x09><0x03>
```

- Get 回覆 `STMODE = 0x00`（RAM/Flash 模式）；Set 帶 `0x01` 改為只存 RAM。

---

### 5.61 / 5.62 Get / Set Error Suppression Mode（錯誤抑制模式）— 0x1F2 / 0x1F3

設定/讀取是否抑制 Ex10 錯誤回報。啟用抑制後，模組不再主動回報 Ex10 操作錯誤（見 [Ch4.10 Operation Error](Chapter4_RFIDDataReply.md)）。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><ESMODE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><ESMODE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ESMODE | 1 Byte | 錯誤抑制模式（Error Suppression Mode），見下方對照表 |

#### ESMODE（錯誤抑制模式）中英對照表

| 數值 | 模式（中文） | 原文 |
|---|---|---|
| 0x00 | 回報所有 Ex10 錯誤 | Report all Ex10 Errors |
| 0x01 | 抑制（不回報）所有 Ex10 錯誤 | Suppress all Ex10 Errors |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0xF2><0xFF 0x09><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0xF2><0x00><0x00><0xFF 0x07><0x03>

; Set：啟用錯誤抑制
Host ⇒ <0x02><0x00 0x05><0x01 0xF3><0x01><0xFF 0x06><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0xF3><0x00><0xFF 0x07><0x03>
```

- Get 回覆 `ESMODE = 0x00`（回報所有錯誤）；Set 帶 `0x01` 改為抑制所有 Ex10 錯誤。
