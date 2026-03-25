# 코드 컨벤션 및 네이밍

## 공통 규칙

- 모든 코드 보여줄때 첫줄에 주석으로 경로/파일명 기입
- 항상 주석 주석 필수

## 네이밍 규칙

| 대상          | 규칙                 | 예시                |
| ----------- | ------------------ | ----------------- |
| 클래스         | PascalCase         | `PrinterService`  |
| 인터페이스       | I + PascalCase     | `IPrinterService` |
| 메서드         | PascalCase         | `PrintLabel()`    |
| 변수          | camelCase          | `printerName`     |
| 필드(private) | _camelCase         | `_printerService` |
| 상수          | PascalCase         | `DefaultPort`     |
| 컨트롤         | camelCase + prefix | `btnPrint`        |
| 이벤트         | camelCase + _Event | `btnPrint_Click`  |

### 1️⃣ WinForms 컨트롤 규칙

✔ Prefix 규칙

| 컨트롤          | Prefix | 예시             |
| ------------ | ------ | -------------- |
| Button       | btn    | `btnPrint`     |
| TextBox      | txt    | `txtTspl`      |
| ComboBox     | cmb    | `cmbPrinter`   |
| Label        | lbl    | `lblStatus`    |
| CheckBox     | chk    | `chkAutoPrint` |
| DataGridView | dgv    | `dgvLogs`      |

✔ 이벤트 규칙

```C#
private void btnPrint_Click(object sender, EventArgs e)
```

### 2️⃣ Form Naming

```C#
MainForm
SettingsForm
LabelPreviewForm
PrinterConfigForm
```

👉 항상 PascalCase

### 3️⃣ Service / Domain (클린 아키텍처)

✔ 인터페이스

```C#
public interface IPrinterService
{
    void Print(string printerName, string tspl);
}
```

✔ 구현체

```C#
public class UsbPrinterService : IPrinterService
```

✔ Builder

```C#
public class TsplBuilder
```

✔ DTO

```C#
public class PrintRequest
public class LabelDto
```

### 4️⃣ 필드 / DI 규칙

```C#
private readonly IPrinterService _printerService;
private readonly TsplBuilder _tsplBuilder;
```

👉 _camelCase 유지

### 5️⃣ 메서드 Naming

✔ 기본

```C#
Print()
Build()
LoadPrinters()
InitializePrinter()
```

✔ 이벤트 메서드

```C#
btnBuild_Click
btnPrint_Click
cmbPrinter_SelectedIndexChanged
```
### 6️⃣ 파일 Naming

```C#
PrinterService.cs
RawPrinterHelper.cs
TsplBuilder.cs
MainForm.cs
```
### 7️⃣ Namespace 규칙

```C#
XPrinter.XP108B.POC
XPrinter.XP108B.POC.UI
XPrinter.XP108B.POC.Services
```

### 8️⃣ 상수 / 설정

```C#
public const int DefaultPort = 9100;
public const string DefaultPrinter = "XPrinter";
```

### 9️⃣ 로그 메시지 규칙

```C#
Log("프린터 초기화 완료");
Log("TSPL 생성 완료");
Log("출력 요청 전송");
```

### 🔥 실전 적용 예시 (MainForm)

```
public partial class MainForm : Form
{
    private readonly IPrinterService _printerService;
    private readonly TsplBuilder _tsplBuilder;

    public MainForm()
    {
        InitializeComponent();

        _printerService = new UsbPrinterService();
        _tsplBuilder = new TsplBuilder();

        LoadPrinters();
    }

    private void btnBuild_Click(object sender, EventArgs e)
    {
        var tspl = _tsplBuilder
            .Init()
            .Text("HELLO")
            .Build();

        txtTspl.Text = tspl;
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
        var printer = cmbPrinter.SelectedItem?.ToString();

        _printerService.Print(printer, txtTspl.Text);
    }
}
```

### 🚫 하지 말아야 할 것

```
❌ BtnPrint (PascalCase 컨트롤)
❌ btn_print (snake_case)
❌ printButton (prefix 없음)
❌ BtnPrint_Click (디자이너 충돌 가능)
```

### 💡 확장 대비 규칙 (중요)

👉 나중에 LAN / Agent 붙을 때:

```C#
UsbPrinterService
NetworkPrinterService
AgentPrinterService
```
👉 그대로 확장 가능