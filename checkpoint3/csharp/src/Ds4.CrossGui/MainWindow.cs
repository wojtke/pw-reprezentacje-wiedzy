using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Ds4.Core.Api;

namespace Ds4.CrossGui;

public sealed class MainWindow : Window
{
    private readonly ComboBox _examples = new();
    private readonly TextBox _domain = MultilineBox();
    private readonly TextBox _query = MultilineBox();
    private readonly TextBox _result = MultilineBox(isReadOnly: true);
    private readonly TextBlock _status = new() { Text = "Gotowe", VerticalAlignment = VerticalAlignment.Center };

    public MainWindow()
    {
        Title = "DS4 Reasoner";
        Width = 1280;
        Height = 820;
        MinWidth = 900;
        MinHeight = 600;
        Content = BuildLayout();
        LoadExamples();
    }

    private Control BuildLayout()
    {
        var root = new DockPanel { LastChildFill = true, Margin = new Thickness(12) };

        var top = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Margin = new Thickness(0, 0, 0, 10)
        };
        DockPanel.SetDock(top, Dock.Top);

        _examples.Width = 420;
        var load = new Button { Content = "Wczytaj przykład", MinWidth = 130 };
        var solve = new Button { Content = "Oblicz", MinWidth = 120 };
        var clear = new Button { Content = "Wyczyść wynik", MinWidth = 130 };

        load.Click += (_, _) => LoadSelectedExample();
        solve.Click += (_, _) => SolveCurrentInput();
        clear.Click += (_, _) => _result.Text = string.Empty;
        _examples.SelectionChanged += (_, _) => LoadSelectedExample();

        top.Children.Add(new TextBlock { Text = "Przykład:", VerticalAlignment = VerticalAlignment.Center });
        top.Children.Add(_examples);
        top.Children.Add(load);
        top.Children.Add(solve);
        top.Children.Add(clear);
        top.Children.Add(_status);
        root.Children.Add(top);

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            RowDefinitions = new RowDefinitions("*")
        };

        var left = LabeledPanel("Dziedzina", _domain);
        var right = new Grid
        {
            RowDefinitions = new RowDefinitions("*,*"),
            Margin = new Thickness(10, 0, 0, 0)
        };
        var queryPanel = LabeledPanel("Kwerenda", _query);
        var resultPanel = LabeledPanel("Wynik", _result);
        Grid.SetRow(queryPanel, 0);
        Grid.SetRow(resultPanel, 1);
        right.Children.Add(queryPanel);
        right.Children.Add(resultPanel);

        Grid.SetColumn(left, 0);
        Grid.SetColumn(right, 1);
        grid.Children.Add(left);
        grid.Children.Add(right);
        root.Children.Add(grid);

        return root;
    }

    private static TextBox MultilineBox(bool isReadOnly = false)
    {
        var box = new TextBox
        {
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = FontFamily.Parse("Consolas, Cascadia Mono, monospace"),
            FontSize = 15,
            IsReadOnly = isReadOnly
        };

        ScrollViewer.SetHorizontalScrollBarVisibility(box, ScrollBarVisibility.Disabled);
        ScrollViewer.SetVerticalScrollBarVisibility(box, ScrollBarVisibility.Auto);

        return box;
    }

    private static Control LabeledPanel(string title, Control content)
    {
        var panel = new DockPanel { Margin = new Thickness(0, 0, 0, 8), LastChildFill = true };
        var label = new TextBlock
        {
            Text = title,
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(0, 0, 0, 4)
        };
        DockPanel.SetDock(label, Dock.Top);
        panel.Children.Add(label);
        panel.Children.Add(content);
        return panel;
    }

    private void LoadExamples()
    {
        var items = Ds4Facade.ListExamples()
            .Select(e => new ExampleItem(e.Id, e.Name, e.Description))
            .ToArray();

        _examples.ItemsSource = items;
        if (items.Length > 0)
        {
            _examples.SelectedIndex = 0;
            LoadSelectedExample();
        }
        else
        {
            _status.Text = "Brak przykładów w folderze examples";
        }
    }

    private void LoadSelectedExample()
    {
        if (_examples.SelectedItem is not ExampleItem item) return;

        try
        {
            var example = Ds4Facade.LoadExample(item.Id);
            _domain.Text = Normalize(example.Domain);
            _query.Text = Normalize(string.Join(Environment.NewLine, example.Queries));
            _result.Text = string.Empty;
            _status.Text = example.Description;
        }
        catch (Exception ex)
        {
            _status.Text = "Błąd ładowania przykładu";
            _result.Text = ex.Message;
        }
    }

    private void SolveCurrentInput()
    {
        var result = Ds4Facade.Solve(_domain.Text ?? string.Empty, _query.Text ?? string.Empty);
        if (!result.Ok)
        {
            _result.Text = "BŁĄD" + Environment.NewLine + result.Error;
            _status.Text = "Błąd";
            return;
        }

        var answer = result.Answer == true ? "TAK" : "NIE";
        _result.Text =
            "ODPOWIEDŹ: " + answer + Environment.NewLine +
            "Sigma: " + result.SigmaCount + Environment.NewLine +
            "Sigma0: " + result.Sigma0Count + Environment.NewLine +
            Environment.NewLine +
            result.Explanation + Environment.NewLine +
            Environment.NewLine +
            "TRACE:" + Environment.NewLine +
            result.Trace;
        _status.Text = "Obliczono";
    }

    private static string Normalize(string text)
        => text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);

    private sealed record ExampleItem(string Id, string Name, string Description)
    {
        public override string ToString() => Name;
    }
}
