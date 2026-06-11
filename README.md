# Unitech RFID Tools

針對 **Unitech RFID RM300P** 讀取模組開發的測試工具專案。

## 專案簡介

本專案的目標，是為 Unitech RFID RM300P 設備開發一套**開發測試工具**。
我們以原廠提供的通訊規格文件
[`docs/Unitech_RFID_Command_Set_V1.02_20250814_User.pdf`](docs/Unitech_RFID_Command_Set_V1.02_20250814_User.pdf)
（V1.02，2025/08/14，適用韌體 RM300P V0.1.0.14 或更新版本）為基礎，
透過**解析、分類**其中的指令集，來決定開發測試工具所需的規則與功能涵蓋範圍。

整個專案分多個主階段推進：

1. **規格文件轉換**（進行中）— 將原廠 PDF 逐章拆解為繁體中文 Markdown，便於解析與分類。
2. 開發規格文件定義
3. 實作
4. 測試
5. 驗收

> 專案的階段定錨、硬性規定與進度追蹤方式，統一記錄於 [`CLAUDE.md`](CLAUDE.md)；
> 階段 1 的逐章子任務進度記錄於 [`docs/執行提示詞與確認清單.md`](docs/執行提示詞與確認清單.md)。

## 通訊協定概觀

RM300P 採用 **UART** 介面（鮑率 115200 / 460800 / 921600），主機（Host）與模組（RFID）
之間以封包格式溝通。每個封包以 `<STX>`(0x02) 起始、`<ETX>`(0x03) 結束，並含長度、
指令 ID 與檢查碼（CHECKSUM）欄位確保完整性。詳見
[`docs/Chapter1_CommandFormat.md`](docs/Chapter1_CommandFormat.md)。

## 通訊內容包含的操作功能

原廠規格文件共分 8 章，涵蓋以下操作功能類別：

### 1. 一般指令（General Commands）
查詢與裝置層級控制：
- 讀取韌體版本、機型名稱、序號、SKU ID
- 讀取環境溫度、PA（功率放大器）溫度
- 重置裝置（重新開機 / 恢復原廠設定）

### 2. RFID 標籤操作（RFID Operation）
針對 Gen2（ISO 18000-6C）標籤的核心讀寫操作：
- 標籤盤點（Inventory）、取消操作（Cancel）
- 標籤讀取（Read）、寫入（Write）、區塊寫入（BlockWrite）
- 標籤鎖定（Lock）、永久鎖定（BlockPermalock）、Kill（銷毀）
- 標籤認證（Authenticate）、標籤選取（Select）
- 帶使用者指定資料的盤點（Inventory with User-Specified Data）
- 邊界讀取（MarginRead）

### 3. RFID 資料回覆（RFID Data Reply）
模組主動回傳的標籤資料與狀態：
- 盤點資料（含 EPC / TID / 使用者指定資料 / 相位 Phase / 自訂格式）
- 盤點狀態、標籤存取資料（Tag Access Data）
- 操作錯誤、系統錯誤回報

### 4. RFID 參數設定（RFID Configuration）
最完整的設定類指令（含成對的 Get / Set），涵蓋：
- 區域（Region）、天線設定（功率 / Dwell Time / 啟用狀態）
- RF 模式、Gen2 演算法（Q 值）、Gen2 查詢群組（Session / Target）
- Bi-Static 天線、LBT（先聽後送）、FastID、TagFocus
- 操作模式（連續 / 非連續）、固定頻率、Tx On/Off 時間、相位資料輸出
- 環境/PA 溫度保護門檻與保護模式
- 天線盤點輪次、盤點資料自訂格式、系統時間
- 省電模式與逾時、UART 預設鮑率
- GPIO 腳位設定與狀態、LBT 門檻
- 單一天線/單一頻率的切換模式、天線偵測、設定儲存模式、錯誤抑制模式

### 5. RFID 擴充操作（RFID Extended Operation）
射頻測試用途：
- CW（連續載波）開啟 / 關閉
- 發送隨機資料（Tx Random Data）

### 6. 事件通知（Event Notification Commands）
模組主動通知主機的事件：
- 啟用/停用事件通知
- 天線開始/結束狀態通知
- LBT（干擾偵測）狀態通知

### 7. 韌體更新（Firmware Update Commands）
- 開始檔案傳輸、傳送檔案資料、執行韌體更新（File Update / Y-Modem 模式）

## 目錄結構

```
.
├── CLAUDE.md                    # 專案定錨指標、硬性規定、階段路線圖
├── README.md                    # 本檔案
└── docs/
    ├── Unitech_RFID_Command_Set_V1.02_20250814_User.pdf   # 原廠規格（來源）
    ├── 執行提示詞與確認清單.md     # 階段 1 子任務清單與各章提示詞
    ├── Chapter1_CommandFormat.md   # 第 1 章：封包格式
    ├── Chapter2_GeneralCommands.md # 第 2 章：一般指令
    └── ...                         # 第 3~8 章（陸續產出）
```

## 規格文件轉換進度

| 章節 | 內容 | 狀態 |
|---|---|---|
| 1 | Command Format（封包格式） | ✅ |
| 2 | General Commands（一般指令） | ✅ |
| 3 | RFID Operation（標籤操作） | ⬜ |
| 4 | RFID Data Reply（資料回覆） | ⬜ |
| 5 | RFID Configuration（參數設定） | ⬜ |
| 6 | RFID Extended Operation（擴充操作） | ⬜ |
| 7 | Event Notification（事件通知） | ⬜ |
| 8 | Firmware Update（韌體更新） | ⬜ |

> 最新進度以 [`docs/執行提示詞與確認清單.md`](docs/執行提示詞與確認清單.md) 為準。
