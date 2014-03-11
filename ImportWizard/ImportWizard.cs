using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using EMBA.DocumentValidator;
using EMBA.Validator;
using FISCA;

namespace EMBA.Import
{
    /// <summary>
    /// 匯入功能抽象類別
    /// </summary>
    public abstract class ImportWizard : IRowStreamImport
    {
        private List<string> mCommands = new List<string>(new string[]{
                "EMBA.ImportWizard/SelectSource",
                "EMBA.ImportWizard/SelectKey",
                "EMBA.ImportWizard/SelectFields",
                "EMBA.ImportWizard/SelectValidate",
                "EMBA.ImportWizard/SelectImport"
            });
        //                "EMBA.ImportWizard/AdvancedFields",
        /// <summary>
        /// 驗證規則
        /// </summary>
        private XDocument mValidateRule;

        /// <summary>
        /// 驗證欄位解析器
        /// </summary>
        private FieldProcessor mFieldProcessor;

        /// <summary>
        /// 驗證規則
        /// </summary>
        public XDocument ValidateRule { get { return mValidateRule; } }

        /// <summary>
        /// 驗證欄位解析器
        /// </summary>
        public FieldProcessor FieldProcessor { get { return mFieldProcessor; } }

        /// <summary>
        /// 客製驗證規則
        /// </summary>
        public Action<List<IRowStream>, RowMessages> CustomValidate { get; set; }

        /// <summary>
        /// 匯入完成後，要執行的動作。參數為匯入資料的鍵值
        /// </summary>
        public Action<System.Windows.Forms.Form> ImportCompleteAction { get; set; }

        /// <summary>
        /// 匯入完成函式，函式可傳回要在畫面上顯示的資訊，例如成功新增或更新幾筆。
        /// </summary>
        public Func<string> Complete { get; set; }

        /// <summary>
        /// 匯入訊息
        /// </summary>
        public ImportMessages ImportMessages { get; set; }

        /// <summary>
        /// 是否顯示「進階設定(目前僅可選擇空白不匯入欄位)」畫面，預設為「False」
        /// </summary>
        public bool ShowAdvancedForm { get; set; }

        /// <summary>
        /// 是否分批，預設為True
        /// </summary>
        public bool IsSplit { get; set; }

        /// <summary>
        /// 分批的最大執行緒數量，預設為3，只有在IsSplit為True才會有用
        /// </summary>
        public int SplitThreadCount { get; set; }

        /// <summary>
        /// 每次分批的資料量，預設為1000，只有在IsSplit為True才會有用
        /// </summary>
        public int SplitSize { get; set;}

        /// <summary>
        /// 取得使用者選取的鍵值
        /// </summary>
        public List<string> SelectedKeyFields { get; set; }

        /// <summary>
        /// 取得使用者選取的欄位
        /// </summary>
        public List<string> SelectedFields { get; set; }

        /// <summary>
        /// 驗證規則樣版
        /// </summary>
        public XDocument ValidateRuleFormater { get; set; }

        /// <summary>
        /// 建構式
        /// </summary>
        public ImportWizard()
        {
            ImportMessages = new ImportMessages();
            ShowAdvancedForm = false;
            IsSplit = true;
            SplitThreadCount = 3;
            SplitSize = 1000;
        }

        /// <summary>
        /// 匯入完成時要執行的代理程式之按鈕的標題文字
        /// </summary>
        public string ImportCompleteActionButtonTitle { get; set; }

        /// <summary>
        /// 執行匯入功能
        /// </summary>
        public void Execute()
        {
            try
            {
                LoadRule();

                mFieldProcessor = new FieldProcessor(mValidateRule.Root);
            }
            catch (Exception e)
            {
                MessageBox.Show("下載驗證規則時發生錯誤，以下為詳細訊息："+ System.Environment.NewLine +e.Message);

                return;
            }

            try
            {
                ArgDictionary args = new ArgDictionary();

                args.SetValue("EMBA.ImportWizard", this);

                Features.Invoke(args, mCommands.ToArray());
            }
            catch (Exception e)
            {
                MessageBox.Show("開啟匯入表單時發生錯誤，以下為詳細訊息："+System.Environment.NewLine+e.Message);

                return;
            }
        }

        #region LoadRule
        /// <summary>
        /// 載入驗證規則
        /// </summary>
        private void LoadRule()
        {
            try
            {
                mValidateRule = GetValidateRule();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region IRowStreamImport 成員

        /// <summary>
        /// 取得驗證規則 return XDocument restored in Resource
        /// </summary>
        /// <returns></returns>
        public abstract XDocument GetValidateRule();

        /// <summary>
        /// 取得支援的匯入動作
        /// </summary>
        /// <returns></returns>
        public abstract ImportAction GetSupportActions();

        /// <summary>
        /// 準備匯入
        /// </summary>
        /// <param name="Option"></param>
        public abstract void Prepare(ImportOption Option);

        /// <summary>
        /// 實際分批匯入
        /// </summary>
        /// <param name="Rows"></param>
        /// <returns></returns>
        public abstract string Import(List<IRowStream> Rows);
        #endregion
    }
}