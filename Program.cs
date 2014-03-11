using FISCA;
using System.Collections.Generic;

namespace EMBA.Import
{
    public class Program
    {
        [FISCA.MainMethod()]
        public static void Main()
        {
            Features.Register("EMBA.ImportWizard/SelectSource", arg =>
            {
                ContinueDirection Direction = new SelectSource(arg).ShowWizardDialog();
                return Direction;
            });

            Features.Register("EMBA.ImportWizard/SelectKey", arg =>
            {
                ImportWizard mImportWizard = arg["EMBA.ImportWizard"] as ImportWizard;

                if (mImportWizard.FieldProcessor.KeyFields.Count == 0)
                    return ContinueDirection.Skip;

                ContinueDirection Direction = new SelectKey(arg).ShowWizardDialog();

                return Direction;
            });

            Features.Register("EMBA.ImportWizard/SelectFields", arg =>
            {
                #region 判斷若可選取的欄位為0，則跳過本步驟
                List<string> SelectableFields = arg.TryGetList<string>("SelectableFields");

                if (SelectableFields.Count == 0)
                    return ContinueDirection.Skip;
                #endregion

                ContinueDirection Direction = new SelectFields(arg).ShowWizardDialog();
                return Direction;
            });

            Features.Register("EMBA.ImportWizard/SelectValidate", arg =>
            {
                ContinueDirection Direction = new SelectValidate(arg).ShowWizardDialog();

                return Direction;
            });

            Features.Register("EMBA.ImportWizard/AdvancedFields", arg =>
            {
                #region 判斷若「進階設定」的畫面被某匯入程式取消，則跳過本步驟
                ImportWizard mImportWizard = arg["EMBA.ImportWizard"] as ImportWizard;

                if (mImportWizard.ShowAdvancedForm == false)
                    return ContinueDirection.Skip;
                #endregion

                ContinueDirection Direction = new AdvancedFields(arg).ShowWizardDialog();
                return Direction;
            });

            Features.Register("EMBA.ImportWizard/SelectImport", arg =>
            {
                ContinueDirection Direction = new SelectImport(arg).ShowWizardDialog();
                return Direction;
            });
        }
    }
}