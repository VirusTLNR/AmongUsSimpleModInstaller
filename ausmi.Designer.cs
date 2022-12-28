
namespace AmongUsSimpleModInstaller
{
    partial class ausmi
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblSteamInstallPath = new System.Windows.Forms.Label();
            this.btnInstall = new System.Windows.Forms.Button();
            this.cmbModSelection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTargetLocation = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnModsFolder = new System.Windows.Forms.Button();
            this.lblVanillaWarning = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblSteamInstallPath
            // 
            this.lblSteamInstallPath.AutoSize = true;
            this.lblSteamInstallPath.Location = new System.Drawing.Point(55, 9);
            this.lblSteamInstallPath.Name = "lblSteamInstallPath";
            this.lblSteamInstallPath.Size = new System.Drawing.Size(23, 13);
            this.lblSteamInstallPath.TabIndex = 0;
            this.lblSteamInstallPath.Text = "null";
            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(334, 57);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 23);
            this.btnInstall.TabIndex = 4;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // cmbModSelection
            // 
            this.cmbModSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModSelection.FormattingEnabled = true;
            this.cmbModSelection.Items.AddRange(new object[] {
            "The Other Roles",
            "Town Of Us-Reactivated",
            "Las Monjas"});
            this.cmbModSelection.Location = new System.Drawing.Point(12, 59);
            this.cmbModSelection.Name = "cmbModSelection";
            this.cmbModSelection.Size = new System.Drawing.Size(316, 21);
            this.cmbModSelection.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(149, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Select the mod you wish to install";
            // 
            // lblTargetLocation
            // 
            this.lblTargetLocation.AutoSize = true;
            this.lblTargetLocation.Location = new System.Drawing.Point(74, 89);
            this.lblTargetLocation.Name = "lblTargetLocation";
            this.lblTargetLocation.Size = new System.Drawing.Size(23, 13);
            this.lblTargetLocation.TabIndex = 7;
            this.lblTargetLocation.Text = "null";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(415, 62);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 8;
            // 
            // btnModsFolder
            // 
            this.btnModsFolder.Location = new System.Drawing.Point(12, 115);
            this.btnModsFolder.Name = "btnModsFolder";
            this.btnModsFolder.Size = new System.Drawing.Size(109, 23);
            this.btnModsFolder.TabIndex = 9;
            this.btnModsFolder.Text = "Open Mods Folder";
            this.btnModsFolder.UseVisualStyleBackColor = true;
            this.btnModsFolder.Click += new System.EventHandler(this.btnModsFolder_Click);
            // 
            // lblVanillaWarning
            // 
            this.lblVanillaWarning.AutoSize = true;
            this.lblVanillaWarning.Location = new System.Drawing.Point(127, 121);
            this.lblVanillaWarning.Name = "lblVanillaWarning";
            this.lblVanillaWarning.Size = new System.Drawing.Size(337, 13);
            this.lblVanillaWarning.TabIndex = 10;
            this.lblVanillaWarning.Text = "Vanilla files installed must be correct ones for the mod before installing.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Will install to:";
            // 
            // ausmi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 143);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblVanillaWarning);
            this.Controls.Add(this.btnModsFolder);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblTargetLocation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbModSelection);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.lblSteamInstallPath);
            this.Name = "ausmi";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSteamInstallPath;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.ComboBox cmbModSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTargetLocation;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnModsFolder;
        private System.Windows.Forms.Label lblVanillaWarning;
        private System.Windows.Forms.Label label2;
    }
}

