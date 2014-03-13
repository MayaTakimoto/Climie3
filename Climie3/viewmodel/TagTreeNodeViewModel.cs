using Livet;
using ProtoBuf;
using System.Collections.ObjectModel;

namespace Climie3.viewmodel
{
    /// <summary>
    /// タグ管理画面・ツリーノードのVMクラス
    /// </summary>
    [ProtoContract]
    public class TagTreeNodeViewModel : ViewModel
    {
        private bool isSelected;
        private bool isExpanded;
        private bool isFocusable;

        [ProtoMember(1)]
        private string name;

        [ProtoMember(2)]
        private int maxNodesCnt;

        [ProtoMember(3)]
        private ObservableCollection<TagTreeNodeViewModel> children { get; set; }

        [ProtoMember(4)]
        private TagTreeNodeViewModel parent;

        
        /// <summary>
        /// 
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }
            set
            {
                if (this.isExpanded != value)
                {
                    this.isExpanded = value;
                    RaisePropertyChanged("IsExpanded");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFocusable
        {
            get
            {
                return this.isFocusable;
            }
            set
            {
                if (this.isFocusable != value)
                {
                    this.isFocusable = value;
                    RaisePropertyChanged("IsFocusable");
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                RaisePropertyChanged("Name");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxNodesCount
        {
            get
            {
                return this.maxNodesCnt;
            }
            set
            {
                this.maxNodesCnt = value;
                RaisePropertyChanged("MaxNodesCount");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TagTreeNodeViewModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
                RaisePropertyChanged("Parent");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<TagTreeNodeViewModel> Children
        {
            get
            {
                return this.children;
            }
            set
            {
                this.children = value;
                RaisePropertyChanged("Children");
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TagTreeNodeViewModel(TagTreeNodeViewModel parent = null)
        {
            Children = new ObservableCollection<TagTreeNodeViewModel>();
            Name = string.Empty;
            Parent = parent;
            IsSelected = false;
        }

        /// <summary>
        /// ViewModelの破棄
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            name = null;
            parent = null;
            children = null;
            maxNodesCnt = 0;

            base.Dispose(disposing);
        }
    }
}
