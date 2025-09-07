using System.Windows.Forms;

namespace GW2EIParser;

partial class UpdaterForm
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
        lblHeader = new Label();
        gridVersions = new DataGridView();
        dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
        btnUpdate = new Button();
        btnDismiss = new Button();
        linkLblReleaseNotes = new LinkLabel();
        ((System.ComponentModel.ISupportInitialize)gridVersions).BeginInit();
        SuspendLayout();
        // 
        // lblHeader
        // 
        lblHeader.AutoSize = true;
        lblHeader.Location = new Point(23, 18);
        lblHeader.Name = "lblHeader";
        lblHeader.Size = new Size(179, 15);
        lblHeader.TabIndex = 0;
        lblHeader.Text = "New Elite Insights version found:";
        // 
        // gridVersions
        // 
        gridVersions.AllowUserToAddRows = false;
        gridVersions.AllowUserToDeleteRows = false;
        gridVersions.AllowUserToResizeColumns = false;
        gridVersions.AllowUserToResizeRows = false;
        gridVersions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        gridVersions.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
        gridVersions.DefaultCellStyle.SelectionBackColor = gridVersions.DefaultCellStyle.BackColor;
        gridVersions.DefaultCellStyle.SelectionForeColor = gridVersions.DefaultCellStyle.ForeColor;
        gridVersions.Location = new Point(23, 47);
        gridVersions.Name = "gridVersions";
        gridVersions.ReadOnly = true;
        gridVersions.RowHeadersVisible = false;
        gridVersions.Size = new Size(203, 51);
        gridVersions.TabIndex = 1;
        // 
        // dataGridViewTextBoxColumn1
        // 
        dataGridViewTextBoxColumn1.HeaderText = "Current";
        dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
        dataGridViewTextBoxColumn1.ReadOnly = true;
        // 
        // dataGridViewTextBoxColumn2
        // 
        dataGridViewTextBoxColumn2.HeaderText = "Latest";
        dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
        dataGridViewTextBoxColumn2.ReadOnly = true;
        // 
        // btnUpdate
        // 
        btnUpdate.Location = new Point(151, 151);
        btnUpdate.Name = "btnUpdate";
        btnUpdate.Size = new Size(75, 23);
        btnUpdate.TabIndex = 2;
        btnUpdate.Text = "Update";
        btnUpdate.UseVisualStyleBackColor = true;
        btnUpdate.Click += buttonUpdate_Click;
        // 
        // btnDismiss
        // 
        btnDismiss.Location = new Point(23, 151);
        btnDismiss.Name = "btnDismiss";
        btnDismiss.Size = new Size(75, 23);
        btnDismiss.TabIndex = 3;
        btnDismiss.Text = "Dismiss";
        btnDismiss.UseVisualStyleBackColor = true;
        btnDismiss.Click += buttonDismiss_Click;
        // 
        // linkLblReleaseNotes
        // 
        linkLblReleaseNotes.AutoSize = true;
        linkLblReleaseNotes.Location = new Point(23, 117);
        linkLblReleaseNotes.Name = "linkLblReleaseNotes";
        linkLblReleaseNotes.Size = new Size(80, 15);
        linkLblReleaseNotes.TabIndex = 4;
        linkLblReleaseNotes.TabStop = true;
        linkLblReleaseNotes.Text = "Release Notes";
        // 
        // UpdaterForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(244, 191);
        Controls.Add(linkLblReleaseNotes);
        Controls.Add(btnDismiss);
        Controls.Add(btnUpdate);
        Controls.Add(gridVersions);
        Controls.Add(lblHeader);
        Name = "UpdaterForm";
        Text = "Update Found";
        ((System.ComponentModel.ISupportInitialize)gridVersions).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblHeader;
    private DataGridView gridVersions;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private Button btnUpdate;
    private Button btnDismiss;
    private LinkLabel linkLblReleaseNotes;
}
