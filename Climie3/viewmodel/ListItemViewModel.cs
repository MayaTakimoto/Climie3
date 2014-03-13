using Livet;
using ProtoBuf;

namespace Climie3.viewmodel
{
    /// <summary>
    /// リストアイテムのVMクラス
    /// </summary>
    [ProtoContract]
    public class ListItemViewModel : ViewModel
    {
        // 選択状態フラグ
        [ProtoMember(1)]
        private bool isSelected;

        // データ
        [ProtoMember(2)]
        private string text;

        // タグ
        [ProtoMember(3)]
        private string tags;

        // 選択状態フラグプロパティ
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        // データ本体
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        // タグプロパティ
        public string Tags
        {
            get
            {
                return this.tags;
            }
            set
            {
                this.tags = value;
            }
        }

        /// <summary>
        /// ViewModelの破棄
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            // 変数初期化
            isSelected = false;
            text = null;
            tags = null;

            base.Dispose(disposing);
        }
    }
}
