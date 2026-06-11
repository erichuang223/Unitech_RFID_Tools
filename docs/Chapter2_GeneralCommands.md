# Unitech RFID Command Set — 第 2 章

- **版本 (Version)**: V1.02
- **說明**: 藍色字體標示為新增/更新功能 (New/Updated functions in blue)
- **適用韌體 (Applied to FW)**: RM300P V0.1.0.14 或更新版本

---

## 文件章節目錄 (Document Table of Contents)

| 章節 | 標題 | 檔案 |
|---|---|---|
| 1 | Command Format | [Chapter1_CommandFormat.md](Chapter1_CommandFormat.md) |
| 2 | General Commands | [Chapter2_GeneralCommands.md](Chapter2_GeneralCommands.md)（本章） |
| 3 | RFID Operation | [Chapter3_RFIDOperation.md](Chapter3_RFIDOperation.md) |
| 4 | RFID Data Reply | [Chapter4_RFIDDataReply.md](Chapter4_RFIDDataReply.md) |
| 5 | RFID Configuration | [Chapter5a](Chapter5a_RFIDConfiguration_1.md) / [Chapter5b](Chapter5b_RFIDConfiguration_2.md) / [Chapter5c](Chapter5c_RFIDConfiguration_3.md) |
| 6 | RFID Extended Operation | [Chapter6_RFIDExtendedOperation.md](Chapter6_RFIDExtendedOperation.md) |
| 7 | Event Notification Commands | [Chapter7_EventNotification.md](Chapter7_EventNotification.md) |
| 8 | Firmware Update Commands | [Chapter8_FirmwareUpdate.md](Chapter8_FirmwareUpdate.md) |

---

## 第 2 章：General Commands（一般指令）

### 本章目錄

- [2.1 Read Firmware Version Command（讀取韌體版本）— ID = 0x21](#21-read-firmware-version-command讀取韌體版本--id--0x21)
- [2.2 Read Model Name Command（讀取機型名稱）— ID = 0x22](#22-read-model-name-command讀取機型名稱--id--0x22)
- [2.3 Read Serial Number Command（讀取序號）— ID = 0x24](#23-read-serial-number-command讀取序號--id--0x24)
- [2.4 Read SKU ID Command（讀取 SKU ID）— ID = 0x25](#24-read-sku-id-command讀取-sku-id--id--0x25)
- [2.5 Read Ambient Temperature Command（讀取環境溫度）— ID = 0x28](#25-read-ambient-temperature-command讀取環境溫度--id--0x28)
- [2.6 Read PA Temperature Command（讀取功率放大器溫度）— ID = 0x29](#26-read-pa-temperature-command讀取功率放大器溫度--id--0x29)
- [2.7 Reset Device Command（重置裝置）— ID = 0x2F](#27-reset-device-command重置裝置--id--0x2f)

本章定義一些與 RFID 標籤操作無關、用來查詢裝置基本資訊（韌體版本、機型、序號、SKU、溫度）
以及重新啟動/恢復原廠設定的通用指令。所有指令皆遵循第 1 章定義的封包格式。

---

### 2.1. Read Firmware Version Command（讀取韌體版本）— ID = 0x21

讀取目前裝置上執行的韌體版本號碼，版本格式為 `主版本.次版本.建置版本.修訂版本`。

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><DATA><CHECKSUM><ETX>
```

#### 欄位說明

`DATA = <MAJOR><MINOR><BUILD><REVISION>`

| 欄位 | 長度 | 說明 |
|---|---|---|
| MAJOR | 1 Byte | 主版本號 (Major) |
| MINOR | 1 Byte | 次版本號 (Minor) |
| BUILD | 1 Byte | 建置版本號 (Build) |
| REVISION | 1 Byte | 修訂版本號 (Revision) |

#### 範例

```
Host ⇒ <0x02><0x00 0x04><0x00 0x21><0xFF 0xDB><0x03>
RFID ⇒ <0x02><0x00 0x09><0x00 0x21><0x00><0x00 0x00 0x00 0x01><0xFF 0xD5><0x03>
```

- Host 發送：`ID = 0x00 0x21`，無 DATA，向裝置查詢韌體版本。
- RFID 回覆：`STATUS = 0x00`（成功），`DATA = 0x00 0x00 0x00 0x01`，
  代表 `MAJOR=0, MINOR=0, BUILD=0, REVISION=1`，即韌體版本為 **0.0.0.1**。

---

### 2.2. Read Model Name Command（讀取機型名稱）— ID = 0x22

讀取裝置的機型名稱字串（ASCII 編碼）。

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><DATA><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| DATA | N Byte | 機型名稱字串（ASCII 編碼，例如 "RM300P"） |

#### 範例

```
Host ⇒ <0x02><0x00 0x04><0x00 0x22><0xFF 0xDA><0x03>
RFID ⇒ <0x02><0x00 0x0B><0x00 0x22><0x00><RM300P><0xFE 0x51><0x03>
```

- Host 發送：`ID = 0x00 0x22`，無 DATA，向裝置查詢機型名稱。
- RFID 回覆：`STATUS = 0x00`（成功），`DATA = "RM300P"`（ASCII 字元），
  代表此裝置的機型名稱為 **RM300P**。

---

### 2.3. Read Serial Number Command（讀取序號）— ID = 0x24

讀取裝置的序號字串（ASCII 編碼）。

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><DATA><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| DATA | N Byte | 裝置序號字串（ASCII 編碼） |

#### 範例

```
Host ⇒ <0x02><0x00 0x04><0x00 0x24><0xFF 0xD8><0x03>
RFID ⇒ <0x02><0x00 0x11><0x00 0x24><0x00><123456789012><0xFD 0x5B><0x03>
```

- Host 發送：`ID = 0x00 0x24`，無 DATA，向裝置查詢序號。
- RFID 回覆：`STATUS = 0x00`（成功），`DATA = "123456789012"`（ASCII 字元），
  代表此裝置的序號為 **123456789012**。

---

### 2.4. Read SKU ID Command（讀取 SKU ID）— ID = 0x25

讀取裝置的 SKU（庫存單位）識別碼，用於區分不同的產品料號/組態。

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><DATA><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| DATA | 1 Byte | SKU ID 數值，用於識別產品料號/組態版本 |

#### 範例

```
Host ⇒ <0x02><0x00 0x04><0x00 0x25><0xFF 0xD7><0x03>
RFID ⇒ <0x02><0x00 0x06><0x00 0x25><0x00><0x00><0xFF 0xD5><0x03>
```

- Host 發送：`ID = 0x00 0x25`，無 DATA，向裝置查詢 SKU ID。
- RFID 回覆：`STATUS = 0x00`（成功），`DATA = 0x00`，
  代表此裝置的 SKU ID 為 **0**。

---

### 2.5. Read Ambient Temperature Command（讀取環境溫度）— ID = 0x28

讀取裝置目前感測到的環境溫度（裝置周遭環境的溫度，非射頻功率放大器溫度）。

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><TEMPDATA><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| TEMPDATA | 2 Byte | 溫度數值，單位為 0.1°C（攝氏十分之一度），例如數值 `0x01C0` (448) 代表 **44.8°C** |

#### 範例

```
Host ⇒ <0x02><0x00 0x04><0x00 0x28><0xFF 0xD4><0x03>
RFID ⇒ <0x02><0x00 0x07><0x00 0x28><0x00><0x01 0xC0><0xFF 0x10><0x03>
```

- Host 發送：`ID = 0x00 0x28`，無 DATA，向裝置查詢環境溫度。
- RFID 回覆：`STATUS = 0x00`（成功），`TEMPDATA = 0x01 0xC0` = 448（十進位），
  代表目前環境溫度為 **44.8°C**。

---

### 2.6. Read PA Temperature Command（讀取功率放大器溫度）— ID = 0x29

讀取裝置內部射頻功率放大器（Power Amplifier, PA）的溫度，用於監控發射模組是否過熱。

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><TEMPDATA><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| TEMPDATA | 2 Byte | 溫度數值，單位為 0.1°C（攝氏十分之一度），例如數值 `0x01C0` (448) 代表 **44.8°C** |

#### 範例

```
Host ⇒ <0x02><0x00 0x04><0x00 0x29><0xFF 0xD3><0x03>
RFID ⇒ <0x02><0x00 0x07><0x00 0x29><0x00><0x01 0xC0><0xFF 0x0F><0x03>
```

- Host 發送：`ID = 0x00 0x29`，無 DATA，向裝置查詢 PA 溫度。
- RFID 回覆：`STATUS = 0x00`（成功），`TEMPDATA = 0x01 0xC0` = 448（十進位），
  代表目前 PA 溫度為 **44.8°C**。

> 與 [5.29~5.32](Chapter5b_RFIDConfiguration_2.md) 的溫度保護門檻設定搭配使用，
> 當此處讀到的溫度超過設定門檻時，裝置會依溫度保護模式（[5.33/5.34](Chapter5b_RFIDConfiguration_2.md)）採取對應動作。

---

### 2.7. Reset Device Command（重置裝置）— ID = 0x2F

重新啟動裝置，並可選擇是否同時將設定恢復為原廠預設值。

```
Host ⇒ <STX><LENGTH><ID><FLAGS><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

#### 欄位說明

| 欄位 | 長度 | 說明 |
|---|---|---|
| FLAGS | 1 Byte | 重置模式旗標，詳見下表 |

#### FLAGS 對照表

| 數值 | 說明（中文） | 原文說明 |
|---|---|---|
| 0x00 | 僅重新開機 | Reboot only |
| 0x01 | 重新開機並恢復原廠設定 | Reboot & Factory Reset |

#### 範例

```
Host ⇒ <0x02><0x00 0x05><0x00 0x2F><0x00><0xFF 0xCC><0x03>
RFID ⇒ <0x02><0x00 0x05><0x00 0x2F><0x00><0xFF 0xCC><0x03>
```

- Host 發送：`ID = 0x00 0x2F`，`FLAGS = 0x00`，要求裝置**僅重新開機**（不恢復原廠設定）。
- RFID 回覆：`STATUS = 0x00`（成功），表示已接受重置指令，裝置隨後將重新啟動。
