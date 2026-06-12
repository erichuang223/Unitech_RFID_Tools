# Unitech RFID Command Set — 第 8 章

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
| 7 | Event Notification Commands | [Chapter7_EventNotification.md](Chapter7_EventNotification.md) |
| 8 | Firmware Update Commands | [Chapter8_FirmwareUpdate.md](Chapter8_FirmwareUpdate.md)（本章） |

---

## 第 8 章：Firmware Update Commands（韌體更新指令）

### 本章目錄

- [8.1 Start File Transfer（開始檔案傳輸）— Op-ID = 0x6101](#81-start-file-transfer開始檔案傳輸--op-id--0x6101)
- [8.2 Send File Data（傳送檔案資料）— Op-ID = 0x6102](#82-send-file-data傳送檔案資料--op-id--0x6102)
- [8.3 Perform Firmware Update（執行韌體更新）— Op-ID = 0x6103](#83-perform-firmware-update執行韌體更新--op-id--0x6103)

本章定義韌體更新流程的三個步驟指令。**典型流程**：
1. [8.1 Start File Transfer](#81-start-file-transfer開始檔案傳輸--op-id--0x6101)：告知檔名與檔案大小，開始傳輸。
2. [8.2 Send File Data](#82-send-file-data傳送檔案資料--op-id--0x6102)：分多個封包傳送韌體二進位資料。
3. [8.3 Perform Firmware Update](#83-perform-firmware-update執行韌體更新--op-id--0x6103)：傳輸完成後執行更新。

所有指令皆遵循第 1 章封包格式（此處 ID 欄位帶 Op-ID）。

> **韌體更新模式（DATA 於 8.3）中英對照表**
>
> | 數值 | 模式（中文） | 原文 |
> |---|---|---|
> | 1 | 檔案更新模式（RM300P 適用） | File Update Mode (For RM300P) |
> | 2 | Y-Modem 更新模式 | Y-Modem Update Mode |
>
> 註：8.1 與 8.2 僅適用於「檔案更新模式（File Update Mode）」。

---

### 8.1. Start File Transfer（開始檔案傳輸）— Op-ID = 0x6101

開始韌體檔案傳輸，DATA 中以字串提供檔名與檔案大小（各以 `\0` 結尾）。本操作僅用於檔案更新模式（File Update Mode）。

```
Host ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| DATA | N Byte | `"檔名" + "\0" + "檔案大小" + "\0"`（兩個 ASCII 字串，各以 NULL 結尾） |

DATA 格式：`DATA = "File Name" + "\0" + "File Size" + "\0"`

#### 範例

```
Host ⇒ <0x02><0x00 0x15><0x61 0x01>
  <0x53><0x4C><0x32><0x32><0x30><0x46><0x57><0x2E>
  <0x62><0x69><0x6E><0x00>
  <0x34><0x30><0x39><0x36><0x00>
  <0xFB 0x7F><0x03>
  ; DATA = "SL220FW.bin\04096\0"
RFID ⇒ <0x02><0x00 0x05><0x61 0x01><0x00><0xFF 0xF9><0x03>
```

- Host 發送的 DATA 為兩段 ASCII 字串：
  - 檔名 `"SL220FW.bin"`（位元組 `53 4C 32 32 30 46 57 2E 62 69 6E`）+ `\0`（`0x00`）。
  - 檔案大小 `"4096"`（位元組 `34 30 39 36`）+ `\0`（`0x00`），即韌體大小 **4096** Bytes。
- RFID 回覆：`STATUS = 0x00`（成功），表示已準備接收檔案資料。

---

### 8.2. Send File Data（傳送檔案資料）— Op-ID = 0x6102

傳送韌體二進位資料封包。檔案通常需分多個封包傳送（Multi-Packets），RM300P 每封包最大 1024 Bytes。本操作僅用於檔案更新模式（File Update Mode）。

```
Host ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| DATA | N Byte | 韌體二進位封包（Firmware binary packet）。可分多封包傳送；RM300P 單封包最大 1024 Bytes |

#### 範例

```
Host ⇒ <0x02><0x00 0x15><0x61 0x02>
  <0x20><0x01><0x07><0xE8><0x80><0x02><0x03><0xA1>
  <0x08><0x02><0x3D><0x71><0x08><0x02><0x3C><0x6F>
  <0x08>
  <0xFB 0xDD><0x03>
RFID ⇒ <0x02><0x00 0x05><0x61 0x02><0x00><0xFF 0xF8><0x03>
```

- Host 發送：DATA 為一段韌體二進位資料（本例 17 Bytes）。
- RFID 回覆：`STATUS = 0x00`（成功），表示已接收此封包。後續封包重複此指令直到全部資料傳完。

---

### 8.3. Perform Firmware Update（執行韌體更新）— Op-ID = 0x6103

在檔案傳輸完成後，指示模組執行韌體更新，並指定更新模式。

```
Host ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| DATA | 1 Byte | 韌體更新模式，見下方對照表 |

#### 韌體更新模式對照表

| 數值 | 模式（中文） | 原文 |
|---|---|---|
| 1 | 檔案更新模式（RM300P 適用，搭配 8.1/8.2） | File Update Mode (For RM300P) |
| 2 | Y-Modem 更新模式 | Y-Modem Update Mode |

#### 範例

```
Host ⇒ <0x02><0x00 0x05><0x61 0x03><0x01><0xFF 0x96><0x03>
RFID ⇒ <0x02><0x00 0x05><0x61 0x03><0x00><0xFF 0x97><0x03>
```

- Host 發送：`DATA = 0x01`（檔案更新模式），要求執行韌體更新。
- RFID 回覆：`STATUS = 0x00`（成功），表示已開始執行韌體更新。
