# Unitech RFID Command Set — 第 5 章（上）

- **版本 (Version)**: V1.02
- **說明**: 藍色字體標示為新增/更新功能 (New/Updated functions in blue)
- **適用韌體 (Applied to FW)**: RM300P V0.1.0.14 或更新版本

> 第 5 章 RFID Configuration（RFID 參數設定）內容龐大（5.1~5.62），分為三個子文件：
> 本檔為 **5a（5.1~5.20）**，另有 [5b（5.21~5.42）](Chapter5b_RFIDConfiguration_2.md)、[5c（5.43~5.62）](Chapter5c_RFIDConfiguration_3.md)。

---

## 文件章節目錄 (Document Table of Contents)

| 章節 | 標題 | 檔案 |
|---|---|---|
| 1 | Command Format | [Chapter1_CommandFormat.md](Chapter1_CommandFormat.md) |
| 2 | General Commands | [Chapter2_GeneralCommands.md](Chapter2_GeneralCommands.md) |
| 3 | RFID Operation | [Chapter3_RFIDOperation.md](Chapter3_RFIDOperation.md) |
| 4 | RFID Data Reply | [Chapter4_RFIDDataReply.md](Chapter4_RFIDDataReply.md) |
| 5 | RFID Configuration | [Chapter5a](Chapter5a_RFIDConfiguration_1.md)（本章，5.1~5.20）/ [Chapter5b](Chapter5b_RFIDConfiguration_2.md)（5.21~5.42）/ [Chapter5c](Chapter5c_RFIDConfiguration_3.md)（5.43~5.62） |
| 6 | RFID Extended Operation | [Chapter6_RFIDExtendedOperation.md](Chapter6_RFIDExtendedOperation.md) |
| 7 | Event Notification Commands | [Chapter7_EventNotification.md](Chapter7_EventNotification.md) |
| 8 | Firmware Update Commands | [Chapter8_FirmwareUpdate.md](Chapter8_FirmwareUpdate.md) |

---

## 第 5 章（上）：RFID Configuration（RFID 參數設定）5.1 ~ 5.20

### 本章目錄

- [5.1 / 5.2 Get / Set Region（區域設定）— 0x100 / 0x101](#51--52-get--set-region區域設定--0x100--0x101)
- [5.3 / 5.4 Get / Set Antenna Settings（天線設定）— 0x102 / 0x103](#53--54-get--set-antenna-settings天線設定--0x102--0x103)
- [5.5 / 5.6 Get / Set Antenna State（天線啟用狀態）— 0x104 / 0x105](#55--56-get--set-antenna-state天線啟用狀態--0x104--0x105)
- [5.7 / 5.8 Get / Set RF Mode（射頻模式）— 0x106 / 0x107](#57--58-get--set-rf-mode射頻模式--0x106--0x107)
- [5.9 / 5.10 Get / Set Gen2 Algorithm（Gen2 防碰撞演算法）— 0x108 / 0x109](#59--510-get--set-gen2-algorithmgen2-防碰撞演算法--0x108--0x109)
- [5.11 / 5.12 Get / Set Gen2 Query Group（Gen2 查詢群組）— 0x10A / 0x10B](#511--512-get--set-gen2-query-groupgen2-查詢群組--0x10a--0x10b)
- [5.13 / 5.14 Get / Set Bi-Static Antenna Setting（雙基站天線）— 0x10C / 0x10D](#513--514-get--set-bi-static-antenna-setting雙基站天線--0x10c--0x10d)
- [5.15 / 5.16 Get / Set LBT Settings（聽後再說）— 0x110 / 0x111](#515--516-get--set-lbt-settings聽後再說--0x110--0x111)
- [5.17 / 5.18 Get / Set FastID Settings（FastID）— 0x112 / 0x113](#517--518-get--set-fastid-settingsfastid--0x112--0x113)
- [5.19 / 5.20 Get / Set TagFocus Settings（TagFocus）— 0x114 / 0x115](#519--520-get--set-tagfocus-settingstagfocus--0x114--0x115)

本章定義 RFID 模組的各項參數設定指令，多以「Get（讀取目前設定）」與「Set（寫入新設定）」成對出現。為便於對照，以下將每組 Get/Set 合併於同一小節說明，並同時標註兩個 Command ID。所有指令皆遵循第 1 章封包格式。

> **設定類指令通則**：
> - Get 指令通常無參數（或僅帶索引如 ANTID），回覆中夾帶目前設定值。
> - Set 指令帶入欲設定的參數，回覆通常僅有 `STATUS`（`0x00` 成功）。

---

### 5.1 / 5.2 Get / Set Region（區域設定）— 0x100 / 0x101

設定/讀取裝置的射頻法規區域（不同國家/地區的頻段與功率規範不同），務必依實際使用地正確設定。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><DATA><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| DATA | 1 Byte | 區域代碼（Region），見下方對照表 |

#### Region（區域）中英對照表

| 數值 | 區域（中文） | 原文 |
|---|---|---|
| 0 | 美國 FCC | FCC |
| 1 | 歐盟 ETSI | ETSI |
| 2 | 日本 | JAPAN |
| 3 | 台灣 | TAIWAN |
| 4 | 中國 | CHINA |
| 5 | 香港 | HONG-KONG |

#### 範例

```
; Get：讀取目前區域
Host ⇒ <0x02><0x00 0x04><0x01 0x00><0xFF 0xFB><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0x00><0x00><0x00><0xFF 0xF9><0x03>   ; FCC

; Set：設定為 ETSI
Host ⇒ <0x02><0x00 0x05><0x01 0x01><0x01><0xFF 0xF8><0x03>          ; ETSI
RFID ⇒ <0x02><0x00 0x05><0x01 0x01><0x00><0xFF 0xF9><0x03>
```

- Get 回覆 `DATA = 0x00`，代表目前區域為 **FCC（美國）**。
- Set 帶入 `DATA = 0x01`，將區域設為 **ETSI（歐盟）**；回覆 `STATUS = 0x00`（成功）。

---

### 5.3 / 5.4 Get / Set Antenna Settings（天線設定）— 0x102 / 0x103

設定/讀取指定天線的「停留時間（Dwell Time）」與「發射功率（Power）」。

```
Get  Host ⇒ <STX><LENGTH><ID><ANTID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><ANTID><DWELL><PWR><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><ANTID><DWELL><PWR><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 天線編號（1~4） |
| DWELL | 2 Byte | 停留時間（Dwell Time），單位 ms（毫秒）。即此天線單輪作用時間 |
| PWR | 2 Byte | 天線發射功率，單位 cdBm（百分之一 dBm）。例如 0x0BB8 = 3000 = 30.00 dBm |

#### 範例

```
; Get：讀取第 1 號天線設定
Host ⇒ <0x02><0x00 0x05><0x01 0x02><0x01><0xFF 0xF7><0x03>
RFID ⇒ <0x02><0x00 0x0A><0x01 0x02><0x00><0x01><0x03 0xE8><0x0B 0xB8><0xFE 0x44><0x03>

; Set：設定第 1 號天線
Host ⇒ <0x02><0x00 0x09><0x01 0x03><0x01><0x03 0xE8><0x0B 0xB8><0xFE 0x44><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x03><0x00><0xFF 0xF7><0x03>
```

- `ANTID = 0x01`，`DWELL = 0x03E8` = 1000 ms，`PWR = 0x0BB8` = 3000 cdBm = **30.00 dBm**。

---

### 5.5 / 5.6 Get / Set Antenna State（天線啟用狀態）— 0x104 / 0x105

啟用或停用指定天線。

```
Get  Host ⇒ <STX><LENGTH><ID><ANTID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><ANTID><ANTSTATE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><ANTID><ANTSTATE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ANTID | 1 Byte | 天線編號（1~4） |
| ANTSTATE | 1 Byte | 天線啟用狀態，見下方對照表 |

#### ANTSTATE 對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0 | 停用 | Disable |
| 1 | 啟用 | Enable |

#### 範例

```
; Get：讀取第 1 號天線狀態
Host ⇒ <0x02><0x00 0x05><0x01 0x04><0x01><0xFF 0xF5><0x03>
RFID ⇒ <0x02><0x00 0x07><0x01 0x04><0x00><0x01><0x01><0xFF 0xF2><0x03>

; Set：啟用第 1 號天線
Host ⇒ <0x02><0x00 0x06><0x01 0x05><0x01><0x00><0xFF 0xF3><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x05><0x00><0xFF 0xF5><0x03>
```

- Get 回覆 `ANTID = 0x01`、`ANTSTATE = 0x01`，代表第 1 號天線目前為**啟用**。
- Set 範例帶 `ANTID = 0x01`、`ANTSTATE = 0x00`（停用）。

---

### 5.7 / 5.8 Get / Set RF Mode（射頻模式）— 0x106 / 0x107

設定/讀取 RF Mode（射頻模式編號），不同模式對應不同的資料速率、調變方式等，影響讀取速度與靈敏度。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><RFMODE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><RFMODE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| RFMODE | 2 Byte | 射頻模式編號（RF Mode），例如 103。實際可用模式依韌體/晶片而定 |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x06><0xFF 0xF5><0x03>
RFID ⇒ <0x02><0x00 0x07><0x01 0x06><0x00><0x00 0x94><0xFF 0x5E><0x03>

; Set
Host ⇒ <0x02><0x00 0x06><0x01 0x07><0x00 0x94><0xFF 0x5E><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x07><0x00><0xFF 0xF3><0x03>
```

- `RFMODE = 0x0094` = 148（十進位）為目前/欲設定的射頻模式。

---

### 5.9 / 5.10 Get / Set Gen2 Algorithm（Gen2 防碰撞演算法）— 0x108 / 0x109

設定/讀取 Gen2 防碰撞（Anti-collision）的 Q 值演算法。Q 值決定盤點時的時槽數量，影響多標籤環境下的讀取效率。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><ALG><INITQ><QRANGE><DUALTARGET><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><ALG><INITQ><QRANGE><DUALTARGET><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| ALG | 1 Byte | 演算法類型，見下方 ALG 對照表 |
| INITQ | 1 Byte | Q 值（固定 Q）或起始 Q 值（動態 Q 的 Start Q） |
| QRANGE | 1 Byte | Q 值範圍：Bit 0-3 為最小 Q（Min Q），Bit 4-7 為最大 Q（Max Q） |
| DUALTARGET | 1 Byte | 雙目標（Dual Target）；0：停用，1：啟用 |

#### ALG（演算法）對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0 | 固定 Q 值 | Fix Q |
| 1 | 動態 Q 值 | Dynamic Q |

#### DUALTARGET 對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0 | 停用 | Disable |
| 1 | 啟用 | Enable |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x08><0xFF 0xF3><0x03>
RFID ⇒ <0x02><0x00 0x09><0x01 0x08><0x00><0x00><0x04><0x44><0x01><0xFF 0xA5><0x03>

; Set Ex1：固定 Q，Q=4，Min Q=4，Max Q=4，Dual Target=Enable
Host ⇒ <0x02><0x00 0x08><0x01 0x09><0x00><0x04><0x44><0x01><0xFF 0xA5><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x09><0x00><0xFF 0xF1><0x03>

; Set Ex2：動態 Q，Initial Q=8，Min Q=0，Max Q=15，Dual Target=Enable
Host ⇒ <0x02><0x00 0x08><0x01 0x09><0x01><0x08><0xF0><0x01><0xFE 0xF4><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x09><0x00><0xFF 0xF1><0x03>
```

- Ex1：`ALG=0x00`（固定 Q），`INITQ=0x04`，`QRANGE=0x44`（Min Q=4、Max Q=4），`DUALTARGET=0x01`（啟用）。
- Ex2：`ALG=0x01`（動態 Q），`INITQ=0x08`（起始 Q=8），`QRANGE=0xF0`（Min Q=0、Max Q=15），`DUALTARGET=0x01`。
  （QRANGE 的低 4 bit 為 Min Q、高 4 bit 為 Max Q，故 0xF0 = Max 15 / Min 0。）

---

### 5.11 / 5.12 Get / Set Gen2 Query Group（Gen2 查詢群組）— 0x10A / 0x10B

設定/讀取 Gen2 Query 指令的群組參數（Select 條件、Session、Target），決定盤點時針對哪一群標籤、使用哪個 Session 與初始 Target。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><SELECT><SESSION><TARGET><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><SELECT><SESSION><TARGET><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| SELECT | 1 Byte | 篩選模式（Select Mode），見下方對照表 |
| SESSION | 1 Byte | Session（S0~S3），數值 0~3 對應 S0~S3 |
| TARGET | 1 Byte | 初始盤點目標，見下方對照表 |

#### SELECT（篩選模式）對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 1 | 全部標籤 | All |
| 2 | 未被選取（De-Selected） | De-Selected |
| 3 | 已被選取（Selected） | Selected |

#### TARGET（盤點目標）對照表

| 數值 | 說明（中文） | 原文 |
|---|---|---|
| 0 | 目標 A | Target A |
| 1 | 目標 B | Target B |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x0A><0xFF 0xF1><0x03>
RFID ⇒ <0x02><0x00 0x08><0x01 0x0A><0x00><0x01><0x00><0x00><0xFF 0xEC><0x03>

; Set
Host ⇒ <0x02><0x00 0x07><0x01 0x0B><0x01><0x00><0x00><0xFF 0xEC><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x0B><0x00><0xFF 0xEF><0x03>
```

- `SELECT = 0x01`（All，全部標籤），`SESSION = 0x00`（S0），`TARGET = 0x00`（Target A）。

---

### 5.13 / 5.14 Get / Set Bi-Static Antenna Setting（雙基站天線）— 0x10C / 0x10D

啟用/停用 Bi-Static（雙基站）天線模式，即發射與接收使用不同天線埠。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><BISTATICEN><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><BISTATICEN><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| BISTATICEN | 1 Byte | 雙基站啟用旗標；0：停用（Disable），1：啟用（Enable） |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x0C><0xFF 0xEF><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0x0C><0x00><0x00><0xFF 0xED><0x03>

; Set：啟用
Host ⇒ <0x02><0x00 0x05><0x01 0x0D><0x01><0xFF 0xEC><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x0D><0x00><0xFF 0xED><0x03>
```

- Get 回覆 `BISTATICEN = 0x00`（目前停用）；Set 帶 `0x01` 改為啟用。

---

### 5.15 / 5.16 Get / Set LBT Settings（聽後再說）— 0x110 / 0x111

啟用/停用 LBT（Listen Before Talk，聽後再說）。LBT 是部分地區法規要求：發射前先偵測通道是否被佔用，避免干擾。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><LBTENABLE><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><LBTENABLE><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| LBTENABLE | 1 Byte | LBT 啟用旗標；0：停用（Disable），1：啟用（Enable） |

> LBT 的詳細參數（門檻、偵測時間等）另見 [5.51/5.52 LBT Configure Settings](Chapter5c_RFIDConfiguration_3.md)。

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x10><0xFF 0xEB><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0x10><0x00><0x00><0xFF 0xE9><0x03>

; Set：啟用
Host ⇒ <0x02><0x00 0x05><0x01 0x11><0x01><0xFF 0xE8><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x11><0x00><0xFF 0xE9><0x03>
```

- Get 回覆 `LBTENABLE = 0x00`（停用）；Set 帶 `0x01` 改為啟用。

---

### 5.17 / 5.18 Get / Set FastID Settings（FastID）— 0x112 / 0x113

啟用/停用 FastID。FastID（Impinj Monza 標籤功能）可在盤點 EPC 的同時一併取得 TID，免去額外讀取步驟。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><FASTIDEN><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><FASTIDEN><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| FASTIDEN | 1 Byte | FastID 啟用旗標；0：停用（Disable），1：啟用（Enable） |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x12><0xFF 0xE9><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0x12><0x00><0x00><0xFF 0xE7><0x03>

; Set：啟用
Host ⇒ <0x02><0x00 0x05><0x01 0x13><0x01><0xFF 0xE6><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x13><0x00><0xFF 0xE7><0x03>
```

- Get 回覆 `FASTIDEN = 0x00`（停用）；Set 帶 `0x01` 改為啟用。

---

### 5.19 / 5.20 Get / Set TagFocus Settings（TagFocus）— 0x114 / 0x115

啟用/停用 TagFocus。TagFocus（Impinj Monza 功能）讓已盤點過的標籤在同一盤點循環中保持靜默，避免重複回報、提升新標籤被讀到的機會。

```
Get  Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><TAGFOCUSEN><CHECKSUM><ETX>
Set  Host ⇒ <STX><LENGTH><ID><TAGFOCUSEN><CHECKSUM><ETX>
     RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| TAGFOCUSEN | 1 Byte | TagFocus 啟用旗標；0：停用（Disable），1：啟用（Enable） |

#### 範例

```
; Get
Host ⇒ <0x02><0x00 0x04><0x01 0x14><0xFF 0xE7><0x03>
RFID ⇒ <0x02><0x00 0x06><0x01 0x14><0x00><0x00><0xFF 0xE5><0x03>

; Set：啟用
Host ⇒ <0x02><0x00 0x05><0x01 0x15><0x01><0xFF 0xE4><0x03>
RFID ⇒ <0x02><0x00 0x05><0x01 0x15><0x00><0xFF 0xE5><0x03>
```

- Get 回覆 `TAGFOCUSEN = 0x00`（停用）；Set 帶 `0x01` 改為啟用。
