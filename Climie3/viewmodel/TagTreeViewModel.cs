using Livet;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Climie3.viewmodel
{
    [ProtoContract]
    public class TagTreeViewModel : ViewModel
    {
        [ProtoMember(1)]
        private ObservableCollection<TagTreeNodeViewModel> nodes;
    }
}
