using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using PdfCompressor.App.Configuration;
using PdfCompressor.App.Localization;
using PdfCompressor.Core.Logging;
using PdfCompressor.Windows.ContextMenu;

namespace PdfCompressor.App.UI;

internal sealed class SettingsForm : Form
{
    private static readonly string[] LanguageCodes =
        { AppSettings.LanguageAuto, AppSettings.LanguagePtBr, AppSettings.LanguageEn };

    private static readonly string[] PresetCodes =
        { AppSettings.PresetScreen, AppSettings.PresetEbook, AppSettings.PresetPrinter };

    private readonly AppSettings _settings;
    private readonly string _executablePath;
    private bool _updatingTexts;

    private readonly Label _titleLabel = new();
    private readonly Label _versionLabel = new();
    private readonly Label _languageLabel = new();
    private readonly ComboBox _languageCombo = new();
    private readonly Label _presetLabel = new();
    private readonly ComboBox _presetCombo = new();
    private readonly Label _statusLabel = new();
    private readonly Button _installButton = new();
    private readonly Button _removeButton = new();
    private readonly Button _closeButton = new();

    public SettingsForm(AppSettings settings, string executablePath)
    {
        _settings = settings;
        _executablePath = executablePath;

        BuildLayout();
        ApplyTexts();
        LoadValues();
        RefreshContextMenuStatus();
    }

    private void BuildLayout()
    {
        SuspendLayout();

        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = true;
        StartPosition = FormStartPosition.CenterScreen;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Padding = new Padding(16);
        AcceptButton = _closeButton;
        CancelButton = _closeButton;

        try
        {
            Icon = Icon.ExtractAssociatedIcon(_executablePath);
        }
        catch (Exception ex)
        {
            AppLogger.Error("Não foi possível carregar o ícone do executável.", ex);
        }

        var root = new TableLayoutPanel
        {
            ColumnCount = 1,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Fill,
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _titleLabel.AutoSize = true;
        _titleLabel.Font = new Font(Font.FontFamily, Font.Size + 4f, FontStyle.Bold);
        _titleLabel.Margin = new Padding(0, 0, 0, 2);

        _versionLabel.AutoSize = true;
        _versionLabel.ForeColor = SystemColors.GrayText;
        _versionLabel.Margin = new Padding(0, 0, 0, 12);

        _languageLabel.AutoSize = true;
        _languageLabel.Margin = new Padding(0, 0, 0, 4);

        _languageCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _languageCombo.Width = 400;
        _languageCombo.Margin = new Padding(0, 0, 0, 12);
        _languageCombo.SelectedIndexChanged += OnLanguageChanged;

        _presetLabel.AutoSize = true;
        _presetLabel.Margin = new Padding(0, 0, 0, 4);

        _presetCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _presetCombo.Width = 400;
        _presetCombo.Margin = new Padding(0, 0, 0, 12);
        _presetCombo.SelectedIndexChanged += OnPresetChanged;

        _statusLabel.AutoSize = true;
        _statusLabel.Margin = new Padding(0, 0, 0, 6);

        var contextMenuButtons = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 16),
        };

        _installButton.AutoSize = true;
        _installButton.Padding = new Padding(8, 2, 8, 2);
        _installButton.Margin = new Padding(0, 0, 8, 0);
        _installButton.Click += OnInstallClicked;

        _removeButton.AutoSize = true;
        _removeButton.Padding = new Padding(8, 2, 8, 2);
        _removeButton.Margin = new Padding(0);
        _removeButton.Click += OnRemoveClicked;

        contextMenuButtons.Controls.Add(_installButton);
        contextMenuButtons.Controls.Add(_removeButton);

        var closeRow = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
        };

        _closeButton.AutoSize = true;
        _closeButton.Padding = new Padding(12, 2, 12, 2);
        _closeButton.Margin = new Padding(0);
        _closeButton.Click += (_, _) => Close();
        closeRow.Controls.Add(_closeButton);

        root.Controls.Add(_titleLabel);
        root.Controls.Add(_versionLabel);
        root.Controls.Add(_languageLabel);
        root.Controls.Add(_languageCombo);
        root.Controls.Add(_presetLabel);
        root.Controls.Add(_presetCombo);
        root.Controls.Add(_statusLabel);
        root.Controls.Add(contextMenuButtons);
        root.Controls.Add(closeRow);

        Controls.Add(root);

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        ResumeLayout(false);
        PerformLayout();
    }

    private void LoadValues()
    {
        _updatingTexts = true;
        _languageCombo.SelectedIndex = Math.Max(0, Array.IndexOf(LanguageCodes, _settings.Language));
        _presetCombo.SelectedIndex = Math.Max(0, Array.IndexOf(PresetCodes, _settings.QualityPreset));
        _updatingTexts = false;
    }

    private void ApplyTexts()
    {
        _updatingTexts = true;

        Text = Strings.App_Title;
        _titleLabel.Text = Strings.App_Title;
        _versionLabel.Text = GetVersionText();
        _languageLabel.Text = Strings.Settings_LanguageLabel;
        _presetLabel.Text = Strings.Settings_QualityLabel;
        _installButton.Text = Strings.Settings_InstallOrUpdate;
        _removeButton.Text = Strings.Settings_Remove;
        _closeButton.Text = Strings.Settings_Close;

        ReplaceItems(_languageCombo, Strings.Language_Auto, Strings.Language_PtBr, Strings.Language_En);
        ReplaceItems(
            _presetCombo,
            $"{Strings.Preset_Screen_Name} — {Strings.Preset_Screen_Description}",
            $"{Strings.Preset_Ebook_Name} — {Strings.Preset_Ebook_Description}",
            $"{Strings.Preset_Printer_Name} — {Strings.Preset_Printer_Description}");

        _updatingTexts = false;
    }

    private static void ReplaceItems(ComboBox combo, params string[] items)
    {
        var selected = combo.SelectedIndex;
        combo.BeginUpdate();
        combo.Items.Clear();
        combo.Items.AddRange(items);
        combo.SelectedIndex = selected < 0 ? 0 : selected;
        combo.EndUpdate();
    }

    private static string GetVersionText()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return version is null ? string.Empty : "v" + version.ToString(3);
    }

    private void RefreshContextMenuStatus()
    {
        var installed = ContextMenuStatus.IsInstalled(_executablePath);
        _statusLabel.Text = installed
            ? Strings.Settings_ContextMenuInstalled
            : Strings.Settings_ContextMenuNotInstalled;
        _removeButton.Enabled = installed;
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        if (_updatingTexts || _languageCombo.SelectedIndex < 0)
            return;

        var language = LanguageCodes[_languageCombo.SelectedIndex];
        if (language == _settings.Language)
            return;

        _settings.Language = language;
        SaveSettings();
        AppCulture.Apply(language);
        ApplyTexts();

        // Menu instalado guarda o rótulo no Registro: regrava no idioma novo.
        if (ContextMenuStatus.IsInstalled(_executablePath))
            InstallContextMenu();

        RefreshContextMenuStatus();
    }

    private void OnPresetChanged(object? sender, EventArgs e)
    {
        if (_updatingTexts || _presetCombo.SelectedIndex < 0)
            return;

        var preset = PresetCodes[_presetCombo.SelectedIndex];
        if (preset == _settings.QualityPreset)
            return;

        _settings.QualityPreset = preset;
        SaveSettings();
    }

    private void OnInstallClicked(object? sender, EventArgs e)
    {
        InstallContextMenu();
        RefreshContextMenuStatus();
    }

    private void OnRemoveClicked(object? sender, EventArgs e)
    {
        try
        {
            ContextMenuUninstaller.Uninstall();
        }
        catch (Exception ex)
        {
            AppLogger.Error("Falha ao remover o menu de contexto.", ex);
            ShowContextMenuError();
        }

        RefreshContextMenuStatus();
    }

    private void InstallContextMenu()
    {
        try
        {
            ContextMenuInstaller.Install(_executablePath, Strings.ContextMenu_CompressPdf);
        }
        catch (Exception ex)
        {
            AppLogger.Error("Falha ao instalar/atualizar o menu de contexto.", ex);
            ShowContextMenuError();
        }
    }

    private void SaveSettings()
    {
        try
        {
            AppSettingsStore.Save(_settings);
        }
        catch (Exception ex)
        {
            AppLogger.Error("Falha ao salvar settings.json.", ex);
        }
    }

    // Detalhe técnico fica no log; a UI mostra só a mensagem localizada.
    private void ShowContextMenuError() =>
        MessageBox.Show(this, Strings.Settings_ContextMenuError, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
}
