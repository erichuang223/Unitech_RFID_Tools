# Unitech RFID Command Set — 第 4 章

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
| 4 | RFID Data Reply | [Chapter4_RFIDDataReply.md](Chapter4_RFIDDataReply.md)（本章） |
| 5 | RFID Configuration | [Chapter5a](Chapter5a_RFIDConfiguration_1.md) / [Chapter5b](Chapter5b_RFIDConfiguration_2.md) / [Chapter5c](Chapter5c_RFIDConfiguration_3.md) |
| 6 | RFID Extended Operation | [Chapter6_RFIDExtendedOperation.md](Chapter6_RFIDExtendedOperation.md) |
| 7 | Event Notification Commands | [Chapter7_EventNotification.md](Chapter7_EventNotification.md) |
| 8 | Firmware Update Commands | [Chapter8_FirmwareUpdate.md](Chapter8_FirmwareUpdate.md) |

---

## 第 4 章：RFID Data Reply（RFID 資料回覆格式）

### 本章目錄

- [4.1 RFID Data Reply – Inventory Data（盤點資料）— ID = 0xC0](#41-rfid-data-reply--inventory-data盤點資料--id--0xc0)
- [4.2 RFID Data Reply – Inventory Data with TID（盤點資料含 TID）— ID = 0xC1](#42-rfid-data-reply--inventory-data-with-tid盤點資料含-tid--id--0xc1)
- [4.3 RFID Data Reply – Inventory Data with User-Specified Data（盤點資料含指定記憶體）— ID = 0xC2](#43-rfid-data-reply--inventory-data-with-user-specified-data盤點資料含指定記憶體--id--0xc2)
- [4.4 RFID Data Reply – Inventory Data/Phase（盤點資料含相位）— ID = 0xB0](#44-rfid-data-reply--inventory-dataphase盤點資料含相位--id--0xb0)
- [4.5 RFID Data Reply – Inventory Data/Phase with TID（含相位與 TID）— ID = 0xB1](#45-rfid-data-reply--inventory-dataphase-with-tid含相位與-tid--id--0xb1)
- [4.6 RFID Data Reply – Inventory Data/Phase with User-Specified Data（含相位與指定記憶體）— ID = 0xB2](#46-rfid-data-reply--inventory-dataphase-with-user-specified-data含相位與指定記憶體--id--0xb2)
- [4.7 RFID Data Reply – Inventory Data with Customized Format（自訂格式盤點資料）— ID = 0xA0](#47-rfid-data-reply--inventory-data-with-customized-format自訂格式盤點資料--id--0xa0)
- [4.8 RFID Data Reply – Inventory Status（盤點狀態）— ID = 0xC7](#48-rfid-data-reply--inventory-status盤點狀態--id--0xc7)
- [4.9 RFID Data Reply – Tag Access Data（標籤存取結果）— ID = 0xC8](#49-rfid-data-reply--tag-access-data標籤存取結果--id--0xc8)
- [4.10 RFID Data Reply – Operation Error（操作錯誤）— ID = 0xD0](#410-rfid-data-reply--operation-error操作錯誤--id--0xd0)
- [4.11 RFID Data Reply – System Error（系統錯誤）— ID = 0xD1](#411-rfid-data-reply--system-error系統錯誤--id--0xd1)

本章定義 RFID 模組「主動傳送」給主機的各種資料回覆封包。這些封包是第 3 章操作指令（盤點、讀寫等）執行後的實際結果，皆採用第 1 章「RFID Data Send Format」格式：`<STX><LENGTH><ID><DATA><CHECKSUM><ETX>`，不同的 ID 代表不同的回覆內容。

> **共用欄位說明**
>
> | 欄位 | 長度 | 說明 |
> |---|---|---|
> | ANTID | 1 Byte | 讀到此標籤的天線編號（Antenna ID），範圍 1~4 |
> | RSSI | 2 Byte | 接收訊號強度（RSSI value），用於判斷標籤距離/訊號品質 |
> | PC | 2 Byte | EPC 區的協定控制位元（Protocol Control），描述 EPC 長度等資訊 |
> | EPC | N Byte | EPC 區內容（電子產品碼，標籤的主要識別資料） |
> | TID | N Byte | TID 區內容（標籤識別碼，由廠商寫入，唯讀） |
> | PHASEBEGIN | 2 Byte | 相位起始值，單位 0.087 度（in 0.087 degrees） |
> | PHASEEND | 2 Byte | 相位結束值，單位 0.087 度 |
> | FREQ | 4 Byte | 讀取當下的頻率，單位 kHz（例如 902750 = 902.750 MHz） |

---

### 4.1. RFID Data Reply – Inventory Data（盤點資料）— ID = 0xC0

對應 [3.1 Tag Inventory（0x80）](Chapter3_RFIDOperation.md) 的盤點結果，回傳每個讀到標籤的天線、RSSI、PC、EPC。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <ANTID><RSSI><PC><EPC>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 天線編號（1~4） |
| RSSI | 2 Byte | 訊號強度 |
| PC | 2 Byte | EPC 區協定控制位元 |
| EPC | N Byte | EPC 內容 |

#### 範例

```
RFID ⇒ <0x02><0x00 0x15><0x00 0xC0><0x01><0xE9 0x62><0x34 0x00>
  <0xAA 0xAA 0x34 0x12 0xDC 0x03 0x01 0x17 0x16 0x10 0xB9 0x88><0xF9 0xB3><0x03>
```

- `ANTID = 0x01`（第 1 號天線），`RSSI = 0xE9 0x62`，`PC = 0x34 0x00`，
  `EPC = AA AA 34 12 DC 03 01 17 16 10 B9 88`（12 Byte EPC）。

---

### 4.2. RFID Data Reply – Inventory Data with TID（盤點資料含 TID）— ID = 0xC1

在盤點資料中額外附帶 TID 區內容，對應啟用「含 TID 盤點」設定時的盤點結果。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <ANTID><RSSI><PC><EPC><TID>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 天線編號（1~4） |
| RSSI | 2 Byte | 訊號強度 |
| PC | 2 Byte | EPC 區協定控制位元 |
| EPC | N Byte | EPC 內容 |
| TID | N Byte | TID 區內容 |

#### 範例

```
RFID ⇒ <0x02><0x00 0x21><0x00 0xC1><0x01><0xF1 0x2A><0x34 0x00>
  <0xBB 0xBB 0x34 0x12 0xDC 0x03 0x01 0x17 0x16 0x10 0xB9 0x88>
  <0xE2 0x00 0x34 0x12 0x01 0x38 0x01 0x00 0x06 0x7A 0x8E 0x7F>
  <0xF6 0xC5><0x03>
```

- `ANTID = 0x01`，`RSSI = 0xF1 0x2A`，`PC = 0x34 0x00`，
  `EPC = BB BB 34 12 DC 03 01 17 16 10 B9 88`，
  `TID = E2 00 34 12 01 38 01 00 06 7A 8E 7F`。

---

### 4.3. RFID Data Reply – Inventory Data with User-Specified Data（盤點資料含指定記憶體）— ID = 0xC2

對應 [3.11 Tag Inventory with User-Specified Data（0x8B）](Chapter3_RFIDOperation.md)，在盤點資料後附帶讀到的指定記憶體區塊資料。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <ANTID><RSSI><PC><EPC><DATA>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 天線編號（1~4） |
| RSSI | 2 Byte | 訊號強度 |
| PC | 2 Byte | EPC 區協定控制位元 |
| EPC | N Byte | EPC 內容 |
| DATA | N Byte | 指定讀取的記憶體資料（User-Specified Data） |

#### 範例

```
RFID ⇒ <0x02><0x00 0x16><0x00 0xC2><0x01><0xE9 0x62><0x34 0x00>
  <0xAA 0xAA 0x34 0x12 0xDC 0x03 0x01 0x17 0x16 0x10 0xB9 0x88>
  <0x01><0xF9 0xAF><0x03>
```

- `ANTID = 0x01`，`RSSI = 0xE9 0x62`，`PC = 0x34 0x00`，
  `EPC = AA AA 34 12 DC 03 01 17 16 10 B9 88`，後接 User-Specified Data `0x01`。

---

### 4.4. RFID Data Reply – Inventory Data/Phase（盤點資料含相位）— ID = 0xB0

在盤點資料中額外附帶相位（Phase）與頻率（Frequency）資訊，用於需要相位資料的應用（例如定位、移動方向判斷）。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <ANTID><RSSI><PHASEBEGIN><PHASEEND><FREQ><PC><EPC>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 天線編號（1~4） |
| RSSI | 2 Byte | 訊號強度 |
| PHASEBEGIN | 2 Byte | 相位起始值（單位 0.087 度） |
| PHASEEND | 2 Byte | 相位結束值（單位 0.087 度） |
| FREQ | 4 Byte | 頻率（單位 kHz，例如 902750） |
| PC | 2 Byte | EPC 區協定控制位元 |
| EPC | N Byte | EPC 內容 |

#### 範例

```
RFID ⇒ <0x02><0x00 0x1D><0x00 0xB0><0x01><0xE7 0xFC>
  <0x02 0xF5><0x02 0xE8><0x00 0x0D 0xDD 0xCE><0x34 0x04>
  <0x00 0x00 0x00 0x00 0x00 0x00 0x02 0x02 0x10 0x83 0x14 0x16><0xF8 0xBD><0x03>
```

- `ANTID = 0x01`，`RSSI = 0xE7 0xFC`，`PHASEBEGIN = 0x02F5`，`PHASEEND = 0x02E8`，
  `FREQ = 0x000DDDCE`（= 908750 kHz = 908.750 MHz），`PC = 0x34 0x04`，後接 EPC。

---

### 4.5. RFID Data Reply – Inventory Data/Phase with TID（含相位與 TID）— ID = 0xB1

相位盤點資料再加上 TID 區內容。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <ANTID><RSSI><PHASEBEGIN><PHASEEND><FREQ><PC><EPC><TID>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 天線編號（1~4） |
| RSSI | 2 Byte | 訊號強度 |
| PHASEBEGIN | 2 Byte | 相位起始值（單位 0.087 度） |
| PHASEEND | 2 Byte | 相位結束值（單位 0.087 度） |
| FREQ | 4 Byte | 頻率（單位 kHz） |
| PC | 2 Byte | EPC 區協定控制位元 |
| EPC | N Byte | EPC 內容 |
| TID | N Byte | TID 區內容 |

#### 範例

```
RFID ⇒ <0x02><0x00 0x29><0x00 0xB1><0x01><0xF1 0x92>
  <0x01 0x1C><0x01 0x0F><0x00 0x0D 0xCA 0x46><0x34 0x00>
  <0xE2 0x00 0x00 0x1B 0x81 0x09 0x01 0x17 0x8B 0xCC 0xB1 0x47>
  <0xE2 0x00 0x34 0x12 0x01 0x3A 0x01 0x00 0x06 0x7A 0x86 0x3E>
  <0xF5 0x9E><0x03>
```

- `ANTID = 0x01`，`RSSI = 0xF1 0x92`，`PHASEBEGIN = 0x011C`，`PHASEEND = 0x010F`，
  `FREQ = 0x000DCA46`，`PC = 0x34 0x00`，後接 EPC（12 Byte）與 TID（12 Byte）。

---

### 4.6. RFID Data Reply – Inventory Data/Phase with User-Specified Data（含相位與指定記憶體）— ID = 0xB2

相位盤點資料再加上指定記憶體區塊資料。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <ANTID><RSSI><PHASEBEGIN><PHASEEND><FREQ><PC><EPC><DATA>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 天線編號（1~4） |
| RSSI | 2 Byte | 訊號強度 |
| PHASEBEGIN | 2 Byte | 相位起始值（單位 0.087 度） |
| PHASEEND | 2 Byte | 相位結束值（單位 0.087 度） |
| FREQ | 4 Byte | 頻率（單位 kHz） |
| PC | 2 Byte | EPC 區協定控制位元 |
| EPC | N Byte | EPC 內容 |
| DATA | N Byte | 指定讀取的記憶體資料（User-Specified Data） |

#### 範例

```
RFID ⇒ <0x02><0x00 0x1F><0x00 0xB2><0x01><0xE8 0xE3>
  <0x04 0x29><0x04 0x02><0x00 0x0D 0xD9 0xE6><0x34 0x00>
  <0xE2 0x00 0x34 0x12 0xDC 0x03 0x01 0x17 0x75 0x16 0x60 0x13>
  <0xE2 0x00><0xF7 0x31><0x03>
```

- `ANTID = 0x01`，`RSSI = 0xE8 0xE3`，`PHASEBEGIN = 0x0429`，`PHASEEND = 0x0402`，
  `FREQ = 0x000DD9E6`，`PC = 0x34 0x00`，後接 EPC，最後 User-Specified Data `0xE2 0x00`。

---

### 4.7. RFID Data Reply – Inventory Data with Customized Format（自訂格式盤點資料）— ID = 0xA0

可由使用者自訂回傳哪些欄位（透過 [5.37/5.38 Inventory Data with Customized Format Settings](Chapter5b_RFIDConfiguration_2.md) 設定）。每個欄位以一組「參數（Parameter）」表示，封包中依序排列多組參數。

```
RFID ⇒ <STX><LENGTH><ID><PARAMETER1>…<PARAMETERn><CHECKSUM><ETX>
```

#### 參數（PARAMETER）格式

| 欄位 | 長度 | 說明 |
|---|---|---|
| PLEN | 2 Byte | 此參數的長度（含 PLEN、PID、PDATA，以 byte 計） |
| PID | 1 Byte | 參數識別碼（Parameter ID），見下方 PID 對照表 |
| PDATA | N Byte | 參數資料，內容依 PID 而定 |

#### PID（參數識別碼）中英對照表

| PID | 名稱（中文） | 原文 |
|---|---|---|
| 0x00 | EPC | EPC |
| 0x01 | PC（協定控制位元） | PC |
| 0x02 | RSSI（訊號強度） | RSSI |
| 0x03 | 天線編號 | Antenna ID |
| 0x04 | TID | TID |
| 0x05 | 使用者指定記憶體 | User specified memory |
| 0x06 | 相位 | Phase |
| 0x07 | 頻率 | Frequency |
| 0x08 | 時間戳記 | Timestamp |

> 此 PID 對照表與 [5.37/5.38（0x128/0x129）](Chapter5b_RFIDConfiguration_2.md) 設定自訂盤點格式時所用的 PID 相同，兩處互相呼應。

#### PDATA 的特殊子格式

**Phase（PID = 0x06）的 PDATA 結構：**

| 欄位 | 長度 | 說明 |
|---|---|---|
| Phase Begin | 2 Byte | 相位起始值（單位 0.087 度） |
| Phase End | 2 Byte | 相位結束值（單位 0.087 度） |

**Timestamp（PID = 0x08）的 PDATA 結構：**

| 欄位 | 長度 | 說明 |
|---|---|---|
| YEAR | 1 Byte | 年（從 2000 起算，例如 1 代表 2001） |
| MON | 1 Byte | 月 |
| DAY | 1 Byte | 日 |
| HOU | 1 Byte | 時 |
| MIN | 1 Byte | 分 |
| SEC | 1 Byte | 秒 |

#### 範例

```
RFID ⇒ <0x02><0x00 0x1C><0x00 0xA0><0x00 0x0F><0x00>
  <0xE2 0x00 0x34 0x12 0xDC 0x03 0x01 0x11 0x77 0x43 0x31 0x43 0x80>
  <0x00 0x09><0x08><0x19 0x03 0x15 0x0E 0x21 0x39>
  <0xFB 0x79><0x03>
```

- 第 1 組參數：`PLEN = 0x000F`（15 Byte），`PID = 0x00`（EPC），`PDATA = E2 00 34 12 DC 03 01 11 77 43 31 43 80`（EPC 內容）。
- 第 2 組參數：`PLEN = 0x0009`（9 Byte），`PID = 0x08`（Timestamp），`PDATA = 19 03 15 0E 21 39`，
  即 YEAR=0x19(25→2025)、MON=0x03(3 月)、DAY=0x15(21 日)、HOU=0x0E(14 時)、MIN=0x21(33 分)、SEC=0x39(57 秒)。

---

### 4.8. RFID Data Reply – Inventory Status（盤點狀態）— ID = 0xC7

回報盤點流程的狀態，例如通知主機「本輪盤點已結束」。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <INVSTATUS>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| INVSTATUS | 1 Byte | 盤點狀態，見下方對照表 |

#### INVSTATUS 對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0x00 | 盤點完成 | Inventory Done |

#### 範例

```
RFID ⇒ <0x02><0x00 0x05><0x00 0xC7><0x00><0xFF 0x34><0x03>
```

- `INVSTATUS = 0x00`，表示本次盤點已結束（Inventory Done）。

---

### 4.9. RFID Data Reply – Tag Access Data（標籤存取結果）— ID = 0xC8

對應第 3 章各種「存取類」操作（讀 0x82、寫 0x83、鎖 0x84、Kill 0x85、區塊寫 0x86、區塊永久鎖 0x87、認證 0x88、MarginRead 0x8C）的執行結果，回報是哪個操作、成功或錯誤碼，以及（讀取類）讀回的資料。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <OPID><TAERRCODE><MDATA>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| OPID | 2 Byte | 操作識別碼（Operation ID），即對應的第 3 章指令 ID，見下方 OPID 對照表 |
| TAERRCODE | 1 Byte | 標籤存取錯誤碼（Tag Access Error Code），見下方對照表 |
| MDATA | N Byte | 依操作而定的資料內容（見下方說明） |

**MDATA 內容依 OPID 而定：**

| OPID | MDATA 內容 |
|---|---|
| 0x82（讀取） | 讀回的記憶體資料（Memory Data） |
| 0x87（區塊永久鎖） | 遮罩（Mask），僅當 READLOCK=0（讀取鎖定狀態）時回傳 |
| 0x88（認證） | 長度 Len（當 INCREPLEN=1 時）+ 認證回應 Response。Len 為 15 bit 長度 + 1 bit 同位元（Parity） |

#### OPID（操作識別碼）中英對照表

| OPID | 名稱（中文） | 原文 |
|---|---|---|
| 0x82 | 標籤讀取資料 | Tag Read Data |
| 0x83 | 標籤寫入資料 | Tag Write Data |
| 0x84 | 標籤鎖定 | Tag Lock |
| 0x85 | 標籤 Kill | Tag Kill |
| 0x86 | 標籤區塊寫入 | Tag Block Write |
| 0x87 | 標籤區塊永久鎖定 | Tag Block PermaLock |
| 0x88 | 標籤認證 | Tag Authenticate |

#### TAERRCODE（標籤存取錯誤碼）中英對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0x00 | 成功 | Success |
| 0x01 | 找不到標籤 | No-Tag found |
| 0x02 | 標籤遺失 | Tag Lost |
| 0x03 | 標籤動作無回應 | Tag Action Not Reply |
| 0x04 | 密碼無效 | Invalid Password |
| 0x05 | Kill 密碼為零（不可用） | Zero Kill Password |
| 0x80~0x8F | 標籤回覆錯誤（依 Gen2 規格） | Tag-Reply Error (Gen2 Spec) |

#### 範例

```
RFID ⇒ <0x02><0x00 0x09><0x00 0xC8><0x00 0x82><0x00><0xAA 0xAA>
  <0xFD 0x59><0x03>
```

- `OPID = 0x0082`（標籤讀取），`TAERRCODE = 0x00`（成功），`MDATA = 0xAA 0xAA`（讀回的記憶體資料）。

---

### 4.10. RFID Data Reply – Operation Error（操作錯誤）— ID = 0xD0

當操作過程發生與天線/設定相關的錯誤時回報，指出是哪個天線與錯誤碼。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <ANTID><OPERRCODE>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 發生錯誤的天線編號（1~4） |
| OPERRCODE | 1 Byte | 操作錯誤碼（Operation Error Code），見下方對照表 |

#### OPERRCODE（操作錯誤碼）中英對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0x00 | 成功 | Success |
| 0x01~0x11 | Ex10 操作錯誤 | Ex10 Operation Error |
| 0xD0 | Ex10 初始化錯誤 | Ex10 Initialized Error |
| 0xD1 | 未啟用任何天線錯誤 | No Antenna Enabled Error |
| 0xD2 | 天線未連接錯誤 | Antenna Disconnected Error |
| 0xD3 | 停留時間/盤點輪數設定錯誤 | Incorrect DwellTime/RoundCount Error |

> 註：Ex10 為 Impinj 讀取晶片系列名稱，0x01~0x11 屬該晶片回報的操作錯誤。

#### 範例

```
RFID ⇒ <0x02><0x00 0x06><0x00 0xD0><0x04><0x0A><0xFF 0x1C><0x03>
```

- `ANTID = 0x04`（第 4 號天線），`OPERRCODE = 0x0A`（屬 Ex10 操作錯誤範圍 0x01~0x11）。

---

### 4.11. RFID Data Reply – System Error（系統錯誤）— ID = 0xD1

回報系統層級的錯誤，目前主要為溫度過高保護。

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <SYSERRCODE>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| SYSERRCODE | 1 Byte | 系統錯誤碼（System Error Code），見下方對照表 |

#### SYSERRCODE（系統錯誤碼）中英對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0x01 | 環境溫度過高 | Ambient Temperature Too High |
| 0x02 | PA（功率放大器）溫度過高 | PA Temperature Too High |

> 與 [2.5/2.6 溫度讀取](Chapter2_GeneralCommands.md) 及 [5.29~5.34 溫度保護設定](Chapter5b_RFIDConfiguration_2.md) 相關：當溫度超過設定門檻時，模組會主動送出此系統錯誤。

#### 範例

```
RFID ⇒ <0x02><0x00 0x05><0x00 0xD1><0x01><0xFF 0x29><0x03>
```

- `SYSERRCODE = 0x01`，表示**環境溫度過高**，模組已觸發溫度保護。
