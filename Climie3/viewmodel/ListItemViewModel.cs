using Livet;
using ProtoBuf;
using System.Collections.Generic;

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
        private List<string> tags;

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
        public List<string> Tags
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
        /// コンストラクタ
        /// </summary>
        public ListItemViewModel()
        {
            Tags = new List<string>();
        }
    }
}
