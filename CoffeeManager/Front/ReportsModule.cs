using CoffeeManager.Models.Class;

public class ReportsModule : UserControl
{
    private readonly Store _store;
    private readonly ReportService _reports = new();

    private ComboBox cbType = null!;
    private DateTimePicker dtDate = null!;
    private TextBox txtOutput = null!;
    private Button btnGenerate = null!;
    private Button btnExport = null!;
    private Button btnPrint = null!;

    public ReportsModule(Store store)
    {
        _store = store;
        Dock = DockStyle.Fill;
        BackColor = Color.White;
        InitializeUI();
    }

    private void InitializeUI()
    {
        var lblTitle = new Label
        {
            Text = "Reportes",
            Font = new Font("Segoe UI", 26, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };
        Controls.Add(lblTitle);

        cbType = new ComboBox
        {
            Location = new Point(20, 90),
            Width = 250,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 10)
        };
        cbType.Items.Add("Reporte de ventas");
        cbType.Items.Add("Reporte de inventario");
        cbType.Items.Add("Reporte de pérdidas");
        cbType.SelectedIndex = 0;
        Controls.Add(cbType);

        dtDate = new DateTimePicker
        {
            Location = new Point(290, 90),
            Width = 200,
            Font = new Font("Segoe UI", 10)
        };
        Controls.Add(dtDate);

        btnGenerate = new Button
        {
            Text = "Generar",
            Location = new Point(510, 85),
            Size = new Size(120, 35),
            BackColor = Color.FromArgb(70, 180, 160),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnGenerate.Click += GenerateReport;
        Controls.Add(btnGenerate);

        btnPrint = new Button
        {
            Text = "Imprimir",
            Location = new Point(640, 85),
            Size = new Size(120, 35),
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnPrint.Click += PrintReport;
        Controls.Add(btnPrint);

        btnExport = new Button
        {
            Text = "Exportar TXT",
            Location = new Point(770, 85),
            Size = new Size(120, 35),
            BackColor = Color.FromArgb(40, 120, 200),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnExport.Click += ExportReport;
        Controls.Add(btnExport);

        txtOutput = new TextBox
        {
            Location = new Point(20, 140),
            Size = new Size(870, 450),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Consolas", 10),
            ReadOnly = true
        };
        Controls.Add(txtOutput);
    }

    private void GenerateReport(object? sender, EventArgs e)
    {
        string type = cbType.SelectedItem.ToString()!;
        DateTime date = dtDate.Value;

        string report = type switch
        {
            "Reporte de ventas" => _reports.GenerateDailyReport(_store, date),
            "Reporte de inventario" => GenerateInventoryReport(),
            "Reporte de pérdidas" => GenerateLossReport(),
            _ => "Tipo de reporte no válido."
        };

        txtOutput.Text = report;
    }

    private string GenerateInventoryReport()
    {
        string report = "===== INVENTARIO =====\n\n";

        foreach (var item in _store.Warehouse.Items)
            report += $"{item.Name} — {item.Quantity} {item.Unit}\n";

        return report;
    }

    private string GenerateLossReport()
    {
        return
            "===== PÉRDIDAS =====\n\n" +
            $"Total pérdidas: ${_store.TotalLosses:F2}\n" +
            $"Total desperdicio: ${_store.TotalWaste:F2}\n";
    }

    private void PrintReport(object? sender, EventArgs e)
    {
        MessageBox.Show("Imprimiendo reporte...\n\n" + txtOutput.Text,
            "Imprimir", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ExportReport(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtOutput.Text))
        {
            MessageBox.Show("No hay reporte para exportar.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SaveFileDialog dlg = new SaveFileDialog
        {
            Filter = "Archivo de texto (*.txt)|*.txt",
            FileName = "Reporte.txt"
        };

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            File.WriteAllText(dlg.FileName, txtOutput.Text);
            MessageBox.Show("Reporte exportado correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
