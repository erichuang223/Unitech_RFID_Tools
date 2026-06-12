# Unitech RFID Command Set — 第 7 章

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
| 6 | RFID Extended Operation | [Chapter6_RFIDExtendedOperation.md](Chapter6_RFIDExtendedOperation.md) |
| 7 | Event Notification Commands | [Chapter7_EventNotification.md](Chapter7_EventNotification.md)（本章） |
| 8 | Firmware Update Commands | [Chapter8_FirmwareUpdate.md](Chapter8_FirmwareUpdate.md) |

---

## 第 7 章：Event Notification Commands（事件通知指令）

### 本章目錄

- [7.1 Event Notification Enable/Disable Command（事件通知開關）— ID = 0x90](#71-event-notification-enabledisable-command事件通知開關--id--0x90)
- [7.2 Event Notification Command - Antenna Status（天線狀態事件）— ID = 0xE0](#72-event-notification-command---antenna-status天線狀態事件--id--0xe0)
- [7.3 Event Notification Command - LBT Status（LBT 狀態事件）— ID = 0xE1](#73-event-notification-command---lbt-status lbt-狀態事件--id--0xe1)

本章定義事件通知機制：主機可透過 7.1 啟用/停用特定事件，啟用後模組會在事件發生時主動送出對應的通知封包（7.2 天線狀態、7.3 LBT 狀態）。事件通知封包採用第 1 章「RFID Data Send Format」格式主動傳送。

> **EVENTID（事件識別碼）中英對照表**
>
> | 數值 | 事件（中文） | 原文 |
> |---|---|---|
> | 0xE0 | RFID 天線開始/結束 | RFID Antenna Begin/End |
> | 0xE1 | RFID LBT 狀態 | RFID LBT Status |

---

### 7.1. Event Notification Enable/Disable Command（事件通知開關）— ID = 0x90

啟用或停用指定事件的通知。啟用後，該事件發生時模組會主動送出對應通知（7.2 / 7.3）。

```
Host ⇒ <STX><LENGTH><ID><EVENTID><DATA><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| EVENTID | 1 Byte | 事件識別碼，見上方 EVENTID 對照表（0xE0：天線；0xE1：LBT） |
| DATA | 1 Byte | 啟用旗標；1：啟用（Enable），0：停用（Disable） |

#### 範例

```
Host ⇒ <0x02><0x00 0x06><0x00 0x90><0xE0><0x01><0xFE 0x89><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x90><0x00><0xFF 0x68><0x03>
```

- Host 發送：`EVENTID = 0xE0`（天線開始/結束事件），`DATA = 0x01`（啟用）。
- RFID 回覆：`STATUS = 0x00`（成功），表示已啟用天線狀態事件通知。

---

### 7.2. Event Notification Command - Antenna Status（天線狀態事件）— ID = 0xE0

當天線開始或結束作用時，模組主動送出此通知（需先以 7.1 啟用 EVENTID = 0xE0）。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <ANTID><ANTSTATE>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 天線編號（1~4） |
| ANTSTATE | 1 Byte | 天線狀態，見下方對照表 |

#### ANTSTATE（天線狀態）中英對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0x00 | 天線開始作用 | Begin |
| 0x01 | 天線結束作用 | End |

> 註：此處 ANTSTATE 表示「天線作用的開始/結束」，與 [5.5/5.6 Antenna State](Chapter5a_RFIDConfiguration_1.md) 的「啟用/停用」語意不同，請勿混淆。

#### 範例

```
RFID ⇒ <0x02><0x00 0x06><0x00 0xE0><0x01><0x00><0xFF 0x19><0x03>
```

- `ANTID = 0x01`（第 1 號天線），`ANTSTATE = 0x00`（Begin，開始作用）。

---

### 7.3. Event Notification Command - LBT Status（LBT 狀態事件）— ID = 0xE1

當 LBT（聽後再說）偵測到通道狀態變化時，模組主動送出此通知（需先以 7.1 啟用 EVENTID = 0xE1）。回報是否偵測到干擾、訊號強度與頻率。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <INTERF><RSSI><FREQ>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| INTERF | 1 Byte | 干擾狀態（Interferer status），見下方對照表 |
| RSSI | 2 Byte | 偵測到的訊號強度（RSSI value） |
| FREQ | 4 Byte | 偵測頻率，單位 kHz（例如 902750 = 902.750 MHz） |

#### INTERF（干擾狀態）中英對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0 | 無干擾源 | No Interferer |
| 1 | 偵測到干擾源 | Interferer present |

#### 範例

```
RFID ⇒ <0x02><0x00 0x0B><0x00 0xE1><0x00><0xE1 0x10><0x00 0x0D 0xE7 0x92>
  <0xFC 0x9D><0x03>
```

- `INTERF = 0x00`（無干擾源），`RSSI = 0xE1 0x10`，
  `FREQ = 0x000DE792` = 911250 kHz = **911.250 MHz**。

> LBT 的啟用見 [5.15/5.16](Chapter5a_RFIDConfiguration_1.md)，門檻設定見 [5.51/5.52](Chapter5c_RFIDConfiguration_3.md)。
