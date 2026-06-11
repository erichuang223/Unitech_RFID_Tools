# Unitech RFID Command Set

- **版本 (Version)**: V1.02
- **說明**: 藍色字體標示為新增/更新功能 (New/Updated functions in blue)
- **適用韌體 (Applied to FW)**: RM300P V0.1.0.14 或更新版本

---

## 文件章節目錄 (Document Table of Contents)

| 章節 | 標題 | 說明 |
|---|---|---|
| 1 | Command Format | 通訊封包格式定義（本章） |
| 2 | General Commands | 一般指令（讀取版本、型號、序號、溫度、重置等） |
| 3 | RFID Operation | RFID 標籤操作指令（盤點、讀寫、鎖定、Kill 等） |
| 4 | RFID Data Reply | RFID 資料回覆封包格式 |
| 5 | RFID Configuration | RFID 參數設定（區域、天線、Gen2、GPIO 等） |
| 6 | RFID Extended Operation | RFID 擴充操作（CW On/Off、發送隨機資料） |
| 7 | Event Notification Commands | 事件通知指令 |
| 8 | Firmware Update Commands | 韌體更新指令 |

---

## 第 1 章：Command Format（通訊封包格式）

### 本章目錄

- [1.1 Host Command Formats（主機指令格式）](#11-host-command-formats主機指令格式)
- [1.2 RFID Response Formats（RFID 回覆格式）](#12-rfid-response-formats-rfid-回覆格式)
- [1.3 RFID Data Send Format（RFID 資料傳送格式）](#13-rfid-data-send-format-rfid-資料傳送格式)

本章定義主機（Host）與 RFID 模組之間溝通所使用的封包格式，包含：主機下達指令的格式、RFID 模組回覆的格式，以及 RFID 模組主動傳送資料給主機的格式。所有封包都以 `<STX>` 開頭、`<ETX>` 結尾，並包含長度與檢查碼欄位以確保資料正確性。

---

### 1.1. Host Command Formats（主機指令格式）

主機（Host）發送指令給 RFID 模組時，使用以下兩種封包格式之一：

```
<STX><LENGTH><ID><CHECKSUM><ETX>
<STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

- 第一種格式：不含 `DATA` 欄位，用於不需要附加參數的指令（例如：讀取版本）。
- 第二種格式：包含 `DATA` 欄位，用於需要附加參數的指令（例如：寫入標籤資料）。

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| STX | 1 Byte | 封包起始碼，固定為 `0x02`（Start of the command，封包開始） |
| LENGTH | 2 Byte | 指令封包長度（不含 CHECKSUM 與 STX/ETX 本身的位元組數） |
| ID | 2 Byte | 指令識別碼（Command ID），用來辨別此封包要執行的指令種類 |
| DATA | N Byte | 指令參數資料，依指令不同而長度不同（N 為可變長度） |
| CHECKSUM | 2 Byte | 檢查碼，為 `LENGTH + ID + DATA` 加總後取 2's complement（二補數），但不包含 CHECKSUM 本身與 STX/ETX |
| ETX | 1 Byte | 封包結束碼，固定為 `0x03`（End of the command，封包結束） |

---

### 1.2. RFID Response Formats（RFID 回覆格式）

RFID 模組收到主機指令後，會回覆以下兩種封包格式之一：

```
<STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
<STX><LENGTH><ID><STATUS><DATA><CHECKSUM><ETX>
```

- 第一種格式：僅回覆執行狀態 `STATUS`，不含額外資料（例如：設定類指令的回覆）。
- 第二種格式：除了 `STATUS` 外，還包含回覆資料 `DATA`（例如：讀取類指令的回覆）。

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| STX | 1 Byte | 封包起始碼，固定為 `0x02`（Start of the command，封包開始） |
| LENGTH | 2 Byte | 回覆封包長度（不含 CHECKSUM 與 STX/ETX 本身的位元組數） |
| ID | 2 Byte | 對應指令的識別碼（Command ID），與主機發出的指令 ID 相同 |
| STATUS | 1 Byte | 執行狀態碼；`0x00` 表示成功，其餘代表各種錯誤（詳見下表） |
| DATA | N Byte | 回覆資料內容，依指令不同而長度不同（N 為可變長度） |
| CHECKSUM | 2 Byte | 檢查碼，為 `LENGTH + ID + DATA` 加總後取 2's complement（二補數），但不包含 CHECKSUM 本身與 STX/ETX |
| ETX | 1 Byte | 封包結束碼，固定為 `0x03`（End of the command，封包結束） |

#### STATUS 狀態碼對照表

| 數值 | 說明（中文） | 原文說明 |
|---|---|---|
| 0x00 | 成功 | Success |
| 0x01 | 裝置忙碌中 | Busy |
| 0x02 | 參數無效 | Invalid parameters |
| 0x03 | 不支援此指令 | Not Supported |
| 0x81 | 封包格式錯誤（ID:0xFFFF）— 長度不正確 | Format Error (ID:0xFFFF) – incorrect Length |
| 0x82 | 封包格式錯誤（ID:0xFFFF）— CRC/檢查碼不正確 | Format Error (ID:0xFFFF) – incorrect CRC |
| 0x83 | 封包格式錯誤（ID:0xFFFF）— 結束碼不正確 | Format Error (ID:0xFFFF) – incorrect End-Of-Packet |
| 0x84 | 封包格式錯誤（ID:0xFFFF）— 封包逾時 | Format Error (ID:0xFFFF) – packet timeout |
| 0xE0 | 其他錯誤 | Other Error |
| 0xE1 | 找不到目標標籤 | Target Not Found |

---

### 1.3. RFID Data Send Format（RFID 資料傳送格式）

當 RFID 模組需要主動將資料傳送給主機時（例如：盤點過程中持續回報讀到的標籤資料），使用以下封包格式：

```
<STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| STX | 1 Byte | 封包起始碼，固定為 `0x02`（Start of the command，封包開始） |
| LENGTH | 2 Byte | 封包長度（不含 CHECKSUM 與 STX/ETX 本身的位元組數） |
| ID | 2 Byte | 此資料傳送封包固定為 `0x80` |
| DATA | N Byte | 傳送給主機的資料內容（N 為可變長度） |
| CHECKSUM | 2 Byte | 檢查碼，為 `LENGTH + ID + DATA` 加總後取 2's complement（二補數），但不包含 CHECKSUM 本身與 STX/ETX |
| ETX | 1 Byte | 封包結束碼，固定為 `0x03`（End of the command，封包結束） |
