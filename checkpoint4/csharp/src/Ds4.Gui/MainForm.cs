using Ds4.Core.Api;
using System.Drawing;

namespace Ds4.Gui;

public sealed class MainForm : Form
{
    private readonly Font _editorFont = new("Cascadia Mono", 11.0f, FontStyle.Regular, GraphicsUnit.Point);
    private readonly Font _uiFont = new("Segoe UI", 10.5f, FontStyle.Regular, GraphicsUnit.Point);
    private readonly Font _titleFont = new("Segoe UI", 10.5f, FontStyle.Bold, GraphicsUnit.Point);
    private readonly Font _resultFont = new("Segoe UI", 11.0f, FontStyle.Regular, GraphicsUnit.Point);

    private readonly TextBox _domain = new()
    {
        Multiline = true,
        ScrollBars = ScrollBars.Vertical,
        WordWrap = true,
        Dock = DockStyle.Fill,
        AcceptsTab = true,
        AcceptsReturn = true
    };

    private readonly TextBox _query = new()
    {
        Multiline = true,
        ScrollBars = ScrollBars.Vertical,
        WordWrap = true,
        Dock = DockStyle.Fill,
        AcceptsTab = true,
        AcceptsReturn = true
    };

    private readonly TextBox _result = new()
    {
        Multiline = true,
        ScrollBars = ScrollBars.Vertical,
        WordWrap = true,
        ReadOnly = true,
        Dock = DockStyle.Fill
    };

    private readonly Label _status = new()
    {
        Text = "Gotowe",
        Dock = DockStyle.Bottom,
        Height = 28,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(8, 0, 8, 0)
    };

    private readonly ComboBox _examples = new()
    {
        DropDownStyle = ComboBoxStyle.DropDownList,
        Width = 320,
        Visible = false
    };

    private readonly Button _toggleExamplesButton = new()
    {
        Text = "Pokaż przykłady",
        Height = 38,
        Width = 130
    };

    private readonly Button _loadExampleButton = new()
    {
        Text = "Wczytaj",
        Height = 38,
        Width = 90,
        Visible = false
    };

    private readonly Button _graphButton = new()
    {
        Text = "Graf DOT",
        Height = 38,
        Width = 100
    };

    private string _lastGraphDot = string.Empty;

    private readonly Button _solveButton = new()
    {
        Text = "Oblicz",
        Height = 38,
        Width = 120,
        Anchor = AnchorStyles.Right | AnchorStyles.Top
    };

    private readonly Button _clearButton = new()
    {
        Text = "Wyczyść wynik",
        Height = 38,
        Width = 130,
        Anchor = AnchorStyles.Right | AnchorStyles.Top
    };

    public MainForm()
    {
        Text = "DS4 Reasoner - procesy działań złożonych";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 680);
        Width = 1220;
        Height = 820;
        Font = _uiFont;
        BackColor = Color.FromArgb(248, 248, 248);

        _domain.Font = _editorFont;
        _query.Font = _editorFont;
        _result.Font = _resultFont;
        _result.BackColor = Color.White;
        _status.Font = _uiFont;
        _examples.Font = _uiFont;
        _solveButton.Font = _titleFont;
        _clearButton.Font = _uiFont;
        _toggleExamplesButton.Font = _uiFont;
        _loadExampleButton.Font = _uiFont;
        _graphButton.Font = _uiFont;

        BuildLayout();
        LoadExamples();

        _toggleExamplesButton.Click += (_, _) => ToggleExamples();
        _loadExampleButton.Click += (_, _) => LoadSelectedExample();
        _solveButton.Click += (_, _) => Solve();
        _clearButton.Click += (_, _) => SetTextNoUndo(_result, "");
        _graphButton.Click += (_, _) => ShowGraph();
        _examples.SelectedIndexChanged += (_, _) => LoadSelectedExample();

        if (_examples.Items.Count > 0)
            _examples.SelectedIndex = 0;
        _status.Text = "Gotowe. Przykłady są opcjonalne, możesz od razu wpisać własną dziedzinę i kwerendę.";
    }

    private void BuildLayout()
    {
        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(10),
        };
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(main);
        Controls.Add(_status);

        var top = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 7,
            RowCount = 1,
            Padding = new Padding(0, 0, 0, 8)
        };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 340));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
        main.Controls.Add(top, 0, 0);

        top.Controls.Add(_toggleExamplesButton, 0, 0);
        top.Controls.Add(_examples, 1, 0);
        top.Controls.Add(_loadExampleButton, 2, 0);
        top.Controls.Add(_clearButton, 4, 0);
        top.Controls.Add(_solveButton, 5, 0);
        top.Controls.Add(_graphButton, 6, 0);

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        main.Controls.Add(content, 0, 1);

        content.Controls.Add(Group("Dziedzina", _domain), 0, 0);

        var right = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(8, 0, 0, 0)
        };
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        right.Controls.Add(Group("Kwerenda", _query), 0, 0);
        right.Controls.Add(Group("Wynik", _result), 0, 1);
        content.Controls.Add(right, 1, 0);
    }

    private Control Group(string title, Control inner)
    {
        var box = new GroupBox
        {
            Text = title,
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            Margin = new Padding(0, 0, 0, 8),
            Font = _titleFont,
            BackColor = Color.FromArgb(248, 248, 248)
        };
        inner.Font = title == "Wynik" ? _resultFont : _editorFont;
        box.Controls.Add(inner);
        return box;
    }

    private void LoadExamples()
    {
        _examples.Items.Clear();
        foreach (var ex in Ds4Facade.ListExamples())
            _examples.Items.Add(new ExampleItem(ex.Id, ex.Name));
    }


    private void ToggleExamples()
    {
        var visible = !_examples.Visible;
        _examples.Visible = visible;
        _loadExampleButton.Visible = visible;
        _toggleExamplesButton.Text = visible ? "Ukryj przykłady" : "Pokaż przykłady";
        _status.Text = visible ? "Wybierz przykład albo wpisz własną dziedzinę i kwerendę." : "Tryb prostego IDE.";
    }

    private void ShowGraph()
    {
        using var form = new Form
        {
            Text = "Graf wykonania - DOT",
            Width = 980,
            Height = 720,
            StartPosition = FormStartPosition.CenterParent
        };
        var box = new TextBox
        {
            Multiline = true,
            WordWrap = true,
            ScrollBars = ScrollBars.Both,
            Dock = DockStyle.Fill,
            ReadOnly = true,
            Font = _editorFont,
            Text = string.IsNullOrWhiteSpace(_lastGraphDot)
                ? "Najpierw kliknij Oblicz, żeby wygenerować graf trace w formacie DOT."
                : _lastGraphDot
        };
        form.Controls.Add(box);
        form.ShowDialog(this);
    }

    private void LoadSelectedExample()
    {
        if (!_examples.Visible) return;
        if (_examples.SelectedItem is not ExampleItem item) return;

        var ex = Ds4Facade.LoadExample(item.Id);
        SetTextNoUndo(_domain, ex.Domain);
        SetTextNoUndo(_query, ex.Queries.FirstOrDefault() ?? "");
        SetTextNoUndo(_result,
            "Dostępne kwerendy dla przykładu:" + Environment.NewLine + Environment.NewLine +
            string.Join(Environment.NewLine, ex.Queries.Select(q => "• " + q)) +
            Environment.NewLine + Environment.NewLine +
            "Możesz wkleić jedną z nich do pola Kwerenda i kliknąć Oblicz.");
        _status.Text = ex.Name;
    }

    private void Solve()
    {
        _solveButton.Enabled = false;
        Cursor = Cursors.WaitCursor;
        _status.Text = "Obliczam...";

        try
        {
            // Ważne: TextBox trzyma własny undo-buffer. Jeżeli tylko nadpisujemy Text,
            // stare duże wyniki mogą zostawać w historii cofania i wyglądać jak wyciek pamięci.
            SetTextNoUndo(_result, "Obliczam...");

            var result = Ds4Facade.Solve(_domain.Text, _query.Text);
            if (!result.Ok)
            {
                SetTextNoUndo(_result, "BŁĄD:" + Environment.NewLine + result.Error);
                _status.Text = "Błąd";
                return;
            }

            _lastGraphDot = result.TraceGraphDot;
            var output =
                (result.Answer == true ? "TAK" : "NIE") + Environment.NewLine + Environment.NewLine +
                result.Explanation + Environment.NewLine + Environment.NewLine +
                $"|Σ| = {result.SigmaCount}, |Σ₀| = {result.Sigma0Count}" + Environment.NewLine +
                Environment.NewLine + "TRACE:" + Environment.NewLine + result.Trace;

            SetTextNoUndo(_result, output);
            _status.Text = "Obliczono";
        }
        finally
        {
            _solveButton.Enabled = true;
            Cursor = Cursors.Default;

            // W małej aplikacji demonstracyjnej warto po dużym trace poprosić GC o sprzątanie,
            // żeby kolejne kliknięcia nie wyglądały jak stały wzrost pamięci w Menedżerze zadań.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }

    private static void SetTextNoUndo(TextBox box, string text)
    {
        // WinForms TextBox na Windowsie najpewniej wyświetla nowe linie jako CRLF.
        // Przykłady z raw stringów w C# mogą mieć same LF, co czasem wygląda jak jedna długa linia.
        var normalizedText = NormalizeLineEndings(text);

        box.SuspendLayout();
        try
        {
            box.Clear();
            box.ClearUndo();
            box.Text = normalizedText;
            box.SelectionStart = 0;
            box.SelectionLength = 0;
            box.ScrollToCaret();
            box.ClearUndo();
        }
        finally
        {
            box.ResumeLayout();
        }
    }

    private static string NormalizeLineEndings(string text)
    {
        return text
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Replace("\n", Environment.NewLine);
    }

    private sealed record ExampleItem(string Id, string Name)
    {
        public override string ToString() => Name;
    }
}
