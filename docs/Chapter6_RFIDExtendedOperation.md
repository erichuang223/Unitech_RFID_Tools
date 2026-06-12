# Unitech RFID Command Set — 第 6 章

- **版本 (Version)**: V1.02
- **說明**: 藍色字體標示為新增/更新功能 (New/Updated functions in blue)
- **適用韌體 (Applied to FW)**: RM300P V0.1.0.14 或更新版本

---

## 文件章節目錄 (Document Table of Contents)

| 章節 | 標題 | 檔案 |
|---|---|---|
| 1 | Command Format | [Chapter1_CommandFormat.md](Chapter1_CommandFormat.md) |
| 2 | General Commands | [Chapter2_GeneralCommands.md](Chapter2_GeneralCommands.md) |
| 3 | RFID Operation | [Chapter3_RFIDOperation.md](Chapter3_RFIDOperation.md) |
| 4 | RFID Data Reply | [Chapter4_RFIDDataReply.md](Chapter4_RFIDDataReply.md) |
| 5 | RFID Configuration | [Chapter5a](Chapter5a_RFIDConfiguration_1.md) / [Chapter5b](Chapter5b_RFIDConfiguration_2.md) / [Chapter5c](Chapter5c_RFIDConfiguration_3.md) |
| 6 | RFID Extended Operation | [Chapter6_RFIDExtendedOperation.md](Chapter6_RFIDExtendedOperation.md)（本章） |
| 7 | Event Notification Commands | [Chapter7_EventNotification.md](Chapter7_EventNotification.md) |
| 8 | Firmware Update Commands | [Chapter8_FirmwareUpdate.md](Chapter8_FirmwareUpdate.md) |

---

## 第 6 章：RFID Extended Operation（RFID 擴充操作）

### 本章目錄

- [6.1 CW On Command（開啟連續載波）— ID = 0xF00](#61-cw-on-command開啟連續載波--id--0xf00)
- [6.2 CW Off Command（關閉連續載波）— ID = 0xF01](#62-cw-off-command關閉連續載波--id--0xf01)
- [6.3 Tx Random Data Command（發送隨機資料）— ID = 0xF02](#63-tx-random-data-command發送隨機資料--id--0xf02)

本章為測試/驗證用途的擴充操作指令：輸出連續載波（CW, Continuous Wave）與發送隨機資料，常用於射頻測試、頻譜量測或法規認證。所有指令皆遵循第 1 章封包格式。

> **共用測試欄位**
>
> | 欄位 | 長度 | 說明 |
> |---|---|---|
> | ANTID | 1 Byte | 測試用天線編號（1~4） |
> | PWR | 2 Byte | 測試發射功率，單位 cdBm（百分之一 dBm）。例如 0x0BB8 = 3000 = 30.00 dBm |
> | FREQ | 4 Byte | 測試頻率，單位 kHz（例如 902750 = 902.750 MHz） |

---

### 6.1. CW On Command（開啟連續載波）— ID = 0xF00

在指定天線、功率、頻率上開啟連續載波（CW）輸出，用於射頻測試。開啟後會持續發射，直到以 [6.2 CW Off](#62-cw-off-command關閉連續載波--id--0xf01) 關閉。

```
Host ⇒ <STX><LENGTH><ID><ANTID><PWR><FREQ><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 測試用天線編號（1~4） |
| PWR | 2 Byte | 測試發射功率，單位 cdBm |
| FREQ | 4 Byte | 測試頻率，單位 kHz |

#### 範例

```
Host ⇒ <0x02><0x00 0x0B><0x0F 0x00><0x01><0x0B 0xB8><0x00 0x0D 0xC6 0x5E><0xFD 0xF1><0x03>
RFID ⇒ <0x02><0x00 0x05><0x0F 0x00><0x00><0xFF 0xEC><0x03>
```

- Host 發送：`ANTID = 0x01`（第 1 號天線），`PWR = 0x0BB8` = 3000 cdBm = **30.00 dBm**，
  `FREQ = 0x000DC65E` = 902750 kHz = **902.750 MHz**。
- RFID 回覆：`STATUS = 0x00`（成功），表示已開啟 CW 輸出。

---

### 6.2. CW Off Command（關閉連續載波）— ID = 0xF01

關閉由 [6.1 CW On](#61-cw-on-command開啟連續載波--id--0xf00) 開啟的連續載波輸出。本指令不需要參數。

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 範例

```
Host ⇒ <0x02><0x00 0x04><0x0F 0x01><0xFF 0xEC><0x03>
RFID ⇒ <0x02><0x00 0x05><0x0F 0x01><0x00><0xFF 0xEB><0x03>
```

- Host 發送：`ID = 0x0F 0x01`，無 DATA，要求關閉 CW 輸出。
- RFID 回覆：`STATUS = 0x00`（成功），表示已關閉 CW 輸出。

---

### 6.3. Tx Random Data Command（發送隨機資料）— ID = 0xF02

在指定天線、功率、頻率上發送隨機調變資料一段時間，用於調變訊號的射頻測試。可指定自動關閉時間（DURATION）。

```
Host ⇒ <STX><LENGTH><ID><ANTID><PWR><FREQ><DURATION><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 測試用天線編號（1~4） |
| PWR | 2 Byte | 測試發射功率，單位 cdBm |
| FREQ | 4 Byte | 測試頻率，單位 kHz |
| DURATION | 4 Byte | 測試自動關閉時間（Auto Off duration），單位 ms。時間到自動停止發射 |

#### 範例

```
Host ⇒ <0x02><0x00 0x0F><0x0F 0x02><0x01><0x0B 0xB8><0x00 0x0D 0xC6 0x5E>
  <0x00 0x00 0x0B 0xB8><0xFD 0x28><0x03>
RFID ⇒ <0x02><0x00 0x05><0x0F 0x02><0x00><0xFF 0xEA><0x03>
```

- Host 發送：`ANTID = 0x01`，`PWR = 0x0BB8` = 30.00 dBm，`FREQ = 0x000DC65E` = 902.750 MHz，
  `DURATION = 0x00000BB8` = 3000 ms（3 秒後自動停止）。
- RFID 回覆：`STATUS = 0x00`（成功），表示已開始發送隨機資料。
