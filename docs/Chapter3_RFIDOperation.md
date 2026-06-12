# Unitech RFID Command Set — 第 3 章

- **版本 (Version)**: V1.02
- **說明**: 藍色字體標示為新增/更新功能 (New/Updated functions in blue)
- **適用韌體 (Applied to FW)**: RM300P V0.1.0.14 或更新版本

---

## 文件章節目錄 (Document Table of Contents)

| 章節 | 標題 | 檔案 |
|---|---|---|
| 1 | Command Format | [Chapter1_CommandFormat.md](Chapter1_CommandFormat.md) |
| 2 | General Commands | [Chapter2_GeneralCommands.md](Chapter2_GeneralCommands.md) |
| 3 | RFID Operation | [Chapter3_RFIDOperation.md](Chapter3_RFIDOperation.md)（本章） |
| 4 | RFID Data Reply | [Chapter4_RFIDDataReply.md](Chapter4_RFIDDataReply.md) |
| 5 | RFID Configuration | [Chapter5a](Chapter5a_RFIDConfiguration_1.md) / [Chapter5b](Chapter5b_RFIDConfiguration_2.md) / [Chapter5c](Chapter5c_RFIDConfiguration_3.md) |
| 6 | RFID Extended Operation | [Chapter6_RFIDExtendedOperation.md](Chapter6_RFIDExtendedOperation.md) |
| 7 | Event Notification Commands | [Chapter7_EventNotification.md](Chapter7_EventNotification.md) |
| 8 | Firmware Update Commands | [Chapter8_FirmwareUpdate.md](Chapter8_FirmwareUpdate.md) |

---

## 第 3 章：RFID Operation（RFID 標籤操作指令）

### 本章目錄

- [3.1 Tag Inventory Operation Command（標籤盤點）— ID = 0x80](#31-tag-inventory-operation-command標籤盤點--id--0x80)
- [3.2 Cancel Operation Command（取消操作）— ID = 0x81](#32-cancel-operation-command取消操作--id--0x81)
- [3.3 Tag Read Operation Command（讀取標籤）— ID = 0x82](#33-tag-read-operation-command讀取標籤--id--0x82)
- [3.4 Tag Write Operation Command（寫入標籤）— ID = 0x83](#34-tag-write-operation-command寫入標籤--id--0x83)
- [3.5 Tag Lock Operation Command（鎖定標籤）— ID = 0x84](#35-tag-lock-operation-command鎖定標籤--id--0x84)
- [3.6 Tag Kill Operation Command（永久停用標籤）— ID = 0x85](#36-tag-kill-operation-command永久停用標籤--id--0x85)
- [3.7 Tag BlockWrite Operation Command（區塊寫入）— ID = 0x86](#37-tag-blockwrite-operation-command區塊寫入--id--0x86)
- [3.8 Tag BlockPermalock Operation Command（區塊永久鎖定）— ID = 0x87](#38-tag-blockpermalock-operation-command區塊永久鎖定--id--0x87)
- [3.9 Tag Authenticate Operation Command（標籤認證）— ID = 0x88](#39-tag-authenticate-operation-command標籤認證--id--0x88)
- [3.10 Tag Select Operation Command（標籤篩選）— ID = 0x8A](#310-tag-select-operation-command標籤篩選--id--0x8a)
- [3.11 Tag Inventory with User-Specified Data（盤點並附帶指定記憶體資料）— ID = 0x8B](#311-tag-inventory-with-user-specified-data-operation-command盤點並附帶指定記憶體資料--id--0x8b)
- [3.12 Tag MarginRead Operation Command（餘裕讀取）— ID = 0x8C](#312-tag-marginread-operation-command餘裕讀取--id--0x8c)

本章定義對 EPC Gen2 / ISO 18000-6C 標籤進行各種操作的指令：盤點（Inventory）、讀取、寫入、鎖定、Kill、區塊寫入/永久鎖定、認證、篩選（Select）等。所有指令皆遵循第 1 章定義的封包格式。

> **重要說明：兩段式回覆**
> 本章大部分操作指令的回覆分為兩個階段：
> 1. **指令受理回覆（RFID Response）**：模組收到指令後立即回覆 `STATUS`，表示是否成功開始執行（例如 `0x00` 成功）。
> 2. **資料回覆（Tag Data Reply）**：實際操作標籤所得的結果，由模組主動以資料傳送封包送回，格式定義於第 4 章。盤點類結果見 [Ch4.1](Chapter4_RFIDDataReply.md)，存取類（讀/寫/鎖/Kill 等）結果見 [Ch4.9 Tag Access Data (0xC8)](Chapter4_RFIDDataReply.md)。

> **共用欄位說明（多個指令會用到）**
>
> | 欄位 | 長度 | 說明 |
> |---|---|---|
> | ACCPW | 4 Byte | 存取密碼（Access Password）。標籤受密碼保護時需提供，否則填 `0x00000000` |
> | KILLPW | 4 Byte | Kill 密碼（Kill Password），用於永久停用標籤 |
> | MBANK | 1 Byte | 記憶體區塊（Memory Bank），見下方 MBANK 對照表 |
> | MADDR | 2 Byte | 記憶體起始位址（Memory Start Address），單位依指令而定（word / bit / block） |
> | MLEN | 1 Byte | 記憶體長度（Memory Length），單位依指令而定（word / bit / block） |
> | MDATA | N Byte | 記憶體資料（Memory Data），寫入或比對用的資料內容 |

> **MBANK（記憶體區塊）中英對照表**
>
> | 數值 | 名稱 | 說明（中文） | 原文說明 |
> |---|---|---|---|
> | 0 | RESV | 保留區（含 Kill 密碼與存取密碼） | Reserved (Kill PW & Access PW) |
> | 1 | EPC | EPC 區（電子產品碼） | EPC |
> | 2 | TID | TID 區（標籤識別碼，唯讀，由廠商寫入） | TID (Tag ID) |
> | 3 | USER | 使用者區（可自由讀寫的資料區） | User |

---

### 3.1. Tag Inventory Operation Command（標籤盤點）— ID = 0x80

啟動一次標籤盤點操作，讓模組在天線範圍內讀取所有可讀到的標籤 EPC。盤點到的標籤資料會以「Inventory Data」格式（[Ch4.1](Chapter4_RFIDDataReply.md)）主動回傳給主機。

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

本指令不需要參數（無 DATA 欄位）。

#### 範例

```
Host ⇒ <0x02><0x00 0x04><0x00 0x80><0xFF 0x7C><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x80><0x00><0xFF 0x7B><0x03>

; Tag Data Reply – Inventory Data. See Ch4.1
RFID ⇒ <0x02><0x00 0x15><0x00 0xC0><0x01><0xE9 0x62><0x34 0x00>
  <0xAA 0xAA 0x34 0x12 0xDC 0x03 0x01 0x17 0x16 0x10 0xB9 0x88><0xF9 0xB3><0x03>
```

- Host 發送：`ID = 0x00 0x80`，無 DATA，要求開始盤點。
- RFID 回覆：`STATUS = 0x00`（成功），表示已開始盤點。
- 隨後模組主動送出 **Inventory Data（ID = 0xC0）** 封包，內含讀到的標籤資料（EPC = `AA AA 34 12 DC 03 01 17 16 10 B9 88`），其詳細欄位結構見 [Ch4.1](Chapter4_RFIDDataReply.md)。

---

### 3.2. Cancel Operation Command（取消操作）— ID = 0x81

取消目前正在進行的操作（例如停止持續進行中的盤點）。

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

本指令不需要參數（無 DATA 欄位）。

#### 範例

```
Host ⇒ <0x02><0x00 0x04><0x00 0x81><0xFF 0x7B><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x81><0x00><0xFF 0x7A><0x03>
```

- Host 發送：`ID = 0x00 0x81`，無 DATA，要求取消目前操作。
- RFID 回覆：`STATUS = 0x00`（成功），表示已取消。

---

### 3.3. Tag Read Operation Command（讀取標籤）— ID = 0x82

從標籤指定的記憶體區塊讀取資料。讀取結果以 **Tag Access Data（[Ch4.9](Chapter4_RFIDDataReply.md)）** 回傳。

```
Host ⇒ <STX><LENGTH><ID><ACCPW><MBANK><MADDR><MLEN><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ACCPW | 4 Byte | 存取密碼 |
| MBANK | 1 Byte | 記憶體區塊（RESV/EPC/TID/USER，見 MBANK 對照表） |
| MADDR | 2 Byte | 記憶體起始位址（單位：word，每 word = 2 Byte） |
| MLEN | 1 Byte | 讀取長度（單位：word） |

#### 範例

```
Host ⇒ <0x02><0x00 0x0C><0x00 0x82><0x12 0x34 0x56 0x78><0x01>
  <0x00 0x02><0x01><0xFE 0x5A><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x82><0x00><0xFF 0x79><0x03>

; Tag Data Reply – Tag Access Data. See Ch4.2
RFID ⇒ <0x02><0x00 0x09><0x00 0xC8><0x00 0x82><0x00><0xAA 0xAA>
  <0xFD 0x59><0x03>
```

- Host 發送：`ACCPW = 0x12345678`，`MBANK = 0x01`（EPC 區），`MADDR = 0x0002`（第 2 個 word），`MLEN = 0x01`（讀 1 個 word）。
- RFID 回覆：`STATUS = 0x00`（成功），表示已開始讀取。
- 隨後送出 **Tag Access Data（ID = 0xC8）**：對應指令 `0x0082`、`STATUS = 0x00`、讀回資料 `0xAA 0xAA`，詳見 [Ch4.9](Chapter4_RFIDDataReply.md)。

---

### 3.4. Tag Write Operation Command（寫入標籤）— ID = 0x83

將資料寫入標籤指定的記憶體區塊。

```
Host ⇒ <STX><LENGTH><ID><ACCPW><MBANK><MADDR><MLEN><MDATA><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ACCPW | 4 Byte | 存取密碼 |
| MBANK | 1 Byte | 記憶體區塊（RESV/EPC/TID/USER） |
| MADDR | 2 Byte | 記憶體起始位址（單位：word） |
| MLEN | 1 Byte | 寫入長度（單位：word） |
| MDATA | 2N Byte | 寫入資料（單位：word，故為 2N Byte） |

#### 範例

```
Host ⇒ <0x02><0x00 0x0E><0x00 0x83><0x12 0x34 0x56 0x78>
    <0x01><0x00 0x02><0x01><0xBB 0xBB><0xFC 0xE1><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x83><0x00><0xFF 0x78><0x03>

; Tag Data Reply – Tag Access Data. See Ch4.2
RFID ⇒ <0x02><0x00 0x07><0x00 0xC8><0x00 0x83><0x00><0xFE 0xAE><0x03>
```

- Host 發送：`ACCPW = 0x12345678`，`MBANK = 0x01`（EPC 區），`MADDR = 0x0002`，`MLEN = 0x01`，`MDATA = 0xBB 0xBB`（寫入 1 word）。
- RFID 回覆：`STATUS = 0x00`（成功）。
- 隨後送出 **Tag Access Data（ID = 0xC8）**：對應指令 `0x0083`、`STATUS = 0x00`（寫入成功），見 [Ch4.9](Chapter4_RFIDDataReply.md)。

---

### 3.5. Tag Lock Operation Command（鎖定標籤）— ID = 0x84

鎖定/解鎖標籤的特定記憶體區塊或密碼，鎖定行為由 `LDATA` 定義（依 ISO 18000-6C 的 Lock 指令格式）。

```
Host ⇒ <STX><LENGTH><ID><ACCPW><LDATA><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ACCPW | 4 Byte | 存取密碼 |
| LDATA | 3 Byte | 鎖定指令資料（Lock Command Data），定義要鎖定哪些區塊及鎖定方式，格式參照 ISO 18000-6C |

#### 範例

```
Host ⇒ <0x02><0x00 0x0B><0x00 0x84><0x12 0x34 0x56 0x78><0x00 0x80 0x20>
  <0xFD 0xBD><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x84><0x00><0xFF 0x77><0x03>

; Tag Data Reply – Tag Access Data. See Ch4.2
RFID ⇒ <0x02><0x00 0x07><0x00 0xC8><0x00 0x84><0x00><0xFE 0xAD><0x03>
```

- Host 發送：`ACCPW = 0x12345678`，`LDATA = 0x00 0x80 0x20`（鎖定遮罩與動作位元）。
- RFID 回覆：`STATUS = 0x00`（成功）。
- 隨後送出 **Tag Access Data（ID = 0xC8）**：對應指令 `0x0084`、`STATUS = 0x00`，見 [Ch4.9](Chapter4_RFIDDataReply.md)。

---

### 3.6. Tag Kill Operation Command（永久停用標籤）— ID = 0x85

使用 Kill 密碼永久停用標籤。Kill 成功後標籤將無法再被讀取或使用（不可逆）。

```
Host ⇒ <STX><LENGTH><ID><KILLPW><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| KILLPW | 4 Byte | Kill 密碼。需與標籤內設定的 Kill 密碼相符才能成功，且不可為 `0x00000000` |

#### 範例

```
Host ⇒ <0x02><0x00 0x08><0x00 0x85><0xFF 0xFF 0xFF 0xFF><0xFB 0x77><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x85><0x00><0xFF 0x76><0x03>

; Tag Data Reply – Tag Access Data. See Ch4.2
RFID ⇒ <0x02><0x00 0x07><0x00 0xC8><0x00 0x85><0x00><0xFE 0xAC><0x03>
```

- Host 發送：`KILLPW = 0xFFFFFFFF`，要求 Kill 標籤。
- RFID 回覆：`STATUS = 0x00`（成功）。
- 隨後送出 **Tag Access Data（ID = 0xC8）**：對應指令 `0x0085`、`STATUS = 0x00`（Kill 成功），見 [Ch4.9](Chapter4_RFIDDataReply.md)。

---

### 3.7. Tag BlockWrite Operation Command（區塊寫入）— ID = 0x86

以「區塊（Block）」方式一次寫入多個 word，相較於一般 Write 可提升大量資料寫入效率。

```
Host ⇒ <STX><LENGTH><ID><ACCPW><MBANK><MADDR><MLEN><MDATA><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ACCPW | 4 Byte | 存取密碼 |
| MBANK | 1 Byte | 記憶體區塊（RESV/EPC/TID/USER） |
| MADDR | 2 Byte | 記憶體起始位址（單位：word） |
| MLEN | 1 Byte | 寫入長度（單位：word） |
| MDATA | 2N Byte | 寫入資料（單位：word，故為 2N Byte） |

#### 範例

```
Host ⇒ <0x02><0x00 0x14><0x00 0x86><0x12 0x34 0x56 0x78><0x03>
  <0x00 0x00><0x04><0xAA 0xAA 0xBB 0xBB 0xCC 0xCC 0xDD 0xDD>
  <0xF8 0x2F><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x86><0x00><0xFF 0x75><0x03>

; Tag Data Reply – Tag Access Data. See Ch4.2
RFID ⇒ <0x02><0x00 0x07><0x00 0xC8><0x00 0x86><0x00><0xFE 0xAB><0x03>
```

- Host 發送：`ACCPW = 0x12345678`，`MBANK = 0x03`（USER 區），`MADDR = 0x0000`，`MLEN = 0x04`（寫 4 個 word），`MDATA = AA AA BB BB CC CC DD DD`（共 8 Byte = 4 word）。
- RFID 回覆：`STATUS = 0x00`（成功）。
- 隨後送出 **Tag Access Data（ID = 0xC8）**：對應指令 `0x0086`、`STATUS = 0x00`，見 [Ch4.9](Chapter4_RFIDDataReply.md)。

---

### 3.8. Tag BlockPermalock Operation Command（區塊永久鎖定）— ID = 0x87

對 USER 區的記憶體區塊進行「永久鎖定（Permalock）」或「讀取鎖定狀態」。永久鎖定後該區塊不可逆，永遠無法再寫入。

```
Host ⇒ <STX><LENGTH><ID><ACCPW><READLOCK><MBANK><MADDR><MLEN><MSAK><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ACCPW | 4 Byte | 存取密碼 |
| READLOCK | 1 Byte | 動作選擇：讀取鎖定狀態或設定永久鎖定，見下方 READLOCK 對照表 |
| MBANK | 1 Byte | 記憶體區塊（RESV/EPC/TID/USER） |
| MADDR | 2 Byte | 記憶體起始位址（單位：16 個區塊為一單位 / in 16 blocks） |
| MLEN | 1 Byte | 範圍（單位：16 個區塊為一單位 / in 16 blocks） |
| MASK | 2N Byte | 遮罩，指定保留現狀或設為永久鎖定（Retain current or Assert Permalock） |

> 註：原文 Note 欄位名稱為 `MSAK`/`MASK`，此處統一視為遮罩欄位 **MASK**。

#### READLOCK 對照表

| 數值 | 說明（中文） | 原文說明 |
|---|---|---|
| 0 | 讀取目前鎖定狀態 | READ |
| 1 | 設定永久鎖定 | LOCK (Permalock) |

#### 範例

```
Host ⇒ <0x02><0x00 0x0F><0x00 0x87><0x00 0x00 0x00 0x00><0x01><0x03>
  <0x00 0x00><0x01><0x80 0x00><0xFE 0xE5><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x87><0x00><0xFF 0x74><0x03>

; Tag Data Reply – Tag Access Data. See Ch4.2
RFID ⇒ <0x02><0x00 0x07><0x00 0xC8><0x00 0x87><0x00><0xFE 0xAA><0x03>
```

- Host 發送：`ACCPW = 0x00000000`，`READLOCK = 0x01`（永久鎖定），`MBANK = 0x03`（USER 區），`MADDR = 0x0000`，`MLEN = 0x01`，`MASK = 0x80 0x00`（指定要鎖定的區塊位元）。
- RFID 回覆：`STATUS = 0x00`（成功）。
- 隨後送出 **Tag Access Data（ID = 0xC8）**：對應指令 `0x0087`、`STATUS = 0x00`，見 [Ch4.9](Chapter4_RFIDDataReply.md)。

---

### 3.9. Tag Authenticate Operation Command（標籤認證）— ID = 0x88

對支援密碼套件（Cryptographic Suite）的標籤執行認證流程（ISO 18000-6C 的 Authenticate 指令），用於安全標籤的身分驗證。

```
Host ⇒ <STX><LENGTH><ID><ACCPW><SENREP><INCREPLEN><CSI><LEN><MSG><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ACCPW | 4 Byte | 存取密碼 |
| SENREP | 1 Byte | 結果處理方式：儲存於標籤或送回（Store or Send） |
| INCREPLEN | 1 Byte | 回覆中是否包含長度資訊（Includes Length in reply） |
| CSI | 1 Byte | 密碼套件識別碼（Cryptographic Suite Identifier） |
| LEN | 2 Byte | 訊息長度（單位：bit / in bits） |
| MSG | N Byte | 認證訊息內容（依 CSI 而定 / Depends on CSI） |
| EXPREPLEN | 2 Byte | 預期回應長度（Expected Length of Response，單位：bit） |

#### 範例

```
Host ⇒ <0x02><0x00 0x15><0x00 0x88><0x00 0x00 0x00 0x00><0x01><0x01><0x01>
  <0x00 0x30><0x05 0x02 0x03 0x04 0x05 0x06><0x00 0x80><0xFE 0x97><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x88><0x00><0xFF 0x73><0x03>

; Tag Data Reply – Tag Access Data. See Ch4.2
; LenRep: 0x80; Parity: 0x01; Response: 11A210614C45FD00E4896532AFB40030
RFID ⇒ <0x02><0x00 0x19><0x00 0xC8><0x00 0x88><0x00><0x01 0x01 0x11 0xA2
    0x10 0x61 0x4C 0x45 0xFD 0x00 0xE4 0x89 0x65 0x32 0xAF 0xB4 0x00 0x30>
  <0xF8 0x4C><0x03>
```

- Host 發送：`ACCPW = 0x00000000`，`SENREP = 0x01`，`INCREPLEN = 0x01`，`CSI = 0x01`，`LEN = 0x0030`（48 bit），`MSG = 05 02 03 04 05 06`，`EXPREPLEN = 0x0080`（128 bit）。
- RFID 回覆：`STATUS = 0x00`（成功）。
- 隨後送出 **Tag Access Data（ID = 0xC8）**：認證回應 `Response = 11A210614C45FD00E4896532AFB40030`（128 bit），見 [Ch4.9](Chapter4_RFIDDataReply.md)。

---

### 3.10. Tag Select Operation Command（標籤篩選）— ID = 0x8A

設定 Select 條件（ISO 18000-6C Select），用於在盤點前先篩選出符合特定記憶體資料樣式的標籤族群，縮小後續操作範圍。

```
Host ⇒ <STX><LENGTH><ID><MODE><SELTARGET><ACTION><TRUNC><MBANK><MADDR><MLEN><MDATA><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| MODE | 1 Byte | Select 規則的操作模式，見下方 MODE 對照表 |
| SELTARGET | 1 Byte | 篩選目標旗標/Session，見下方 SELTARGET 對照表 |
| ACTION | 1 Byte | 篩選動作（Select Action），定義符合/不符合條件標籤的 SL 或 Inventoried 旗標如何變化（參照 ISO 18000-6C） |
| TRUNC | 1 Byte | 是否啟用截斷回覆（Enable Truncation）；0 = 停用，1 = 啟用 |
| MBANK | 1 Byte | 記憶體區塊（RESV/EPC/TID/USER） |
| MADDR | 2 Byte | 記憶體起始位址（單位：bit / in bits） |
| MLEN | 1 Byte | 比對長度（單位：bit / in bits） |
| MDATA | N Byte | 比對的資料樣式（Memory Data-Pattern） |

#### MODE（Select 模式）對照表

| 數值 | 名稱 | 說明（中文） | 原文說明 |
|---|---|---|---|
| 0 | Clear | 清除現有 Select 規則 | Clear |
| 1 | Add | 新增一條 Select 規則 | Add |
| 2 | Clear_Add | 先清除再新增 | Clear_Add |
| 3 | AddOnce | 新增單次套用的規則 | AddOnce |

#### SELTARGET（篩選目標）對照表

| 數值 | 名稱 | 說明（中文） | 原文說明 |
|---|---|---|---|
| 0 | S0 | Session 0 | S0 |
| 1 | S1 | Session 1 | S1 |
| 2 | S2 | Session 2 | S2 |
| 3 | S3 | Session 3 | S3 |
| 4 | SL | SL 旗標 | SL |

#### 範例

```
Host ⇒ <0x02><0x00 0x0E><0x00 0x8A><0x02><0x04><0x00><0x00><0x01>
  <0x00 0x20><0x10><0xAA 0xAA><0xFD 0xDD><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x8A><0x00><0xFF 0x71><0x03>
```

- Host 發送：`MODE = 0x02`（Clear_Add），`SELTARGET = 0x04`（SL），`ACTION = 0x00`，`TRUNC = 0x00`（停用截斷），`MBANK = 0x01`（EPC 區），`MADDR = 0x0020`（第 32 bit 起），`MLEN = 0x10`（比對 16 bit），`MDATA = 0xAA 0xAA`。
- RFID 回覆：`STATUS = 0x00`（成功），表示 Select 規則已設定。本指令僅設定篩選條件，不會主動回傳標籤資料。

---

### 3.11. Tag Inventory with User-Specified Data Operation Command（盤點並附帶指定記憶體資料）— ID = 0x8B

盤點的同時，額外讀取每個標籤上指定記憶體區塊的資料一併回傳，結果以 **Inventory Data with User-Specified Data（[Ch4.3](Chapter4_RFIDDataReply.md)，ID = 0xC2）** 格式回傳。

```
Host ⇒ <STX><LENGTH><ID><ACCPW><MBANK><MADDR><MLEN><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ACCPW | 4 Byte | 存取密碼 |
| MBANK | 1 Byte | 要附帶讀取的記憶體區塊（RESV/EPC/TID/USER） |
| MADDR | 2 Byte | 記憶體起始位址（單位：word） |
| MLEN | 1 Byte | 讀取長度（單位：word） |

#### 範例

```
Host ⇒ <0x02><0x00 0x0C><0x00 0x8B><0x12 0x34 0x56 0x78><0x01>
  <0x00 0x02><0x01><0xFE 0x4D><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x8B><0x00><0xFF 0x70><0x03>

; Tag Data Reply – Inventory Data. See Ch4.1
RFID ⇒ <0x02><0x00 0x16><0x00 0xC2><0x01><0xE9 0x62><0x34 0x00>
  <0xAA 0xAA 0x34 0x12 0xDC 0x03 0x01 0x17 0x16 0x10 0xB9 0x88>
  <0x1><0xF9 0xAF><0x03>
```

- Host 發送：`ACCPW = 0x12345678`，`MBANK = 0x01`（EPC 區），`MADDR = 0x0002`，`MLEN = 0x01`，要求盤點時附帶讀取此區塊資料。
- RFID 回覆：`STATUS = 0x00`（成功）。
- 隨後送出 **Inventory Data with User-Specified Data（ID = 0xC2）**：除 EPC 外另附帶讀到的指定資料，詳見 [Ch4.3](Chapter4_RFIDDataReply.md)。

---

### 3.12. Tag MarginRead Operation Command（餘裕讀取）— ID = 0x8C

執行 MarginRead（餘裕讀取）操作，用於測試標籤記憶體中某段資料的讀取餘裕/可靠度（標籤是否能穩定讀出指定樣式），常用於品質檢測。

```
Host ⇒ <STX><LENGTH><ID><ACCPW><MBANK><MADDR><MLEN><MDATA><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ACCPW | 4 Byte | 存取密碼 |
| MBANK | 1 Byte | 記憶體區塊（RESV/EPC/TID/USER） |
| MADDR | 2 Byte | 記憶體起始位址（單位：bit / in bits） |
| MLEN | 1 Byte | 比對長度（單位：bit / in bits） |
| MDATA | N Byte | 比對的資料樣式（Memory Data-Pattern） |

#### 範例

```
Host ⇒ <0x02><0x00 0x0D><0x00 0x8C><0x00 0x00 0x00 0x00><0x03><0x00 0x00>
  <0x08><0x05><0xFF 0x07><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x8C><0x00><0xFF 0x6F><0x03>

; Tag Data Reply – Tag Access Data.
; Tag Data Reply – Success
RFID ⇒ <0x02><0x00 0x07><0x00 0xC8><0x00 0x8C><0x00><0xFE 0xA5><0x03>
; Tag Data Reply – Fail
RFID ⇒ <0x02><0x00 0x07><0x00 0xC8><0x00 0x8C><0x80><0xFE 0x25><0x03>
; Tag Data Reply – No Tag Found
RFID ⇒ <0x02><0x00 0x07><0x00 0xC8><0x00 0x8C><0x01><0xFE 0xA4><0x03>
; Tag Data Reply – Tag No Reply
RFID ⇒ <0x02><0x00 0x07><0x00 0xC8><0x00 0x8C><0x03><0xFE 0xA2><0x03>
```

- Host 發送：`ACCPW = 0x00000000`，`MBANK = 0x03`（USER 區），`MADDR = 0x0000`，`MLEN = 0x08`（8 bit），`MDATA = 0x05`。
- RFID 回覆：`STATUS = 0x00`（成功），表示已開始 MarginRead。
- 隨後送出 **Tag Access Data（ID = 0xC8）**，依結果回傳不同 STATUS：

| STATUS | 結果（中文） | 原文 |
|---|---|---|
| 0x00 | 成功（餘裕讀取通過） | Success |
| 0x80 | 失敗 | Fail |
| 0x01 | 找不到標籤 | No Tag Found |
| 0x03 | 標籤無回應 | Tag No Reply |
