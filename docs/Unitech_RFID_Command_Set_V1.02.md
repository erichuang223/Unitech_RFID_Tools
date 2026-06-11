# Unitech RFID Command Set — V1.02

**設備型號：** RM300P  
**適用韌體：** V0.1.0.14 或更新版本  
**原始文件：** Unitech_RFID_Command_Set_V1.02_20250814_User.pdf  
**通訊介面：** UART（Baud Rate 115200 / 460800 / 921600）

---

## 目錄

1. [封包格式](#1-封包格式)
2. [General Commands（一般指令）](#2-general-commands)
3. [RFID Operation（操作指令）](#3-rfid-operation)
4. [RFID Data Reply（資料回應）](#4-rfid-data-reply)
5. [RFID Configuration（設定指令）](#5-rfid-configuration)
6. [RFID Extended Operation（擴展操作）](#6-rfid-extended-operation)
7. [Event Notification Commands（事件通知）](#7-event-notification-commands)
8. [Firmware Update Commands（韌體更新）](#8-firmware-update-commands)

---

## 1. 封包格式

### 1.1 Host → RFID（主機指令）

```
<STX><LENGTH><ID><CHECKSUM><ETX>
<STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

| 欄位 | 長度 | 說明 |
|------|------|------|
| STX | 1 Byte | `0x02`（封包起始） |
| LENGTH | 2 Byte | 封包長度（不含 CHECKSUM 與 STX/ETX） |
| ID | 2 Byte | 指令識別碼 |
| DATA | N Byte | 指令資料（選填） |
| CHECKSUM | 2 Byte | LENGTH+ID+DATA 的 2's complement sum |
| ETX | 1 Byte | `0x03`（封包結束） |

### 1.2 RFID → Host（設備回應）

```
<STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
<STX><LENGTH><ID><STATUS><DATA><CHECKSUM><ETX>
```

| STATUS 值 | 說明 |
|-----------|------|
| `0x00` | Success |
| `0x01` | Busy |
| `0x02` | Invalid parameters |
| `0x03` | Not Supported |
| `0x81` | Format Error — incorrect Length |
| `0x82` | Format Error — incorrect CRC |
| `0x83` | Format Error — incorrect End-Of-Packet |
| `0x84` | Format Error — packet timeout |
| `0xE0` | Other Error |
| `0xE1` | Target Not Found |

### 1.3 RFID Data Send Format（設備主動推送）

```
<STX><LENGTH><ID><DATA><CHECKSUM><ETX>
```

ID 固定為 `0x80`，DATA 為設備主動傳送至主機的資料。

---

## 2. General Commands

### 2.1 Read Firmware Version — `0x21`

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><DATA><CHECKSUM><ETX>
```
- DATA = `<MAJOR><MINOR><BUILD><REVISION>`（各 1 Byte）

**範例：**
```
Host ⇒ <0x02><0x00 0x04><0x00 0x21><0xFF 0xDB><0x03>
RFID ⇒ <0x02><0x00 0x09><0x00 0x21><0x00><0x00 0x00 0x00 0x01><0xFF 0xD5><0x03>  ; v0.0.0.1
```

### 2.2 Read Model Name — `0x22`

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><DATA><CHECKSUM><ETX>
```

**範例：**
```
Host ⇒ <0x02><0x00 0x04><0x00 0x22><0xFF 0xDA><0x03>
RFID ⇒ <0x02><0x00 0x0B><0x00 0x22><0x00><RM300P><0xFE 0x51><0x03>
```

### 2.3 Read Serial Number — `0x24`

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><DATA><CHECKSUM><ETX>
```

### 2.4 Read SKU ID — `0x25`

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><DATA><CHECKSUM><ETX>
```

### 2.5 Read Ambient Temperature — `0x28`

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><TEMPDATA><CHECKSUM><ETX>
```
- TEMPDATA：2 Byte，單位為 0.1°C（例：`0x01 0xC0` = 44.8°C）

### 2.6 Read PA Temperature — `0x29`

同 2.5 格式，讀取 PA（Power Amplifier）溫度。

### 2.7 Reset Device — `0x2F`

```
Host ⇒ <STX><LENGTH><ID><FLAGS><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

| FLAGS | 說明 |
|-------|------|
| `0x00` | 僅重開機 |
| `0x01` | 重開機並恢復出廠設定 |

---

## 3. RFID Operation

### 3.1 Tag Inventory（盤點）— `0x80`

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```
回應後持續推送 Tag 資料（參見 Ch4.1）。

### 3.2 Cancel Operation（取消操作）— `0x81`

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

### 3.3 Tag Read（讀取 Tag 記憶體）— `0x82`

```
Host ⇒ <STX><LENGTH><ID><ACCPW><MBANK><MADDR><MLEN><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```

| 欄位 | 長度 | 說明 |
|------|------|------|
| ACCPW | 4 Byte | Access Password |
| MBANK | 1 Byte | Memory Bank（RESV/EPC/TID/USER） |
| MADDR | 2 Byte | 起始位址（以 word 為單位） |
| MLEN | 1 Byte | 長度（以 word 為單位） |

結果透過 `0xC8` 回應（參見 Ch4.9）。

### 3.4 Tag Write（寫入 Tag 記憶體）— `0x83`

```
Host ⇒ <STX><LENGTH><ID><ACCPW><MBANK><MADDR><MLEN><MDATA><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```
- MDATA：2N Byte（以 word 為單位）

### 3.5 Tag Lock（鎖定 Tag）— `0x84`

```
Host ⇒ <STX><LENGTH><ID><ACCPW><LDATA><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```
- LDATA：3 Byte，參考 ISO 18000-6C

### 3.6 Tag Kill（永久停用 Tag）— `0x85`

```
Host ⇒ <STX><LENGTH><ID><KILLPW><CHECKSUM><ETX>
RFID ⇒ <STX><LENGTH><ID><STATUS><CHECKSUM><ETX>
```
- KILLPW：4 Byte

### 3.7 Tag BlockWrite（區塊寫入）— `0x86`

```
Host ⇒ <STX><LENGTH><ID><ACCPW><MBANK><MADDR><MLEN><MDATA><CHECKSUM><ETX>
```

### 3.8 Tag BlockPermalock（永久鎖定區塊）— `0x87`

```
Host ⇒ <STX><LENGTH><ID><ACCPW><READLOCK><MBANK><MADDR><MLEN><MASK><CHECKSUM><ETX>
```

| 欄位 | 長度 | 說明 |
|------|------|------|
| READLOCK | 1 Byte | READ 或 LOCK |
| MADDR | 2 Byte | 起始位址（以 16 blocks 為單位） |
| MLEN | 1 Byte | 範圍（以 16 blocks 為單位） |
| MASK | 2N Byte | 保留或設定永久鎖定 |

### 3.9 Tag Authenticate（認證）— `0x88`

```
Host ⇒ <STX><LENGTH><ID><ACCPW><SENREP><INCREPLEN><CSI><LEN><MSG><CHECKSUM><ETX>
```

| 欄位 | 長度 | 說明 |
|------|------|------|
| SENREP | 1 Byte | Store 或 Send |
| INCREPLEN | 1 Byte | 回應是否含長度 |
| CSI | 1 Byte | Cryptographic Suite |
| LEN | 2 Byte | 訊息長度（bits） |
| EXPREPLEN | 2 Byte | 預期回應長度（bits） |

### 3.10 Tag Select（選擇 Tag）— `0x8A`

```
Host ⇒ <STX><LENGTH><ID><MODE><SELTARGET><ACTION><TRUNC><MBANK><MADDR><MLEN><MDATA><CHECKSUM><ETX>
```

| 欄位 | 值 | 說明 |
|------|----|------|
| MODE | 0/1/2/3 | Clear / Add / Clear_Add / AddOnce |
| SELTARGET | 0~4 | S0/S1/S2/S3/SL |
| MADDR | 2 Byte | 起始位址（bits） |
| MLEN | 1 Byte | 長度（bits） |

### 3.11 Tag Inventory with User-Specified Data — `0x8B`

```
Host ⇒ <STX><LENGTH><ID><ACCPW><MBANK><MADDR><MLEN><CHECKSUM><ETX>
```
同時回傳 EPC 及自訂記憶體資料（ID=`0xC2`）。

### 3.12 Tag MarginRead — `0x8C`

```
Host ⇒ <STX><LENGTH><ID><ACCPW><MBANK><MADDR><MLEN><MDATA><CHECKSUM><ETX>
```
- MADDR / MLEN：以 **bits** 為單位

---

## 4. RFID Data Reply

### 4.1 Inventory Data — `0xC0`

```
RFID ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
DATA = <ANTID><RSSI><PC><EPC>
```

| 欄位 | 長度 | 說明 |
|------|------|------|
| ANTID | 1 Byte | 天線編號 1~4 |
| RSSI | 2 Byte | RSSI 值 |
| PC | 2 Byte | EPC bank Protocol Control |
| EPC | N Byte | EPC 資料 |

### 4.2 Inventory Data with TID — `0xC1`

```
DATA = <ANTID><RSSI><PC><EPC><TID>
```

### 4.3 Inventory Data with User-Specified Data — `0xC2`

```
DATA = <ANTID><RSSI><PC><EPC><DATA>
```

### 4.4 Inventory Data/Phase — `0xB0`

```
DATA = <ANTID><RSSI><PHASEBEGIN><PHASEEND><FREQ><PC><EPC>
```

| 欄位 | 長度 | 說明 |
|------|------|------|
| PHASEBEGIN | 2 Byte | 相位起始，單位 0.087° |
| PHASEEND | 2 Byte | 相位結束，單位 0.087° |
| FREQ | 4 Byte | 頻率（kHz），例：`902750` |

### 4.5 Inventory Data/Phase with TID — `0xB1`

```
DATA = <ANTID><RSSI><PHASEBEGIN><PHASEEND><FREQ><PC><EPC><TID>
```

### 4.6 Inventory Data/Phase with User-Specified Data — `0xB2`

```
DATA = <ANTID><RSSI><PHASEBEGIN><PHASEEND><FREQ><PC><EPC><DATA>
```

### 4.7 Inventory Data with Customized Format — `0xA0`

```
RFID ⇒ <STX><LENGTH><ID><PARAMETER1>…<PARAMETERn><CHECKSUM><ETX>
```

PARAMETER 格式：`<PLEN(2B)><PID(1B)><PDATA(NB)>`

| PID | 說明 |
|-----|------|
| `0x00` | EPC |
| `0x01` | PC |
| `0x02` | RSSI |
| `0x03` | Antenna ID |
| `0x04` | TID |
| `0x05` | User Specified Memory |
| `0x06` | Phase（Begin 2B + End 2B） |
| `0x07` | Frequency |
| `0x08` | Timestamp（YEAR/MON/DAY/HOU/MIN/SEC 各 1B，年份從 2000 起算） |

### 4.8 Inventory Status — `0xC7`

```
RFID ⇒ <STX><LENGTH><ID><INVSTATUS><CHECKSUM><ETX>
```
- INVSTATUS = `0x00`：Inventory Done

### 4.9 Tag Access Data — `0xC8`

```
RFID ⇒ <STX><LENGTH><ID><OPID><TAERRCODE><MDATA><CHECKSUM><ETX>
```

| OPID | 說明 |
|------|------|
| `0x82` | Tag Read |
| `0x83` | Tag Write |
| `0x84` | Tag Lock |
| `0x85` | Tag Kill |
| `0x86` | Tag Block Write |
| `0x87` | Tag Block PermaLock |
| `0x88` | Tag Authenticate |

| TAERRCODE | 說明 |
|-----------|------|
| `0x00` | Success |
| `0x01` | No Tag Found |
| `0x02` | Tag Lost |
| `0x03` | Tag Action Not Reply |
| `0x04` | Invalid Password |
| `0x05` | Zero Kill Password |
| `0x80~0x8F` | Tag-Reply Error（Gen2 Spec） |

### 4.10 Operation Error — `0xD0`

```
DATA = <ANTID><OPERRCODE>
```

| OPERRCODE | 說明 |
|-----------|------|
| `0x00` | Success |
| `0x01~0x11` | Ex10 Operation Error |
| `0xD0` | Ex10 Initialized Error |
| `0xD1` | No Antenna Enabled Error |
| `0xD2` | Antenna Disconnected Error |
| `0xD3` | Incorrect DwellTime/RoundCount Error |

### 4.11 System Error — `0xD1`

```
DATA = <SYSERRCODE>
```

| SYSERRCODE | 說明 |
|------------|------|
| `0x01` | Ambient Temperature Too High |
| `0x02` | PA Temperature Too High |

---

## 5. RFID Configuration

### 5.1 / 5.2 Region — `0x100` / `0x101`

| 值 | Region |
|----|--------|
| `0` | FCC |
| `1` | ETSI |
| `2` | JAPAN |
| `3` | TAIWAN |
| `4` | CHINA |
| `5` | HONG-KONG |

### 5.3 / 5.4 Antenna Settings — `0x102` / `0x103`

```
Response: <ANTID><DWELL><PWR>
```

| 欄位 | 長度 | 說明 |
|------|------|------|
| ANTID | 1 Byte | 天線編號 1~4 |
| DWELL | 2 Byte | Dwell Time（ms） |
| PWR | 2 Byte | 天線功率（cdBm，百分之一 dBm） |

### 5.5 / 5.6 Antenna State — `0x104` / `0x105`

- ANTSTATE：`0x00` Disable / `0x01` Enable

### 5.7 / 5.8 RF Mode — `0x106` / `0x107`

- RFMODE：2 Byte，例如 `103`

### 5.9 / 5.10 Gen2 Algorithm — `0x108` / `0x109`

| 欄位 | 說明 |
|------|------|
| ALG | `0`: Fixed Q / `1`: Dynamic Q |
| INITQ | Q 值或初始 Q |
| QRANGE | Bit0-3: Min Q；Bit4-7: Max Q |
| DUALTARGET | Dual Target 啟用 |

### 5.11 / 5.12 Gen2 Query Group — `0x10A` / `0x10B`

| 欄位 | 說明 |
|------|------|
| SELECT | `1`: All / `2`: De-Selected / `3`: Selected |
| SESSION | S0~S3 |
| TARGET | `0`: Target A / `1`: Target B |

### 5.13 / 5.14 Bi-Static Antenna — `0x10C` / `0x10D`

- BISTATICEN：`0` Disable / `1` Enable

### 5.15 / 5.16 LBT Settings — `0x110` / `0x111`

- LBTENABLE：`0` Disable / `1` Enable

### 5.17 / 5.18 FastID Settings — `0x112` / `0x113`

- FASTIDEN：`0` Disable / `1` Enable

### 5.19 / 5.20 TagFocus Settings — `0x114` / `0x115`

- TAGFOCUSEN：`0` Disable / `1` Enable

### 5.21 / 5.22 Operation Mode — `0x116` / `0x117`

- OPMODE：`0` Continuous / `1` Non-Continuous

### 5.23 / 5.24 Fixed Frequency — `0x118` / `0x119`

```
Response: <NUM><FREQ1>…<FREQn>
```
- NUM：固定頻率數量（`0` 為停用）
- FREQ：4 Byte，單位 kHz（例：`902750`）

### 5.25 / 5.26 Tx On/Off Time — `0x11A` / `0x11B`

- ONTIME / OFFTIME：各 2 Byte，單位 ms

### 5.27 / 5.28 Phase Data Settings — `0x11C` / `0x11D`

- PHASEEN：`0` Disable / `1` Enable

### 5.29 / 5.30 Ambient Temperature Protection — `0x120` / `0x121`

- TEMPTHSD：2 Byte，單位 0.1°C

### 5.31 / 5.32 PA Temperature Protection — `0x122` / `0x123`

- TEMPTHSD：2 Byte，單位 0.1°C

### 5.33 / 5.34 Temperature Protection Mode — `0x124` / `0x125`

| TPMODE | 說明 |
|--------|------|
| `0x00` | Stop Mode（立即停止） |
| `0x01` | Non-Stop Mode（溫度降低後恢復） |

### 5.35 / 5.36 Antenna Inventory Round Count — `0x126` / `0x127`

- ANTID：天線編號
- INVRNDCNT：2 Byte，盤點循環次數

### 5.37 / 5.38 Inventory Data Customized Format Settings — `0x128` / `0x129`

設定 Customized Format 要輸出的 PID 清單（同 Ch4.7 的 PID 表）。  
傳空 PID 清單 = 停用 Customized Format。

### 5.39 / 5.40 System Time — `0x12A` / `0x12B`

```
Fields: <YEAR><MON><DAY><HOUR><MIN><SEC>（各 1 Byte）
```
- YEAR：從 2000 起算（例：`0x19` = 2025）

### 5.41 / 5.42 Power Saving Mode — `0x130` / `0x131`

| PSMODE | 說明 |
|--------|------|
| `0x00` | Power-Saving Mode |
| `0x01` | Power-On Mode（常開） |

### 5.43 / 5.44 Power Saving Timeout — `0x132` / `0x133`

- PSTIME：2 Byte，單位 ms（閒置多久後進入休眠）

### 5.45 / 5.46 UART Default Baud Rate — `0x150` / `0x151`

- BAUDRATE：4 Byte，可選 `115200 / 460800 / 921600`
- **注意：新 Baud Rate 於下次重開機後生效**

### 5.47 / 5.48 GPIO Pins Configuration — `0x180` / `0x181`

| 欄位 | 說明 |
|------|------|
| PINNUM | GPIO 腳位編號 |
| PINDIR | `0`: Input / `1`: Output / `2`: Alt-Func |

### 5.49 / 5.50 GPIO Pins State — `0x182` / `0x183`

| PINSTATE | 說明 |
|----------|------|
| `0` | Low |
| `1` | High |

### 5.51 / 5.52 LBT Configure Settings — `0x190` / `0x191`

- RSSITHSD：2 Byte，LBT RSSI 門檻（cdBm），例：`-7400` = -74 dBm

### 5.53 / 5.54 Antenna Switching Mode — `0x1A0` / `0x1A1`

| ASMODE | 說明 |
|--------|------|
| `0x00` | Stop Mode（切換天線時停止 RF） |
| `0x01` | Non-Stop Mode（切換天線時保持 RF） |

### 5.55 / 5.56 Frequency Switching Mode — `0x1A2` / `0x1A3`

| FSMODE | 說明 |
|--------|------|
| `0x00` | Stop Mode（切換頻率時停止 RF） |
| `0x01` | Non-Stop Mode（切換頻率時保持 RF） |

### 5.57 / 5.58 Antenna Detection Settings — `0x1A4` / `0x1A5`

- ANTDET：`0x00` Disable / `0x01` Enable

### 5.59 / 5.60 Configuration Storage Mode — `0x1F0` / `0x1F1`

| STMODE | 說明 |
|--------|------|
| `0x00` | Save to RAM & Flash |
| `0x01` | Save to RAM only |

### 5.61 / 5.62 Error Suppression Mode — `0x1F2` / `0x1F3`

| ESMODE | 說明 |
|--------|------|
| `0x00` | 回報所有 Ex10 錯誤 |
| `0x01` | 抑制所有 Ex10 錯誤 |

---

## 6. RFID Extended Operation

### 6.1 CW On（連續波開啟）— `0xF00`

```
Host ⇒ <STX><LENGTH><ID><ANTID><PWR><FREQ><CHECKSUM><ETX>
```

| 欄位 | 長度 | 說明 |
|------|------|------|
| ANTID | 1 Byte | 測試天線編號 1~4 |
| PWR | 2 Byte | 功率（cdBm） |
| FREQ | 4 Byte | 頻率（kHz） |

### 6.2 CW Off（連續波關閉）— `0xF01`

```
Host ⇒ <STX><LENGTH><ID><CHECKSUM><ETX>
```

### 6.3 Tx Random Data（發送隨機資料）— `0xF02`

```
Host ⇒ <STX><LENGTH><ID><ANTID><PWR><FREQ><DURATION><CHECKSUM><ETX>
```
- DURATION：4 Byte，自動關閉時間（ms）

---

## 7. Event Notification Commands

### 7.1 Event Notification Enable/Disable — `0x90`

```
Host ⇒ <STX><LENGTH><ID><EVENTID><DATA><CHECKSUM><ETX>
```
- DATA：`0x01` Enable / `0x00` Disable

| EVENTID | 事件 |
|---------|------|
| `0xE0` | RFID Antenna Begin/End |
| `0xE1` | RFID LBT Status |

### 7.2 Event: Antenna Status — `0xE0`

```
RFID ⇒ <STX><LENGTH><ID><ANTID><ANTSTATE><CHECKSUM><ETX>
```
- ANTSTATE：`0x00` Begin / `0x01` End

### 7.3 Event: LBT Status — `0xE1`

```
RFID ⇒ <STX><LENGTH><ID><INTERF><RSSI><FREQ><CHECKSUM><ETX>
```

| 欄位 | 說明 |
|------|------|
| INTERF | `0`: No Interferer / `1`: Interferer Present |
| RSSI | 2 Byte |
| FREQ | 4 Byte，單位 kHz |

---

## 8. Firmware Update Commands

### 8.1 Start File Transfer — `0x6101`

```
Host ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
DATA = "File Name" + "\0" + "File Size" + "\0"
```
- 僅供 File Update Mode 使用
- RM300P 每封包最大 1024 Bytes

**範例：**
```
DATA = "SL220FW.bin\04096\0"
```

### 8.2 Send File Data — `0x6102`

```
Host ⇒ <STX><LENGTH><ID><DATA><CHECKSUM><ETX>
DATA = 韌體二進位封包（Multi-Packets，最大 1024 Bytes）
```
- 僅供 File Update Mode 使用

---

## 快速指令對照表

| ID | 功能 |
|----|------|
| `0x21` | Read Firmware Version |
| `0x22` | Read Model Name |
| `0x24` | Read Serial Number |
| `0x25` | Read SKU ID |
| `0x28` | Read Ambient Temperature |
| `0x29` | Read PA Temperature |
| `0x2F` | Reset Device |
| `0x80` | Tag Inventory |
| `0x81` | Cancel Operation |
| `0x82` | Tag Read |
| `0x83` | Tag Write |
| `0x84` | Tag Lock |
| `0x85` | Tag Kill |
| `0x86` | Tag BlockWrite |
| `0x87` | Tag BlockPermalock |
| `0x88` | Tag Authenticate |
| `0x8A` | Tag Select |
| `0x8B` | Tag Inventory with User-Specified Data |
| `0x8C` | Tag MarginRead |
| `0x90` | Event Notification Enable/Disable |
| `0xA0` | Inventory Data with Customized Format（回應） |
| `0xB0` | Inventory Data/Phase（回應） |
| `0xB1` | Inventory Data/Phase with TID（回應） |
| `0xB2` | Inventory Data/Phase with User-Specified Data（回應） |
| `0xC0` | Inventory Data（回應） |
| `0xC1` | Inventory Data with TID（回應） |
| `0xC2` | Inventory Data with User-Specified Data（回應） |
| `0xC7` | Inventory Status（回應） |
| `0xC8` | Tag Access Data（回應） |
| `0xD0` | Operation Error（回應） |
| `0xD1` | System Error（回應） |
| `0xE0` | Event: Antenna Status（主動推送） |
| `0xE1` | Event: LBT Status（主動推送） |
| `0x100` | Get Region |
| `0x101` | Set Region |
| `0x102` | Get Antenna Settings |
| `0x103` | Set Antenna Settings |
| `0x104` | Get Antenna State |
| `0x105` | Set Antenna State |
| `0x106` | Get RF Mode |
| `0x107` | Set RF Mode |
| `0x108` | Get Gen2 Algorithm |
| `0x109` | Set Gen2 Algorithm |
| `0x10A` | Get Gen2 Query Group |
| `0x10B` | Set Gen2 Query Group |
| `0x10C` | Get Bi-Static Antenna Setting |
| `0x10D` | Set Bi-Static Antenna Settings |
| `0x110` | Get LBT Settings |
| `0x111` | Set LBT Settings |
| `0x112` | Get FastID Settings |
| `0x113` | Set FastID Settings |
| `0x114` | Get TagFocus Settings |
| `0x115` | Set TagFocus Settings |
| `0x116` | Get Operation Mode |
| `0x117` | Set Operation Mode |
| `0x118` | Get Fixed Frequency |
| `0x119` | Set Fixed Frequency |
| `0x11A` | Get Tx On/Off Time |
| `0x11B` | Set Tx On/Off Time |
| `0x11C` | Get Phase Data Settings |
| `0x11D` | Set Phase Data Settings |
| `0x120` | Get Ambient Temperature Protection |
| `0x121` | Set Ambient Temperature Protection |
| `0x122` | Get PA Temperature Protection |
| `0x123` | Set PA Temperature Protection |
| `0x124` | Get Temperature Protection Mode |
| `0x125` | Set Temperature Protection Mode |
| `0x126` | Get Antenna Inventory Round Count |
| `0x127` | Set Antenna Inventory Round Count |
| `0x128` | Get Inventory Customized Format Settings |
| `0x129` | Set Inventory Customized Format Settings |
| `0x12A` | Get System Time |
| `0x12B` | Set System Time |
| `0x130` | Get Power Saving Mode |
| `0x131` | Set Power Saving Mode |
| `0x132` | Get Power Saving Timeout |
| `0x133` | Set Power Saving Timeout |
| `0x150` | Get UART Baud Rate |
| `0x151` | Set UART Baud Rate |
| `0x180` | Get GPIO Pins Configuration |
| `0x181` | Set GPIO Pins Configuration |
| `0x182` | Get GPIO Pins State |
| `0x183` | Set GPIO Pins State |
| `0x190` | Get LBT Configure Settings |
| `0x191` | Set LBT Configure Settings |
| `0x1A0` | Get Antenna Switching Mode |
| `0x1A1` | Set Antenna Switching Mode |
| `0x1A2` | Get Frequency Switching Mode |
| `0x1A3` | Set Frequency Switching Mode |
| `0x1A4` | Get Antenna Detection Settings |
| `0x1A5` | Set Antenna Detection Settings |
| `0x1F0` | Get Configuration Storage Mode |
| `0x1F1` | Set Configuration Storage Mode |
| `0x1F2` | Get Error Suppression Mode |
| `0x1F3` | Set Error Suppression Mode |
| `0xF00` | CW On |
| `0xF01` | CW Off |
| `0xF02` | Tx Random Data |
| `0x6101` | Start File Transfer（FW Update） |
| `0x6102` | Send File Data（FW Update） |
