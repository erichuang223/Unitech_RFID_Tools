# Unitech RFID Command Set — 第 5 章（中）

- **版本 (Version)**: V1.02
- **說明**: 藍色字體標示為新增/更新功能 (New/Updated functions in blue)
- **適用韌體 (Applied to FW)**: RM300P V0.1.0.14 或更新版本

> 第 5 章 RFID Configuration（RFID 參數設定）內容龐大（5.1~5.62），分為三個子文件：
> [5a（5.1~5.20）](Chapter5a_RFIDConfiguration_1.md)、本檔為 **5b（5.21~5.42）**、[5c（5.43~5.62）](Chapter5c_RFIDConfiguration_3.md)。

---

## 文件章節目錄 (Document Table of Contents)

| 章節 | 標題 | 檔案 |
|---|---|---|
| 1 | Command Format | [Chapter1_CommandFormat.md](Chapter1_CommandFormat.md) |
| 2 | General Commands | [Chapter2_GeneralCommands.md](Chapter2_GeneralCommands.md) |
| 3 | RFID Operation | [Chapter3_RFIDOperation.md](Chapter3_RFIDOperation.md) |
| 4 | RFID Data Reply | [Chapter4_RFIDDataReply.md](Chapter4_RFIDDataReply.md) |
| 5 | RFID Configuration | [Chapter5a](Chapter5a_RFIDConfiguration_1.md)（5.1~5.20）/ [Chapter5b](Chapter5b_RFIDConfiguration_2.md)（本章，5.21~5.42）/ [Chapter5c](Chapter5c_RFIDConfiguration_3.md)（5.43~5.62） |
| 6 | RFID Extended Operation | [Chapter6_RFIDExtendedOperation.md](Chapter6_RFIDExtendedOperation.md) |
| 7 | Event Notification Commands | [Chapter7_EventNotification.md](Chapter7_EventNotification.md) |
| 8 | Firmware Update Commands | [Chapter8_FirmwareUpdate.md](Chapter8_FirmwareUpdate.md) |

---

## 第 5 章（中）：RFID Configuration（RFID 參數設定）5.21 ~ 5.42

### 本章目錄

- [5.21 / 5.22 Get / Set Operation Mode Settings（操作模式）— 0x116 / 0x117](#521--522-get--set-operation-mode-settings操作模式--0x116--0x117)
- [5.23 / 5.24 Get / Set Fixed Frequency（固定頻率）— 0x118 / 0x119](#523--524-get--set-fixed-frequency固定頻率--0x118--0x119)
- [5.25 / 5.26 Get / Set Tx On/Off Time（發射開關時間）— 0x11A / 0x11B](#525--526-get--set-tx-onoff-time發射開關時間--0x11a--0x11b)
- [5.27 / 5.28 Get / Set Phase Data Settings（相位資料輸出）— 0x11C / 0x11D](#527--528-get--set-phase-data-settings相位資料輸出--0x11c--0x11d)
- [5.29 / 5.30 Get / Set Ambient Temperature Protection（環境溫度保護）— 0x120 / 0x121](#529--530-get--set-ambient-temperature-protection環境溫度保護--0x120--0x121)
- [5.31 / 5.32 Get / Set PA Temperature Protection（PA 溫度保護）— 0x122 / 0x123](#531--532-get--set-pa-temperature-protectionpa-溫度保護--0x122--0x123)
- [5.33 / 5.34 Get / Set Temperature Protection Mode（溫度保護模式）— 0x124 / 0x125](#533--534-get--set-temperature-protection-mode溫度保護模式--0x124--0x125)
- [5.35 / 5.36 Get / Set Antenna Inventory Round Count Settings（天線盤點輪數）— 0x126 / 0x127](#535--536-get--set-antenna-inventory-round-count-settings天線盤點輪數--0x126--0x127)
- [5.37 / 5.38 Get / Set Inventory Data with Customized Format Settings（自訂盤點格式）— 0x128 / 0x129](#537--538-get--set-inventory-data-with-customized-format-settings自訂盤點格式--0x128--0x129)
- [5.39 / 5.40 Get / Set System Time（系統時間）— 0x12A / 0x12B](#539--540-get--set-system-time系統時間--0x12a--0x12b)
- [5.41 / 5.42 Get / Set Power Saving Mode（省電模式）— 0x130 / 0x131](#541--542-get--set-power-saving-mode省電模式--0x130--0x131)

本章續接 5a，涵蓋操作模式、頻率、發射時間、相位輸出、溫度保護、盤點輪數、自訂盤點格式、系統時間與省電模式等設定。每組 Get/Set 合併說明並標註兩個 Command ID。

---

### 5.21 / 5.22 Get / Set Operation Mode Settings（操作模式）— 0x116 / 0x117

設定/讀取盤點的操作模式：連續（Continuous）或非連續（Non-Continuous）。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><OPMODE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><OPMODE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| OPMODE | 1 Byte | 操作模式，見下方對照表 |

#### OPMODE（操作模式）對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0 | 連續模式（持續盤點直到取消） | Continuous |
| 1 | 非連續模式（盤點一輪後停止） | Non-Continuous |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x16><0xFF 0xE5><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0x16><0x00><0x00><0xFF 0xE3><0x03>

; Set：設為非連續
Host ⇒ <0x02><0x00 0x05><0x01 0x17><0x01><0xFF 0xE2><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x17><0x00><0xFF 0xE3><0x03>
```

- Get 回覆 `OPMODE = 0x00`（連續模式）；Set 帶 `0x01` 改為非連續模式。

---

### 5.23 / 5.24 Get / Set Fixed Frequency（固定頻率）— 0x118 / 0x119

設定/讀取固定頻率清單。設為固定頻率後，裝置只在指定頻點發射（而非跳頻）；`NUM = 0` 表示停用固定頻率（恢復一般跳頻）。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><NUM><FREQ1>…<FREQn><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><NUM><FREQ1>…<FREQn><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| NUM | 1 Byte | 固定頻率的數量；0 表示停用（Disable，使用一般跳頻） |
| FREQ | 4 Byte | 每個固定頻率，單位 kHz（例如 902750 = 902.750 MHz）。共 NUM 個 |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x18><0xFF 0xE3><0x03>
RFID ⇒ <0x02><0x00 0x0A><0x01 0x18><0x00><0x01><0x00 0x0D 0xC6 0x5E><0xFE 0xAB><0x03>

; Set：設定 1 個固定頻率
Host ⇒ <0x02><0x00 0x04><0x01 0x19><0x01><0x00 0x0D 0xC6 0x5E><0xFE 0xAB><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x19><0x00><0xFF 0xE1><0x03>
```

- `NUM = 0x01`（1 個固定頻率），`FREQ = 0x000DC65E` = 902750 kHz = **902.750 MHz**。

---

### 5.25 / 5.26 Get / Set Tx On/Off Time（發射開關時間）— 0x11A / 0x11B

設定/讀取發射器的 On/Off 時間（Duty Cycle），用於控制發射工作週期、散熱或符合法規。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><ONTIME><OFFTIME><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><ONTIME><OFFTIME><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ONTIME | 2 Byte | 發射開啟時間（Tx On time），單位 ms |
| OFFTIME | 2 Byte | 發射關閉時間（Tx Off time），單位 ms |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x1A><0xFF 0xE1><0x03>
RFID ⇒ <0x02><0x00 0x09><0x01 0x1A><0x00><0x00 0x64><0x00 0x64><0xFF 0x14><0x03>

; Set
Host ⇒ <0x02><0x00 0x08><0x01 0x1B><0x00 0x64><0x00 0x64><0xFF 0x14><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x1B><0x00><0xFF 0xDF><0x03>
```

- `ONTIME = 0x0064` = 100 ms，`OFFTIME = 0x0064` = 100 ms。

---

### 5.27 / 5.28 Get / Set Phase Data Settings（相位資料輸出）— 0x11C / 0x11D

啟用/停用相位資料（Phase Data）輸出。啟用後盤點資料會改用含相位的回覆格式（[Ch4.4~4.6](Chapter4_RFIDDataReply.md)）。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><PHASEEN><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><PHASEEN><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| PHASEEN | 1 Byte | 相位資料輸出旗標；0：停用（Disable），1：啟用（Enable） |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x1C><0xFF 0xDF><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0x1C><0x00><0x00><0xFF 0xDD><0x03>

; Set：啟用
Host ⇒ <0x02><0x00 0x05><0x01 0x1D><0x01><0xFF 0xDC><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x1D><0x00><0xFF 0xDD><0x03>
```

- Get 回覆 `PHASEEN = 0x00`（停用）；Set 帶 `0x01` 改為啟用。

---

### 5.29 / 5.30 Get / Set Ambient Temperature Protection（環境溫度保護）— 0x120 / 0x121

設定/讀取環境溫度保護的門檻溫度。當環境溫度超過此門檻，裝置依溫度保護模式（[5.33/5.34](#533--534-get--set-temperature-protection-mode溫度保護模式--0x124--0x125)）採取保護動作，並可能送出系統錯誤（[Ch4.11](Chapter4_RFIDDataReply.md)）。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><TEMPTHSD><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><TEMPTHSD><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| TEMPTHSD | 2 Byte | 溫度門檻（Temperature Threshold），單位 0.1°C（攝氏十分之一度） |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x20><0xFF 0xDB><0x03>
RFID ⇒ <0x02><0x00 0x07><0x01 0x20><0x00><0x02 0xBC><0xFF 0x1A><0x03>

; Set
Host ⇒ <0x02><0x00 0x06><0x01 0x21><0x02 0xBC><0xFF 0x1A><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x21><0x00><0xFF 0xD9><0x03>
```

- `TEMPTHSD = 0x02BC` = 700（十進位），代表門檻 **70.0°C**。

---

### 5.31 / 5.32 Get / Set PA Temperature Protection（PA 溫度保護）— 0x122 / 0x123

設定/讀取功率放大器（PA）溫度保護門檻，意義同 5.29/5.30，但針對 PA 溫度（見 [2.6 Read PA Temperature](Chapter2_GeneralCommands.md)）。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><TEMPTHSD><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><TEMPTHSD><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| TEMPTHSD | 2 Byte | PA 溫度門檻，單位 0.1°C |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x22><0xFF 0xD9><0x03>
RFID ⇒ <0x02><0x00 0x07><0x01 0x22><0x00><0x02 0xBC><0xFF 0x18><0x03>

; Set
Host ⇒ <0x02><0x00 0x06><0x01 0x23><0x02 0xBC><0xFF 0x18><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x23><0x00><0xFF 0xD7><0x03>
```

- `TEMPTHSD = 0x02BC` = 700，代表 PA 溫度門檻 **70.0°C**。

---

### 5.33 / 5.34 Get / Set Temperature Protection Mode（溫度保護模式）— 0x124 / 0x125

設定/讀取超過溫度門檻後的處理模式。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><TPMODE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><TPMODE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| TPMODE | 1 Byte | 溫度保護模式（Temperature Protection Mode），見下方對照表 |

#### TPMODE（溫度保護模式）中英對照表

| 數值 | 模式（中文） | 原文說明 |
|---|---|---|
| 0x00 | 停止模式：超溫立即停止發射 | Stop Mode. Stop Immediately |
| 0x01 | 不停止模式：危險溫度時暫不發射，溫度一降下立即恢復發射 | Non-Stop Mode. No transmission at dangerous levels, but transmit again as soon as the temperature drops |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x24><0xFF 0xD7><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0x24><0x00><0x00><0xFF 0xD5><0x03>

; Set：設為 Non-Stop Mode
Host ⇒ <0x02><0x00 0x05><0x01 0x25><0x01><0xFF 0xD4><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x25><0x00><0xFF 0xD5><0x03>
```

- Get 回覆 `TPMODE = 0x00`（停止模式）；Set 帶 `0x01` 改為不停止模式。

> 註：原文 5.34 封包格式列為 `<TOMODE>`，應為 `<TPMODE>` 的筆誤，此處統一以 TPMODE 表示。

---

### 5.35 / 5.36 Get / Set Antenna Inventory Round Count Settings（天線盤點輪數）— 0x126 / 0x127

設定/讀取每個天線的盤點輪數（Inventory Round Count），即每次切換到該天線時要執行幾輪盤點。

```
Get  Host ⇒ <STX><LENGTH><ID><ANTID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><ANTID><INVRNDCNT><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><ANTID><INVRNDCNT><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 天線編號（1~4） |
| INVRNDCNT | 2 Byte | 盤點輪數（Inventory Round Count） |

#### 範例

```
; Get：讀取第 1 號天線
Host ⇒ <0x02><0x00 0x05><0x01 0x26><0x01><0xFF 0xD3><0x03>
RFID ⇒ <0x02><0x00 0x08><0x01 0x26><0x00><0x01><0x00 0x01><0xFF 0xCF><0x03>

; Set：設定第 1 號天線
Host ⇒ <0x02><0x00 0x07><0x01 0x27><0x01><0x00 0x00><0xFF 0xD0><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x27><0x00><0xFF 0xD3><0x03>
```

- Get 回覆 `ANTID = 0x01`、`INVRNDCNT = 0x0001`（1 輪）。

---

### 5.37 / 5.38 Get / Set Inventory Data with Customized Format Settings（自訂盤點格式）— 0x128 / 0x129

設定/讀取自訂盤點回覆格式：以一串 PID 指定盤點資料回覆（[Ch4.7 自訂格式，ID = 0xA0](Chapter4_RFIDDataReply.md)）要包含哪些欄位、依何順序排列。Set 時若給空的 PID 清單，表示停用自訂格式。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><PID1>…<PIDn><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><PID1>…<PIDn><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| PID | 1 Byte（每個） | 參數識別碼清單，指定要顯示的欄位與順序。空清單表示停用自訂格式 |

#### PID（參數識別碼）中英對照表

> 此對照表與 [Ch4.7（ID = 0xA0）自訂格式回覆](Chapter4_RFIDDataReply.md) 的 PID 相同，兩處互相呼應。

| PID | 顯示欄位（中文） | 原文（…in Data Reply if possible） |
|---|---|---|
| 0x00 | EPC | Display EPC |
| 0x01 | PC（協定控制位元） | Display PC |
| 0x02 | RSSI（訊號強度） | Display RSSI |
| 0x03 | 天線編號 | Display Antenna ID |
| 0x04 | TID | Display TID |
| 0x05 | 使用者指定記憶體 | Display User specified memory |
| 0x06 | 相位 | Display Phase |
| 0x07 | 頻率 | Display Frequency |
| 0x08 | 時間戳記 | Display Timestamp |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x28><0xFF 0xD3><0x03>
RFID ⇒ <0x02><0x00 0x09><0x01 0x28><0x00><0x00><0x00 0x01 0x04 0x05><0xFF 0xC4><0x03>

; Set：設定回覆只含 Timestamp
Host ⇒ <0x02><0x00 0x06><0x01 0x29><0x00 0x08><0xFF 0xC8><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x29><0x00><0xFF 0xD1><0x03>
```

- Get 回覆的 PID 清單 `00 01 04 05`，代表自訂格式依序輸出 **EPC、PC、TID、使用者指定記憶體**。
- Set 範例帶 PID `0x00 0x08`，代表設定自訂格式輸出 **EPC、Timestamp**。

---

### 5.39 / 5.40 Get / Set System Time（系統時間）— 0x12A / 0x12B

設定/讀取裝置的系統時間（供 Timestamp 欄位使用，見 [Ch4.7 Timestamp](Chapter4_RFIDDataReply.md)）。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><YEAR><MON><DAY><HOUR><MIN><SEC><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><YEAR><MON><DAY><HOUR><MIN><SEC><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| YEAR | 1 Byte | 年（從 2000 起算，例如 1 代表 2001） |
| MON | 1 Byte | 月 |
| DAY | 1 Byte | 日 |
| HOUR | 1 Byte | 時 |
| MIN | 1 Byte | 分 |
| SEC | 1 Byte | 秒 |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x2A><0xFF 0xC9><0x03>
RFID ⇒ <0x02><0x00 0x0A><0x01 0x2A><0x00><YEAR><MON><DAY><HOUR><MIN><SEC><CHECKSUM><0x03>

; Set
Host ⇒ <0x02><0x00 0x0A><0x01 0x2B><YEAR><MON><DAY><HOUR><MIN><SEC><CHECKSUM><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x2B><0x00><CHECKSUM><0x03>
```

> 註：原文 5.39/5.40 的「Example」誤植了 5.35/5.36（0x126/0x127）的封包，與本指令 ID（0x12A/0x12B）及欄位結構（YEAR~SEC 共 6 Byte）不符。此處依封包格式定義還原正確的欄位結構，範例封包以欄位名稱示意，實際數值請依現場為準。

---

### 5.41 / 5.42 Get / Set Power Saving Mode（省電模式）— 0x130 / 0x131

設定/讀取省電模式：閒置時是否進入省電（睡眠），或永遠維持開機。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><PSMODE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><PSMODE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| PSMODE | 1 Byte | 省電模式（Power Saving Mode），見下方對照表 |

#### PSMODE（省電模式）中英對照表

| 數值 | 模式（中文） | 原文 |
|---|---|---|
| 0x00 | 省電模式（閒置時進入睡眠） | Power-Saving Mode |
| 0x01 | 開機模式（永遠維持開機） | Power-On Mode. Always Power-On |

> 進入睡眠前的閒置時間由 [5.43/5.44 Power Saving Timeout](Chapter5c_RFIDConfiguration_3.md) 設定。

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x30><0xFF 0xCB><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0x30><0x00><0x00><0xFF 0xC9><0x03>

; Set：設為 Power-On Mode
Host ⇒ <0x02><0x00 0x05><0x01 0x31><0x01><0xFF 0xC8><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x31><0x00><0xFF 0xC9><0x03>
```

- Get 回覆 `PSMODE = 0x00`（省電模式）；Set 帶 `0x01` 改為永遠開機。
