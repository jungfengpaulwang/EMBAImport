﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FISCA;

namespace EMBA.Import
{
    /// <summary>
    /// 使用者選擇欄位
    /// 1.若是沒有可選擇欄位，此畫面可不用出現，直接進入到下個畫面。
    /// 2.顯示工作表所有欄位名稱部份暫不設計。
    /// </summary>
    public partial class AdvancedFields : WizardForm
    {
        private ArgDictionary mArgs;
        private ImportFullOption mImportOption;
        private ImportWizard mImportWizard;
        private List<string> mSelectedFields;

        public AdvancedFields()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 建構式，傳入精靈參數
        /// </summary>
        /// <param name="args"></param>
        public AdvancedFields(ArgDictionary args)
            : base(args)
        {
            InitializeComponent();
            //介面初始化設定
            NextButtonTitle = "開始匯入";
            //NextButtonEnabled = false;
            //NextButtonTitle = "開始驗證";
            mArgs = args;
            mImportWizard = mArgs["EMBA.ImportWizard"] as ImportWizard;
            mImportOption = mArgs["ImportOption"] as ImportFullOption;
            mSelectedFields = mImportOption.SelectedFields;
            mImportOption.AdvancedFields.Clear();

            this.Text = mImportWizard.ValidateRule.Root.GetAttributeText("Name") + "-" + this.Text;
            this.Text += "(" + CurrentStep + "/" + TotalStep + ")"; //功能名稱(目前頁數/總頁數)

            RefreshFields();

            chkSelectAll.CheckedChanged += (sender,e) =>
            {
                foreach (ListViewItem Item in lvSourceFieldList.Items)
                {
                    Item.Checked = chkSelectAll.Checked;
                }
            };

            //若是沒有使用者可選擇的欄位，則直接跳到下個畫面；目前設這會有問題...
            //if (mSelectableFields.Count == 0)
            //    this.OnNextButtonClick();
        }

        /// <summary>
        /// 列出使用者可選擇匯入的欄位
        /// </summary>
        private void RefreshFields()
        {
            if (mSelectedFields != null)
            {
                lvSourceFieldList.Items.Clear();

                mSelectedFields.ForEach(x => 
                    {
                        ListViewItem Item = new ListViewItem();
                        Item.Name = x;
                        Item.Text = x;
                        Item.Checked = true;
                        Item.ForeColor = Color.Blue;
                        lvSourceFieldList.Items.Add(Item);
                    }
                );
            }
        }

        /// <summary>
        /// 驗證是否可進行到下個畫面
        /// </summary>
        /// <returns></returns>
        private bool ValidateNext()
        {
            return true;
        }

        protected override void OnNextButtonClick()
        {
            if (ValidateNext())
            {
                foreach (ListViewItem Item in lvSourceFieldList.Items)
                    if (Item.Checked && !mImportOption.AdvancedFields.Contains(Item.Name))
                        mImportOption.AdvancedFields.Add(Item.Name);

                mArgs.SetValue("ImportOption",mImportOption);

                base.OnNextButtonClick();
            }
        }
    }
}