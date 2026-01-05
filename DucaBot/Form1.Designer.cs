namespace DucaBot
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            headerPanel = new Panel();
            pipeRestartLabel = new Label();
            statusLabel = new Label();
            titleLabel = new Label();
            controlPanel = new Panel();
            recordButton = new Button();
            autoWalkButton = new Button();
            refreshButton = new Button();
            processCombo = new ComboBox();
            processLabel = new Label();
            keyDelayLabel = new Label();
            keyDelayText = new TextBox();
            delayLabel = new Label();
            delayText = new TextBox();
            actionsPanel = new Panel();
            clearGridButton = new Button();
            setIntervalsButton = new Button();
            addCurrentButton = new Button();
            importButton = new Button();
            saveButton = new Button();
            valuesPanel = new Panel();
            goToValue = new Label();
            goToLabel = new Label();
            goToCheck = new CheckBox();
            creatureRawValue = new Label();
            creatureRawLabel = new Label();
            autoLootCheck = new CheckBox();
            autoLootLabel = new Label();
            monsterValue = new Label();
            monsterLabel = new Label();
            posZValue = new Label();
            posYValue = new Label();
            posXValue = new Label();
            posZLabel = new Label();
            posYLabel = new Label();
            posXLabel = new Label();
            historyGrid = new DataGridView();
            colX = new DataGridViewTextBoxColumn();
            colY = new DataGridViewTextBoxColumn();
            colZ = new DataGridViewTextBoxColumn();
            colInterval = new DataGridViewTextBoxColumn();
            headerPanel.SuspendLayout();
            controlPanel.SuspendLayout();
            actionsPanel.SuspendLayout();
            valuesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)historyGrid).BeginInit();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.FromArgb(32, 36, 48);
            headerPanel.Controls.Add(pipeRestartLabel);
            headerPanel.Controls.Add(statusLabel);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(784, 56);
            headerPanel.TabIndex = 0;
            // 
            // pipeRestartLabel
            // 
            pipeRestartLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pipeRestartLabel.AutoSize = true;
            pipeRestartLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            pipeRestartLabel.ForeColor = Color.FromArgb(255, 100, 100);
            pipeRestartLabel.Location = new Point(524, 35);
            pipeRestartLabel.Name = "pipeRestartLabel";
            pipeRestartLabel.Size = new Size(200, 15);
            pipeRestartLabel.TabIndex = 2;
            pipeRestartLabel.Text = "Reinicie o BOT para conectar ao pipe";
            pipeRestartLabel.Visible = false;
            // 
            // statusLabel
            // 
            statusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font("Segoe UI", 9F);
            statusLabel.ForeColor = Color.LightGray;
            statusLabel.Location = new Point(524, 20);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(146, 15);
            statusLabel.TabIndex = 1;
            statusLabel.Text = "Aguardando configuração";
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(16, 14);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(78, 25);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Cav Bot";
            // 
            // controlPanel
            // 
            controlPanel.BackColor = Color.FromArgb(24, 27, 38);
            controlPanel.Controls.Add(recordButton);
            controlPanel.Controls.Add(autoWalkButton);
            controlPanel.Controls.Add(refreshButton);
            controlPanel.Controls.Add(processCombo);
            controlPanel.Controls.Add(processLabel);
            controlPanel.Controls.Add(keyDelayLabel);
            controlPanel.Controls.Add(keyDelayText);
            controlPanel.Controls.Add(delayLabel);
            controlPanel.Controls.Add(delayText);
            controlPanel.Dock = DockStyle.Top;
            controlPanel.Location = new Point(0, 56);
            controlPanel.Name = "controlPanel";
            controlPanel.Padding = new Padding(12, 10, 12, 10);
            controlPanel.Size = new Size(784, 78);
            controlPanel.TabIndex = 1;
            // 
            // recordButton
            // 
            recordButton.BackColor = Color.FromArgb(52, 92, 170);
            recordButton.FlatAppearance.BorderSize = 0;
            recordButton.FlatStyle = FlatStyle.Flat;
            recordButton.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            recordButton.ForeColor = Color.White;
            recordButton.Location = new Point(640, 42);
            recordButton.Name = "recordButton";
            recordButton.Size = new Size(120, 32);
            recordButton.TabIndex = 8;
            recordButton.Text = "Record Walk";
            recordButton.UseVisualStyleBackColor = false;
            recordButton.Click += recordButton_Click;
            // 
            // autoWalkButton
            // 
            autoWalkButton.BackColor = Color.FromArgb(46, 180, 120);
            autoWalkButton.FlatAppearance.BorderSize = 0;
            autoWalkButton.FlatStyle = FlatStyle.Flat;
            autoWalkButton.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            autoWalkButton.ForeColor = Color.White;
            autoWalkButton.Location = new Point(640, 10);
            autoWalkButton.Name = "autoWalkButton";
            autoWalkButton.Size = new Size(120, 30);
            autoWalkButton.TabIndex = 7;
            autoWalkButton.Text = "Auto Walk";
            autoWalkButton.UseVisualStyleBackColor = false;
            autoWalkButton.Click += autoWalkButton_Click;
            // 
            // refreshButton
            // 
            refreshButton.BackColor = Color.FromArgb(76, 130, 255);
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.FlatStyle = FlatStyle.Flat;
            refreshButton.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            refreshButton.ForeColor = Color.White;
            refreshButton.Location = new Point(510, 20);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(120, 32);
            refreshButton.TabIndex = 6;
            refreshButton.Text = "Selecionar proc.";
            refreshButton.UseVisualStyleBackColor = false;
            refreshButton.Click += refreshButton_Click;
            // 
            // processCombo
            // 
            processCombo.BackColor = Color.FromArgb(36, 39, 50);
            processCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            processCombo.FlatStyle = FlatStyle.Flat;
            processCombo.Font = new Font("Segoe UI", 10F);
            processCombo.ForeColor = Color.White;
            processCombo.FormattingEnabled = true;
            processCombo.Location = new Point(90, 22);
            processCombo.Name = "processCombo";
            processCombo.Size = new Size(180, 25);
            processCombo.TabIndex = 1;
            processCombo.SelectedIndexChanged += processCombo_SelectedIndexChanged;
            // 
            // processLabel
            // 
            processLabel.AutoSize = true;
            processLabel.Font = new Font("Segoe UI", 9F);
            processLabel.ForeColor = Color.Gainsboro;
            processLabel.Location = new Point(20, 26);
            processLabel.Name = "processLabel";
            processLabel.Size = new Size(57, 15);
            processLabel.TabIndex = 0;
            processLabel.Text = "Processo:";
            // 
            // keyDelayLabel
            // 
            keyDelayLabel.AutoSize = true;
            keyDelayLabel.Font = new Font("Segoe UI", 9F);
            keyDelayLabel.ForeColor = Color.Gainsboro;
            keyDelayLabel.Location = new Point(280, 46);
            keyDelayLabel.Name = "keyDelayLabel";
            keyDelayLabel.Size = new Size(64, 15);
            keyDelayLabel.TabIndex = 4;
            keyDelayLabel.Text = "Tecla (ms):";
            // 
            // keyDelayText
            // 
            keyDelayText.BackColor = Color.FromArgb(36, 39, 50);
            keyDelayText.BorderStyle = BorderStyle.FixedSingle;
            keyDelayText.Font = new Font("Segoe UI", 10F);
            keyDelayText.ForeColor = Color.White;
            keyDelayText.Location = new Point(355, 42);
            keyDelayText.Name = "keyDelayText";
            keyDelayText.Size = new Size(60, 25);
            keyDelayText.TabIndex = 5;
            keyDelayText.Text = "50";
            // 
            // delayLabel
            // 
            delayLabel.AutoSize = true;
            delayLabel.Font = new Font("Segoe UI", 9F);
            delayLabel.ForeColor = Color.Gainsboro;
            delayLabel.Location = new Point(280, 26);
            delayLabel.Name = "delayLabel";
            delayLabel.Size = new Size(66, 15);
            delayLabel.TabIndex = 2;
            delayLabel.Text = "Delay (ms):";
            // 
            // delayText
            // 
            delayText.BackColor = Color.FromArgb(36, 39, 50);
            delayText.BorderStyle = BorderStyle.FixedSingle;
            delayText.Font = new Font("Segoe UI", 10F);
            delayText.ForeColor = Color.White;
            delayText.Location = new Point(355, 12);
            delayText.Name = "delayText";
            delayText.Size = new Size(60, 25);
            delayText.TabIndex = 3;
            delayText.Text = "100";
            // 
            // actionsPanel
            // 
            actionsPanel.BackColor = Color.FromArgb(24, 27, 38);
            actionsPanel.Controls.Add(clearGridButton);
            actionsPanel.Controls.Add(setIntervalsButton);
            actionsPanel.Controls.Add(addCurrentButton);
            actionsPanel.Controls.Add(importButton);
            actionsPanel.Controls.Add(saveButton);
            actionsPanel.Dock = DockStyle.Top;
            actionsPanel.Location = new Point(0, 134);
            actionsPanel.Name = "actionsPanel";
            actionsPanel.Padding = new Padding(12, 4, 12, 6);
            actionsPanel.Size = new Size(784, 46);
            actionsPanel.TabIndex = 9;
            // 
            // clearGridButton
            // 
            clearGridButton.BackColor = Color.FromArgb(180, 90, 90);
            clearGridButton.FlatAppearance.BorderSize = 0;
            clearGridButton.FlatStyle = FlatStyle.Flat;
            clearGridButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            clearGridButton.ForeColor = Color.White;
            clearGridButton.Location = new Point(620, 8);
            clearGridButton.Name = "clearGridButton";
            clearGridButton.Size = new Size(140, 28);
            clearGridButton.TabIndex = 4;
            clearGridButton.Text = "Limpar grid";
            clearGridButton.UseVisualStyleBackColor = false;
            clearGridButton.Click += clearGridButton_Click;
            // 
            // setIntervalsButton
            // 
            setIntervalsButton.BackColor = Color.FromArgb(90, 90, 110);
            setIntervalsButton.FlatAppearance.BorderSize = 0;
            setIntervalsButton.FlatStyle = FlatStyle.Flat;
            setIntervalsButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            setIntervalsButton.ForeColor = Color.White;
            setIntervalsButton.Location = new Point(470, 8);
            setIntervalsButton.Name = "setIntervalsButton";
            setIntervalsButton.Size = new Size(140, 28);
            setIntervalsButton.TabIndex = 3;
            setIntervalsButton.Text = "Setar intervalo da grid";
            setIntervalsButton.UseVisualStyleBackColor = false;
            setIntervalsButton.Click += setIntervalsButton_Click;
            // 
            // addCurrentButton
            // 
            addCurrentButton.BackColor = Color.FromArgb(100, 140, 255);
            addCurrentButton.FlatAppearance.BorderSize = 0;
            addCurrentButton.FlatStyle = FlatStyle.Flat;
            addCurrentButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            addCurrentButton.ForeColor = Color.White;
            addCurrentButton.Location = new Point(320, 8);
            addCurrentButton.Name = "addCurrentButton";
            addCurrentButton.Size = new Size(140, 28);
            addCurrentButton.TabIndex = 2;
            addCurrentButton.Text = "Adicionar posição";
            addCurrentButton.UseVisualStyleBackColor = false;
            addCurrentButton.Click += addCurrentButton_Click;
            // 
            // importButton
            // 
            importButton.BackColor = Color.FromArgb(46, 180, 120);
            importButton.FlatAppearance.BorderSize = 0;
            importButton.FlatStyle = FlatStyle.Flat;
            importButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            importButton.ForeColor = Color.White;
            importButton.Location = new Point(170, 8);
            importButton.Name = "importButton";
            importButton.Size = new Size(140, 28);
            importButton.TabIndex = 1;
            importButton.Text = "Importar Hunt";
            importButton.UseVisualStyleBackColor = false;
            importButton.Click += importButton_Click;
            // 
            // saveButton
            // 
            saveButton.BackColor = Color.FromArgb(76, 130, 255);
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            saveButton.ForeColor = Color.White;
            saveButton.Location = new Point(20, 8);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(140, 28);
            saveButton.TabIndex = 0;
            saveButton.Text = "Salvar Hunt";
            saveButton.UseVisualStyleBackColor = false;
            saveButton.Click += saveButton_Click;
            // 
            // valuesPanel
            // 
            valuesPanel.BackColor = Color.FromArgb(18, 20, 28);
            valuesPanel.Controls.Add(autoLootCheck);
            valuesPanel.Controls.Add(autoLootLabel);
            valuesPanel.Controls.Add(monsterValue);
            valuesPanel.Controls.Add(monsterLabel);
            valuesPanel.Controls.Add(goToValue);
            valuesPanel.Controls.Add(goToLabel);
            valuesPanel.Controls.Add(goToCheck);
            valuesPanel.Controls.Add(creatureRawValue);
            valuesPanel.Controls.Add(creatureRawLabel);
            valuesPanel.Controls.Add(posZValue);
            valuesPanel.Controls.Add(posYValue);
            valuesPanel.Controls.Add(posXValue);
            valuesPanel.Controls.Add(posZLabel);
            valuesPanel.Controls.Add(posYLabel);
            valuesPanel.Controls.Add(posXLabel);
            valuesPanel.Dock = DockStyle.Top;
            valuesPanel.Location = new Point(0, 180);
            valuesPanel.Name = "valuesPanel";
            valuesPanel.Padding = new Padding(20);
            valuesPanel.Size = new Size(784, 230);
            valuesPanel.TabIndex = 2;
            // 
            // goToValue
            // 
            goToValue.AutoSize = true;
            goToValue.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            goToValue.ForeColor = Color.White;
            goToValue.Location = new Point(340, 120);
            goToValue.Name = "goToValue";
            goToValue.Size = new Size(22, 25);
            goToValue.TabIndex = 13;
            goToValue.Text = "0";
            // 
            // goToLabel
            // 
            goToLabel.AutoSize = true;
            goToLabel.Font = new Font("Segoe UI", 12F);
            goToLabel.ForeColor = Color.Gainsboro;
            goToLabel.Location = new Point(220, 125);
            goToLabel.Name = "goToLabel";
            goToLabel.Size = new Size(110, 21);
            goToLabel.TabIndex = 12;
            goToLabel.Text = "GoTo Oppon:";
            // 
            // goToCheck
            // 
            goToCheck.AutoSize = true;
            goToCheck.Location = new Point(380, 129);
            goToCheck.Name = "goToCheck";
            goToCheck.Size = new Size(15, 14);
            goToCheck.TabIndex = 14;
            goToCheck.UseVisualStyleBackColor = true;
            goToCheck.CheckedChanged += goToCheck_CheckedChanged;
            // 
            // creatureRawValue
            // 
            creatureRawValue.AutoSize = true;
            creatureRawValue.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            creatureRawValue.ForeColor = Color.White;
            creatureRawValue.Location = new Point(340, 150);
            creatureRawValue.Name = "creatureRawValue";
            creatureRawValue.Size = new Size(22, 25);
            creatureRawValue.TabIndex = 11;
            creatureRawValue.Text = "0";
            // 
            // creatureRawLabel
            // 
            creatureRawLabel.AutoSize = true;
            creatureRawLabel.Font = new Font("Segoe UI", 12F);
            creatureRawLabel.ForeColor = Color.Gainsboro;
            creatureRawLabel.Location = new Point(220, 155);
            creatureRawLabel.Name = "creatureRawLabel";
            creatureRawLabel.Size = new Size(107, 21);
            creatureRawLabel.TabIndex = 10;
            creatureRawLabel.Text = "Creature byte:";
            // 
            // autoLootCheck
            // 
            autoLootCheck.AutoSize = true;
            autoLootCheck.Location = new Point(166, 212);
            autoLootCheck.Name = "autoLootCheck";
            autoLootCheck.Size = new Size(15, 14);
            autoLootCheck.TabIndex = 9;
            autoLootCheck.UseVisualStyleBackColor = true;
            autoLootCheck.CheckedChanged += autoLootCheck_CheckedChanged;
            // 
            // autoLootLabel
            // 
            autoLootLabel.AutoSize = true;
            autoLootLabel.Font = new Font("Segoe UI", 12F);
            autoLootLabel.ForeColor = Color.Gainsboro;
            autoLootLabel.Location = new Point(40, 206);
            autoLootLabel.Name = "autoLootLabel";
            autoLootLabel.Size = new Size(113, 21);
            autoLootLabel.TabIndex = 8;
            autoLootLabel.Text = "Auto Loot: OFF";
            // 
            // monsterValue
            // 
            monsterValue.AutoSize = true;
            monsterValue.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            monsterValue.ForeColor = Color.White;
            monsterValue.Location = new Point(140, 150);
            monsterValue.Name = "monsterValue";
            monsterValue.Size = new Size(41, 25);
            monsterValue.TabIndex = 7;
            monsterValue.Text = "NO";
            // 
            // monsterLabel
            // 
            monsterLabel.AutoSize = true;
            monsterLabel.Font = new Font("Segoe UI", 12F);
            monsterLabel.ForeColor = Color.Gainsboro;
            monsterLabel.Location = new Point(40, 155);
            monsterLabel.Name = "monsterLabel";
            monsterLabel.Size = new Size(78, 21);
            monsterLabel.TabIndex = 6;
            monsterLabel.Text = "Attacking:";
            // 
            // posZValue
            // 
            posZValue.AutoSize = true;
            posZValue.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            posZValue.ForeColor = Color.White;
            posZValue.Location = new Point(140, 110);
            posZValue.Name = "posZValue";
            posZValue.Size = new Size(25, 30);
            posZValue.TabIndex = 5;
            posZValue.Text = "0";
            // 
            // posYValue
            // 
            posYValue.AutoSize = true;
            posYValue.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            posYValue.ForeColor = Color.White;
            posYValue.Location = new Point(140, 70);
            posYValue.Name = "posYValue";
            posYValue.Size = new Size(25, 30);
            posYValue.TabIndex = 3;
            posYValue.Text = "0";
            // 
            // posXValue
            // 
            posXValue.AutoSize = true;
            posXValue.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            posXValue.ForeColor = Color.White;
            posXValue.Location = new Point(140, 30);
            posXValue.Name = "posXValue";
            posXValue.Size = new Size(25, 30);
            posXValue.TabIndex = 1;
            posXValue.Text = "0";
            // 
            // posZLabel
            // 
            posZLabel.AutoSize = true;
            posZLabel.Font = new Font("Segoe UI", 12F);
            posZLabel.ForeColor = Color.Gainsboro;
            posZLabel.Location = new Point(40, 115);
            posZLabel.Name = "posZLabel";
            posZLabel.Size = new Size(78, 21);
            posZLabel.TabIndex = 4;
            posZLabel.Text = "Posição Z:";
            // 
            // posYLabel
            // 
            posYLabel.AutoSize = true;
            posYLabel.Font = new Font("Segoe UI", 12F);
            posYLabel.ForeColor = Color.Gainsboro;
            posYLabel.Location = new Point(40, 75);
            posYLabel.Name = "posYLabel";
            posYLabel.Size = new Size(78, 21);
            posYLabel.TabIndex = 2;
            posYLabel.Text = "Posição Y:";
            // 
            // posXLabel
            // 
            posXLabel.AutoSize = true;
            posXLabel.Font = new Font("Segoe UI", 12F);
            posXLabel.ForeColor = Color.Gainsboro;
            posXLabel.Location = new Point(40, 35);
            posXLabel.Name = "posXLabel";
            posXLabel.Size = new Size(78, 21);
            posXLabel.TabIndex = 0;
            posXLabel.Text = "Posição X:";
            // 
            // historyGrid
            // 
            historyGrid.AllowUserToAddRows = false;
            historyGrid.AllowUserToDeleteRows = false;
            historyGrid.AllowUserToResizeRows = false;
            historyGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            historyGrid.BackgroundColor = Color.FromArgb(18, 20, 28);
            historyGrid.BorderStyle = BorderStyle.None;
            historyGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            historyGrid.Columns.AddRange(new DataGridViewColumn[] { colX, colY, colZ, colInterval });
            historyGrid.Dock = DockStyle.Fill;
            historyGrid.EnableHeadersVisualStyles = false;
            historyGrid.Location = new Point(0, 410);
            historyGrid.MultiSelect = false;
            historyGrid.Name = "historyGrid";
            historyGrid.RowHeadersVisible = false;
            historyGrid.RowTemplate.Height = 24;
            historyGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            historyGrid.Size = new Size(784, 78);
            historyGrid.TabIndex = 3;
            // 
            // colX
            // 
            colX.HeaderText = "PosX";
            colX.Name = "colX";
            // 
            // colY
            // 
            colY.HeaderText = "PosY";
            colY.Name = "colY";
            // 
            // colZ
            // 
            colZ.HeaderText = "PosZ";
            colZ.Name = "colZ";
            // 
            // colInterval
            // 
            colInterval.HeaderText = "Intervalo (ms)";
            colInterval.Name = "colInterval";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(18, 20, 28);
            ClientSize = new Size(784, 488);
            Controls.Add(historyGrid);
            Controls.Add(valuesPanel);
            Controls.Add(actionsPanel);
            Controls.Add(controlPanel);
            Controls.Add(headerPanel);
            MinimumSize = new Size(760, 380);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cav Bot";
            TopMost = true;
            Load += Form1_Load;
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            controlPanel.ResumeLayout(false);
            controlPanel.PerformLayout();
            actionsPanel.ResumeLayout(false);
            valuesPanel.ResumeLayout(false);
            valuesPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)historyGrid).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label pipeRestartLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Button recordButton;
        private System.Windows.Forms.Button autoWalkButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ComboBox processCombo;
        private System.Windows.Forms.Label processLabel;
        private System.Windows.Forms.Label delayLabel;
        private System.Windows.Forms.TextBox delayText;
        private System.Windows.Forms.Label keyDelayLabel;
        private System.Windows.Forms.TextBox keyDelayText;
        private System.Windows.Forms.Panel valuesPanel;
        private System.Windows.Forms.Label posZValue;
        private System.Windows.Forms.Label posYValue;
        private System.Windows.Forms.Label posXValue;
        private System.Windows.Forms.Label posZLabel;
        private System.Windows.Forms.Label posYLabel;
        private System.Windows.Forms.Label posXLabel;
        private System.Windows.Forms.DataGridView historyGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colX;
        private System.Windows.Forms.DataGridViewTextBoxColumn colY;
        private System.Windows.Forms.DataGridViewTextBoxColumn colZ;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInterval;
        private System.Windows.Forms.Panel actionsPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button addCurrentButton;
        private System.Windows.Forms.Button setIntervalsButton;
        private System.Windows.Forms.Button clearGridButton;
        private System.Windows.Forms.Label monsterValue;
        private System.Windows.Forms.Label monsterLabel;
        private System.Windows.Forms.Label goToValue;
        private System.Windows.Forms.Label goToLabel;
        private System.Windows.Forms.CheckBox goToCheck;
        private System.Windows.Forms.Label creatureRawValue;
        private System.Windows.Forms.Label creatureRawLabel;
        private System.Windows.Forms.CheckBox autoLootCheck;
        private System.Windows.Forms.Label autoLootLabel;
    }
}
