using Climie3.config;
//-----------------------------------------------------------------------
// <summary>メインウィンドウViewModel</summary>
// <author>MayaTakimoto</author>
// <date>$Date: 2014-01-31 00:00:00 +9:00 $</date>
// <copyright file="$Name: MainViewModel.cs $" >
// Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------
using Climie3.model;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using ProtoBuf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace Climie3.viewmodel
{
    /// <summary>
    /// メインVMクラス
    /// </summary>
    [ProtoContract]
    public class MainViewModel : ViewModel
    {
        // データ保持用リスト
        [ProtoMember(1)]
        private ObservableCollection<ListItemViewModel> listMain;

        // 表示用リスト
        private ObservableCollection<ListItemViewModel> listDsp;

        // 履歴件数
        private string listCount;

        // 検索モード
        private string[] searchModeList;

        // クリップボード監視オブジェクト
        private ClipboradWatcherModel cWatch;

        // スレッド
        private Thread thread;

        // 正規表現パターン生成オブジェクト
        private RegexManagerModel regMan;

        // 
        private ViewModelCommand _ShowPasswordWindowCommand;


        /// <summary>
        /// データ保持用リストプロパティ
        /// </summary>
        /// <remarks>取得した値はこちらに保持される</remarks>
        private ObservableCollection<ListItemViewModel> ListMain
        {
            get
            {
                return this.listMain;
            }
            set
            {
                this.listMain = value;

                // 画面に反映させる
                ListDsp = this.listMain;
            }
        }

        /// <summary>
        /// 表示用リストプロパティ
        /// </summary>
        /// <remarks>画面にはこのリストの内容が反映される</remarks>
        public ObservableCollection<ListItemViewModel> ListDsp
        {
            get
            {
                return this.listDsp;
            }
            set
            {
                this.listDsp = value;
                RaisePropertyChanged("ListDsp");
            }
        }

        /// <summary>
        /// 履歴件数プロパティ
        /// </summary>
        public string ListCount
        {
            get
            {
                return this.listCount;
            }
            set
            {
                this.listCount = value;
                RaisePropertyChanged("ListCount");
            }
        }

        /// <summary>
        /// 検索モードプロパティ
        /// </summary>
        public string[] SearchModeList
        {
            get
            {
                if (searchModeList == null)
                {
                    searchModeList = new string[] { "Text", "Tags" };
                }

                return searchModeList;
            }
        }

        /// <summary>
        /// 現在の検索モード
        /// </summary>
        public string SearchMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ViewModelCommand ShowPasswordWindowCommand
        {
            get
            {
                if (_ShowPasswordWindowCommand == null)
                {
                    _ShowPasswordWindowCommand = new ViewModelCommand(ShowPasswordWindow);
                }
                return _ShowPasswordWindowCommand;
            }
        }
         

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            cWatch = new ClipboradWatcherModel();

            CompositeDisposable.Add(cWatch);
            CompositeDisposable.Add(regMan);
            CompositeDisposable.Add(KeyInfo.Instance);

            // データ復元
            this.Load();

            ListCount = "Count : " + ListMain.Count;
            this.ThreadStart();
        }

        /// <summary>
        /// クリップボード変更検知時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cWatch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // クリップボードが空の場合は何もしない
            if (string.IsNullOrEmpty(cWatch.CbText))
            {
                return;
            }

            // 同じテキストデータがリスト内に存在する場合は何もしない
            if (ListMain.Any(item => item.Text.Equals(cWatch.CbText)))
            {
                return;
            }

            // テキストデータをリストに追加
            Application.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    ListItemViewModel newItem = new ListItemViewModel();
                    newItem.Text = cWatch.CbText;                           // クリップボード内テキスト
                    newItem.Tags = DateTime.Now.ToString("yyyyMMddHHmmss"); // 現在日時

                    ListMain.Add(newItem);
                })
            );

            // 最新の履歴件数を取得
            ListCount = "Count : " + ListMain.Count;
        }

        /// <summary>
        /// クリップボード監視の開始
        /// </summary>
        public void ThreadStart()
        {
            if (thread == null)
            {
                // クリップボード変更検知イベントを登録
                cWatch.PropertyChanged += new PropertyChangedEventHandler(cWatch_PropertyChanged);

                thread = new Thread(new ThreadStart(cWatch.WatchClipboard));
                thread.SetApartmentState(ApartmentState.STA);

                // 監視開始
                cWatch.StopFlg = false;
                thread.Start();
            }
        }

        /// <summary>
        /// クリップボード監視の終了
        /// </summary>
        public void ThreadStop()
        {
            if (thread != null)
            {
                cWatch.StopFlg = true;

                // 監視終了
                thread.Abort();
                thread = null;

                // クリップボード変更検知イベントの登録を解除
                cWatch.PropertyChanged -= cWatch_PropertyChanged;
            }
        }

        /// <summary>
        /// 取得済みデータ保存
        /// </summary>
        public void Save()
        {
            DataSaveLoadModel dataWriter = new DataSaveLoadModel();
            dataWriter.Save<ListItemViewModel>(ListMain, "data.dat");
        }

        /// <summary>
        /// 取得済みデータ復元
        /// </summary>
        public void Load()
        {
            DataSaveLoadModel dataReader = new DataSaveLoadModel();
            ListMain = dataReader.Load<ListItemViewModel>("data.dat");
        }

        /// <summary>
        /// 検索前処理
        /// </summary>
        public void InitSearch()
        {
            regMan = new RegexManagerModel();
        }

        /// <summary>
        /// 検索処理
        /// </summary>
        /// <param name="pattern"></param>
        public void Search(string pattern)
        {
            Regex regex = null;

            if (string.IsNullOrEmpty(pattern) || "/".Equals(pattern))
            {
                ListDsp = ListMain;
            }
            else
            {
                regex = regMan.GetRegex(pattern);

                if (regex != null)
                {
                    ListDsp = new ObservableCollection<ListItemViewModel>(ListMain.Where((item) =>
                    {
                        if ("Text".Equals(SearchMode))
                        {
                            return regex.IsMatch(item.Text);
                        }
                        else
                        {
                            return regex.IsMatch(item.Tags);
                        }
                    }));
                }
            }
        }

        /// <summary>
        /// 検索後処理
        /// </summary>
        public void EndSearch()
        {
            if (regMan != null)
            {
                regMan.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowPasswordWindow()
        {
            Messenger.Raise(new TransitionMessage(new PasswordViewModel(), "ShowPasswordWindow"));
        }

        /// <summary>
        /// アプリケーションの終了
        /// </summary>
        public void Shutdown()
        {
            this.ThreadStop();

            // 取得済みデータの保存
            this.Save();

            Application.Current.Shutdown();
        }

        /// <summary>
        /// ViewModelの破棄
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            // 変数初期化
            listMain = null;
            listDsp = null;
            listCount = null;
            
            //this.ThreadStop();
            //if (cWatch != null)
            //{
            //    cWatch.Dispose();
            //}

            //if (regMan != null)
            //{
            //    regMan.Dispose();
            //}

            //KeyInfo.Instance.Dispose();

            base.Dispose(disposing);
        }
    }
}
